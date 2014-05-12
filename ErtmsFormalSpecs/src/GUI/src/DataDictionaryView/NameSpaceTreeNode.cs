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
using System.Windows.Forms;

namespace GUI.DataDictionaryView
{
    public class NameSpaceTreeNode : ModelElementTreeNode<DataDictionary.Types.NameSpace>
    {
        private class ItemEditor : CommentableEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        NameSpaceSubNameSpacesTreeNode subNameSpaces;
        RangesTreeNode ranges;
        EnumerationsTreeNode enumerations;
        StructuresTreeNode structures;
        CollectionsTreeNode collections;
        StateMachinesTreeNode stateMachines;
        FunctionsTreeNode functions;
        NameSpaceProceduresTreeNode procedures;
        NameSpaceVariablesTreeNode variables;
        NameSpaceRulesTreeNode rules;

        bool isDirectory = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        public NameSpaceTreeNode(DataDictionary.Types.NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            subNameSpaces = new NameSpaceSubNameSpacesTreeNode(Item, buildSubNodes);
            ranges = new RangesTreeNode(Item, buildSubNodes);
            enumerations = new EnumerationsTreeNode(Item, buildSubNodes);
            structures = new StructuresTreeNode(Item, buildSubNodes);
            collections = new CollectionsTreeNode(Item, buildSubNodes);
            stateMachines = new StateMachinesTreeNode(Item, buildSubNodes);
            functions = new FunctionsTreeNode(Item, buildSubNodes);
            procedures = new NameSpaceProceduresTreeNode(Item, buildSubNodes);
            variables = new NameSpaceVariablesTreeNode(Item, buildSubNodes);
            rules = new NameSpaceRulesTreeNode(Item, buildSubNodes);

            Nodes.Add(subNameSpaces);
            Nodes.Add(ranges);
            Nodes.Add(enumerations);
            Nodes.Add(structures);
            Nodes.Add(collections);
            Nodes.Add(stateMachines);
            Nodes.Add(functions);
            Nodes.Add(procedures);
            Nodes.Add(variables);
            Nodes.Add(rules);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        protected NameSpaceTreeNode(DataDictionary.Types.NameSpace item, bool buildSubNodes, string name, bool isFolder)
            : base(item, buildSubNodes, name, isFolder)
        {
            isDirectory = true;
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        private void AddNamespaceHandler(object sender, EventArgs args)
        {
            subNameSpaces.AddHandler(sender, args);
        }

        private void AddRangeHandler(object sender, EventArgs args)
        {
            ranges.AddHandler(sender, args);
        }

        private void AddEnumerationHandler(object sender, EventArgs args)
        {
            enumerations.AddHandler(sender, args);
        }

        private void AddStructureHandler(object sender, EventArgs args)
        {
            structures.AddHandler(sender, args);
        }

        private void AddCollectionHandler(object sender, EventArgs args)
        {
            collections.AddHandler(sender, args);
        }

        private void AddStateMachineHandler(object sender, EventArgs args)
        {
            stateMachines.AddHandler(sender, args);
        }

        private void AddFunctionHandler(object sender, EventArgs args)
        {
            functions.AddHandler(sender, args);
        }

        private void AddProcedureHandler(object sender, EventArgs args)
        {
            procedures.AddHandler(sender, args);
        }

        private void AddVariableHandler(object sender, EventArgs args)
        {
            variables.AddHandler(sender, args);
        }

        private void AddRuleHandler(object sender, EventArgs args)
        {
            rules.AddHandler(sender, args);
        }


        /// <summary>
        /// Adds a namespace in the corresponding namespace
        /// </summary>
        /// <param name="nameSpace"></param>
        public NameSpaceTreeNode AddNameSpace(DataDictionary.Types.NameSpace nameSpace)
        {
            return subNameSpaces.AddSubNameSpace(nameSpace);
        }

        /// <summary>
        /// Shows the functional view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ShowFunctionalViewHandler(object sender, EventArgs args)
        {
            FunctionalView.FunctionalAnalysisWindow window = new FunctionalView.FunctionalAnalysisWindow();
            GUIUtils.MDIWindow.AddChildWindow(window);
            window.SetNameSpaceContainer(Item);
            window.Text = Item.Name + " functional view";
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Namespace", new EventHandler(AddNamespaceHandler)));
            newItem.MenuItems.Add(new MenuItem("Range", new EventHandler(AddRangeHandler)));
            newItem.MenuItems.Add(new MenuItem("Enumeration", new EventHandler(AddEnumerationHandler)));
            newItem.MenuItems.Add(new MenuItem("Structure", new EventHandler(AddStructureHandler)));
            newItem.MenuItems.Add(new MenuItem("Collection", new EventHandler(AddCollectionHandler)));
            newItem.MenuItems.Add(new MenuItem("State machine", new EventHandler(AddStateMachineHandler)));
            newItem.MenuItems.Add(new MenuItem("Function", new EventHandler(AddFunctionHandler)));
            newItem.MenuItems.Add(new MenuItem("Procedure", new EventHandler(AddProcedureHandler)));
            newItem.MenuItems.Add(new MenuItem("Variable", new EventHandler(AddVariableHandler)));
            newItem.MenuItems.Add(new MenuItem("Rule", new EventHandler(AddRuleHandler)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(5, new MenuItem("-"));
            retVal.Insert(6, new MenuItem("Functional view", new EventHandler(ShowFunctionalViewHandler)));

            return retVal;
        }

        /// <summary>
        /// Accepts a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (isDirectory)
            {
                BaseTreeNode parent = Parent as BaseTreeNode;
                parent.AcceptDrop(SourceNode);
            }
            else
            {
                if (SourceNode is VariableTreeNode)
                {
                    variables.AcceptDrop(SourceNode);
                }
                else if (SourceNode is ProcedureTreeNode)
                {
                    procedures.AcceptDrop(SourceNode);
                }
                else if (SourceNode is RuleTreeNode)
                {
                    rules.AcceptDrop(SourceNode);
                }
                else if (SourceNode is StructureTreeNode)
                {
                    structures.AcceptDrop(SourceNode);
                }
                else if (SourceNode is NameSpaceTreeNode)
                {
                    DialogResult result = MessageBox.Show("This will move the namespace, are you sure ? ", "Confirm moving the namespace", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        NameSpaceTreeNode nameSpaceTreeNode = SourceNode as NameSpaceTreeNode;
                        DataDictionary.Types.NameSpace nameSpace = nameSpaceTreeNode.Item;

                        nameSpaceTreeNode.Delete();
                        AddNameSpace(nameSpace);
                    }
                }
            }
        }


        /// <summary>
        /// Update counts according to the selected namespace
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            List<DataDictionary.Types.NameSpace> namespaces = new List<DataDictionary.Types.NameSpace>();
            namespaces.Add(Item);
            Window window = BaseForm as Window;
            if (window != null && window.modelDiagramPanel.Model != Item)
            {
                window.modelDiagramPanel.Model = Item;
                window.modelDiagramPanel.RefreshControl();
            }

            GUIUtils.MDIWindow.SetStatus(CreateStatMessage(namespaces, false));
        }


        /// <summary>
        /// Creates the stat message according to the list of namespaces provided
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static string CreateStatMessage(List<DataDictionary.Types.NameSpace> namespaces, bool isFolder)
        {
            string result = "";

            int ranges = 0;
            int enumerations = 0;
            int structures = 0;
            int collections = 0;
            int functions = 0;
            int procedures = 0;
            int variables = 0;
            int rules = 0;

            List<DataDictionary.Types.NameSpace> allNamespaces = new List<DataDictionary.Types.NameSpace>();
            foreach (DataDictionary.Types.NameSpace aNamespace in namespaces)
            {
                allNamespaces.AddRange(collectNamespaces(aNamespace));
            }

            foreach (DataDictionary.Types.NameSpace aNamespace in allNamespaces)
            {
                ranges += aNamespace.Ranges.Count;
                enumerations += aNamespace.Enumerations.Count;
                structures += aNamespace.Structures.Count;
                collections += aNamespace.Collections.Count;
                functions += aNamespace.Functions.Count;
                procedures += aNamespace.Procedures.Count;
                variables += aNamespace.Variables.Count;
                rules += aNamespace.Rules.Count;
            }

            if (!isFolder)
            {
                result += "The namespace " + namespaces[0].Name + " contains ";
            }
            else
            {
                result += namespaces.Count + (namespaces.Count > 1 ? " namespaces " : " namespace ") + "selected, containing ";
            }

            result += (allNamespaces.Count - namespaces.Count) + (allNamespaces.Count - namespaces.Count > 1 ? " sub-namespaces, " : " sub-namespace, ") +
                      ranges + (ranges > 1 ? " ranges, " : " range, ") +
                      enumerations + (enumerations > 1 ? " enumerations, " : " enumeration, ") +
                      structures + (structures > 1 ? " structures, " : " structure, ") +
                      collections + (collections > 1 ? " collections, " : " collection, ") +
                      functions + (functions > 1 ? " functions, " : " function, ") +
                      procedures + (procedures > 1 ? " procedures, " : " procedure, ") +
                      variables + (variables > 1 ? " variables and " : " variable and ") +
                      rules + (rules > 1 ? " rules." : " rule.");

            return result;

        }


        private static List<DataDictionary.Types.NameSpace> collectNamespaces(DataDictionary.Types.NameSpace aNamespace)
        {
            List<DataDictionary.Types.NameSpace> result = new List<DataDictionary.Types.NameSpace>();
            result.Add(aNamespace);
            foreach (DataDictionary.Types.NameSpace aSubNamespace in aNamespace.NameSpaces)
            {
                result.AddRange(collectNamespaces(aSubNamespace));
            }
            return result;
        }
    }
}
