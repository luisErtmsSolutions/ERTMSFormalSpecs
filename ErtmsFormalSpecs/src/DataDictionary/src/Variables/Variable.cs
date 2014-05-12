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
using Utils;

namespace DataDictionary.Variables
{
    public class Variable : Generated.Variable, IVariable, Utils.ISubDeclarator, TextualExplain, Types.IDefaultValueElement, IGraphicalDisplay
    {
        /// <summary>
        /// Indicates that the DeclaredElement dictionary is currently being built
        /// </summary>
        private bool BuildingDeclaredElements = false;

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
        }

        /// <summary>
        /// The elements declared by this variable
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements
        {
            get
            {
                Dictionary<string, List<Utils.INamable>> retVal = new Dictionary<string, List<Utils.INamable>>();

                if (!BuildingDeclaredElements)
                {
                    try
                    {
                        BuildingDeclaredElements = true;

                        Values.StructureValue structureValue = Value as Values.StructureValue;
                        if (structureValue != null)
                        {
                            if (structureValue.DeclaredElements == null)
                            {
                                structureValue.InitDeclaredElements();
                            }
                            retVal = structureValue.DeclaredElements;
                        }
                    }
                    finally
                    {
                        BuildingDeclaredElements = false;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Indicates if this Procedure contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (DataDictionary.Variables.Variable subVariable in allSubVariables())
                {
                    if (subVariable.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<Utils.INamable> retVal)
        {
            if (!BuildingDeclaredElements)
            {
                try
                {
                    BuildingDeclaredElements = true;

                    Values.StructureValue structureValue = Value as Values.StructureValue;
                    if (structureValue != null)
                    {
                        structureValue.Find(name, retVal);
                    }

                    // Dereference of an empty value holds the empty value
                    Values.EmptyValue emptyValue = Value as Values.EmptyValue;
                    if (emptyValue != null)
                    {
                        retVal.Add(emptyValue);
                    }
                }
                finally
                {
                    BuildingDeclaredElements = false;
                }
            }
        }

        /// <summary>
        /// The enclosing name space
        /// </summary>
        public Types.NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        /// Provides the mode of the variable
        /// </summary>
        public DataDictionary.Generated.acceptor.VariableModeEnumType Mode
        {
            get { return getVariableMode(); }
            set { setVariableMode(value); }
        }

        /// <summary>
        /// The default value
        /// </summary>
        public string Default
        {
            get { return getDefaultValue(); }
            set { setDefaultValue(value); }
        }


        public override string ExpressionText
        {
            get
            {
                string retVal = Default;

                if (retVal == null)
                {
                    retVal = "";
                }

                return retVal;
            }
            set
            {
                Default = value;
                __expression = null;
            }
        }

        /// <summary>
        /// Provides the expression tree associated to this action's expression
        /// </summary>
        private Interpreter.Expression __expression;
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
        /// Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
        }

        /// <summary>
        /// Creates the tree according to the expression text
        /// </summary>
        public Interpreter.InterpreterTreeNode Compile()
        {
            // Side effect, builds the statement if it is not already built
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

            Interpreter.Expression tree = EFSSystem.Parser.Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
        }

        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                return NameSpace.Variables;
            }
        }

        /// <summary>
        /// Provides the type name of the variable
        /// </summary>
        public string TypeName
        {
            get
            {
                return getTypeName();
            }
            set
            {
                setTypeName(value);
                type = null;
                theValue = null;

                // Ensure types and typename are synchronized
                type = Type;
            }
        }

        /// <summary>
        /// The type associated to this variable
        /// </summary>
        private Types.Type type;
        public Types.Type Type
        {
            get
            {
                if (type == null)
                {
                    type = EFSSystem.findType(NameSpace, getTypeName());
                }
                return type;
            }
            set
            {
                type = value;
                if (value != null)
                {
                    setTypeName(value.FullName);
                }
                else
                {
                    setTypeName(null);
                }
            }
        }

        /// <summary>
        /// The enclosed variables
        /// </summary>
        public Dictionary<string, IVariable> SubVariables
        {
            get
            {
                Dictionary<string, IVariable> retVal = retVal = new Dictionary<string, IVariable>();

                Values.StructureValue structureValue = Value as Values.StructureValue;
                if (structureValue != null)
                {
                    retVal = structureValue.SubVariables;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the variable's default value
        /// </summary>
        public Values.IValue DefaultValue
        {
            get
            {
                Values.IValue retVal = null;

                if (Type != null)
                {
                    if (Utils.Utils.isEmpty(getDefaultValue()))
                    {
                        // The variable does not define a default value, get the one from the type
                        retVal = Type.DefaultValue;
                    }
                    else
                    {
                        // The variable defines a default value, try to interpret it
                        if (Expression != null)
                        {
                            retVal = Expression.GetValue(new Interpreter.InterpretationContext(Type));
                            if (retVal != null && !Type.Match(retVal.Type))
                            {
                                AddError("Default value type (" + retVal.Type.Name + ")does not match variable type (" + Type.Name + ")");
                                retVal = null;
                            }
                        }
                    }
                }
                else
                {
                    AddError("Cannot find type of variable (" + getTypeName() + ")");
                }

                if (retVal == null)
                {
                    AddError("Cannot create default value");
                }
                else
                {
                    retVal = retVal.RightSide(this, false, true);
                }

                return retVal;
            }
        }

        public override void AddElementLog(Utils.ElementLog log)
        {
            if (Enclosing is DataDictionary.Types.NameSpace)
            {
                base.AddElementLog(log);
            }
            else
            {
                IEnclosed current = Enclosing as IEnclosed;
                while (current != null)
                {
                    ModelElement element = current as ModelElement;
                    if (element != null)
                    {
                        element.AddElementLog(log);
                        current = null;
                    }
                    else
                    {
                        current = current.Enclosing as IEnclosed;
                    }
                }
            }
        }

        /// <summary>
        /// The variable's value
        /// </summary>
        public Values.IValue theValue;
        public Values.IValue Value
        {
            get
            {
                if (theValue == null)
                {
                    theValue = DefaultValue;
                }
                return theValue;
            }
            set {
                theValue = value;
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            base.AddModelElement(element);
        }

        /// <summary>
        /// Explains the current item
        /// </summary>
        /// <param name="subElements"></param>
        /// <returns></returns>
        public string getExplain(bool subElements)
        {
            string retVal = "";

            if (Type != null)
            {
                if (!Utils.Utils.isEmpty(Type.Comment))
                {
                    retVal = retVal + TextualExplainUtilities.Comment(Type.Comment, 0) + "\n";
                }
            }

            if (!Utils.Utils.isEmpty(Comment))
            {
                retVal = retVal + TextualExplainUtilities.Comment(Comment, 0) + "\n";
            }

            if (Value != null)
            {
                retVal = retVal + Name + " : " + TypeName + " = " + Value.LiteralName;
            }

            return TextualExplainUtilities.Encapsule(retVal);
        }

        public override string ToString()
        {
            string retVal = "Variable " + Name + " : " + TypeName;
            if (Value != null)
            {
                retVal += " = " + Value.ToString();
            }
            else
            {
                retVal += " is null";
            }

            return retVal;
        }

        /// <summary>
        /// Provides the text of the default value
        /// </summary>
        /// <returns></returns>
        public string GetDefaultValueText()
        {
            string retVal = getDefaultValue();

            if (string.IsNullOrEmpty(retVal) && Type != null)
            {
                retVal = Type.getDefault();
            }

            if (string.IsNullOrEmpty(retVal))
            {
                Types.Structure structure = Type as Types.Structure;
                if (structure != null)
                {
                    retVal = structure.FullName + "{}";
                }
                else
                {
                    Types.Collection collection = Type as Types.Collection;
                    if (collection != null)
                    {
                        retVal = "[]";
                    }
                    else
                    {
                        retVal = "0";
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// The X position
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        /// The Y position
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        /// The width
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        /// The height
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        /// The name to be displayed
        /// </summary>
        public string GraphicalName { get { return Name; } }

        /// <summary>
        /// Indicates whether the namespace is hidden
        /// </summary>
        public bool Hidden
        {
            get { return getHidden(); }
            set { setHidden(value); }
        }

        /// <summary>
        /// Indicates that the element is pinned
        /// </summary>
        public bool Pinned { get { return getPinned(); } set { setPinned(value); } }
    }
}
