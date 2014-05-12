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
    public abstract class Statement : InterpreterTreeNode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Statement(ModelElement root, ModelElement log, int start, int end)
            : base(root, log, start, end)
        {
        }

        /// <summary>
        /// Indicates whether the semantical analysis has already been performed
        /// </summary>
        protected bool SemanticalAnalysisDone { get; set; }

        /// <summary>
        /// Performs the semantic analysis of the statement
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>true if semantical analysis should be performed</returns>
        public virtual bool SemanticAnalysis(Utils.INamable instance = null)
        {
            bool retVal = !SemanticalAnalysisDone;

            if (retVal)
            {
                StaticUsage = new Usages();
                SemanticalAnalysisDone = true;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the type of this designator
        /// </summary>
        /// <returns></returns>
        public Types.Type getExpressionType()
        {
            Types.Type retVal = null;

            return retVal;
        }

        /// <summary>
        /// Checks the statement for semantical errors
        /// </summary>
        public abstract void CheckStatement();

        /// <summary>
        /// Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public abstract VariableUpdateStatement Modifies(Types.ITypedElement variable);

        /// <summary>
        /// Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public abstract void UpdateStatements(List<VariableUpdateStatement> retVal);

        /// <summary>
        /// Indicates whether this statement reads the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public virtual bool Reads(Types.ITypedElement variable)
        {
            bool retVal = false;

            List<Types.ITypedElement> variablesRead = new List<Types.ITypedElement>();
            ReadElements(variablesRead);
            retVal = variablesRead.Contains(variable);

            return retVal;
        }

        /// <summary>
        /// Provides the list of variables read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public abstract void ReadElements(List<Types.ITypedElement> retVal);

        /// <summary>
        /// Provides the changes performed by this statement
        /// </summary>
        /// <param name="context">The context on which the changes should be computed</param>
        /// <param name="changes">The changes performed by this statement</param>
        /// <param name="explanation">The explanatino to fill, if any</param>
        /// <param name="apply">Indicates that the changes should be applied immediately</param>
        /// <param name="runner"></param>
        public abstract void GetChanges(Interpreter.InterpretationContext context, ChangeList changes, Interpreter.ExplanationPart explanation, bool apply, Tests.Runner.Runner runner);

        /// <summary>
        /// Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public abstract string ShortShortDescription();

        /// <summary>
        /// What is affected by this statement
        /// </summary>
        public enum ModeEnum { Unknown, In, Out, InOut, Internal, Call };

        /// <summary>
        /// Converts a VariableModeEnumType into a ModeEnum
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected static ModeEnum ConvertMode(DataDictionary.Generated.acceptor.VariableModeEnumType mode)
        {
            ModeEnum retVal = ModeEnum.Unknown;

            switch (mode)
            {
                case Generated.acceptor.VariableModeEnumType.aIncoming:
                    retVal = ModeEnum.In;
                    break;
                case Generated.acceptor.VariableModeEnumType.aInOut:
                    retVal = ModeEnum.InOut;
                    break;
                case Generated.acceptor.VariableModeEnumType.aInternal:
                    retVal = ModeEnum.Internal;
                    break;
                case Generated.acceptor.VariableModeEnumType.aOutgoing:
                    retVal = ModeEnum.Out;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the usage description done by this statement
        /// </summary>
        /// <returns></returns>
        public virtual ModeEnum UsageDescription()
        {
            ModeEnum retVal = ModeEnum.Unknown;

            ModelElement target = AffectedElement();
            Variables.IVariable variable = target as Variables.IVariable;
            if (variable != null)
            {
                retVal = ConvertMode(variable.Mode);
            }

            Types.StructureElement element = target as Types.StructureElement;
            if (element != null)
            {
                retVal = ConvertMode(element.Mode);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public abstract ModelElement AffectedElement();

    }
}
