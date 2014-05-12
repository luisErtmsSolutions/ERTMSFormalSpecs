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

namespace DataDictionary.Tests
{
    public class Expectation : Generated.Expectation, IExpressionable, TextualExplain
    {
        /// <summary>
        /// The enclosing step, if any
        /// </summary>
        public Step Step
        {
            get { return SubStep.Step; }
        }

        /// <summary>
        /// The enclosing sub-step, if any
        /// </summary>
        public SubStep SubStep
        {
            get { return Enclosing as SubStep; }
        }

        /// <summary>
        /// The enclosing translation, if any
        /// </summary>
        public Translations.Translation Translation
        {
            get
            {
                Translations.Translation result = null;
                if (SubStep != null)
                {
                    result = SubStep.Translation;
                }
                return result;
            }
        }

        /// <summary>
        /// The enclosing frame
        /// </summary>
        public Frame Frame
        {
            get { return Utils.EnclosingFinder<Tests.Frame>.find(this); }
        }

        /// <summary>
        /// The expected value
        /// </summary>
        public string Value
        {
            get { return getValue(); }
            set
            {
                setValue(value);
                __expression = null;
            }
        }

        /// <summary>
        /// Indicates if this expectation is blocking
        /// </summary>
        public bool Blocking
        {
            get { return getBlocking(); }
            set { setBlocking(value); }
        }

        /// <summary>
        /// When blocking, this indicates the deadling before the expectation should be achieved
        /// </summary>
        public double DeadLine
        {
            get { return getDeadLine(); }
            set { setDeadLine(value); }
        }

        public override string ExpressionText
        {
            get
            {
                string retVal = Value;
                if (retVal == null)
                {
                    retVal = "";
                }
                return retVal;
            }
            set { Value = value; }
        }

        public Interpreter.Expression __expression;
        public Interpreter.Expression Expression
        {
            get
            {
                if (__expression == null)
                {
                    __expression = EFSSystem.Parser.Expression(this, ExpressionText);
                }
                return __expression;
            }
            set
            {
                __expression = value;
            }
        }

        public Interpreter.InterpreterTreeNode Tree { get { return Expression; } }


        /// <summary>
        /// Clears the tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
            ConditionTree = null;
        }

        /// <summary>
        /// Creates the tree according to the expression text
        /// </summary>
        public Interpreter.InterpreterTreeNode Compile()
        {
            // Side effect, builds the expressions if they are not already built
            Interpreter.Expression expression = ConditionTree;
            return Tree;
        }


        /// <summary>
        /// Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = false;

            Interpreter.Statement.Statement tree = EFSSystem.Parser.Statement(this, expression, true);
            retVal = tree != null;

            return retVal;
        }

        public Interpreter.Expression conditionTree;
        public Interpreter.Expression ConditionTree
        {
            get
            {
                if (conditionTree == null && getCondition() != null)
                {
                    conditionTree = EFSSystem.Parser.Expression(this, getCondition());
                }
                return conditionTree;
            }
            set
            {
                conditionTree = value;
            }
        }

        public override string Name
        {
            get { return ExpressionText; }
            set { }
        }

        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                System.Collections.ArrayList retVal = null;

                if (SubStep != null)
                {
                    retVal = SubStep.Expectations;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Indicates the name of the checked variable, if any
        /// </summary>
        /// <returns></returns>
        public Interpreter.Designator CheckedVariable()
        {
            Interpreter.Designator retVal = null;

            Interpreter.BinaryExpression binaryExpression = Expression as Interpreter.BinaryExpression;
            if (binaryExpression != null)
            {
                Interpreter.UnaryExpression unaryExpression = binaryExpression.Left as Interpreter.UnaryExpression;
                if (unaryExpression != null && unaryExpression.Term != null && unaryExpression.Term.Designator != null)
                {
                    retVal = unaryExpression.Term.Designator;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
        }

        /// <summary>
        /// Provides an explanation of the step's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel, bool explainSubElements)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            if (Expression != null)
            {
                retVal += TextualExplainUtilities.Pad(Expression.ToString(), indentLevel);
            }

            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the step's behaviour
        /// </summary>

        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = "";

            retVal = getExplain(0, explainSubElements);

            return retVal;
        }
    }
}
