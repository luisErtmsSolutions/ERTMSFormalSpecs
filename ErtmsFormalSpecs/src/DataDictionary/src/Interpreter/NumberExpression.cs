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
using DataDictionary.Interpreter.Filter;
namespace DataDictionary.Interpreter
{
    public class NumberExpression : Expression
    {
        /// <summary>
        /// The number image
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        /// The image of the value
        /// </summary>
        public Values.IValue Value { get; private set; }

        /// <summary>
        /// The value type
        /// </summary>
        public Types.Type Type { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        public NumberExpression(ModelElement root, ModelElement log, string value, Types.Type type)
            : base(root, log)
        {
            Image = value;
            Type = type;
        }

        /// <summary>
        /// Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(Utils.INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Value
                Value = Type.getValue(Image);
                StaticUsage.AddUsage(Type, Root, Usage.ModeEnum.Type);

                if (Value == null)
                {
                    AddError("Cannot evaluate " + ToString() + " as a number");
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the INamable which is referenced by this expression, if any
        /// </summary>
        public override Utils.INamable Ref
        {
            get { return Value; }
        }

        /// <summary>
        /// Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Types.Type GetExpressionType()
        {
            return Type;
        }

        /// <summary>
        /// Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <returns></returns>
        public override Values.IValue GetValue(InterpretationContext context)
        {
            return Value;
        }

        /// <summary>
        /// Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<Utils.INamable> retVal, BaseFilter filter)
        {
            if (filter.AcceptableChoice(Value))
            {
                retVal.Add(Value);
            }
        }

        /// <summary>
        /// Provides the string representation of the binary expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retVal = Image;

            return retVal;
        }
    }
}
