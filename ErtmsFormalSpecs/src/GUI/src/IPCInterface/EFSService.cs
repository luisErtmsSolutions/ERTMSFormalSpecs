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

    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true)]
    public class EFSService : IEFSService
    {
        /// <summary>
        /// Indicates that the explain view should be updated according to the scenario execution
        /// </summary>
        public bool Explain { get; set; }

        /// <summary>
        /// Indicates that the events should be logged
        /// </summary>
        public bool LogEvents { get; set; }

        /// <summary>
        /// The duration (in ms) of an execution cycle
        /// </summary>
        public int CycleDuration { get; set; }

        /// <summary>
        /// The number of events that should be kept in memory
        /// </summary>
        public int KeepEventCount { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EFSService()
        {
            Explain = true;
            LogEvents = false;
            CycleDuration = 100;
            KeepEventCount = 10000;
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
                    Restart();
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

            throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot convert value " + value.ToString()));
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
                variable.Value = value.convertBack(variable.Type);
            }
            else
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot find variable " + variableName));
            }
        }

        /// <summary>
        /// Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="priority"></param>
        public void Cycle(Priority priority)
        {
            Runner.ExecuteOnePriority(convertPriority(priority));

            if (priority == Priority.CleanUp)
            {
                GUIUtils.MDIWindow.Invoke((MethodInvoker)delegate { GUIUtils.MDIWindow.RefreshAfterStep(); });
            }
        }

        /// <summary>
        /// Converts an interface priority to a Runner priority
        /// </summary>
        /// <param name="priority"></param>
        private DataDictionary.Generated.acceptor.RulePriority convertPriority(Priority priority)
        {
            DataDictionary.Generated.acceptor.RulePriority retVal = DataDictionary.Generated.acceptor.RulePriority.defaultRulePriority;

            switch (priority)
            {
                case Priority.Verification:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aVerification;
                    break;

                case Priority.UpdateInternal:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateINTERNAL;
                    break;

                case Priority.Process:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aProcessing;
                    break;

                case Priority.UpdateOutput:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateOUT;
                    break;

                case Priority.CleanUp:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aCleanUp;
                    break;
            }

            return retVal;
        }
    }
}
