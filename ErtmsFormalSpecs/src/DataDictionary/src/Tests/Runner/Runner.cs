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
using System;
using System.Collections.Generic;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner.Events;
using DataDictionary.Values;

namespace DataDictionary.Tests.Runner
{
    public class Runner
    {
        /// <summary>
        /// The Logger
        /// </summary>
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The event time line for this runner
        /// </summary>
        public Events.EventTimeLine EventTimeLine { get; private set; }

        /// <summary>
        /// Indicates whether events should be logged using log4net
        /// </summary>
        public bool LogEvents { get; set; }

        /// <summary>
        /// Indicates whether an explanation should be provided to all actions
        /// </summary>
        public bool Explain { get; set; }

        /// <summary>
        /// The data dictionary
        /// </summary>
        public virtual EFSSystem EFSSystem
        {
            get
            {
                return EFSSystem.INSTANCE;
            }
        }

        /// <summary>
        /// The test case for which this runner has been created
        /// </summary>
        public SubSequence SubSequence { get; private set; }

        /// <summary>
        /// The step between two activations
        /// </summary>
        private double step = 0.1;
        public double Step
        {
            get { return step; }
            set { step = value; }
        }

        /// <summary>
        /// The current time
        /// </summary>
        public double Time
        {
            get { return EventTimeLine.CurrentTime; }
            set { EventTimeLine.CurrentTime = value; }
        }

        /// <summary>
        /// The current time
        /// </summary>
        private double lastActivationTime;
        public double LastActivationTime
        {
            get { return lastActivationTime; }
            set { lastActivationTime = value; }
        }

        /// <summary>
        /// Visitor used to clean caches of functions (graphs, surfaces)
        /// </summary>
        protected class FunctionGraphCache : Generated.Visitor
        {
            /// <summary>
            /// The list of functions to be cleared
            /// </summary>
            public List<Functions.Function> CachedFunctions = new List<Functions.Function>();

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="system"></param>
            public FunctionGraphCache(EFSSystem system)
            {
                // Fill the list of functions to be cleared
                foreach (DataDictionary.Dictionary dictionnary in EFSSystem.INSTANCE.Dictionaries)
                {
                    visit(dictionnary);
                }
            }

