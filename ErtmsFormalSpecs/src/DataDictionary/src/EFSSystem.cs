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
namespace DataDictionary
{
    using System.Collections.Generic;
    using DataDictionary.Types;
    using DataDictionary.Interpreter;
    using DataDictionary.Rules;
    using System;
    using DataDictionary.Interpreter.Statement;
    using DataDictionary.Specification;
    using DataDictionary.Interpreter.Filter;
    using System.Collections;

    /// <summary>
    /// A complete system, along with all dictionaries
    /// </summary>
    public class EFSSystem : Utils.IModelElement, Utils.ISubDeclarator, IHoldsParagraphs
    {
        /// <summary>
        /// The dictionaries used in the system
        /// </summary>
        public List<DataDictionary.Dictionary> Dictionaries { get; private set; }

        /// <summary>
        /// The runner currently set for the system
        /// </summary>
        public Tests.Runner.Runner Runner {get; set;}

        /// <summary>
        /// Indicates wheter the model should be recompiled (after a change or a load)
        /// </summary>
        public bool ShouldRebuild { get; set; }

        /// <summary>
        /// Indicates wheter the model should be saved (after a change)
        /// </summary>
        public bool ShouldSave { get; set; }

        /// <summary>
        /// The marking history
        /// </summary>
        public MarkingHistory Markings { get; private set; }

        /// <summary>
        /// Listener to model changes
        /// </summary>
        public class BaseModelElementChangeListener : XmlBooster.IListener<DataDictionary.Generated.BaseModelElement>
        {
            /// <summary>
            /// The system for which this listener listens
            /// </summary>
            private EFSSystem System { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="system"></param>
            public BaseModelElementChangeListener(EFSSystem system)
            {
                System = system;
            }

            #region Listens to namable changes
            public void HandleChangeEvent(DataDictionary.Generated.BaseModelElement sender)
            {
                System.ShouldRebuild = true;
                System.ShouldSave = true;
            }

            public void HandleChangeEvent(XmlBooster.Lock aLock, DataDictionary.Generated.BaseModelElement sender)
            {
                HandleChangeEvent(sender);
            }
            #endregion
        }

        /// <summary>
        /// The compiler used to compile the system
        /// </summary>
        public Compiler Compiler { get; private set; }

