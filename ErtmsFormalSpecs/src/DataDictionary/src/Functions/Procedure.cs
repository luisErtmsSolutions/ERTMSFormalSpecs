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
using DataDictionary.Interpreter;
using Utils;

namespace DataDictionary.Functions
{
    public class Procedure : Generated.Procedure, Utils.ISubDeclarator, ICallable, TextualExplain, IGraphicalDisplay
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Procedure()
            : base()
        {
        }

        /// <summary>
        /// Indicates if this Procedure contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                foreach (DataDictionary.Rules.Rule rule in Rules)
                {
                    if (rule.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            foreach (Parameter parameter in FormalParameters)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, parameter);
            }
        }

        /// <summary>
        /// The elements declared by this variable
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; set; }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<Utils.INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        /// The enclosing name space
        /// </summary>
        public Types.NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        /// The enclosing structure
        /// </summary>
        public Types.Structure Structure
        {
            get { return Utils.EnclosingFinder<Types.Structure>.find(this); }
        }

        /// <summary>
        /// Parameters of the procedure
        /// </summary>
        public System.Collections.ArrayList FormalParameters
        {
            get
            {
                if (allParameters() == null)
                {
                    setAllParameters(new System.Collections.ArrayList());
                }
                return allParameters();
            }
            set { setAllParameters(value); }
        }


        /// <summary>
        /// Provides the formal parameter whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Parameter getFormalParameter(string name)
        {
            Parameter retVal = null;

            foreach (Parameter parameter in FormalParameters)
            {
                if (parameter.Name.CompareTo(name) == 0)
                {
                    retVal = parameter;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The procedure return type
        /// </summary>
        public Types.Type ReturnType
        {
            get { return EFSSystem.NoType; }
        }

        /// <summary>
        /// Provides the enclosing collection, for deletion
        /// </summary>
        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                if (Structure != null)
                {
                    return Structure.Procedures;
                }
                else if (NameSpace != null)
                {
                    return NameSpace.Procedures;
                }

                return null;
            }
        }

        /// <summary>
        /// The rules declared in this procedure
        /// </summary>
        public System.Collections.ArrayList Rules
        {
            get
            {
                if (allRules() == null)
                {
                    setAllRules(new System.Collections.ArrayList());
                }
                return allRules();
            }
            set
            {
                setAllRules(value);
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            {
                Parameter item = element as Parameter;
                if (item != null)
                {
                    appendParameters(item);
                }
            }
            {
                Rules.Rule item = element as Rules.Rule;
                if (item != null)
                {
                    appendRules(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        /// Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public virtual void additionalChecks(ModelElement root, Dictionary<string, Interpreter.Expression> actualParameters)
        {
        }

        /// <summary>
        /// Provides an explanation of the rule's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel, bool getExplain)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            // Creates the procedure header
            retVal += TextualExplainUtilities.Pad("{ {\\b PROCEDURE } " + Name, indentLevel);
            if (FormalParameters.Count > 0)
            {
                retVal += "(\\par";
                foreach (Parameter parameter in FormalParameters)
                {
                    retVal = retVal + TextualExplainUtilities.Pad(parameter.Name + ":" + parameter.TypeName + ",\\par", indentLevel + 2);
                }
                retVal = retVal + ")}\\par";
            }
            else
            {
                retVal += "()\\par";
            }

            foreach (Rules.Rule rule in Rules)
            {
                retVal += "\\par" + rule.getExplain(indentLevel + 2, true);
            }

            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the rule's behaviour
        /// </summary>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = getExplain(0, true);

            return TextualExplainUtilities.Encapsule(retVal);
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
