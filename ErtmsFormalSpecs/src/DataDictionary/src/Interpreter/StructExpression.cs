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
using Structure = DataDictionary.Types.Structure;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Interpreter
{
    public class StructExpression : Expression
    {
        /// <summary>
        /// The structure instanciated by this structure expression
        /// </summary>
        public Expression Structure { get; private set; }

        /// <summary>
        /// The associations name <-> Expression that is used to initialize this structure
        /// </summary>
        public Dictionary<Designator, Expression> Associations { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public StructExpression(ModelElement root, ModelElement log, Expression structure, Dictionary<Designator, Expression> associations, int start, int end)
            : base(root, log, start, end)
        {
            Structure = structure;
            Associations = associations;
            foreach (Expression expr in Associations.Values)
            {
                expr.Enclosing = this;
            }
        }

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
                // Structure
                Structure.SemanticAnalysis(instance, IsStructure.INSTANCE);
                StaticUsage.AddUsages(Structure.StaticUsage, Usage.ModeEnum.Type);

                Structure structureType = Structure.Ref as Structure;
                // Structure field Association
                foreach (KeyValuePair<Designator, Expression> pair in Associations)
                {
                    if (structureType != null)
                    {
                        pair.Key.Ref = structureType.findStructureElement(pair.Key.Image);
                        StaticUsage.AddUsage(pair.Key.Ref, Root, Usage.ModeEnum.Parameter);
                    }

                    pair.Value.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                    StaticUsage.AddUsages(pair.Value.StaticUsage, Usage.ModeEnum.Read);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return Structure.GetExpressionType();
        }

        /// <summary>
        /// Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            StructureValue retVal = null;

            Structure structureType = Structure.GetExpressionType() as Structure;
            if (structureType != null)
            {
                retVal = new StructureValue(structureType, context.UseDefaultValue);

                try
                {
                    ControllersManager.DesactivateAllNotifications();
                    foreach (KeyValuePair<Designator, Expression> pair in Associations)
                    {
                        IValue val = pair.Value.GetValue(new InterpretationContext(context), explain);
                        if (val != null)
                        {
                            Variable var = (Variable) acceptor.getFactory().createVariable();
                            var.Name = pair.Key.Image;
                            var.Value = val;
                            var.Enclosing = retVal;
                            retVal.set(var);
                        }
                        else
                        {
                            AddError("Cannot evaluate value for " + pair.Value);
                        }
                    }
                }
                finally
                {
                    ControllersManager.ActivateAllNotifications();
                }
            }
            else
            {
                AddError("Cannot determine structure type for " + ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            foreach (Expression expression in Associations.Values)
            {
                expression.fill(retVal, filter);
            }
        }

        /// <summary>
        /// Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = Structure.ToString(indentLevel);
            string indentAccolade = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indentAccolade += "    ";
            }
            string indentText = indentAccolade + "    ";
            bool first = true;
            retVal = retVal + "\n" + indentAccolade + "{";
            foreach (KeyValuePair<Designator, Expression> pair in Associations)
            {
                if (first)
                {
                    retVal = retVal + "\n" + indentText;
                    first = false;
                }
                else
                {
                    retVal = retVal + ",\n" + indentText;
                }
                StructExpression expression = pair.Value as StructExpression;
                if (expression != null)
                {
                    retVal = retVal + pair.Key.Image + " => " + expression.ToString(indentLevel + 1);
                }
                else
                {
                    retVal = retVal + pair.Key.Image + " => " + pair.Value.ToString(indentLevel + 1);
                }
            }
            retVal = retVal + "\n" + indentAccolade + "}";

            return retVal;
        }

        /// <summary>
        /// Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void checkExpression()
        {
            Structure structureType = Structure.GetExpressionType() as Structure;
            if (structureType != null)
            {
                foreach (KeyValuePair<Designator, Expression> pair in Associations)
                {
                    Designator name = pair.Key;
                    Expression expression = pair.Value;

                    List<INamable> targets = new List<INamable>();
                    structureType.Find(name.Image, targets);
                    if (targets.Count > 0)
                    {
                        expression.checkExpression();
                        Type type = expression.GetExpressionType();
                        if (type != null)
                        {
                            foreach (INamable namable in targets)
                            {
                                ITypedElement element = namable as ITypedElement;
                                if (element != null && element.Type != null)
                                {
                                    if (!element.Type.Match(type))
                                    {
                                        AddError("Expression " + expression.ToString() + " type (" + type.FullName + ") does not match the target element " + element.Name + " type (" + element.Type.FullName + ")");
                                    }
                                }
                            }
                        }
                        else
                        {
                            AddError("Expression " + expression.ToString() + " type cannot be found");
                        }
                    }
                    else
                    {
                        Root.AddError("Cannot find " + name + " in structure " + Structure.ToString());
                    }
                }
            }
            else
            {
                AddError("Cannot find structure type " + Structure.ToString());
            }
        }
    }
}