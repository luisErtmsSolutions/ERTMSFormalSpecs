//------------------------------------------------------------------------------
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
using System.Collections;
using System.Collections.Generic;
using DataDictionary.Interpreter;
using Utils;
using DataDictionary.Types.AccessMode;

namespace DataDictionary.Types
{
    public class NameSpace : Generated.NameSpace, Utils.ISubDeclarator, Utils.IFinder, IEnclosesNameSpaces, IGraphicalDisplay, ICommentable
    {
        /// <summary>
        /// Used to temporarily store the list of sub-namespaces
        /// </summary>
        private ArrayList savedNameSpaces;

        /// <summary>
        /// Constructor
        /// </summary>
        public NameSpace()
            : base()
        {
            Utils.FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        /// The sub namespaces
        /// </summary>
        public System.Collections.ArrayList NameSpaces
        {
            get
            {
                if (allNameSpaces() == null)
                {
                    setAllNameSpaces(new System.Collections.ArrayList());
                }
                return allNameSpaces();
            }
        }

        /// <summary>
        /// The ranges types
        /// </summary>
        public System.Collections.ArrayList Ranges
        {
            get
            {
                if (allRanges() == null)
                {
                    setAllRanges(new System.Collections.ArrayList());
                }
                return allRanges();
            }
        }

        /// <summary>
        /// The enumeration types
        /// </summary>
        public System.Collections.ArrayList Enumerations
        {
            get
            {
                if (allEnumerations() == null)
                {
                    setAllEnumerations(new System.Collections.ArrayList());
                }
                return allEnumerations();
            }
        }

        /// <summary>
        /// The structure types
        /// </summary>
        public System.Collections.ArrayList Structures
        {
            get
            {
                if (allStructures() == null)
                {
                    setAllStructures(new System.Collections.ArrayList());
                }
                return allStructures();
            }
        }

        /// <summary>
        /// The collection types
        /// </summary>
        public System.Collections.ArrayList Collections
        {
            get
            {
                if (allCollections() == null)
                {
                    setAllCollections(new System.Collections.ArrayList());
                }
                return allCollections();
            }
        }

        /// <summary>
        /// The state machines types
        /// </summary>
        public System.Collections.ArrayList StateMachines
        {
            get
            {
                if (allStateMachines() == null)
                {
                    setAllStateMachines(new System.Collections.ArrayList());
                }
                return allStateMachines();
            }
        }

        /// <summary>
        /// The functions declared in the namespace
        /// </summary>
        public System.Collections.ArrayList Functions
        {
            get
            {
                if (allFunctions() == null)
                {
                    setAllFunctions(new System.Collections.ArrayList());
                }
                return allFunctions();
            }
        }

        /// <summary>
        /// The procedures declared in the namespace
        /// </summary>
        public System.Collections.ArrayList Procedures
        {
            get
            {
                if (allProcedures() == null)
                {
                    setAllProcedures(new System.Collections.ArrayList());
                }
                return allProcedures();
            }
        }

        /// <summary>
        /// The variables declared in the namespace
        /// </summary>
        public System.Collections.ArrayList Variables
        {
            get
            {
                if (allVariables() == null)
                {
                    setAllVariables(new System.Collections.ArrayList());
                }
                return allVariables();
            }
        }

        /// <summary>
        /// The rules declared in the namespace
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
        }

        /// <summary>
        /// Clears the caches
        /// </summary>
        public void ClearCache()
        {
            cachedVariables = null;
            types = null;
            DeclaredElements = null;
        }

        /// <summary>
        /// Removes temporary files created for reference namespaces
        /// </summary>
        public void ClearTempFiles()
        {
            if (allNameSpaceRefs() != null)
            {
                foreach (NameSpaceRef aReferenceNameSpace in allNameSpaceRefs())
                {
                    aReferenceNameSpace.ClearTempFile();
                }
            }
        }

        /// <summary>
        /// Used to store the list of sub-namespaces before saving the model
        /// </summary>
        public void StoreInfo()
        {
            savedNameSpaces = new ArrayList();
            foreach (NameSpace aNameSpace in allNameSpaces())
            {
                savedNameSpaces.Add(aNameSpace);
            }
            allNameSpaces().Clear();
        }

        /// <summary>
        /// Used to restore the list of sub-namespaces, after having saved the model
        /// </summary>
        public void RestoreInfo()
        {
            setAllNameSpaces(new ArrayList());
            foreach (NameSpace aNameSpace in savedNameSpaces)
            {
                allNameSpaces().Add(aNameSpace);
                aNameSpace.RestoreInfo();
            }
            savedNameSpaces.Clear();
        }