            /// <summary>
            /// Fills the list of functions to be cleared
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.Function obj, bool visitSubNodes)
            {
                Functions.Function function = obj as Functions.Function;

                if (function != null)
                {
                    if (function.getCacheable())
                    {
                        CachedFunctions.Add(function);
                    }
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            /// Clears the caches of all functions
            /// </summary>
            public void ClearCaches()
            {
                foreach (Functions.Function function in CachedFunctions)
                {
                    function.ClearCache();
                }
            }
        }

        /// <summary>
        /// The function cache cleaner
        /// </summary>
        protected FunctionGraphCache FunctionCacheCleaner { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subSequence"></param>
        /// <param name="explain"></param>
        /// <param name="logEvents">Indicates whether events should be logged</param>
        public Runner(SubSequence subSequence, bool explain, bool logEvents)
        {
            EventTimeLine = new Events.EventTimeLine();
            SubSequence = subSequence;
            EFSSystem.Runner = this;
            LogEvents = logEvents;
            Explain = explain;

            // Compile everything
            EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
            EFSSystem.ShouldRebuild = false;

            Setup();
        }

        /// <summary>
        /// A simple runner
        /// </summary>
        public Runner(bool explain, bool logEvents, int step = 100, int storeEventCount = 0)
        {
            EventTimeLine = new Events.EventTimeLine();
            SubSequence = null;
            Step = 100;
            EventTimeLine.MaxNumberOfEvents = storeEventCount;
            EFSSystem.Runner = this;
            LogEvents = logEvents;

            // Compile everything
            EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
            EFSSystem.ShouldRebuild = false;

            Setup();
        }

        /// <summary>
        /// Sets up all variables before any execution on the system
        /// </summary>
        private class Setuper : Generated.Visitor
        {
            /// <summary>
            /// The EFS system for which this setuper is created
            /// </summary>
            public EFSSystem EFSSystem { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="efsSystem"></param>
            public Setuper(EFSSystem efsSystem)
            {
                EFSSystem = efsSystem;
            }

            /// <summary>
            /// Sets the default values to each variable
            /// </summary>
            /// <param name="variable">The variable to set</param>
            /// <param name="subNodes">Indicates whether sub nodes should be considered</param>
            public override void visit(DataDictionary.Generated.Variable variable, bool subNodes)
            {
                DataDictionary.Variables.Variable var = (DataDictionary.Variables.Variable)variable;

                var.Value = var.DefaultValue;

                base.visit(variable, subNodes);
            }

            /// <summary>
            /// Indicates which rules are not active
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.Rule obj, bool visitSubNodes)
            {
                Rules.Rule rule = obj as Rule;
                if (rule != null)
                {
                    rule.ActivationPriorities = null;
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        /// Sets up the runner before performing a test case
        /// </summary>
        public void Setup()
        {
            try
            {
                Generated.ControllersManager.DesactivateAllNotifications();
                // Setup the execution environment
                Setuper setuper = new Setuper(EFSSystem);
                foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    setuper.visit(dictionary);
                }

                // Clears all caches
                Utils.FinderRepository.INSTANCE.ClearCache();

                // Builds the list of functions that will require a cache for their graph 
                FunctionCacheCleaner = new FunctionGraphCache(EFSSystem);

                // Setup the step
                if (SubSequence != null)
                {
                    Expression expression = SubSequence.Frame.CycleDuration;
                    Values.IValue value = expression.GetValue(new InterpretationContext(SubSequence.Frame));
                    Step = Functions.Function.getDoubleValue(value);
                }
            }
            finally
            {
                Generated.ControllersManager.ActivateAllNotifications();
            }
        }

        public class Activation
        {
            /// <summary>
            /// The action to activate
            /// </summary>
            private Rules.RuleCondition ruleCondition;
            public Rules.RuleCondition RuleCondition
            {
                get { return ruleCondition; }
                private set { ruleCondition = value; }
            }

            /// <summary>
            /// The instance on which the action is applied
            /// </summary>
            private Utils.IModelElement instance;
            public Utils.IModelElement Instance
            {
                get { return instance; }
                private set { instance = value; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="ruleCondition">The rule condition which leads to this activation</param>
            /// <param name="instance">The instance on which this rule condition's preconditions are evaluated to true</param>
            public Activation(Rules.RuleCondition ruleCondition, Utils.IModelElement instance)
            {
                RuleCondition = ruleCondition;
                Instance = instance;
            }

            /// <summary>
            /// Indicates that two Activations are the same when they share the action and, 
            /// if specified, the instance on which they are applied
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                bool retVal = false;

                Activation other = obj as Activation;
                if (other != null)
                {
                    retVal = RuleCondition.Equals(other.RuleCondition);
                    if (retVal && Instance != null)
                    {
                        if (other.Instance != null)
                        {
                            retVal = retVal && Instance.Equals(other.Instance);
                        }
                        else
                        {
                            retVal = false;
                        }
                    }
                }
                return retVal;
            }

            /// <summary>
            /// The hash code, according to Equal operator.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                int retVal = RuleCondition.GetHashCode();

                if (Instance != null)
                {
                    retVal = retVal + Instance.GetHashCode();
                }

                return retVal;
            }

            /// <summary>
            /// Registers the actions to be activated, as an activation.
            /// </summary>
            /// <param name="activations"></param>
            /// <param name="actions"></param>
            public static void RegisterRules(HashSet<Activation> activations, List<Rules.RuleCondition> ruleConditions, Utils.IModelElement instance)
            {
                foreach (Rules.RuleCondition ruleCondition in ruleConditions)
                {
                    activations.Add(new Activation(ruleCondition, instance));
                }
            }
        }

        /// <summary>
        /// Provides the order in which rules should be activated
        /// </summary>
        public static Generated.acceptor.RulePriority[] PRIORITIES_ORDER = 
        { 
            Generated.acceptor.RulePriority.aVerification,
            Generated.acceptor.RulePriority.aUpdateINTERNAL,
            Generated.acceptor.RulePriority.aProcessing,
            Generated.acceptor.RulePriority.aUpdateOUT,
            Generated.acceptor.RulePriority.aCleanUp,
        };

        /// <summary>
        /// The current priority
        /// </summary>
        public Generated.acceptor.RulePriority? CurrentPriority { get; private set; }

        /// <summary>
        /// Activates the rules in the dictionary until stabilisation
        /// </summary>
        public void Cycle()
        {
            try
            {
                CurrentPriority = null;

                if (LogEvents)
                {
                    Log.Info("New cycle");
                }
                DataDictionary.Generated.ControllersManager.DesactivateAllNotifications();

                LastActivationTime = Time;

                Utils.ModelElement.Errors = new Dictionary<Utils.ModelElement, List<Utils.ElementLog>>();

                foreach (Generated.acceptor.RulePriority priority in PRIORITIES_ORDER)
                {
                    innerExecuteOnePriority(priority);
                }
                CurrentPriority = null;
                // Clears the cache of functions
                FunctionCacheCleaner.ClearCaches();

                foreach (KeyValuePair<Utils.ModelElement, List<Utils.ElementLog>> pair in Utils.ModelElement.Errors)
                {
                    foreach (Utils.ElementLog log in pair.Value)
                    {
                        switch (log.Level)
                        {
                            case Utils.ElementLog.LevelEnum.Error:
                                ModelInterpretationFailure modelInterpretationFailure = new ModelInterpretationFailure(log, pair.Key as Utils.INamable, null);
                                ModelElement modelElement = pair.Key as ModelElement;
                                if (modelElement != null)
                                {
                                    modelInterpretationFailure.Explanation = modelElement.Explain;
                                }
                                EventTimeLine.AddModelEvent(modelInterpretationFailure, this);
                                break;

                            case Utils.ElementLog.LevelEnum.Warning:
                                break;
                            case Utils.ElementLog.LevelEnum.Info:
                                break;
                        }
                    }
                }

                EventTimeLine.GarbageCollect();
            }
            finally
            {
                DataDictionary.Generated.ControllersManager.ActivateAllNotifications();
            }

            EventTimeLine.CurrentTime += Step;
        }

        /// <summary>
        /// Executes a single rule priority (shared version of the method)
        /// </summary>
        /// <param name="priority"></param>
        private void innerExecuteOnePriority(Generated.acceptor.RulePriority priority)
        {
            CurrentPriority = priority;
            if (LogEvents)
            {
                Log.Info("Priority=" + priority);
            }
            // Clears the cache of functions
            FunctionCacheCleaner.ClearCaches();

            // Activates the processing engine
            HashSet<Activation> activations = new HashSet<Activation>();
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                foreach (DataDictionary.Types.NameSpace nameSpace in dictionary.NameSpaces)
                {
                    SetupNameSpaceActivations(priority, activations, nameSpace, null, this);
                }
            }

            ApplyActivations(activations, priority);
            CheckExpectationsState(priority);
        }
        /// <summary>
        /// Executes the interpretation machine for one priority
        /// </summary>
        /// <param name="priority"></param>
        public void ExecuteOnePriority(DataDictionary.Generated.acceptor.RulePriority priority)
        {
            try
            {
                DataDictionary.Generated.ControllersManager.NamableController.DesactivateNotification();
                LastActivationTime = Time;

                Utils.ModelElement.Errors = new Dictionary<Utils.ModelElement, List<Utils.ElementLog>>();

                // Clears the cache of functions
                FunctionCacheCleaner.ClearCaches();

                // Executes a single rule priority
                innerExecuteOnePriority(priority);

                // Clears the cache of functions
                FunctionCacheCleaner.ClearCaches();
                EventTimeLine.GarbageCollect();
            }
            finally
            {
                DataDictionary.Generated.ControllersManager.NamableController.ActivateNotification();
            }

            if (priority == Generated.acceptor.RulePriority.aCleanUp)
            {
                EventTimeLine.CurrentTime += Step;
            }
        }

        /// <summary>
        /// Determines the set of rules in a specific namespace to be applied.
        /// </summary>
        /// <param name="priority">The priority for which this activation is requested</param>
        /// <param name="activations">The set of activations to be filled</param>
        /// <param name="nameSpace">The namespace to consider</param>
        /// <param name="explanation">The explanation part to be filled</param>
        /// <param name="runner"></param>
        /// <returns></returns>
        protected void SetupNameSpaceActivations(Generated.acceptor.RulePriority priority, HashSet<Activation> activations, Types.NameSpace nameSpace, ExplanationPart explanation, Tests.Runner.Runner runner)
        {
            // Finds all activations in sub namespaces
            foreach (Types.NameSpace subNameSpace in nameSpace.NameSpaces)
            {
                SetupNameSpaceActivations(priority, activations, subNameSpace, explanation, runner);
            }

            List<Rules.RuleCondition> rules = new List<Rules.RuleCondition>();
            foreach (Rule rule in nameSpace.Rules)
            {
                rules.Clear();
                rule.Evaluate(this, priority, rule, rules, explanation);
                Activation.RegisterRules(activations, rules, nameSpace);
            }

            // BUG : This is irrelevant now. Check that and remove
            foreach (Functions.Procedure procedure in nameSpace.Procedures)
            {
                rules.Clear();
                Activation.RegisterRules(activations, rules, procedure);
            }

            foreach (Variables.IVariable variable in nameSpace.Variables)
            {
                EvaluateVariable(priority, activations, variable, explanation, runner);
            }
        }

        /// <summary>
        /// Evaluates the rules associated to a single variable
        /// </summary>
        /// <param name="priority">The priority in which this variable is evaluated</param>
        /// <param name="activations">The activation list result of this evaluation</param>
        /// <param name="variable">The variable to evaluate</param>
        /// <param name="explanation">The explanation part to be filled</param>
        /// <param name="runner"></param>
        private void EvaluateVariable(Generated.acceptor.RulePriority priority, HashSet<Activation> activations, Variables.IVariable variable, ExplanationPart explanation, Tests.Runner.Runner runner)
        {
            if (variable != null)
            {
                if (variable.Type is Types.Structure)
                {
                    List<Rules.RuleCondition> rules = new List<RuleCondition>();
                    Types.Structure structure = variable.Type as Types.Structure;
                    foreach (Rule rule in structure.Rules)
                    {
                        rule.Evaluate(this, priority, variable, rules, explanation);
                    }
                    Activation.RegisterRules(activations, rules, variable);

                    StructureValue value = variable.Value as StructureValue;
                    if (value != null)
                    {
                        foreach (Variables.IVariable subVariable in value.SubVariables.Values)
                        {
                            EvaluateVariable(priority, activations, subVariable, explanation, runner);
                        }
                    }
                }
                else if (variable.Type is Types.StateMachine)
                {
                    List<Rules.RuleCondition> rules = new List<RuleCondition>();
                    EvaluateStateMachine(rules, priority, variable, explanation, runner);
                    Activation.RegisterRules(activations, rules, variable);
                }
                else if (variable.Type is Types.Collection)
                {
                    Types.Collection collectionType = variable.Type as Types.Collection;
                    if (variable.Value != EFSSystem.EmptyValue)
                    {
                        ListValue val = variable.Value as ListValue;

                        if (val != null)
                        {
                            int i = 1;
                            foreach (IValue subVal in val.Val)
                            {
                                Variables.Variable tmp = new Variables.Variable();
                                tmp.Name = variable.Name + '[' + i + ']';
                                tmp.Type = collectionType.Type;
                                tmp.Value = subVal;
                                EvaluateVariable(priority, activations, tmp, explanation, runner);
                                i = i + 1;
                            }
                        }
                        else
                        {
                            ModelElement element = variable as ModelElement;
                            if (element != null)
                            {
                                element.AddError("Variable " + variable.Name + " does not hold a collection but " + variable.Value);
                            }
                            else
                            {
                                throw new System.Exception("Variable " + variable.Name + " does not hold a collection but " + variable.Value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Try to find a rule, in this state machine, or in a sub state machine 
        /// which 
        /// </summary>
        /// <param name="ruleConditions">The rule conditions activated during the evaluation of this state machine</param>
        /// <param name="priority">The priority when this evaluation occurs</param>
        /// <param name="currentStateVariable">The variable which holds the current state of the procedure</param>
        /// <param name="explanation">The explanation part to be filled</param>
        /// <param name="runner"></param>
        private void EvaluateStateMachine(List<Rules.RuleCondition> ruleConditions, Generated.acceptor.RulePriority priority, Variables.IVariable currentStateVariable, ExplanationPart explanation, Tests.Runner.Runner runner)
        {
            if (currentStateVariable != null)
            {
                Constants.State currentState = currentStateVariable.Value as Constants.State;
                Types.StateMachine currentStateMachine = currentState.StateMachine;
                while (currentStateMachine != null)
                {
                    foreach (Rule rule in currentStateMachine.Rules)
                    {
                        rule.Evaluate(this, priority, currentStateVariable, ruleConditions, explanation);
                    }
                    currentStateMachine = currentStateMachine.EnclosingStateMachine;
                }
            }
        }

        /// <summary>
        /// Applies the selected actions and update the system state
        /// </summary>
        /// <param name="updates"></param>
        public void ApplyActivations(HashSet<Activation> activations, Generated.acceptor.RulePriority priority)
        {
            foreach (Activation activation in activations)
            {
                if (LogEvents)
                {
                    Log.Info("Activating " + activation.RuleCondition.FullName);
                }
                if (activation.RuleCondition.Actions.Count > 0)
                {
                    // Register the fact that a rule has been triggered
                    Events.RuleFired ruleFired = new Events.RuleFired(activation.RuleCondition, priority);
                    EventTimeLine.AddModelEvent(ruleFired, this);

                    // Registers all model updates due to this rule triggering
                    foreach (Rules.Action action in activation.RuleCondition.Actions)
                    {
                        if (action.Statement != null)
                        {
                            Events.VariableUpdate variableUpdate = new Events.VariableUpdate(action, activation.Instance, priority);
                            EventTimeLine.AddModelEvent(variableUpdate, this);
                            ruleFired.AddVariableUpdate(variableUpdate);
                        }
                        else
                        {
                            action.AddError("Cannot parse action statement");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setups the sub-step by applying its actions and adding its expects in the expect list
        /// </summary>
        public void SetupSubStep(SubStep subStep)
        {
            try
            {
                DataDictionary.Generated.ControllersManager.DesactivateAllNotifications();
                LogInstance = subStep;

                // No setup can occur when some expectations are still active
                if (ActiveBlockingExpectations().Count == 0)
                {
                    EventTimeLine.AddModelEvent(new SubStepActivated(subStep, CurrentPriority), this);
                }
            }
            finally
            {
                DataDictionary.Generated.ControllersManager.ActivateAllNotifications();
            }
        }

        /// <summary>
        /// Provides the still active expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<Expect> ActiveExpectations()
        {
            return new HashSet<Expect>(EventTimeLine.ActiveExpectations);
        }

        /// <summary>
        /// Provides the still active and blocking expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<Expect> ActiveBlockingExpectations()
        {
            return EventTimeLine.ActiveBlockingExpectations();
        }

        /// <summary>
        /// Provides the failed expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<ModelEvent> FailedExpectations()
        {
            return EventTimeLine.FailedExpectations();
        }

        /// <summary>
        /// Updates the expectation state according to the variables' value
        /// </summary>
        /// <param name="priority">The priority for which this check is performed</param>
        private void CheckExpectationsState(Generated.acceptor.RulePriority priority)
        {
            // Update the state of the expectation according to system's state
            foreach (Events.Expect expect in ActiveExpectations())
            {
                Expectation expectation = expect.Expectation;

                // Determine if the deadline is reached
                if (expect.TimeOut < EventTimeLine.CurrentTime)
                {
                    switch (expect.Expectation.getKind())
                    {
                        case Generated.acceptor.ExpectationKind.aInstantaneous:
                        case Generated.acceptor.ExpectationKind.defaultExpectationKind:
                            // Instantaneous expectation who raised its deadling
                            EventTimeLine.AddModelEvent(new FailedExpectation(expect, CurrentPriority), this);
                            break;

                        case Generated.acceptor.ExpectationKind.aContinuous:
                            // Continuous expectation who raised its deadline
                            EventTimeLine.AddModelEvent(new ExpectationReached(expect, CurrentPriority), this);
                            break;
                    }
                }
                else
                {
                    try
                    {
                        if (expectation.getCyclePhase() == Generated.acceptor.RulePriority.defaultRulePriority || expectation.getCyclePhase() == priority)
                        {
                            switch (expectation.getKind())
                            {
                                case Generated.acceptor.ExpectationKind.aInstantaneous:
                                case Generated.acceptor.ExpectationKind.defaultExpectationKind:
                                    if (getBoolValue(expectation, expectation.Expression))
                                    {
                                        // An instantaneous expectation who reached its satisfactory condition
                                        EventTimeLine.AddModelEvent(new ExpectationReached(expect, priority), this);
                                    }
                                    break;

                                case Generated.acceptor.ExpectationKind.aContinuous:
                                    if (expectation.getCondition() != null)
                                    {
                                        if (!getBoolValue(expectation, expectation.ConditionTree))
                                        {
                                            // An continuous expectation who reached its satisfactory condition
                                            EventTimeLine.AddModelEvent(new ExpectationReached(expect, priority), this);
                                        }
                                        else
                                        {
                                            if (!getBoolValue(expectation, expectation.Expression))
                                            {
                                                // A continuous expectation who reached a case where it is not satisfied
                                                EventTimeLine.AddModelEvent(new FailedExpectation(expect, priority), this);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!getBoolValue(expectation, expectation.Expression))
                                        {
                                            // A continuous expectation who reached a case where it is not satisfied
                                            EventTimeLine.AddModelEvent(new FailedExpectation(expect, priority), this);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        expect.Expectation.AddException(e);
                    }
                }
            }
        }

        /// <summary>
        /// Provides the value of the expression provided
        /// </summary>
        /// <param name="expect"></param>
        /// <returns></returns>
        private bool getBoolValue(ModelElement instance, Expression expression)
        {
            bool retVal = false;

            Interpreter.InterpretationContext context = new Interpreter.InterpretationContext(instance);
            BoolValue val = expression.GetValue(context) as BoolValue;

            if (val != null)
            {
                retVal = val.Val;
            }
            else
            {
                throw new Exception("Cannot evaluate value of " + expression);
            }

            return retVal;
        }

        /// <summary>
        /// Runs until all expectations are reached or failed
        /// </summary>
        public void RunForBlockingExpectations(bool performCycle)
        {
            if (performCycle)
            {
                Cycle();
            }

            while (ActiveBlockingExpectations().Count > 0)
            {
                Cycle();
            }
        }

        /// <summary>
        /// Runs until all expectations are reached or failed
        /// </summary>
        public void RunForExpectations(bool performCycle)
        {
            if (performCycle)
            {
                Cycle();
            }

            while (ActiveExpectations().Count > 0)
            {
                Cycle();
            }
        }

        /// <summary>
        /// Indicates that no test has been run yet
        /// </summary>
        private static int TEST_NOT_RUN = -1;

        /// <summary>
        /// Indicates that the current test case & current step & current sub-step must be rebuilt from the time line
        /// </summary>
        private static int REBUILD_CURRENT_SUB_STEP = -2;

        /// <summary>
        /// Indicates that the current test case & current step & current sub-step must be rebuilt from the time line
        /// </summary>
        private static int NO_MORE_STEP = -3;

        /// <summary>
        /// The index of the last activated sub-step in the current test case
        /// </summary>
        private int currentSubStepIndex = TEST_NOT_RUN;

        /// <summary>
        /// The index of the last activated step in the current test case
        /// </summary>
        private int currentStepIndex = TEST_NOT_RUN;

        /// <summary>
        /// The index of the test case in which the last activated step belongs
        /// </summary>
        private int currentTestCaseIndex = TEST_NOT_RUN;

        /// <summary>
        /// Provides the next test case
        /// </summary>
        /// <returns></returns>
        public TestCase CurrentTestCase()
        {
            TestCase retVal = null;

            if (SubSequence != null && currentTestCaseIndex != NO_MORE_STEP)
            {
                if (currentTestCaseIndex >= 0 && currentTestCaseIndex < SubSequence.TestCases.Count)
                {
                    retVal = (TestCase)SubSequence.TestCases[currentTestCaseIndex];
                }
            }

            return retVal;
        }

        /// <summary>
        /// steps to the next test case
        /// </summary>
        private void NextTestCase()
        {
            if (currentTestCaseIndex != NO_MORE_STEP)
            {
                if (currentTestCaseIndex == REBUILD_CURRENT_SUB_STEP)
                {
                    currentStepIndex = REBUILD_CURRENT_SUB_STEP;
                    NextStep();
                }
                else
                {
                    currentTestCaseIndex += 1;
                    TestCase testCase = CurrentTestCase();
                    while (testCase != null && testCase.Steps.Count == 0 && currentTestCaseIndex < SubSequence.TestCases.Count)
                    {
                        currentTestCaseIndex += 1;
                        testCase = CurrentTestCase();
                    }

                    if (testCase == null)
                    {
                        currentTestCaseIndex = NO_MORE_STEP;
                        currentStepIndex = NO_MORE_STEP;
                    }
                }
            }
        }

        /// <summary>
        /// Provides the current test step
        /// </summary>
        /// <returns></returns>
        public Step CurrentStep()
        {
            Step retVal = null;

            if (currentStepIndex != NO_MORE_STEP)
            {

                TestCase testCase = CurrentTestCase();
                if (testCase != null)
                {
                    if (currentStepIndex >= 0 && currentStepIndex < testCase.Steps.Count)
                    {
                        retVal = (Step)testCase.Steps[currentStepIndex];
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Steps to the next step (either in the current test case, or in the next test case)
        /// </summary>
        private void NextStep()
        {
            if (currentStepIndex != NO_MORE_STEP)
            {
                Step step = CurrentStep();

                do
                {
                    if (currentStepIndex != REBUILD_CURRENT_SUB_STEP)
                    {
                        currentStepIndex += 1;
                        TestCase testCase = CurrentTestCase();
                        if (testCase == null)
                        {
                            NextTestCase();
                            testCase = CurrentTestCase();
                        }

                        if (testCase != null && currentStepIndex >= testCase.Steps.Count)
                        {
                            NextTestCase();
                            testCase = CurrentTestCase();
                            if (testCase != null)
                            {
                                currentStepIndex = 0;
                            }
                            else
                            {
                                currentTestCaseIndex = NO_MORE_STEP;
                                currentStepIndex = NO_MORE_STEP;
                            }
                        }
                        step = CurrentStep();
                    }
                }
                while (step != null && step.IsEmpty());
            }
        }

        /// <summary>
        /// Provides the current test step
        /// </summary>
        /// <returns></returns>
        public SubStep CurrentSubStep()
        {
            SubStep retVal = null;

            if (currentSubStepIndex != NO_MORE_STEP)
            {
                if (currentSubStepIndex == REBUILD_CURRENT_SUB_STEP)
                {
                    currentTestCaseIndex = -1;
                    currentStepIndex = -1;
                    currentSubStepIndex = -1;
                    int previousTestCaseIndex = currentTestCaseIndex;
                    int previousStepIndex = currentStepIndex;
                    int previousSubStepIndex = currentSubStepIndex;

                    NextSubStep();
                    retVal = CurrentSubStep();
                    while (retVal != null && EventTimeLine.SubStepActivationCache.ContainsKey(retVal))
                    {
                        previousTestCaseIndex = currentTestCaseIndex;
                        previousStepIndex = currentStepIndex;
                        previousSubStepIndex = currentSubStepIndex;

                        NextSubStep();
                        retVal = CurrentSubStep();
                    }

                    currentTestCaseIndex = previousTestCaseIndex;
                    currentStepIndex = previousStepIndex;
                    currentSubStepIndex = previousSubStepIndex;
                }

                Step step = CurrentStep();
                if (step != null)
                {
                    if (currentSubStepIndex >= 0 && currentSubStepIndex < step.SubSteps.Count)
                    {
                        retVal = (SubStep)step.SubSteps[currentSubStepIndex];
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Steps to the next sub-step (either in the current test case, or in the next test case)
        /// </summary>
        private void NextSubStep()
        {
            if (currentSubStepIndex != NO_MORE_STEP)
            {
                SubStep subStep = CurrentSubStep();
                Step step;

                do
                {
                    currentSubStepIndex++;
                    step = CurrentStep();
                    if (step == null)
                    {
                        NextStep();
                        step = CurrentStep();
                    }

                    if (step != null && currentSubStepIndex >= step.SubSteps.Count)
                    {
                        NextStep();
                        step = CurrentStep();
                        if (step != null)
                        {
                            currentSubStepIndex = 0;
                        }
                        else
                        {
                            currentTestCaseIndex = NO_MORE_STEP;
                            currentStepIndex = NO_MORE_STEP;
                        }
                    }
                    subStep = CurrentSubStep();
                }
                while (step != null && (step.IsEmpty() || subStep.IsEmpty()));
            }
        }

        /// <summary>
        /// Runs the test case until the step provided is encountered
        /// This does not execute the corresponding step. 
        /// </summary>
        /// <param name="Item"></param>
        public void RunUntilStep(Step target)
        {
            currentStepIndex = NO_MORE_STEP;
            currentTestCaseIndex = NO_MORE_STEP;

            if (target != null)
            {
                RunForBlockingExpectations(false);
            }
            else
            {
                RunForExpectations(false);
            }

            // Run all following steps until the target step is encountered
            foreach (TestCase testCase in SubSequence.TestCases)
            {
                foreach (Step step in testCase.Steps)
                {
                    if (step == target)
                    {
                        currentStepIndex = REBUILD_CURRENT_SUB_STEP;
                        currentTestCaseIndex = REBUILD_CURRENT_SUB_STEP;
                        break;
                    }

                    if (!EventTimeLine.ContainsStep(step))
                    {
                        foreach (SubStep subStep in step.SubSteps)
                        {
                            SetupSubStep(subStep);
                            if (!subStep.getSkipEngine())
                            {
                                if (target != null)
                                {
                                    RunForBlockingExpectations(true);
                                }
                                else
                                {
                                    RunForExpectations(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs the test case until the step provided is encountered
        /// This does not execute the corresponding step. 
        /// </summary>
        /// <param name="Item"></param>
        public void RunUntilTime(double targetTime)
        {
            while (EventTimeLine.CurrentTime < targetTime)
            {
                SubStep subStep = null;
                if (ActiveBlockingExpectations().Count == 0)
                {
                    NextSubStep();
                    subStep = CurrentSubStep();
                    if (subStep != null)
                    {
                        SetupSubStep(subStep);
                    }
                }

                if (subStep == null || !subStep.getSkipEngine())
                {
                    Cycle();
                }
                else
                {
                    EventTimeLine.CurrentTime += Step;
                }
            }
        }

        /// <summary>
        /// Steps one step backward in this run
        /// </summary>
        public void StepBack()
        {
            FunctionCacheCleaner.ClearCaches();
            EventTimeLine.StepBack(step);
            currentSubStepIndex = REBUILD_CURRENT_SUB_STEP;
            currentStepIndex = REBUILD_CURRENT_SUB_STEP;
            currentTestCaseIndex = REBUILD_CURRENT_SUB_STEP;
        }

        /// <summary>
        /// Indicates whether a rule condition has been activated at a given time
        /// </summary>
        /// <param name="ruleCondition">The rule condition that should be activated</param>
        /// <param name="time">the time when the rule condition should be activated</param>
        /// <param name="variable">The variable impacted by this rule condition, if any</param>
        /// <returns>true if the corresponding rule condition has been activated at the time provided</returns>
        public bool RuleActivatedAtTime(DataDictionary.Rules.RuleCondition ruleCondition, double time, Variables.IVariable variable)
        {
            return EventTimeLine.RuleActivatedAtTime(ruleCondition, time, variable);
        }

        /// <summary>
        /// Provides the log instance, an object on which logging should be performed
        /// </summary>
        public ModelElement LogInstance { get; set; }

        /// <summary>
        /// Terminates the execution of a run
        /// </summary>
        public void EndExecution()
        {
            FunctionCacheCleaner.ClearCaches();
        }
    }
}
