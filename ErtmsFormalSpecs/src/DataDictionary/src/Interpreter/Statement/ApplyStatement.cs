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
namespace DataDictionary.Interpreter.Statement
{
    using System.Collections.Generic;
    using DataDictionary.Rules;
    using Interpreter.Filter;

    public class ApplyStatement : Statement, Utils.ISubDeclarator
    {
        /// <summary>
        /// The procedure to call
        /// </summary>
        public ProcedureCallStatement Call { get; private set; }

        /// <summary>
        /// The list on which the procedure should be called
        /// </summary>
        public Expression ListExpression { get; private set; }

        /// <summary>
        /// The list on which the procedure should be called
        /// </summary>
        public Expression ConditionExpression { get; private set; }

        /// <summary>
        /// The iterator variable
        /// </summary>
        public Variables.Variable IteratorVariable { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="call">The corresponding function call designator</param>
        /// <param name="parameters">The expressions used to compute the parameters</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ApplyStatement(ModelElement root, ModelElement log, ProcedureCallStatement call, Expression listExpression, Expression conditionExpression, int start, int end)
            : base(root, log, start, end)
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            Call = call;
            Call.Enclosing = this;

            ListExpression = listExpression;
            ListExpression.Enclosing = this;

            ConditionExpression = conditionExpression;
            if (ConditionExpression != null)
            {
                ConditionExpression.Enclosing = this;
            }

            IteratorVariable = (Variables.Variable)Generated.acceptor.getFactory().createVariable();
            IteratorVariable.Enclosing = this;
            IteratorVariable.Name = "X";
            InitDeclaredElements();
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            Utils.ISubDeclaratorUtils.AppendNamable(this, IteratorVariable);
        }

        /// <summary>
        /// The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; private set; }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<Utils.INamable> retVal)
        {
            Utils.ISubDeclaratorUtils.Find(this, name, retVal);
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
                // ListExpression
                ListExpression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(ListExpression.StaticUsage, Usage.ModeEnum.ReadAndWrite);

                Types.Collection collectionType = ListExpression.GetExpressionType() as Types.Collection;
                if (collectionType != null)
                {
                    IteratorVariable.Type = collectionType.Type;
                }

                // ConditionExpression
                if (ConditionExpression != null)
                {
                    ConditionExpression.SemanticAnalysis(instance);
                    StaticUsage.AddUsages(ConditionExpression.StaticUsage, Usage.ModeEnum.Read);
                }

                Call.SemanticAnalysis(instance);
                StaticUsage.AddUsages(Call.StaticUsage, Usage.ModeEnum.Call);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(Types.ITypedElement variable)
        {
            VariableUpdateStatement retVal = Call.Modifies(variable);

            return retVal;
        }

        /// <summary>
        /// Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
            Call.UpdateStatements(retVal);
        }

        /// <summary>
        /// Provides the list of elements read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<Types.ITypedElement> retVal)
        {
            Call.ReadElements(retVal);
        }

        /// <summary>
        /// Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            if (ListExpression != null)
            {
                ListExpression.checkExpression();
                Types.Collection listExpressionType = ListExpression.GetExpressionType() as Types.Collection;
                if (listExpressionType == null)
                {
                    Root.AddError("Target does not references a list");
                }
            }
            else
            {
                Root.AddError("List should be specified");
            }

            if (ConditionExpression != null)
            {
                ConditionExpression.checkExpression();
            }

            if (Call != null)
            {
                Call.CheckStatement();
            }
            else
            {
                Root.AddError("Procedure should be specified in the APPLY statement");
            }
        }

        /// <summary>
        /// Indicates whether the condition is satisfied with the value provided
        /// Hyp : the value of the iterator variable has been assigned before
        /// </summary>
        /// <returns></returns>
        public bool conditionSatisfied(InterpretationContext context)
        {
            bool retVal = true;

            if (ConditionExpression != null)
            {
                Values.BoolValue b = ConditionExpression.GetValue(context) as Values.BoolValue;
                if (b == null)
                {
                    retVal = false;
                }
                else
                {
                    retVal = b.Val;
                }
            }

            return retVal;
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
            Variables.IVariable variable = ListExpression.GetVariable(context);
            if (variable != null)
            {
                // HacK : ensure that the value is a correct rigth side
                // and keep the result of the right side operation
                Values.ListValue listValue = variable.Value.RightSide(variable, false, false) as Values.ListValue;
                variable.Value = listValue;
                if (listValue != null)
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.setVariable(IteratorVariable);
                    foreach (Values.IValue value in listValue.Val)
                    {
                        if (value != EFSSystem.EmptyValue)
                        {
                            IteratorVariable.Value = value;
                            if (conditionSatisfied(context))
                            {
                                Call.GetChanges(context, changes, explanation, apply, runner);
                            }
                        }
                    }
                    context.LocalScope.PopContext(token);
                }
                else
                {
                    Root.AddError("List expression does not evaluate to a list value");
                }
            }
            else
            {
                Root.AddError("Cannot find variable for " + ListExpression.ToString());
            }
        }

        public override string ToString()
        {
            return "APPLY " + Call.ToString() + " ON " + ListExpression.ToString();
        }

        /// <summary>
        /// Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public override string ShortShortDescription()
        {
            return ListExpression.ToString();
        }

        /// <summary>
        /// Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public override ModelElement AffectedElement()
        {
            return ListExpression.Ref as ModelElement;
        }
    }
}
