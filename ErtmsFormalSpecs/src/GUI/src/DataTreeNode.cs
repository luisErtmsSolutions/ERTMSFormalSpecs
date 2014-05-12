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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Specification;
using GUI.SpecificationView;
using System.Drawing.Design;

namespace GUI
{
    /// <summary>
    /// The base class for all tree nodes
    /// </summary>
    public class BaseTreeNode : TreeNode, IComparable<BaseTreeNode>
    {
        /// <summary>
        /// The editor for this tree node
        /// </summary>
        public class BaseEditor
        {
            /// <summary>
            /// The model element currently edited
            /// </summary>
            [Browsable(false)]
            public Utils.IModelElement Model { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="model"></param>
            protected BaseEditor()
            {
            }
        }

        /// <summary>
        /// The editor used to edit the node contents
        /// </summary>
        public BaseEditor NodeEditor { get; set; }

        /// <summary>
        /// The fixed node name
        /// </summary>
        private string defaultName;
        private string DefaultName
        {
            get { return defaultName; }
            set { defaultName = value; }
        }

        /// <summary>
        /// The model represented by this node
        /// </summary>
        public Utils.IModelElement Model { get; set; }

        /// <summary>
        /// Provides the base tree view which holds this node
        /// </summary>
        public BaseTreeView BaseTreeView
        {
            get
            {
                return TreeView as BaseTreeView;
            }
        }

