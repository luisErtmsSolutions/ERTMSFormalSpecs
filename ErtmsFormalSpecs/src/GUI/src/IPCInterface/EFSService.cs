// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------
namespace GUI.IPCInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataDictionary.Tests.Runner;
    using DataDictionary;
    using System.ServiceModel;
    using System.Windows.Forms;
    using System.Threading;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EFSService : IEFSService
    {
        /// <summary>
        /// Indicates that the explain view should be updated according to the scenario execution
        /// </summary>
        public bool Explain { get; private set; }

        /// <summary>
        /// Indicates that the events should be logged
        /// </summary>
        public bool LogEvents { get; private set; }

        /// <summary>
        /// The duration (in ms) of an execution cycle
        /// </summary>
        public int CycleDuration { get; private set; }

        /// <summary>
        /// The number of events that should be kept in memory
        /// </summary>
        public int KeepEventCount { get; private set; }

        /// <summary>
        /// Resource protection
        /// </summary>
        private Dictionary<Step, Mutex> StepAccess { get; set; }

        /// <summary>
        /// Keeps track of each connection status
        /// </summary>
        private class ConnectionStatus
        {
            /// <summary>
            /// Indicates that the connection is still active
            /// </summary>
            public bool Active { get; set; }

            /// <summary>
            /// The step for which the client is waiting
            /// </summary>
            public Step ExpectedStep { get; set; }

            /// <summary>
            /// The last time a cycle request has been performed
            /// </summary>
            public DateTime LastCycleRequest { get; set; }

            /// <summary>
            /// The last time a cycle activity has been resumed
            /// </summary>
            public DateTime LastCycleResume { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public ConnectionStatus()
            {
                Active = true;
                LastCycleRequest = DateTime.MinValue;
                LastCycleResume = DateTime.MinValue;
            }
        }

        /// <summary>
        /// The list of connection statuses
        /// </summary>
        private List<ConnectionStatus> Connections { get; set; }

        /// <summary>
        /// The last step being executed
        /// </summary>
        private Step LastStep { get; set; }

        /// <summary>
        /// Provides the next step to execute
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private Step NextStep(Step current)
        {
            Step retVal = Step.CleanUp;

            switch (current)
            {
                case Step.CleanUp: retVal = Step.Verification; break;
                case Step.Verification: retVal = Step.UpdateInternal; break;
                case Step.UpdateInternal: retVal = Step.Process; break;
                case Step.Process: retVal = Step.UpdateOutput; break;
                case Step.UpdateOutput: retVal = Step.CleanUp; break;
            }

            return retVal;
        }

        /// <summary>
        /// The thread which is used to launch the runner
        /// </summary>
        private LaunchRunner LaunchRunnerSynchronizer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EFSService()
        {
            Connections = new List<ConnectionStatus>();
            Explain = true;
            LogEvents = false;
            CycleDuration = 100;
            KeepEventCount = 10000;

            LastStep = Step.CleanUp;

            StepAccess = new Dictionary<Step, Mutex>();
            LaunchRunnerSynchronizer = new LaunchRunner(this, 10);
        }

        /// <summary>
        /// Adds a client for this server
        /// </summary>
        /// <returns>The client id</returns>
        private int AddClient()
        {
            int retVal = Connections.Count;

            Connections.Add(new ConnectionStatus());

            return retVal;
        }

        /// <summary>
        /// Connects to the service 
        /// </summary>
        /// <returns>The client identifier</returns>
        public int ConnectUsingDefaultValues()
        {
            return AddClient();
        }

        /// <summary>
        /// Connects to the service 
        /// </summary>
        /// <param name="explain">Indicates that the explain view should be updated according to the scenario execution</param>
        /// <param name="logEvents">Indicates that the events should be logged</param>
        /// <param name="cycleDuration">The duration (in ms) of an execution cycle</param>
        /// <param name="keepEventCount">The number of events that should be kept in memory</param>
        public int Connect(bool explain, bool logEvents, int cycleDuration, int keepEventCount)
        {
            int clientId = AddClient();

            Explain = explain;
            LogEvents = logEvents;
            CycleDuration = cycleDuration;
            KeepEventCount = keepEventCount;

            return clientId;
        }

        /// <summary>
        /// Ensures that the client id is valid
        /// </summary>
        /// <param name="clientId"></param>
        private void checkClient(int clientId)
        {
            if (clientId >= Connections.Count)
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Invalid client id " + clientId));
            }
            else
            {
                if (!Connections[clientId].Active)
                {
                    throw new FaultException<EFSServiceFault>(new EFSServiceFault("Client connection has been closed by server"));
                }
            }
        }

        /// <summary>
        /// Checks that a cycle can be launched, that is each client has voted for his next step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool checkLaunch()
        {
            bool retVal = false;

            // Checks that there are active connections
            foreach (ConnectionStatus status in Connections)
            {
                retVal = retVal || status.Active;
            }

            if (retVal)
            {
                // Checks that all active connection have selected their next step
                foreach (ConnectionStatus status in Connections)
                {
                    if (status.Active)
                    {
                        retVal = retVal && (status.LastCycleRequest > status.LastCycleResume);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates if there is a client pending for a specific step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool pendingClients(Step step)
        {
            bool retVal = false;

            // Checks that there are active connections
            foreach (ConnectionStatus status in Connections)
            {
                if (status.ExpectedStep == step && status.LastCycleRequest > status.LastCycleResume)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// 1s between each client decisions
        /// </summary>
        private static TimeSpan MAX_DELTA = new TimeSpan(0, 0, 60, 0, 0);

        /// <summary>
        /// Performs a single cycle
        /// </summary>
        public void Cycle()
        {
            try
            {
                DateTime now = DateTime.Now;

                // Close inactive connections
                foreach (ConnectionStatus status in Connections)
                {
                    TimeSpan delta = now - status.LastCycleRequest;
                    if (false && delta > MAX_DELTA)
                    {
                        status.Active = false;
                    }
                }

                // Launches the runner when all active client have selected their next step
                while (checkLaunch())
                {
                    LastStep = NextStep(LastStep);
                    Runner.ExecuteOnePriority(convertStep2Priority(LastStep));
                    if (LastStep == Step.CleanUp)
                    {
                        GUIUtils.MDIWindow.Invoke((MethodInvoker)delegate { GUIUtils.MDIWindow.RefreshAfterStep(); });
                    }

                    while (pendingClients(LastStep))
                    {
                        // Let the processes waiting for the end of this step run
                        StepAccess[LastStep].ReleaseMutex();

                        // Let the other processes wake up
                        Thread.Sleep(1);

                        // Wait until all processes for this step have executed their work
                        StepAccess[LastStep].WaitOne();
                    }
                }
            }
            catch ( Exception )
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Continuously launch the runner when all client have selected their next stop step
        /// </summary>
        private class LaunchRunner : GenericSynchronizationHandler<EFSService>
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="service"></param>
            /// <param name="cycleTime"></param>
            public LaunchRunner(EFSService service, int cycleTime)
                : base(service, cycleTime)
            {
            }

            /// <summary>
            /// Initialize the task 
            /// </summary>
            /// <param name="instance"></param>
            public override void Initialize(EFSService instance)
            {
                // Allocates all critical section
                instance.StepAccess[Step.CleanUp] = new Mutex(true, "CleanUp");
                instance.StepAccess[Step.Verification] = new Mutex(true, "Verification");
                instance.StepAccess[Step.UpdateInternal] = new Mutex(true, "UpdateInternal");
                instance.StepAccess[Step.Process] = new Mutex(true, "Process");
                instance.StepAccess[Step.UpdateOutput] = new Mutex(true, "UpdateOutput");
            }

            /// <summary>
            /// Actually performes the synchronization, that is launch the runner when all client are ready
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(EFSService instance)
            {
                instance.Cycle();
            }
        }

        /// <summary>
        /// Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        /// <param name="step">The cycle step to execute</param>
        public void Cycle(int clientId, Step step)
        {
            checkClient(clientId);

            Connections[clientId].LastCycleRequest = DateTime.Now;
            Connections[clientId].LastCycleResume = DateTime.MinValue;
            Connections[clientId].ExpectedStep = step;

            StepAccess[step].WaitOne();

            Connections[clientId].LastCycleResume = DateTime.Now;
            StepAccess[step].ReleaseMutex();
        }

        /// <summary>
        /// Restarts the engine with default values
        /// </summary>
        public void Restart()
        {
            EFSSystem.INSTANCE.Runner = new Runner(Explain, LogEvents, CycleDuration, KeepEventCount);
        }

        /// <summary>
        /// Provides the runner on which the service is applied
        /// </summary>
        public Runner Runner
        {
            get
            {
                EFSSystem efsSystem = EFSSystem.INSTANCE;
                if (efsSystem.Runner == null)
                {
                    EFSSystem.INSTANCE.Runner = new Runner(Explain, LogEvents, CycleDuration, KeepEventCount);
                }

                return efsSystem.Runner;
            }
        }

        /// <summary>
        /// Provides the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public Values.Value GetVariableValue(string variableName)
        {
            Values.Value retVal = null;

            DataDictionary.Variables.IVariable variable = EFSSystem.INSTANCE.findByFullName(variableName) as DataDictionary.Variables.IVariable;
            if (variable != null)
            {
                retVal = convertOut(variable.Value);
            }
            else
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot find variable " + variableName));
            }

            return retVal;
        }

        /// <summary>
        /// Provides the value of an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Values.Value GetExpressionValue(string expression)
        {
            Values.Value retVal = null;

            DataDictionary.Interpreter.Expression expressionTree = EFSSystem.INSTANCE.Parser.Expression(null, expression);
            if (expressionTree != null)
            {
                retVal = convertOut(expressionTree.GetValue(new DataDictionary.Interpreter.InterpretationContext()));
            }
            else
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot evaluate expression " + expression));
            }

            return retVal;
        }

        /// <summary>
        /// Converts a DataDictionary.Values.IValue into an EFSIPCInterface.Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Values.Value convertOut(DataDictionary.Values.IValue value)
        {
            // Handles the boolean case
            {
                DataDictionary.Values.BoolValue v = value as DataDictionary.Values.BoolValue;
                if (v != null)
                {
                    return new Values.BoolValue(v.Val);
                }
            }

            // Handles the integer case
            {
                DataDictionary.Values.IntValue v = value as DataDictionary.Values.IntValue;
                if (v != null)
                {
                    return new Values.IntValue(v.Val);
                }
            }

            // Handles the double case
            {
                DataDictionary.Values.DoubleValue v = value as DataDictionary.Values.DoubleValue;
                if (v != null)
                {
                    return new Values.DoubleValue(v.Val);
                }
            }

            // Handles the string case
            {
                DataDictionary.Values.StringValue v = value as DataDictionary.Values.StringValue;
                if (v != null)
                {
                    return new Values.StringValue(v.Val);
                }
            }

            // Handles the state case
            {
                DataDictionary.Constants.State v = value as DataDictionary.Constants.State;
                if (v != null)
                {
                    return new Values.StateValue(v.FullName);
                }
            }

            // Handles the enumeration value case
            {
                DataDictionary.Constants.EnumValue v = value as DataDictionary.Constants.EnumValue;
                if (v != null)
                {
                    return new Values.EnumValue(v.FullName);
                }
            }

            // Handles the list case
            {
                DataDictionary.Values.ListValue v = value as DataDictionary.Values.ListValue;
                if (v != null)
                {
                    List<Values.Value> list = new List<Values.Value>();

                    foreach (DataDictionary.Values.IValue item in v.Val)
                    {
                        list.Add(convertOut(item));
                    }

                    return new Values.ListValue(list);
                }
            }

            // Handles the structure case
            {
                DataDictionary.Values.StructureValue v = value as DataDictionary.Values.StructureValue;
                if (v != null)
                {
                    Dictionary<string, Values.Value> record = new Dictionary<string, Values.Value>();

                    foreach (KeyValuePair<string, Utils.INamable> pair in v.Val)
                    {
                        DataDictionary.Variables.IVariable variable = pair.Value as DataDictionary.Variables.IVariable;
                        if (variable != null)
                        {
                            record.Add(variable.Name, convertOut(variable.Value));
                        }
                    }

                    return new Values.StructureValue(record);
                }
            }
            // Handles the 'empty' value
            {
                DataDictionary.Values.EmptyValue emptyValue = value as DataDictionary.Values.EmptyValue;
                if (emptyValue != null)
                {
                    return null;
                }
            }

            // Handles the 'empty' value
            {
                DataDictionary.Values.EmptyValue emptyValue = value as DataDictionary.Values.EmptyValue;
                if (emptyValue != null)
                {
                    return null;
                }
            }

            throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot convert value " + value.ToString()));
        }

        private class SyntheticVariableUpdateAction : DataDictionary.Rules.Action
        {
            /// <summary>
            /// The variable identification that is modified by this variable update action
            /// </summary>
            public DataDictionary.Variables.IVariable Variable { get; private set; }

            /// <summary>
            /// The value that is assigned to this variable
            /// </summary>
            public DataDictionary.Values.IValue Value { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="variable"></param>
            /// <param name="value"></param>
            public SyntheticVariableUpdateAction(DataDictionary.Variables.IVariable variable, DataDictionary.Values.IValue value)
            {
                Variable = variable;
                Value = value;
            }

            public override string ExpressionText
            {
                get
                {
                    return Variable.FullName + " <- " + Value.FullName;
                }
            }

            public override void GetChanges(DataDictionary.Interpreter.InterpretationContext context, DataDictionary.Rules.ChangeList changes, DataDictionary.Interpreter.ExplanationPart explanation, bool apply, Runner runner)
            {
                DataDictionary.Rules.Change change = new DataDictionary.Rules.Change(Variable, Variable.Value, Value);
                changes.Add(change, apply, runner);
            }
        }

        /// <summary>
        /// Sets the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void SetVariableValue(string variableName, Values.Value value)
        {
            DataDictionary.Variables.IVariable variable = Runner.EFSSystem.findByFullName(variableName) as DataDictionary.Variables.IVariable;

            if (variable != null)
            {
                SyntheticVariableUpdateAction action = new SyntheticVariableUpdateAction(variable, value.convertBack(variable.Type));
                DataDictionary.Tests.Runner.Events.VariableUpdate variableUpdate = new DataDictionary.Tests.Runner.Events.VariableUpdate(action, null, null);
                Runner.EventTimeLine.AddModelEvent(variableUpdate, Runner);
            }
            else
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot find variable " + variableName));
            }
        }

        /// <summary>
        /// Converts an interface priority to a Runner priority
        /// </summary>
        /// <param name="priority"></param>
        private DataDictionary.Generated.acceptor.RulePriority convertStep2Priority(Step priority)
        {
            DataDictionary.Generated.acceptor.RulePriority retVal = DataDictionary.Generated.acceptor.RulePriority.defaultRulePriority;

            switch (priority)
            {
                case Step.Verification:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aVerification;
                    break;

                case Step.UpdateInternal:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateINTERNAL;
                    break;

                case Step.Process:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aProcessing;
                    break;

                case Step.UpdateOutput:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateOUT;
                    break;

                case Step.CleanUp:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aCleanUp;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Converts an interface priority to a Runner priority
        /// </summary>
        /// <param name="priority"></param>
        private Step convertPriority2Step(DataDictionary.Generated.acceptor.RulePriority priority)
        {
            Step retVal = Step.Process;

            switch (priority)
            {
                case DataDictionary.Generated.acceptor.RulePriority.aUpdateINTERNAL:
                    retVal = Step.UpdateInternal;
                    break;

                case DataDictionary.Generated.acceptor.RulePriority.aVerification:
                    retVal = Step.Verification;
                    break;

                case DataDictionary.Generated.acceptor.RulePriority.aProcessing:
                    retVal = Step.Process;
                    break;

                case DataDictionary.Generated.acceptor.RulePriority.aUpdateOUT:
                    retVal = Step.UpdateOutput;
                    break;

                case DataDictionary.Generated.acceptor.RulePriority.aCleanUp:
                    retVal = Step.CleanUp;
                    break;
            }

            return retVal;
        }

        private static EFSService __instance = null;

        /// <summary>
        /// The service instance
        /// </summary>
        public static EFSService INSTANCE
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new EFSService();
                }

                return __instance;
            }
        }
    }
}
