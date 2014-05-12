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
    using DataDictionary.Interpreter.Filter;

    public class VariableUpdateStatement : Statement
    {
        /// <summary>
        /// The designator which identifies the variable to update
        /// </summary>
        public Expression VariableIdentification { get; private set; }

        /// <summary>
        /// The expression expressing the value to set
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public VariableUpdateStatement(ModelElement root, ModelElement log, Expression variableIdentification, Expression expression, int start, int end)
            : base(root, log, start, end)
        {
            VariableIdentification = variableIdentification;
            VariableIdentification.Enclosing = this;

            Expression = expression;
            Expression.Enclosing = this;
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
                // VariableIdentification
                VariableIdentification.SemanticAnalysis(instance, IsLeftSide.INSTANCE);
                StaticUsage.AddUsages(VariableIdentification.StaticUsage, Usage.ModeEnum.Write);

                // Expression
                Expression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(Expression.StaticUsage, Usage.ModeEnum.Read);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the target of this update statement
        /// </summary>
        public Types.ITypedElement Target
        {
            get
            {
                Types.ITypedElement retVal = VariableIdentification.Ref as Types.ITypedElement;

                return retVal;
            }
        }

        /// <summary>
        /// Provides the type of the target variable
        /// </summary>
        public Types.Type TargetType
        {
            get
            {
                Types.Type retVal = null;

                if (Target != null)
                {
                    retVal = Target.Type;
                }
                else
                {
                    retVal = VariableIdentification.Ref as Types.Type;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the statement which modifies the element
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(Types.ITypedElement variable)
        {
            VariableUpdateStatement retVal = null;

            if (variable == Target)
            {
                retVal = this;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
            retVal.Add(this);
        }

        /// <summary>
        /// Provides the list of elements read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<Types.ITypedElement> retVal)
        {
            retVal.AddRange(Expression.GetVariables());
        }

        /// <summary>
        /// Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            VariableIdentification.checkExpression();
            if (VariableIdentification.Ref is Parameter)
            {
                Root.AddError("Cannot assign a value to a parameter (" + VariableIdentification.ToString() + ")");
            }

            if (VariableIdentification.Ref == null)
            {
                Root.AddError("Cannot assign a value to " + VariableIdentification.ToString());
            }

            Types.Type targetType = VariableIdentification.GetExpressionType();
            if (targetType == null)
            {
                Root.AddError("Cannot determine type of target " + VariableIdentification.ToString());
            }
            else if (Expression != null)
            {
                Expression.checkExpression();

                Types.Type type = Expression.GetExpressionType();
                if (type != null)
                {
                    if (targetType != null)
                    {
                        if (!targetType.Match(type))
                        {
                            UnaryExpression unaryExpression = Expression as UnaryExpression;
                            if (unaryExpression != null && unaryExpression.Term.LiteralValue != null)
                            {
                                Root.AddError("Expression " + Expression.ToString() + " does not fit in variable " + VariableIdentification.ToString());
                            }
                            else
                            {
                                Root.AddError("Expression [" + Expression.ToString() + "] type (" + type.FullName + ") does not match variable [" + VariableIdentification.ToString() + "] type (" + targetType.FullName + ")");
                            }
                        }
                        else
                        {
                            Types.Range rangeType = targetType as Types.Range;
                            if (rangeType != null)
                            {
                                Values.IValue value = Expression.Ref as Values.IValue;
                                if (value != null)
                                {
                                    if (rangeType.convert(value) == null)
                                    {
                                        Root.AddError("Cannot set " + value.LiteralName + " in variable of type " + rangeType.Name);
                                    }
                                }
                            }
                        }

                        if (Expression.Ref == EFSSystem.EmptyValue)
                        {
                            if (targetType is Types.Collection)
                            {
                                Root.AddError("Assignation of " + Expression.Ref.Name + " cannot be performed on variables of type collection. Use [] instead.");
                            }
                        }
                    }
                    else
                    {
                        Root.AddError("Cannot determine variable type");
                    }
                }
                else
                {
                    Root.AddError("Cannot determine expression type (3) for " + Expression.ToString());
                }
            }
            else
            {
                Root.AddError("Invalid expression in assignment");
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
            Variables.IVariable var = VariableIdentification.GetVariable(context);
            if (var != null)
            {
                string tmp = var.FullName;
                Values.IValue value = Expression.GetValue(context);
                if (value != null)
                {
                    value = value.RightSide(var, true, true);
                }
                Rules.Change change = new Rules.Change(var, var.Value, value);
                changes.Add(change, apply, runner);

                if (explanation != null)
                {
                    ExplanationPart part = new ExplanationPart(Root, change);
                    part.SubExplanations.Add(Expression.Explain(context));
                    explanation.SubExplanations.Add(part);
                }
            }
            else
            {
                AddError("Cannot find variable " + VariableIdentification.ToString());
            }
        }

        public override string ToString()
        {
            return VariableIdentification.ToString() + " <- " + Expression.ToString();
        }

        /// <summary>
        /// Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public override string ShortShortDescription()
        {
            string retVal = "";

            if (VariableIdentification.ToString().Trim() == "THIS")
            {
                retVal = Expression.ToString();
            }
            else
            {
                retVal = VariableIdentification.Name;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public override ModelElement AffectedElement()
        {
            return VariableIdentification.Ref as ModelElement;
        }

    }
}
