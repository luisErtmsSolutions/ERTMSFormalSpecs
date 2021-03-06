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
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class ListExpression : Expression
    {
        /// <summary>
        /// The values in the list
        /// </summary>
        public List<Expression> ListElements { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ListExpression(ModelElement root, ModelElement log, List<Expression> elements, int start, int end)
            : base(root, log, start, end)
        {
            ListElements = elements;

            foreach (Expression expr in ListElements)
            {
                expr.Enclosing = this;
            }
        }

        /// <summary>
        /// The type of the collection
        /// </summary>
        private Collection ExpressionType { get; set; }

        /// <summary>
        /// Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                Type elementType = null;

                foreach (Expression expr in ListElements)
                {
                    expr.SemanticAnalysis(instance, expectation);
                    StaticUsage.AddUsages(expr.StaticUsage, null);

                    Type current = expr.GetExpressionType();
                    if (elementType == null)
                    {
                        elementType = current;
                    }
                    else
                    {
                        if (!current.Match(elementType))
                        {
                            AddError("Cannot mix types " + current.ToString() + " and " + elementType.ToString() + "in collection");
                        }
                    }
                }

                if (elementType != null)
                {
                    ExpressionType = (Collection) acceptor.getFactory().createCollection();
                    ExpressionType.Type = elementType;
                    ExpressionType.Name = "ListOf_" + elementType.FullName;
                    ExpressionType.Enclosing = Root.EFSSystem;

                    StaticUsage.AddUsage(elementType, Root, Usage.ModeEnum.Type);
                }
                else
                {
                    ExpressionType = new GenericCollection(EFSSystem);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks the expression
        /// </summary>
        public override void checkExpression()
        {
            foreach (Expression expr in ListElements)
            {
                expr.checkExpression();
            }
        }

        /// <summary>
        /// Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return ExpressionType;
        }

        /// <summary>
        /// Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            List<IValue> elements = new List<IValue>();
            foreach (Expression expr in ListElements)
            {
                IValue val = expr.GetValue(context, explain);
                if (val != null)
                {
                    elements.Add(val);
                }
                else
                {
                    AddError("Cannot evaluate " + expr.ToString());
                }
            }

            IValue retVal = new ListValue(ExpressionType, elements);
            return retVal;
        }

        /// <summary>
        /// Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            foreach (Expression expr in ListElements)
            {
                expr.fill(retVal, filter);
            }
        }

        /// <summary>
        /// Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string indentText = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indentText += "    ";
            }

            string retVal = "\n" + indentText + "[\n" + indentText + "    ";

            bool first = true;
            foreach (Expression expr in ListElements)
            {
                if (!first)
                {
                    retVal += ", ";
                }
                retVal += expr.ToString(indentLevel + 1);

                first = false;
            }

            retVal += "\n" + indentText + "]";

            return retVal;
        }
    }
}