        /// <summary>
        /// Provides the base form which holds this node
        /// </summary>
        public IBaseForm BaseForm
        {
            get
            {
                IBaseForm retVal = null;

                BaseTreeView treeView = BaseTreeView;
                if (treeView != null)
                {
                    retVal = treeView.ParentForm;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BaseTreeNode(Utils.IModelElement value, string name = null, bool isFolder = false)
            : base(name)
        {
            Model = value;

            if (name != null)
            {
                DefaultName = name;
            }

            setImageIndex(isFolder);
            RefreshNode();
        }

        /// <summary>
        /// Indicates that the subNodes have already been built, hence, does not require to build its contents anymore
        /// </summary>
        public bool SubNodesBuilt = false;

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public virtual void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();
            SubNodesBuilt = true;
        }

        /// <summary>
        /// Changes the image index
        /// </summary>
        /// <param name="value"></param>
        protected void ChangeImageIndex(int value)
        {
            ImageIndex = value;
            SelectedImageIndex = value;
        }

        /// <summary>
        /// Sets the image index for this node
        /// </summary>
        /// <param name="isFolder">Indicates whether this item represents a folder</param>
        public virtual void setImageIndex(bool isFolder)
        {
            if (ImageIndex == -1)
            {
                // Image index not yet selected
                ChangeImageIndex(BaseTreeView.ModelImageIndex);

                if (isFolder)
                {
                    ChangeImageIndex(BaseTreeView.ClosedFolderImageIndex);
                }
                else
                {
                    Utils.IModelElement element = Model;
                    while (element != null && ImageIndex == BaseTreeView.ModelImageIndex)
                    {
                        element = element.Enclosing as Utils.IModelElement;
                        if (element is DataDictionary.Tests.Frame
                            || element is DataDictionary.Tests.SubSequence
                            || element is DataDictionary.Tests.TestCase
                            || element is DataDictionary.Tests.Step)
                        {
                            ChangeImageIndex(BaseTreeView.TestImageIndex);
                        }

                        if (element is DataDictionary.Specification.Specification
                            || element is DataDictionary.Specification.Chapter
                            || element is DataDictionary.Specification.Paragraph)
                        {
                            ChangeImageIndex(BaseTreeView.RequirementImageIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called before the selection changes
        /// </summary>
        public virtual void BeforeSelectionChange()
        {
        }

        /// <summary>
        /// Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public virtual void SelectionChanged(bool displayStatistics)
        {
            if (Model != null && BaseTreeView != null && BaseTreeView.RefreshNodeContent)
            {
                IBaseForm baseForm = BaseForm;
                if (baseForm != null)
                {
                    RefreshViewAccordingToModel(baseForm, false);
                }
            }

            if (displayStatistics && !isShortCut())
            {
                GUIUtils.MDIWindow.SetCoverageStatus(EFSSystem.INSTANCE);
            }

            ModelElement modelElement = Model as ModelElement;
            if (modelElement != null && GUIUtils.MDIWindow.HistoryWindow != null)
            {
                if (!isShortCut())
                {
                    GUIUtils.MDIWindow.HistoryWindow.Model = modelElement;
                }
            }
        }

        /// <summary>
        /// Indicates that the model is related to a shortcut
        /// </summary>
        /// <returns></returns>
        private bool isShortCut()
        {
            return Utils.EnclosingFinder<DataDictionary.Shortcuts.ShortcutDictionary>.find(Model) != null
                || (Model is DataDictionary.Shortcuts.ShortcutDictionary);
        }

        /// <summary>
        /// Indicates whether the explain box should be displayed
        /// </summary>
        /// <returns></returns>
        private bool ShouldExplain()
        {
            bool retVal = (Model is DataDictionary.Types.IDefaultValueElement) || !(Model is IExpressionable);

            return retVal;
        }

        /// <summary>
        /// Refreshes the view according to the model
        /// </summary>
        /// <param name="baseForm"></param>
        /// <param name="ignoreFocused"></param>
        public void RefreshViewAccordingToModel(IBaseForm baseForm, bool ignoreFocused)
        {
            if (baseForm.subTreeView != null)
            {
                baseForm.subTreeView.SetRoot(Model);
            }

            // By default, the explain text box is visible
            if (baseForm.ExplainTextBox != null && ShouldExplain())
            {
                if (!(baseForm.ExplainTextBox.ContainsFocus && ignoreFocused))
                {
                    baseForm.ExplainTextBox.SetModel(Model);
                    if (!baseForm.ExplainTextBox.Visible)
                    {
                        baseForm.ExplainTextBox.Visible = true;
                        if (baseForm.ExpressionEditorTextBox != null)
                        {
                            baseForm.ExpressionEditorTextBox.Visible = false;
                        }
                    }
                }
            }

            if (baseForm.RequirementsTextBox != null && !ignoreFocused)
            {
                string requirements = "";

                ReqRef reqRef = Model as ReqRef;
                if (reqRef != null && reqRef.Paragraph != null)
                {
                    requirements = reqRef.RequirementDescription();
                }
                else
                {
                    ReqRelated reqRelated = Utils.EnclosingFinder<ReqRelated>.find(Model, true);
                    if (reqRelated != null)
                    {
                        requirements = reqRelated.RequirementDescription();
                    }
                }

                baseForm.RequirementsTextBox.Text = requirements;
            }

            // Display the expression editor instead of the explain text box when the element can hold an expression
            if (baseForm.ExpressionEditorTextBox != null)
            {
                if (!(baseForm.ExpressionEditorTextBox.ContainsFocus && ignoreFocused))
                {
                    IExpressionable expressionable = Model as IExpressionable;
                    if (expressionable != null && !ShouldExplain())
                    {
                        baseForm.ExpressionEditorTextBox.Instance = Model as DataDictionary.ModelElement;
                        baseForm.ExpressionEditorTextBox.Text = expressionable.ExpressionText;
                        if (!baseForm.ExpressionEditorTextBox.Visible)
                        {
                            baseForm.ExpressionEditorTextBox.Visible = true;
                            baseForm.ExplainTextBox.Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles a double click event on this tree node
        /// </summary>
        public virtual void DoubleClickHandler()
        {
            // By default, nothing to do
        }

        /// <summary>
        /// Compares two tree nodes, used by the sort
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BaseTreeNode other)
        {
            if (Model != null && other.Model != null)
            {
                return Model.CompareTo(other.Model);
            }
            else
            {
                return Text.CompareTo(other.Text);
            }
        }

        /// <summary>
        /// Updates the node color according to the associated messages
        /// </summary>
        public virtual void UpdateColor()
        {
            System.Drawing.Color color = ComputedColor;

            if (color != ForeColor)
            {
                ForeColor = color;
            }

            foreach (BaseTreeNode node in Nodes)
            {
                node.UpdateColor();
            }
        }

        /// <summary>
        /// The colors used to display things
        /// </summary>
        private System.Drawing.Color ERROR_COLOR = System.Drawing.Color.Red;
        private System.Drawing.Color PATH_TO_ERROR_COLOR = System.Drawing.Color.Orange;
        private System.Drawing.Color WARNING_COLOR = System.Drawing.Color.Brown;
        private System.Drawing.Color PATH_TO_WARNING_COLOR = System.Drawing.Color.LightCoral;
        private System.Drawing.Color INFO_COLOR = System.Drawing.Color.Blue;
        private System.Drawing.Color PATH_TO_INFO_COLOR = System.Drawing.Color.LightBlue;
        private System.Drawing.Color NOTHING_COLOR = System.Drawing.Color.Black;

        /// <summary>
        /// Provides the color according to the info status
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected System.Drawing.Color ColorBasedOnInfo(Utils.MessagePathInfoEnum info)
        {
            System.Drawing.Color retVal = NOTHING_COLOR;

            switch (info)
            {
                case Utils.MessagePathInfoEnum.Error:
                    retVal = ERROR_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.PathToError:
                    retVal = PATH_TO_ERROR_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.Warning:
                    retVal = WARNING_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.PathToWarning:
                    retVal = PATH_TO_WARNING_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.Info:
                    retVal = INFO_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.PathToInfo:
                    retVal = PATH_TO_INFO_COLOR;
                    break;

                case Utils.MessagePathInfoEnum.Nothing:
                case Utils.MessagePathInfoEnum.NotComputed:
                    retVal = NOTHING_COLOR;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the path to a message info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Utils.MessagePathInfoEnum PathTo(Utils.MessagePathInfoEnum info)
        {
            Utils.MessagePathInfoEnum retVal = info;

            if (info == Utils.MessagePathInfoEnum.Error)
            {
                retVal = Utils.MessagePathInfoEnum.PathToError;
            }
            else if (info == Utils.MessagePathInfoEnum.Warning)
            {
                retVal = Utils.MessagePathInfoEnum.PathToWarning;
            }
            else if (info == Utils.MessagePathInfoEnum.Info)
            {
                retVal = Utils.MessagePathInfoEnum.PathToInfo;
            }

            return retVal;
        }

        /// <summary>
        /// Combines two colors
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private Utils.MessagePathInfoEnum CombineInfo(Utils.MessagePathInfoEnum info1, Utils.MessagePathInfoEnum info2)
        {
            Utils.MessagePathInfoEnum retVal;

            if (info1 < info2)
            {
                retVal = info2;
            }
            else
            {
                retVal = info1;
            }

            return retVal;
        }

        /// <summary>
        /// Computes this node's color based on its sub nodes
        /// </summary>
        /// <returns></returns>
        protected System.Drawing.Color ComputeColorBasedOnItsSubNodes()
        {
            if (!SubNodesBuilt)
            {
                BuildSubNodes(false);
                UpdateColor();
            }

            Utils.MessagePathInfoEnum retVal = Utils.MessagePathInfoEnum.Nothing;
            foreach (BaseTreeNode subNode in Nodes)
            {
                retVal = CombineInfo(retVal, PathTo(subNode.Model.MessagePathInfo));
            }

            return ColorBasedOnInfo(retVal);
        }

        /// <summary>
        /// Provides the computed color
        /// </summary>
        public virtual System.Drawing.Color ComputedColor
        {
            get
            {
                return System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// Updates the node name text according to the modelized item
        /// </summary>
        public virtual void UpdateText()
        {
            string name = "";
            if (DefaultName != null)
            {
                name = DefaultName;
            }
            else
            {
                if (Model != null)
                {
                    name = Model.Name;
                }
            }
            if (Text != name && !Utils.Utils.isEmpty(name))
            {
                Text = name;
            }
        }

        /// <summary>
        /// Deletes the item modelised by this tree node
        /// </summary>
        public virtual void Delete()
        {
            BaseTreeNode parent = Parent as BaseTreeNode;
            if ((parent != null) && (parent.Nodes != null))
            {
                parent.Nodes.Remove(this);
                Model.Delete();

                if (Model is DataDictionary.ReqRelated)
                {
                    DataDictionary.ReqRelated reqRelated = (DataDictionary.ReqRelated)Model;
                    reqRelated.setVerified(false);
                }

                DataDictionary.Generated.ControllersManager.BaseModelElementController.alertChange(null, null);
            }
            else
            {
                Model.Delete();
            }
        }

        /// <summary>
        /// Deletes the selected item
        /// </summary>
        public virtual void DeleteHandler(object sender, EventArgs args)
        {
            Delete();
        }

        /// <summary>
        /// Marks all model elements as implemented
        /// </summary>
        private class MarkAsImplementedVisitor : DataDictionary.Generated.Visitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public MarkAsImplementedVisitor(Utils.IModelElement element)
            {
                if (element is DataDictionary.ModelElement)
                {
                    visit((DataDictionary.ModelElement)element);
                    dispatch((DataDictionary.ModelElement)element);
                }
            }

            /// <summary>
            /// Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(DataDictionary.Generated.ReqRelated obj, bool visitSubNodes)
            {
                obj.setImplemented(true);

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        /// Recursively marks all model elements as implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Check(object sender, EventArgs e)
        {
            GUIUtils.MDIWindow.ClearMarks();
            DataDictionary.ModelElement modelElement = Model as DataDictionary.ModelElement;
            if (modelElement != null)
            {
                RuleCheckerVisitor visitor = new RuleCheckerVisitor(modelElement.Dictionary);

                visitor.visit(modelElement, true);
            }
        }

        /// <summary>
        /// Recursively marks all model elements as implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MarkAsImplemented(object sender, EventArgs e)
        {
            MarkAsImplementedVisitor visitor = new MarkAsImplementedVisitor(Model);
        }

        /// <summary>
        /// Marks all model elements as verified
        /// </summary>
        private class MarkAsVerifiedVisitor : DataDictionary.Generated.Visitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public MarkAsVerifiedVisitor(Utils.IModelElement element)
            {
                if (element is DataDictionary.ModelElement)
                {
                    visit((DataDictionary.ModelElement)element);
                    dispatch((DataDictionary.ModelElement)element);
                }
            }

            /// <summary>
            /// Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(DataDictionary.Generated.ReqRelated obj, bool visitSubNodes)
            {
                obj.setVerified(true);

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        /// Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MarkAsVerified(object sender, EventArgs e)
        {
            MarkAsVerifiedVisitor visitor = new MarkAsVerifiedVisitor(Model);
        }

        /// <summary>
        /// Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RefreshNodeHandler(object sender, EventArgs e)
        {
            RefreshNode();
        }

        /// <summary>
        /// Launches label editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void LabelEditHandler(object sender, EventArgs args)
        {
            BeginEdit();
        }

        /// <summary>
        /// Selects this element to display its history in the history view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ShowHistoryHandler(object sender, EventArgs args)
        {
            if (GUIUtils.MDIWindow.HistoryWindow != null)
            {
                GUIUtils.MDIWindow.HistoryWindow.Model = Model;
            }
        }

        /// <summary>
        /// Provides the menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected virtual List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Rename", new EventHandler(LabelEditHandler)));
            retVal.Add(new MenuItem("-"));
            MenuItem newItem = new MenuItem("Recursive actions...");
            newItem.MenuItems.Add(new MenuItem("Mark as implemented", new EventHandler(MarkAsImplemented)));
            newItem.MenuItems.Add(new MenuItem("Mark as verified", new EventHandler(MarkAsVerified)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Show history", new EventHandler(ShowHistoryHandler)));
            retVal.Add(new MenuItem("Check", new EventHandler(Check)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Refresh", new EventHandler(RefreshNodeHandler)));

            return retVal;
        }

        /// <summary>
        /// Provides the context menu for this item
        /// </summary>
        public override ContextMenu ContextMenu
        {
            get
            {
                return new ContextMenu(GetMenuItems().ToArray());
            }
        }

        /// <summary>
        /// Provides the sub node whose text matches the string provided as parameter
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public BaseTreeNode findSubNode(string text)
        {
            BaseTreeNode retVal = null;

            foreach (TreeNode node in Nodes)
            {
                if (node.Text.CompareTo(text) == 0)
                {
                    retVal = node as BaseTreeNode;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// refreshes the node text and color
        /// </summary>
        public virtual void RefreshNode()
        {
            UpdateText();
        }

        /// <summary>
        /// Clears messages for all nodes under this node
        /// </summary>
        public void ClearMessages()
        {
            Model.Messages.Clear();
            foreach (BaseTreeNode node in Nodes)
            {
                node.ClearMessages();
            }
        }

        /// <summary>
        /// Sort the sub nodes of this node
        /// </summary>
        public virtual void SortSubNodes()
        {
            List<BaseTreeNode> subNodes = new List<BaseTreeNode>();

            foreach (BaseTreeNode node in Nodes)
            {
                subNodes.Add(node);
            }
            subNodes.Sort();

            Nodes.Clear();
            foreach (BaseTreeNode node in subNodes)
            {
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptDrop(BaseTreeNode SourceNode)
        {
        }

        /// <summary>
        /// Generates new GUID for the element
        /// </summary>
        private class RegererateGuidVisitor : DataDictionary.Generated.Visitor
        {
            /// <summary>
            /// Ensures that all elements have a new Guid
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(DataDictionary.Generated.BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement)obj;

                // Side effect : creates a new Guid if it is empty
                element.setGuid(null);
                string guid = element.Guid;

                base.visit(obj, visitSubNodes);
            }

        }

        /// <summary>
        /// Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptCopy(BaseTreeNode SourceNode)
        {
            XmlBooster.XmlBBase xmlBBase = SourceNode.Model as XmlBooster.XmlBBase;
            if (xmlBBase != null)
            {
                string data = xmlBBase.ToXMLString();
                XmlBooster.XmlBStringContext ctxt = new XmlBooster.XmlBStringContext(data);
                try
                {
                    DataDictionary.ModelElement copy = DataDictionary.Generated.acceptor.accept(ctxt) as DataDictionary.ModelElement;
                    RegererateGuidVisitor visitor = new RegererateGuidVisitor();
                    visitor.visit(copy, true);

                    Model.AddModelElement(copy);
                    System.Collections.ArrayList targetCollection = copy.EnclosingCollection;
                    copy.Delete();
                    Utils.INamable namable = copy as Utils.INamable;
                    if (namable != null && targetCollection != null)
                    {
                        int previousIndex = -1;
                        int index = 0;
                        while (previousIndex != index)
                        {
                            previousIndex = index;
                            foreach (Utils.INamable other in targetCollection)
                            {
                                if (index > 0)
                                {
                                    if (other.Name.Equals(namable.Name + "_" + index))
                                    {
                                        index += 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (other.Name.Equals(namable.Name))
                                    {
                                        index += 1;
                                        break;
                                    }
                                }
                            }
                        }

                        // Renaming is mandatory
                        if (index > 0)
                        {
                            namable.Name = namable.Name + "_" + index;
                        }
                    }

                    Model.AddModelElement(copy);
                    Nodes.Clear();
                    BuildSubNodes(true);
                    UpdateColor();
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot copy element\n" + data);
                }
            }
        }

        /// <summary>
        /// Accepts the move of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptMove(BaseTreeNode SourceNode)
        {
            System.Collections.ArrayList SourceCollection = SourceNode.Model.EnclosingCollection;
            System.Collections.ArrayList ThisCollection = Model.EnclosingCollection;

            if (ThisCollection != null && SourceCollection == ThisCollection)
            {
                // This is a reordering
                int sourceIndex = ThisCollection.IndexOf(SourceNode.Model);
                int thisIndex = ThisCollection.IndexOf(Model);
                if (thisIndex >= 0 && thisIndex >= 0 && thisIndex != sourceIndex)
                {
                    ThisCollection.Remove(SourceNode.Model);
                    thisIndex = ThisCollection.IndexOf(Model);
                    ThisCollection.Insert(thisIndex, SourceNode.Model);

                    BaseTreeNode parentNode = Parent as BaseTreeNode;
                    if (parentNode != null)
                    {
                        parentNode.Nodes.Clear();
                        parentNode.BuildSubNodes(true);
                        parentNode.UpdateColor();
                    }
                    else
                    {
                        GUIUtils.MDIWindow.RefreshModel();
                    }
                }
            }
        }

        /// <summary>
        /// Called when an expand event is performed in this tree node
        /// </summary>
        public virtual void HandleExpand()
        {
            if (ImageIndex == BaseTreeView.ClosedFolderImageIndex)
            {
                ChangeImageIndex(BaseTreeView.ExpandedFolderImageIndex);
            }
        }

        /// <summary>
        /// Called when a collapse event is performed in this tree node
        /// </summary>
        public virtual void HandleCollapse()
        {
            if (ImageIndex == BaseTreeView.ExpandedFolderImageIndex)
            {
                ChangeImageIndex(BaseTreeView.ClosedFolderImageIndex);
            }
        }

        /// <summary>
        /// Called when a label edit event is performed in this tree node
        /// </summary>
        public void HandleLabelEdit(string newLabel)
        {
            if (newLabel != null && newLabel != "")
            {
                if (Model.Name != newLabel)
                {
                    EFSSystem.INSTANCE.Compiler.Compile_Synchronous(false, true);
                    Model.Name = newLabel;
                    EFSSystem.INSTANCE.Compiler.Refactor(Model as ModelElement);
                }
            }
        }
    }

    /// <summary>
    /// A tree node which hold a reference to a data item. 
    /// This item can be edited by a PropertyGrid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ModelElementTreeNode<T> : BaseTreeNode
        where T : class, Utils.IModelElement
    {
        /// <summary>
        /// An editor for an item. It is the responsibility of this class to implement attributes 
        /// for the elements to be edited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Editor : BaseTreeNode.BaseEditor
        {
            /// <summary>
            /// The item that is edited. 
            /// </summary>
            private T item;

            [Browsable(false)]
            public T Item
            {
                get { return item; }
                set
                {
                    item = value;
                    Model = value;
                    UpdateActivation();
                }
            }

            /// <summary>
            /// The node that holds the item. 
            /// </summary>
            private ModelElementTreeNode<T> node;
            internal ModelElementTreeNode<T> Node
            {
                get { return node; }
                set { node = value; }
            }

            public void RefreshNode()
            {
                Node.RefreshNode();
            }

            public void RefreshTree()
            {
                Node.BaseTreeView.Refresh();
            }

            /// <summary>
            /// Constructor
            /// </summary>
            protected Editor()
                : base()
            {
            }

            /// <summary>
            /// Updates the field activation according to the displayed data 
            /// </summary>
            protected virtual void UpdateActivation()
            {
            }

            /// <summary>
            /// Updates the activation of a single field
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            protected void UpdateFieldActivation(string name, bool value)
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[name];
                ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
                FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldToChange.SetValue(attribute, value);
            }
        }

        /// <summary>
        /// The element that is represented by this tree node
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The element to be represented by this tree node</param>
        /// <param name="buildSubNodes">Indicates that subnodes should also be built</param>
        /// <param name="name">The display name of the node</param>
        /// <param name="isFolder">Indicates whether this node is a folder</param>
        protected ModelElementTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false)
            : base(item, name, isFolder)
        {
            Item = item;
            BaseBuildSubNodes(buildSubNodes);
        }

        /// <summary>
        /// Builds the sub nodes of this node if required
        /// </summary>
        /// <param name="buildSubNodes"></param>
        protected void BaseBuildSubNodes(bool buildSubNodes)
        {
            if (buildSubNodes)
            {
                BuildSubNodes(false);
                UpdateColor();
                RefreshNode();
            }
        }

        /// <summary>
        /// Lazy create the subnodes when it is expanded
        /// </summary>
        public override void HandleExpand()
        {
            foreach (BaseTreeNode node in Nodes)
            {
                if (!node.SubNodesBuilt)
                {
                    node.BuildSubNodes(false);
                    node.UpdateColor();
                }
            }
            RefreshNode();


            base.HandleExpand();
        }


        /// <summary>
        /// An editor for an item. It is the responsibility of this class to implement attributes 
        /// for the elements to be edited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class NamedEditor : ModelElementTreeNode<T>.Editor
        {
            /// <summary>
            /// The item name
            /// </summary>
            [Category("Description")]
            public virtual string Name
            {
                get { return Item.Name; }
                set
                {
                    if (Item.EnclosingCollection != null)
                    {
                        bool unique = true;
                        foreach (Utils.IModelElement model in Item.EnclosingCollection)
                        {
                            Utils.INamable namable = model as Utils.INamable;
                            Utils.INamable namableItem = Item as Utils.INamable;
                            if (namable != namableItem && namable != null && namable.Name.CompareTo(value) == 0)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique)
                        {
                            Item.Name = value;
                        }
                        else
                        {
                            MessageBox.Show("Cannot set the name to " + value + "because it conflits with another element of the same collection", "Name conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Item.Name = value;
                    }
                    RefreshNode();
                }
            }

            /// <summary>
            /// Provides the unique identifier
            /// </summary>
            [Category("Meta data")]
            public virtual string UniqueIdentifier
            {
                get { return Item.FullName; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            protected NamedEditor()
            {
            }
        }

        /// <summary>
        /// The editor for a namable which can hold a comment
        /// </summary>
        public abstract class CommentableEditor : NamedEditor
        {
            [Category("Description")]
            [System.ComponentModel.Editor(typeof(Converters.CommentableUITypedEditor), typeof(UITypeEditor))]
            [System.ComponentModel.TypeConverter(typeof(Converters.CommentableUITypeConverter))]
            public ICommentable Comment
            {
                get { return Item as ICommentable; }
                set
                {
                    Item = value as T;
                    RefreshNode();
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            protected CommentableEditor()
            {
            }
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Editor createEditor();

        /// <summary>
        /// Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);

            if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
            {
                IBaseForm baseForm = BaseForm;
                if (baseForm != null)
                {
                    Editor editor = createEditor();
                    editor.Item = Item;
                    editor.Node = this;
                    NodeEditor = editor;

                    if (baseForm.Properties != null)
                    {
                        baseForm.Properties.SelectedObject = NodeEditor;
                    }
                }
            }
        }

        public override System.Drawing.Color ComputedColor
        {
            get
            {
                System.Drawing.Color retVal = base.ComputedColor;

                if (Item != null)
                {
                    BaseTreeNode parentNode;

                    switch (Item.MessagePathInfo)
                    {
                        case Utils.MessagePathInfoEnum.Nothing:
                        case Utils.MessagePathInfoEnum.NotComputed:
                            retVal = System.Drawing.Color.Black;
                            break;

                        case Utils.MessagePathInfoEnum.Error:
                            parentNode = (BaseTreeNode)Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = System.Drawing.Color.Red;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case Utils.MessagePathInfoEnum.PathToError:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;

                        case Utils.MessagePathInfoEnum.Warning:
                            parentNode = (BaseTreeNode)Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = System.Drawing.Color.Brown;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case Utils.MessagePathInfoEnum.PathToWarning:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;

                        case Utils.MessagePathInfoEnum.Info:
                            parentNode = (BaseTreeNode)Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = System.Drawing.Color.Blue;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case Utils.MessagePathInfoEnum.PathToInfo:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;
                    }
                }

                return retVal;
            }
        }
    }
}
