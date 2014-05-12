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
using System.Collections.Generic;
using DataDictionary.Rules;

namespace DataDictionary.Interpreter.Statement
{
    public class ProcedureCallStatement : Statement
    {
        /// <summary>
        /// The Logger
        /// </summary>
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The designator which identifies the procedure to call
        /// </summary>
        public Call Call { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="call">The corresponding function call designator</param>
        /// <param name="parameters">The expressions used to compute the parameters</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ProcedureCallStatement(ModelElement root, ModelElement log, Call call, int start, int end)
            : base(root, log, start, end)
        {
            Call = call;
            Call.Enclosing = this;
        }

        /// <summary>
        /// Performs the semantic analysis of the statement
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(Utils.INamable instance)
        {
            bool retVal = base.SemanticAnalysis(instance);

            if (retVal)
            {
                Call.SemanticAnalysis(instance);
                StaticUsage.AddUsages(Call.StaticUsage, Usage.ModeEnum.Call);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the rules associates to this procedure call statement
        /// </summary>
        public System.Collections.ArrayList Rules
        {
            get
            {
                InterpretationContext ctxt = getContext(new InterpretationContext(Root));
                if (Call != null)
                {
                    Functions.Procedure procedure = Call.getProcedure(ctxt);
                    if (procedure != null)
                    {
                        return procedure.Rules;
                    }
                }

                return new System.Collections.ArrayList();
            }
        }

        /// <summary>
        /// Provides the list of actions performed during this procedure call
        /// </summary>
        public List<Rules.Action> Actions
        {
            get
            {
                List<Rules.Action> retVal = new List<Rules.Action>();

                foreach (Rules.Rule rule in Rules)
                {
                    foreach (Rules.RuleCondition condition in rule.RuleConditions)
                    {
                        foreach (Rules.Action action in condition.Actions)
                        {
                            retVal.Add(action);
                        }
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(Types.ITypedElement variable)
        {
            VariableUpdateStatement retVal = null;

            foreach (Rules.Action action in Actions)
            {
                retVal = action.Modifies(variable);
                if (retVal != null)
                {
                    return retVal;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
            foreach (Rules.Action action in Actions)
            {
                if (action.Statement != null)
                {
                    action.Statement.UpdateStatements(retVal);
                }
            }
        }

        /// <summary>
        /// Indicates whether this statement reads the element
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public override bool Reads(Types.ITypedElement variable)
        {
            foreach (Rules.Action action in Actions)
            {
                if (action.Reads(variable))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Provides the list of elements read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<Types.ITypedElement> retVal)
        {
            foreach (Rules.Action action in Actions)
            {
                if (action.Statement != null)
                {
                    action.Statement.ReadElements(retVal);
                }
            }
        }

        /// <summary>
        /// Provides the context on which function evaluation should be performed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private InterpretationContext getContext(InterpretationContext context)
        {
            InterpretationContext retVal = context;

            DerefExpression deref = Call.Called as DerefExpression;
            if (deref != null)
            {
                Values.IValue value = deref.GetPrefixValue(context, deref.Arguments.Count - 1) as Values.IValue;
                if (value != null)
                {
                    retVal = new InterpretationContext(context, value);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            if (Call != null)
            {
                Call.checkExpression();

                Functions.Procedure procedure = Call.Called.Ref as Functions.Procedure;
                if (procedure == null)
                {
                    if (Call.Called.Ref is Functions.Function)
                    {
                        Root.AddError("Invalid call : Function " + Call.Called + " called as a procedure");
                    }
                    else
                    {
                        Root.AddError("Cannot determine called procedure " + Call.Called);
                    }
                }
                else
                {
                    if (procedure.Enclosing is Types.Structure)
                    {
                        DerefExpression deref = Call.Called as DerefExpression;
                        if (deref != null)
                        {
                            int count = deref.Arguments.Count;
                            if ((deref.Arguments[count - 2].Ref is Types.NameSpace) || (deref.Arguments[count - 2].Ref is Types.Structure))
                            {
                                Root.AddError("Invalid procedure call : context should be the instance on which the call is performed");
                            }
                        }
                    }
                }
            }
            else
            {
                Root.AddError("Cannot parse called procedure for " + ToString());
            }
        }

        /// <summary>
        /// Provides the changes performed by this statement
        /// </summary>
        /// <param name="context">The context on which the changes should be computed</param>
        /// <param name="changes">The list to fill with the changes</param>
        /// <param name="explanation">The explanatino to fill, if any</param>
        /// <param name="apply">Indicates that the changes should be applied immediately</param>
        /// <param name="runner"></param>
        public override void GetChanges(InterpretationContext context, ChangeList changes, ExplanationPart explanation, bool apply, Tests.Runner.Runner runner)
        {
            if (Call != null)
            {
                InterpretationContext ctxt = getContext(context);
                Functions.Procedure procedure = Call.getProcedure(ctxt);
                if (procedure != null)
                {
                    // If the procedure has been defined in a structure, 
                    // ensure that it is applied to an instance of that structure
                    Types.Structure structure = procedure.Enclosing as Types.Structure;
                    if (structure != null)
                    {
                        Types.ITypedElement current = ctxt.Instance as Types.ITypedElement;
                        while (current != null)
                        {
                            if (current.Type != structure)
                            {
                                Utils.IEnclosed enclosed = current as Utils.IEnclosed;
                                if (enclosed != null)
                                {
                                    current = enclosed.Enclosing as Types.ITypedElement;
                                }
                                else
                                {
                                    current = null;
                                }
                            }
                            else
                            {
                                ctxt.Instance = current;
                                current = null;
                            }
                        }
                    }

                    ExplanationPart part = null;
                    if (explanation != null)
                    {
                        part = new ExplanationPart(Root, procedure.FullName);
                        explanation.SubExplanations.Add(part);
                    }

                    int token = ctxt.LocalScope.PushContext();
                    foreach (KeyValuePair<Variables.Actual, Values.IValue> pair in Call.AssignParameterValues(context, procedure, true))
                    {
                        ctxt.LocalScope.setVariable(pair.Key, pair.Value);
                    }

                    foreach (Rules.Rule rule in Rules)
                    {
                        ApplyRule(rule, changes, ctxt, part, runner);
                    }

                    ctxt.LocalScope.PopContext(token);
                }
                else
                {
                    AddError("Cannot determine the called procedure for " + ToString());
                }
            }
            else
            {
                AddError("Expression " + ToString() + " is not a valid procedure call");
            }
        }

        /// <summary>
        /// Applies a rule defined in a procedure
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="changes"></param>
        /// <param name="ctxt"></param>
        /// <param name="explanation"></param>
        /// <param name="runner"></param>
        private void ApplyRule(Rules.Rule rule, ChangeList changes, InterpretationContext ctxt, ExplanationPart explanation, Tests.Runner.Runner runner)
        {
            foreach (Rules.RuleCondition condition in rule.RuleConditions)
            {
                ExplanationPart conditionExplanation = null;
                if (explanation != null)
                {
                    conditionExplanation = new ExplanationPart(condition, condition.Name);
                    explanation.SubExplanations.Add(conditionExplanation);
                }

                if (condition.EvaluatePreConditions(ctxt, conditionExplanation, runner))
                {
                    if (conditionExplanation != null)
                    {
                        conditionExplanation.Message = "SATISIFIED " + rule.Name + "." + condition.Name;
                    }
                    if (runner.LogEvents)
                    {
                        Log.Info("SATISIFIED " + rule.Name + "." + condition.Name);
                    }
                    foreach (Rules.Action action in condition.Actions)
                    {
                        action.GetChanges(ctxt, changes, conditionExplanation, true, runner);
                    }

                    foreach (Rules.Rule subRule in condition.SubRules)
                    {
                        ApplyRule(subRule, changes, ctxt, conditionExplanation, runner);
                    }
                    break;
                }
                else
                {
                    if (conditionExplanation != null)
                    {
                        conditionExplanation.Message = "FAILED " + rule.Name + "." + condition.Name;
                    }
                }
            }
        }

        public override string ToString()
        {
            return Call.ToString();
        }

        /// <summary>
        /// Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public override string ShortShortDescription()
        {
            return Call.Called.Name;
        }

        /// <summary>
        /// Provides the usage description done by this statement
        /// </summary>
        /// <returns></returns>
        public override ModeEnum UsageDescription()
        {
            ModeEnum retVal = ModeEnum.Call;

            return retVal;
        }

        /// <summary>
        /// Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public override ModelElement AffectedElement()
        {
            return Call.Called.Ref as ModelElement;
        }
    }
}