        /// <summary>
        /// Provides all the values available through this namespace
        /// </summary>
        private List<Variables.IVariable> cachedVariables;
        public List<Variables.IVariable> AllVariables
        {
            get
            {
                if (cachedVariables == null)
                {
                    cachedVariables = new List<Variables.IVariable>();

                    foreach (Variables.IVariable value in Variables)
                    {
                        cachedVariables.Add(value);
                    }
                }

                return cachedVariables;
            }
        }
        /// <summary>
        /// Provides all the types available through this namespace
        /// </summary>
        private List<Types.Type> types;
        public List<Types.Type> Types
        {
            get
            {
                if (types == null)
                {
                    types = new List<Types.Type>();

                    foreach (Range range in Ranges)
                    {
                        types.Add(range);
                    }
                    foreach (Enum enumeration in Enumerations)
                    {
                        types.Add(enumeration);
                    }
                    foreach (Structure structure in Structures)
                    {
                        types.Add(structure);
                    }
                    foreach (Collection collection in Collections)
                    {
                        types.Add(collection);
                    }
                    foreach (StateMachine stateMachine in StateMachines)
                    {
                        types.Add(stateMachine);
                    }
                }

                return types;
            }
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            foreach (NameSpace nameSpace in NameSpaces)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, nameSpace);
            }

            foreach (Types.Type type in Types)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, type);
            }

            foreach (Variables.IVariable variable in AllVariables)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, variable);
            }

            foreach (Functions.Procedure proc in Procedures)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, proc);
            }

            foreach (Functions.Function function in Functions)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, function);
            }
        }

        /// <summary>
        /// The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; set; }

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
        /// The types defined in this namespace, an the sub namespaces
        /// </summary>
        public HashSet<Type> DefinedTypes
        {
            get { return TypeFinder.INSTANCE.find(this); }
        }

        /// <summary>
        /// Provides the namespace which corresponds to the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NameSpace findNameSpaceByName(string name)
        {
            return (NameSpace)Utils.INamableUtils.findByName(name, NameSpaces);
        }

        /// <summary>
        /// Provides the type which corresponds to the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type findTypeByName(string name)
        {
            return innerFindTypeByName(name, true);
        }

        /// <summary>
        /// Provides the type which corresponds to the given name
        /// </summary>
        /// <param name="name">the type name to find</param>
        /// <param name="findInEnclosingNameSpaces">indicates that the search must be performed in the enclosing namespaces</param>
        /// <returns></returns>
        private Type innerFindTypeByName(string name, bool findInEnclosingNameSpaces)
        {
            Type retVal = null;

            string[] names = name.Split('.');
            if (names.Length == 1)
            {
                retVal = (Type)Utils.INamableUtils.findByName(name, Types);
            }
            else
            {
                NameSpace nameSpace = (NameSpace)Utils.INamableUtils.findByName(names[0], NameSpaces);
                if (nameSpace != null)
                {
                    retVal = nameSpace.innerFindTypeByName(name.Substring(nameSpace.Name.Length + 1), false);
                }
            }

            if (retVal == null && findInEnclosingNameSpaces && EnclosingNameSpace != null)
            {
                retVal = EnclosingNameSpace.innerFindTypeByName(name, true);
            }

            return retVal;
        }

        /// <summary>
        /// The enclosing dictionary
        /// </summary>
        public Dictionary EnclosingDictionary
        {
            get { return Enclosing as Dictionary; }
        }

        /// <summary>
        /// The enclosing namespace
        /// </summary>
        public NameSpace EnclosingNameSpace
        {
            get { return Enclosing as NameSpace; }
        }

        /// <summary>
        /// The enclosing collection
        /// </summary>
        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                System.Collections.ArrayList retVal = null;

                if (EnclosingNameSpace != null)
                {
                    retVal = EnclosingNameSpace.NameSpaces;
                }
                else if (EnclosingDictionary != null)
                {
                    retVal = EnclosingDictionary.NameSpaces;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            {
                Range item = element as Range;
                if (item != null)
                {
                    appendRanges(item);
                }
            }
            {
                Enum item = element as Enum;
                if (item != null)
                {
                    appendEnumerations(item);
                }
            }
            {
                Structure item = element as Structure;
                if (item != null)
                {
                    appendStructures(item);
                }
            }
            {
                Collection item = element as Collection;
                if (item != null)
                {
                    appendCollections(item);
                }
            }
            {
                Functions.Function item = element as Functions.Function;
                if (item != null)
                {
                    appendFunctions(item);
                }
            }
            {
                Functions.Procedure item = element as Functions.Procedure;
                if (item != null)
                {
                    appendProcedures(item);
                }
            }
            {
                Rules.Rule item = element as Rules.Rule;
                if (item != null)
                {
                    appendRules(item);
                }
            }
            {
                Variables.Variable item = element as Variables.Variable;
                if (item != null)
                {
                    appendVariables(item);
                }
            }
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

        /// <summary>
        /// Provides an explanation of the namespace
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            retVal += TextualExplainUtilities.Pad("{{\\b NAMESPACE} " + Name, indentLevel);

            return retVal;
        }

        /// <summary>
        /// An explanation of the namespace
        /// </summary>
        /// <param name="inner"></param>
        /// <returns></returns>
        public string getExplain(bool inner)
        {
            string retVal = getExplain(0);

            return retVal;
        }

        /// <summary>
        /// The namespace ref which instanciated this namespace
        /// </summary>
        public NameSpaceRef NameSpaceRef { get; set; }

        /// <summary>
        /// The comment related to this element
        /// </summary>
        public string Comment
        {
            get { return getComment(); }
            set { setComment(value); }
        }

    }
}