        /// <summary>
        /// Provides the history
        /// </summary>
        public Compare.History History { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        private EFSSystem()
        {
            Dictionaries = new List<Dictionary>();

            DataDictionary.Generated.acceptor.setFactory(new DataDictionary.ObjectFactory());
            Compiler = new Interpreter.Compiler(this);
            Markings = new MarkingHistory(this);

            // Reads the history file and updates the blame information stored in it
            Compare.Factory historyFactory = DataDictionary.Compare.Factory.INSTANCE;
            History = (Compare.History)HistoricalData.HistoryUtils.Load(Compare.History.HISTORY_FILE_NAME, historyFactory);
            if (History == null)
            {
                History = (Compare.History)historyFactory.createHistory();
            }
            History.UpdateBlame();

            MaxExplainSize = 5000;
            Generated.ControllersManager.BaseModelElementController.Listeners.Insert(0, new BaseModelElementChangeListener(this));
        }

        /// <summary>
        /// Adds a new dictionary in the system
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddDictionary(Dictionary dictionary)
        {
            if (dictionary != null)
            {
                dictionary.Enclosing = this;
                Dictionaries.Add(dictionary);
            }
        }

        /// <summary>
        /// The enclosing model element
        /// </summary>
        public object Enclosing { get { return null; } set { } }

        /// <summary>
        /// The EFS System name
        /// </summary>
        public string Name
        {
            get { return "System"; }
            set { }
        }

        public string FullName
        {
            get { return Name; }
        }

        /// <summary>
        /// The sub elements of this model element
        /// </summary>
        public System.Collections.ArrayList SubElements
        {
            get
            {
                System.Collections.ArrayList retVal = new System.Collections.ArrayList();

                foreach (DataDictionary.Dictionary dictionary in Dictionaries)
                {
                    retVal.Add(dictionary);
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the collection which holds this instance
        /// </summary>
        public System.Collections.ArrayList EnclosingCollection { get { return null; } }

        /// <summary>
        /// Deletes the element from its enclosing node
        /// </summary>
        public void Delete()
        {
        }

        /// <summary>
        /// The expression text data of this model element
        /// </summary>
        /// <param name="text"></param>
        public string ExpressionText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// The messages logged on the model element
        /// </summary>
        public List<Utils.ElementLog> Messages
        {
            get { return new List<Utils.ElementLog>(); }
        }

        /// <summary>
        /// Clears the messages associated to the system
        /// </summary>
        public void ClearMessages()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Utils.IModelElement other)
        {
            if (other == this)
            {
                return 0;
            }

            return -1;
        }

        /// --------------------------------------------------
        ///   PREDEFINED ITEMS
        /// --------------------------------------------------
        /// 
        /// <summary>
        /// The predefined empty value
        /// </summary>
        private Values.EmptyValue emptyValue;
        public Values.EmptyValue EmptyValue
        {
            get
            {
                if (emptyValue == null)
                {
                    emptyValue = new Values.EmptyValue(this);
                }
                return emptyValue;
            }
        }

        /// <summary>
        /// The predefined any type
        /// </summary>
        private Types.AnyType anyType;
        public Types.AnyType AnyType
        {
            get
            {
                if (anyType == null)
                {
                    anyType = new Types.AnyType(this);
                }
                return anyType;
            }
        }

        /// <summary>
        /// The predefined no type
        /// </summary>
        private Types.NoType noType;
        public Types.NoType NoType
        {
            get
            {
                if (noType == null)
                {
                    noType = new Types.NoType(this);
                }
                return noType;
            }
        }

        /// <summary>
        /// The predefined bool type
        /// </summary>
        private Types.BoolType boolType;
        public Types.BoolType BoolType
        {
            get
            {
                if (boolType == null)
                {
                    boolType = new Types.BoolType(this);
                }
                return boolType;
            }
        }

        /// <summary>
        /// The predefined integer type
        /// </summary>
        private Types.IntegerType integerType;
        public Types.IntegerType IntegerType
        {
            get
            {
                if (integerType == null)
                {
                    integerType = new Types.IntegerType(this);
                }
                return integerType;
            }
        }

        /// <summary>
        /// The predefined double type
        /// </summary>
        private Types.DoubleType doubleType;
        public Types.DoubleType DoubleType
        {
            get
            {
                if (doubleType == null)
                {
                    doubleType = new Types.DoubleType(this);
                }
                return doubleType;
            }
        }

        /// <summary>
        /// The predefined string type
        /// </summary>
        private Types.StringType stringType;
        public Types.StringType StringType
        {
            get
            {
                if (stringType == null)
                {
                    stringType = new Types.StringType(this);
                }
                return stringType;
            }
        }

        /// <summary>
        /// The generic collection type
        /// </summary>
        /// <returns></returns>
        private Types.Collection genericCollection;
        public Types.Collection GenericCollection
        {
            get
            {
                if (genericCollection == null)
                {
                    genericCollection = new Types.GenericCollection(this);
                }

                return genericCollection;
            }
        }

        /// <summary>
        /// The predefined types
        /// </summary>
        private Dictionary<string, Types.Type> predefinedTypes;
        public Dictionary<string, Types.Type> PredefinedTypes
        {
            get
            {
                if (predefinedTypes == null)
                {
                    PredefinedTypes = new Dictionary<string, Types.Type>();
                    PredefinedTypes[BoolType.Name] = BoolType;
                    PredefinedTypes[IntegerType.Name] = IntegerType;
                    PredefinedTypes[DoubleType.Name] = DoubleType;
                    PredefinedTypes[StringType.Name] = StringType;
                }
                return predefinedTypes;
            }
            set { predefinedTypes = value; }
        }

        /// <summary>
        /// Gets the boolean value which corresponds to the bool provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Values.IValue GetBoolean(bool val)
        {
            if (val)
            {
                return BoolType.True;
            }
            else
            {
                return BoolType.False;
            }
        }

        /// <summary>
        /// The predefined allocate function
        /// </summary>
        private Functions.PredefinedFunctions.Allocate allocatePredefinedFunction;
        public Functions.PredefinedFunctions.Allocate AllocatePredefinedFunction
        {
            get
            {
                if (allocatePredefinedFunction == null)
                {
                    allocatePredefinedFunction = new Functions.PredefinedFunctions.Allocate(this);
                }
                return allocatePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined available function
        /// </summary>
        private Functions.PredefinedFunctions.Available availablePredefinedFunction;
        public Functions.PredefinedFunctions.Available AvailablePredefinedFunction
        {
            get
            {
                if (availablePredefinedFunction == null)
                {
                    availablePredefinedFunction = new Functions.PredefinedFunctions.Available(this);
                }
                return availablePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined not function
        /// </summary>
        private Functions.PredefinedFunctions.Not notPredefinedFunction;
        public Functions.PredefinedFunctions.Not NotPredefinedFunction
        {
            get
            {
                if (notPredefinedFunction == null)
                {
                    notPredefinedFunction = new Functions.PredefinedFunctions.Not(this);
                }
                return notPredefinedFunction;
            }
        }


        /// <summary>
        /// The predefined min function
        /// </summary>
        private Functions.PredefinedFunctions.Min minPredefinedFunction;
        public Functions.PredefinedFunctions.Min MinPredefinedFunction
        {
            get
            {
                if (minPredefinedFunction == null)
                {
                    minPredefinedFunction = new Functions.PredefinedFunctions.Min(this);
                }
                return minPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined MinSurface function
        /// </summary>
        private Functions.PredefinedFunctions.MinSurface minSurfacePredefinedFunction;
        public Functions.PredefinedFunctions.MinSurface MinSurfacePredefinedFunction
        {
            get
            {
                if (minSurfacePredefinedFunction == null)
                {
                    minSurfacePredefinedFunction = new Functions.PredefinedFunctions.MinSurface(this);
                }
                return minSurfacePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined max function
        /// </summary>
        private Functions.PredefinedFunctions.Max maxPredefinedFunction;
        public Functions.PredefinedFunctions.Max MaxPredefinedFunction
        {
            get
            {
                if (maxPredefinedFunction == null)
                {
                    maxPredefinedFunction = new Functions.PredefinedFunctions.Max(this);
                }
                return maxPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined targets function
        /// </summary>
        private Functions.PredefinedFunctions.Targets targetsPredefinedFunction;
        public Functions.PredefinedFunctions.Targets TargetsPredefinedFunction
        {
            get
            {
                if (targetsPredefinedFunction == null)
                {
                    targetsPredefinedFunction = new Functions.PredefinedFunctions.Targets(this);
                }
                return targetsPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined RoundToMultiple function
        /// </summary>
        private Functions.PredefinedFunctions.RoundToMultiple roundToMultiplePredefinedFunction;
        public Functions.PredefinedFunctions.RoundToMultiple RoundToMultiplePredefinedFunction
        {
            get
            {
                if (roundToMultiplePredefinedFunction == null)
                {
                    roundToMultiplePredefinedFunction = new Functions.PredefinedFunctions.RoundToMultiple(this);
                }
                return roundToMultiplePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined DoubleToInteger function
        /// </summary>
        private Functions.PredefinedFunctions.DoubleToInteger doubleToIntegerPredefinedFunction;
        public Functions.PredefinedFunctions.DoubleToInteger DoubleToIntegerPredefinedFunction
        {
            get
            {
                if (doubleToIntegerPredefinedFunction == null)
                {
                    doubleToIntegerPredefinedFunction = new Functions.PredefinedFunctions.DoubleToInteger(this);
                }
                return doubleToIntegerPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined deceleration profile function
        /// </summary>
        private Functions.PredefinedFunctions.DecelerationProfile decelerationProfilePredefinedFunction;
        public Functions.PredefinedFunctions.DecelerationProfile DecelerationProfilePredefinedFunction
        {
            get
            {
                if (decelerationProfilePredefinedFunction == null)
                {
                    decelerationProfilePredefinedFunction = new Functions.PredefinedFunctions.DecelerationProfile(this);
                }
                return decelerationProfilePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined before function
        /// </summary>
        private Functions.PredefinedFunctions.Before beforePredefinedFunction;
        public Functions.PredefinedFunctions.Before BeforePredefinedFunction
        {
            get
            {
                if (beforePredefinedFunction == null)
                {
                    beforePredefinedFunction = new Functions.PredefinedFunctions.Before(this);
                }
                return beforePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined AddIncrement function
        /// </summary>
        private Functions.PredefinedFunctions.AddIncrement addIncrementPredefinedFunction;
        public Functions.PredefinedFunctions.AddIncrement AddIncrementPredefinedFunction
        {
            get
            {
                if (addIncrementPredefinedFunction == null)
                {
                    addIncrementPredefinedFunction = new Functions.PredefinedFunctions.AddIncrement(this);
                }
                return addIncrementPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined override function
        /// </summary>
        private Functions.PredefinedFunctions.Override overridePredefinedFunction;
        public Functions.PredefinedFunctions.Override OverridePredefinedFunction
        {
            get
            {
                if (overridePredefinedFunction == null)
                {
                    overridePredefinedFunction = new Functions.PredefinedFunctions.Override(this);
                }
                return overridePredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined DistanceForSpeed function
        /// </summary>
        private Functions.PredefinedFunctions.DistanceForSpeed distanceForSpeedPredefinedFunction;
        public Functions.PredefinedFunctions.DistanceForSpeed DistanceForSpeedPredefinedFunction
        {
            get
            {
                if (distanceForSpeedPredefinedFunction == null)
                {
                    distanceForSpeedPredefinedFunction = new Functions.PredefinedFunctions.DistanceForSpeed(this);
                }
                return distanceForSpeedPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined IntersectAt function
        /// </summary>
        private Functions.PredefinedFunctions.IntersectAt intersectAtFunction;
        public Functions.PredefinedFunctions.IntersectAt IntersectAtFunction
        {
            get
            {
                if (intersectAtFunction == null)
                {
                    intersectAtFunction = new Functions.PredefinedFunctions.IntersectAt(this);
                }
                return intersectAtFunction;
            }
        }

        /// <summary>
        /// The predefined functions
        /// </summary>
        private Dictionary<string, Functions.PredefinedFunctions.PredefinedFunction> predefinedFunctions;
        public Dictionary<string, Functions.PredefinedFunctions.PredefinedFunction> PredefinedFunctions
        {
            get
            {
                if (predefinedFunctions == null)
                {
                    predefinedFunctions = new Dictionary<string, Functions.PredefinedFunctions.PredefinedFunction>();
                    predefinedFunctions[AvailablePredefinedFunction.Name] = AvailablePredefinedFunction;
                    predefinedFunctions[AllocatePredefinedFunction.Name] = AllocatePredefinedFunction;
                    predefinedFunctions[NotPredefinedFunction.Name] = NotPredefinedFunction;
                    predefinedFunctions[MinPredefinedFunction.Name] = MinPredefinedFunction;
                    predefinedFunctions[MinSurfacePredefinedFunction.Name] = MinSurfacePredefinedFunction;
                    predefinedFunctions[MaxPredefinedFunction.Name] = MaxPredefinedFunction;
                    predefinedFunctions[TargetsPredefinedFunction.Name] = TargetsPredefinedFunction;
                    predefinedFunctions[RoundToMultiplePredefinedFunction.Name] = RoundToMultiplePredefinedFunction;
                    predefinedFunctions[DoubleToIntegerPredefinedFunction.Name] = DoubleToIntegerPredefinedFunction;
                    predefinedFunctions[DecelerationProfilePredefinedFunction.Name] = DecelerationProfilePredefinedFunction;
                    predefinedFunctions[BeforePredefinedFunction.Name] = BeforePredefinedFunction;
                    predefinedFunctions[AddIncrementPredefinedFunction.Name] = AddIncrementPredefinedFunction;
                    predefinedFunctions[OverridePredefinedFunction.Name] = OverridePredefinedFunction;
                    predefinedFunctions[DistanceForSpeedPredefinedFunction.Name] = DistanceForSpeedPredefinedFunction;
                    predefinedFunctions[IntersectAtFunction.Name] = IntersectAtFunction;
                }
                return predefinedFunctions;
            }
            set { predefinedFunctions = value; }
        }


        /// <summary>
        /// All predefined items in the system
        /// </summary>
        private Dictionary<string, Utils.INamable> predefinedItems;
        public Dictionary<string, Utils.INamable> PredefinedItems
        {
            get
            {
                if (predefinedItems == null)
                {
                    predefinedItems = new Dictionary<string, Utils.INamable>();

                    foreach (KeyValuePair<string, Functions.PredefinedFunctions.PredefinedFunction> pair in PredefinedFunctions)
                    {
                        predefinedItems.Add(pair.Key, pair.Value);
                    }
                    foreach (KeyValuePair<string, Types.Type> pair in PredefinedTypes)
                    {
                        predefinedItems.Add(pair.Key, pair.Value);
                        Types.IEnumerateValues enumerator = pair.Value as Types.IEnumerateValues;
                        if (enumerator != null)
                        {
                            Dictionary<string, object> constants = new Dictionary<string, object>();
                            enumerator.Constants("", constants);
                            foreach (KeyValuePair<string, object> pair2 in constants)
                            {
                                if (pair2.Value is Utils.INamable)
                                {
                                    predefinedItems.Add(pair2.Key, (Utils.INamable)pair2.Value);
                                }
                            }
                        }
                    }

                    PredefinedItems.Add(EmptyValue.Name, EmptyValue);
                }

                return predefinedItems;
            }
            set
            {
                predefinedItems = value;
            }
        }
        /// <summary>
        /// Provides the predefined item, based on its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Utils.INamable getPredefinedItem(string name)
        {
            Utils.INamable namable = null;

            PredefinedItems.TryGetValue(name, out namable);

            return namable;
        }

        /// <summary>
        /// Indicates that at least one message of type levelEnum is attached to the element
        /// </summary>
        /// <param name="levelEnum"></param>
        /// <returns></returns>
        public bool HasMessage(Utils.ElementLog.LevelEnum levelEnum)
        {
            bool retVal = false;

            return retVal;
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            try
            {
                DataDictionary.Generated.ControllersManager.DesactivateAllNotifications();

                Utils.ISubDeclaratorUtils.AppendNamable(this, EmptyValue);
                foreach (Types.Type type in PredefinedTypes.Values)
                {
                    Utils.ISubDeclaratorUtils.AppendNamable(this, type);
                }
                foreach (Functions.PredefinedFunctions.PredefinedFunction function in PredefinedFunctions.Values)
                {
                    Utils.ISubDeclaratorUtils.AppendNamable(this, function);
                }

                // Adds the namable from the default namespace as directly accessible
                foreach (Dictionary dictionary in Dictionaries)
                {
                    foreach (NameSpace nameSpace in dictionary.NameSpaces)
                    {
                        if (nameSpace.Name.CompareTo("Default") == 0)
                        {
                            if (nameSpace.DeclaredElements == null)
                            {
                                nameSpace.InitDeclaredElements();
                            }
                            foreach (List<Utils.INamable> namables in nameSpace.DeclaredElements.Values)
                            {
                                foreach (Utils.INamable namable in namables)
                                {
                                    Utils.ISubDeclaratorUtils.AppendNamable(this, namable);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                DataDictionary.Generated.ControllersManager.ActivateAllNotifications();
            }
        }

        /// <summary>
        /// Provides the list of declared elements in this System
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
        /// Finds all namable which match the full name provided
        /// </summary>
        /// <param name="fullname">The full name used to search the namable</param>
        public Utils.INamable findByFullName(string fullname)
        {
            Utils.INamable retVal = null;

            foreach (Dictionary dictionary in Dictionaries)
            {
                retVal = dictionary.findByFullName(fullname);
                if (retVal != null)
                {
                    // TODO : only finds the first occurence of the namable in all opened dictionaries.
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the type associated to the name
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Types.Type findType(Types.NameSpace nameSpace, string name)
        {
            Types.Type retVal = null;

            if (name != null)
            {
                foreach (Dictionary dictionary in Dictionaries)
                {
                    retVal = dictionary.findType(nameSpace, name);
                    if (retVal != null)
                    {
                        break;
                    }
                }

                if (retVal == null)
                {
                    PredefinedTypes.TryGetValue(name, out retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Finds a rule according to its full name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Rules.Rule findRule(string fullName)
        {
            Rules.Rule retVal = null;

            foreach (Dictionary dictionary in Dictionaries)
            {
                retVal = dictionary.findRule(fullName);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public void AddModelElement(Utils.IModelElement element)
        {
            {
                Dictionary item = element as Dictionary;
                if (item != null)
                {
                    Dictionaries.Add(item);
                }
            }
        }

        /// <summary>
        /// Indicates whether a rule is disabled
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool isDisabled(Rules.Rule rule)
        {
            bool retVal = false;

            foreach (Dictionary dictionary in Dictionaries)
            {
                retVal = dictionary.Disabled(rule);
                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        /// The evaluator for this dictionary
        /// </summary>
        private Interpreter.Parser parser;
        public Interpreter.Parser Parser
        {
            get
            {
                if (parser == null)
                {
                    parser = new Interpreter.Parser(this);
                }
                return parser;
            }
        }

        /// <summary>
        /// Parses the statement provided
        /// </summary>
        /// <param name="root">the root element for which this statement is created</param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Interpreter.Statement.Statement ParseStatement(ModelElement root, string expression)
        {
            return Parser.Statement(root, expression);
        }

        /// <summary>
        /// The EFS System instance
        /// </summary>
        private static EFSSystem instance = null;
        public static EFSSystem INSTANCE
        {
            get
            {
                if (instance == null)
                {
                    instance = new EFSSystem();
                }
                return instance;
            }
        }

        /// <summary>
        /// Provides an RTF explanation of the system
        /// </summary>
        /// <returns></returns>
        public string getExplain()
        {
            return "";
        }

        /// <summary>
        /// The visitor who shall find all references
        /// </summary>
        private class ReferenceVisitor : Generated.Visitor
        {
            /// <summary>
            /// The references found
            /// </summary>
            public List<Usage> Usages { get; private set; }

            /// <summary>
            /// The element to be found
            /// </summary>
            private ModelElement Model { get; set; }

            /// <summary>
            /// The filter to apply to the selection
            /// </summary>
            private BaseFilter Filter { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="model">The model element to be found</param>
            public ReferenceVisitor(ModelElement model)
            {
                Usages = new List<Usage>();
                Model = model;
                Filter = null;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="filter">The filter to apply to the search</param>
            public ReferenceVisitor(BaseFilter filter)
            {
                Usages = new List<Usage>();
                Model = null;
                Filter = filter;
            }

            /// <summary>
            /// Takes an interpreter tree into consideration
            /// </summary>
            /// <param name="statement"></param>
            private void ConsiderInterpreterTreeNode(InterpreterTreeNode tree)
            {
                if (tree != null && tree.StaticUsage != null)
                {
                    if (Model != null)
                    {
                        List<Usage> usages = tree.StaticUsage.Find(Model);
                        foreach (Usage usage in usages)
                        {
                            Usages.Add(usage);
                        }
                    }
                    else
                    {
                        foreach (Usage usage in tree.StaticUsage.AllUsages)
                        {
                            if (Filter.AcceptableChoice(usage.Referenced))
                            {
                                Usages.Add(usage);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Considers the type of the element provided as parameter
            /// </summary>
            /// <param name="element">The element which uses this type</param>
            private void ConsiderTypeOfElement(ITypedElement element)
            {
                ModelElement modelElement = element as ModelElement;
                if (modelElement != null)
                {
                    if (Model != null)
                    {
                        if ((element.Type == Model) && (element != Model))
                        {
                            Usages.Add(new Usage(Model, modelElement, Usage.ModeEnum.Type));
                        }
                    }
                    else
                    {
                        if (Filter.AcceptableChoice(element.Type))
                        {
                            Usages.Add(new Usage(element.Type, modelElement, Usage.ModeEnum.Type));
                        }
                    }
                }
            }

            /// <summary>
            /// Walk through all elements
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.BaseModelElement obj, bool visitSubNodes)
            {
                IExpressionable expressionable = obj as IExpressionable;
                if (expressionable != null)
                {
                    ConsiderInterpreterTreeNode(expressionable.Tree);
                }

                ITypedElement element = obj as ITypedElement;
                if (element != null)
                {
                    ConsiderTypeOfElement(element);
                }

                Functions.Function function = obj as Functions.Function;
                if (function != null)
                {
                    if (function.ReturnType == Model && function != Model)
                    {
                        Usages.Add(new Usage(element.Type, function, Usage.ModeEnum.Type));
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///  Provides the list of references of a given model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Usage> FindReferences(ModelElement model)
        {
            List<Usage> retVal;

            if (model != null)
            {
                // Find references
                ReferenceVisitor visitor = new ReferenceVisitor(model);
                bool prev = ModelElement.BeSilent;
                try
                {
                    ModelElement.BeSilent = true;
                    foreach (Dictionary dictionary in Dictionaries)
                    {
                        visitor.visit(dictionary, true);
                    }
                    visitor.Usages.Sort();
                }
                finally
                {
                    ModelElement.BeSilent = prev;
                }

                retVal = visitor.Usages;
            }
            else
            {
                retVal = new List<Usage>();
            }

            return retVal;
        }


        /// <summary>
        ///  Provides the list of references for a given filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Usage> FindReferences(BaseFilter filter)
        {
            // Find references
            ReferenceVisitor visitor = new ReferenceVisitor(filter);
            bool prev = ModelElement.BeSilent;
            try
            {
                ModelElement.BeSilent = true;
                foreach (Dictionary dictionary in Dictionaries)
                {
                    visitor.visit(dictionary, true);
                }
                visitor.Usages.Sort();
            }
            finally
            {
                ModelElement.BeSilent = prev;
            }

            return visitor.Usages;
        }

        /// <summary>
        /// Indicates whether enclosing messages should be displayed
        /// </summary>
        public bool DisplayEnclosingMessages { get; set; }

        public bool DisplayRequirementsAsList { get; set; }

        /// <summary>
        /// Stops the system
        /// </summary>
        public void Stop()
        {
            Compiler.DoCompile = false;
        }

        /// <summary>
        /// Gets all paragraphs from EFS System
        /// </summary>
        /// <returns></returns>
        public void GetParagraphs(List<DataDictionary.Specification.Paragraph> paragraphs)
        {
            foreach (Dictionary dictionary in Dictionaries)
            {
                dictionary.GetParagraphs(paragraphs);
            }
        }

        /// <summary>
        /// Indicates if the element holds messages, or is part of a path to a message 
        /// </summary>
        public Utils.MessagePathInfoEnum MessagePathInfo { get { return Utils.MessagePathInfoEnum.Nothing; } }

        /// <summary>
        /// The maximum size of an explain part message
        /// </summary>
        public int MaxExplainSize { get; set; }

        /// <summary>
        /// Provides the list of requirement sets in the system
        /// </summary>
        public List<RequirementSet> RequirementSets
        {
            get
            {
                List<RequirementSet> retVal = new List<RequirementSet>();

                foreach (DataDictionary.Dictionary dictionary in Dictionaries)
                {
                    foreach (RequirementSet requirementSet in dictionary.RequirementSets)
                    {
                        retVal.Add(requirementSet);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Marks the requirements for a specific requirement set
        /// </summary>
        private class RequirementSetMarker : Generated.Visitor
        {
            /// <summary>
            /// The requirement set for which marking is done
            /// </summary>
            private RequirementSet RequirementSet { get; set; }

            /// <summary>
            /// Indicates if the requirement must belong to the requirement set, or not
            /// </summary>
            private bool Belonging { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="requirementSet"></param>
            /// <param name="belonging">Indicates whether the paragraph should belong to the requirement set 
            /// or whether the requirement should not belong to that requirement set</param>
            public RequirementSetMarker(RequirementSet requirementSet, bool belonging)
            {
                RequirementSet = requirementSet;
                Belonging = belonging;
            }

            /// <summary>
            /// Marks the paragraph
            /// </summary>
            /// <param name="paragraph"></param>
            /// <param name="recursively">Indicates that the paragraph should be marked recursively</param>
            /// <returns>true if marking recursively was applied</returns>
            private bool MarkBelongingParagraph(Paragraph paragraph, bool recursively)
            {
                paragraph.AddInfo("Requirement set" + RequirementSet.Name);
                if (recursively)
                {
                    foreach (Paragraph subParagraph in paragraph.SubParagraphs)
                    {
                        MarkBelongingParagraph(subParagraph, recursively);
                    }
                }

                return recursively;
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph)obj;

                if (paragraph.BelongsToRequirementSet(RequirementSet))
                {
                    if (Belonging)
                    {
                        if (!MarkBelongingParagraph(paragraph, RequirementSet.getRecursiveSelection()))
                        {
                            base.visit(obj, visitSubNodes);
                        }
                    }
                    else
                    {
                        base.visit(obj, visitSubNodes);
                    }
                }
                else
                {
                    if (!Belonging)
                    {
                        if (paragraph.getType() == Generated.acceptor.Paragraph_type.aREQUIREMENT)
                        {
                            paragraph.AddInfo("Requirement does not belong to requirement set " + RequirementSet.Name);
                        }
                    }
                    base.visit(obj, visitSubNodes);
                }
            }
        }

        /// <summary>
        /// Marks the requirements which relate to the corresponding requirement set
        /// </summary>
        /// <param name="requirementSet"></param>
        public void MarkRequirementsForRequirementSet(RequirementSet requirementSet)
        {
            RequirementSetMarker marker = new RequirementSetMarker(requirementSet, true);
            foreach (DataDictionary.Dictionary dictionary in Dictionaries)
            {
                dictionary.ClearMessages();
                marker.visit(dictionary);
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
        }

        /// <summary>
        /// Marks the requirements which relate to the corresponding requirement set
        /// </summary>
        /// <param name="requirementSet"></param>
        public void MarkRequirementsWhichDoNotBelongToRequirementSet(RequirementSet requirementSet)
        {
            RequirementSetMarker marker = new RequirementSetMarker(requirementSet, false);
            foreach (DataDictionary.Dictionary dictionary in Dictionaries)
            {
                dictionary.ClearMessages();
                marker.visit(dictionary);
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
        }

        /// <summary>
        /// Provides the requirement set whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RequirementSet findRequirementSet(string name)
        {
            RequirementSet retVal = null;

            foreach (DataDictionary.Dictionary dictionary in Dictionaries)
            {
                foreach (RequirementSet requirementSet in dictionary.RequirementSets)
                {
                    if (requirementSet.Name == name)
                    {
                        retVal = requirementSet;
                        break;
                    }
                }
            }

            return retVal;
        }
    }
}
