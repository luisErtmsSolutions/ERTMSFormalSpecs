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
using System.Windows.Forms;
using DataDictionary;
using System.Threading;
using DataDictionary.Specification;

namespace GUI.SpecificationView
{
    public class ParagraphTreeNode : ReferencesParagraphTreeNode<DataDictionary.Specification.Paragraph>
    {
        /// <summary>
        /// The value editor
        /// </summary>
        private class ItemEditor : UnnamedReferencesParagraphEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            /// The item name
            /// </summary>
            [Category("\t\tDescription")]
            public string Id
            {
                get { return Item.getId(); }
                set
                {
                    Item.setId(value);
                    RefreshNode();
                }
            }

            /// <summary>
            /// Provides the type of the paragraph
            /// </summary>
            [Category("\t\tDescription"), TypeConverter(typeof(Converters.SpecTypeConverter))]
            public virtual DataDictionary.Generated.acceptor.Paragraph_type Type
            {
                get { return Item.getType(); }
                set
                {
                    Item.SetType(value);
                    RefreshNode();
                }
            }

            /// <summary>
            /// Indicates if the paragraph has been reviewed (content & structure)
            /// </summary>
            [Category("Meta data")]
            public virtual bool Reviewed
            {
                get { return Item.getReviewed(); }
                set { Item.setReviewed(value); }
            }

            /// <summary>
            /// Indicates if the paragraph can be implemented by the EFS
            /// </summary>
            [Category("Meta data"), TypeConverter(typeof(Converters.ImplementationStatusConverter))]
            public virtual DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM ImplementationStatus
            {
                get { return Item.getImplementationStatus(); }
                set { Item.setImplementationStatus(value); }
            }

            /// <summary>
            /// Indicates if the paragraph has been tested
            /// </summary>
            [Category("Meta data")]
            public virtual bool Tested
            {
                get { return Item.getTested(); }
                set { Item.setTested(value); }
            }

            /// <summary>
            /// Indicates that more information is required to understand the requirement
            /// </summary>
            [Category("Meta data")]
            public virtual bool MoreInfoRequired
            {
                get { return Item.getMoreInfoRequired(); }
                set { Item.setMoreInfoRequired(value); }
            }

            /// <summary>
            /// Indicates that this paragraph has an issue
            /// </summary>
            [Category("Meta data")]
            public virtual bool SpecIssue
            {
                get { return Item.getSpecIssue(); }
                set { Item.setSpecIssue(value); }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public ParagraphTreeNode(DataDictionary.Specification.Paragraph item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (DataDictionary.Specification.Paragraph paragraph in Item.SubParagraphs)
            {
                Nodes.Add(new ParagraphTreeNode(paragraph, buildSubNodes));
            }
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        /// Update counts according to the selected chapter
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.specBrowserRuleView.Nodes.Clear();
                foreach (DataDictionary.ReqRef reqRef in Item.Implementations)
                {
                    window.specBrowserRuleView.Nodes.Add(new ReqRefTreeNode(reqRef, true, false, reqRef.Model.Name));
                }

                window.functionalBlocksTreeView.SetRoot(Model);
                window.functionalBlocksTreeView.RefreshModel();
            }

            GUIUtils.MDIWindow.SetCoverageStatus(Item);
        }

        public void ImplementedHandler(object sender, EventArgs args)
        {
            if (Item.isApplicable())
            {
                Item.setImplementationStatus(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented);
            }

            RefreshNode();
        }

        public void NotImplementableHandler(object sender, EventArgs args)
        {
            Item.setImplementationStatus(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable);
            RefreshNode();
        }

        /// <summary>
        /// Adds a new paragraph in this paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        public void AddParagraph(DataDictionary.Specification.Paragraph paragraph)
        {
            Item.appendParagraphs(paragraph);
            Nodes.Add(new ParagraphTreeNode(paragraph, true));
            RefreshNode();
        }

        public void AddParagraphHandler(object sender, EventArgs args)
        {
            DataDictionary.Specification.Paragraph paragraph = (DataDictionary.Specification.Paragraph)DataDictionary.Generated.acceptor.getFactory().createParagraph();
            paragraph.FullId = Item.GetNewSubParagraphId(false);
            paragraph.Text = "";
            paragraph.setType(DataDictionary.Generated.acceptor.Paragraph_type.aREQUIREMENT);

            SetupDefaultRequirementSets(paragraph);

            AddParagraph(paragraph);
        }

        private void SetupDefaultRequirementSets(DataDictionary.Specification.Paragraph paragraph)
        {
            foreach (RequirementSet requirementSet in Item.EFSSystem.RequirementSets)
            {
                if (requirementSet.getDefault())
                {
                    paragraph.AppendToRequirementSet(requirementSet);
                }
            }
        }

        public void AddParagraphFromClipboardHandler(object sender, EventArgs args)
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText(TextDataFormat.Text);

                string id;
                string data;

                int i = text.IndexOf(' ');
                int k = text.IndexOf('\t');
                if (k < i)
                {
                    i = k;
                }
                int j = text.IndexOf('\n');
                if (i < 0)
                {
                    i = j;
                }
                else
                {
                    if (j > 0)
                    {
                        i = Math.Min(i, j);
                    }
                }
                if (i > 0)
                {
                    id = text.Substring(0, i).Trim();
                    if (id.Length > 0 && char.IsDigit(id[0]))
                    {
                        data = text.Substring(i + 1);
                    }
                    else
                    {
                        id = Item.GetNewSubParagraphId(true);
                        data = text;
                    }
                }
                else
                {
                    id = Item.GetNewSubParagraphId(true);
                    data = text;
                }
                data = data.Replace("\r", "");
                data = data.Replace("\n", "");

                DataDictionary.Specification.Paragraph paragraph = (DataDictionary.Specification.Paragraph)DataDictionary.Generated.acceptor.getFactory().createParagraph();
                paragraph.FullId = id;
                paragraph.Text = data;
                paragraph.setType(DataDictionary.Generated.acceptor.Paragraph_type.aREQUIREMENT);
                SetupDefaultRequirementSets(paragraph);
                AddParagraph(paragraph);
            }
        }

        /// <summary>
        /// Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is ParagraphTreeNode)
            {
                Paragraph current = Item;
                while (current != null && current != SourceNode.Model)
                {
                    current = current.EnclosingParagraph;
                }

                if (current == null)
                {
                    if (MessageBox.Show("Are you sure you want to move the corresponding paragraph?", "Move paragraph", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ParagraphTreeNode paragraphTreeNode = (ParagraphTreeNode)SourceNode;

                        DataDictionary.Specification.Paragraph paragraph = paragraphTreeNode.Item;
                        paragraphTreeNode.Delete();
                        AddParagraph(paragraph);
                    }
                }
                else
                {
                    MessageBox.Show("Cannot move a paragraph in its sub paragraphs", "Move paragraph", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                base.AcceptDrop(SourceNode);
            }
        }

        public override void AcceptCopy(BaseTreeNode SourceNode)
        {
            if (SourceNode is SpecificationView.ParagraphTreeNode)
            {
                if (HandleRequirements && ReqReferences == null)
                {
                    ReqReferences = new ReqRefsTreeNode(Item, true);
                    Nodes.Add(ReqReferences);
                    RefreshNode();
                }

                if (ReqReferences != null)
                {
                    SpecificationView.ParagraphTreeNode paragraphTreeNode = (SpecificationView.ParagraphTreeNode)SourceNode;
                    ReqReferences.CreateReqRef(paragraphTreeNode.Item);
                }
            }
        }

        /// <summary>
        /// Updates the paragraph name by appending the "Table " string at its beginning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddTableHandler(object sender, EventArgs args)
        {
            Item.setId("Table " + Item.getId());
            RefreshNode();
        }

        /// <summary>
        /// Updates the paragraph name by appending the "Entry " string at its beginning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddEntryHandler(object sender, EventArgs args)
        {
            Item.setId("Entry " + Item.getId());
            RefreshNode();
        }

        /// <summary>
        /// Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveRequirementSets(object sender, EventArgs e)
        {
            RequirementSetReference.RemoveReferencesVisitor remover = new RequirementSetReference.RemoveReferencesVisitor();
            remover.visit(Item);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add paragraph", new EventHandler(AddParagraphHandler)));
            retVal.Add(new MenuItem("Add paragraph from clipboard", new EventHandler(AddParagraphFromClipboardHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            MenuItem newItem = new MenuItem("Mark as...");
            newItem.MenuItems.Add(new MenuItem("Implemented", new EventHandler(ImplementedHandler)));
            newItem.MenuItems.Add(new MenuItem("Not implementable", new EventHandler(NotImplementableHandler)));
            retVal.Insert(4, newItem);
            retVal.Insert(7, new MenuItem("Add Table to Id", new EventHandler(AddTableHandler)));
            retVal.Insert(8, new MenuItem("Add Entry to Id", new EventHandler(AddEntryHandler)));
            retVal.Insert(9, new MenuItem("-"));

            MenuItem recursiveActions = retVal.Find(x => x.Text.StartsWith("Recursive"));
            if (recursiveActions != null)
            {
                recursiveActions.MenuItems.Add(new MenuItem("-"));
                recursiveActions.MenuItems.Add(new MenuItem("Remove requirement sets", new EventHandler(RemoveRequirementSets)));
            }

            return retVal;
        }

        /// <summary>
        /// Holds metrics computed on a set of paragraphs
        /// </summary>
        public struct ParagraphSetMetrics
        {
            public int subParagraphCount;
            public int implementableCount;
            public int implementedCount;
            public int unImplementedCount;
            public int notImplementable;
            public int newRevisionAvailable;
            public int testedCount;
        }

        /// <summary>
        /// Creates the stat message according to the list of paragraphs provided
        /// </summary>
        /// <param name="efsSystem"></param>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static ParagraphSetMetrics CreateParagraphSetMetrics(EFSSystem efsSystem, List<DataDictionary.Specification.Paragraph> paragraphs)
        {
            ParagraphSetMetrics retVal = new ParagraphSetMetrics();

            retVal.subParagraphCount = paragraphs.Count;

            Dictionary<DataDictionary.Specification.Paragraph, List<ReqRef>> paragraphsReqRefDictionary = null;
            foreach (DataDictionary.Specification.Paragraph p in paragraphs)
            {
                if (paragraphsReqRefDictionary == null)
                {
                    paragraphsReqRefDictionary = p.Dictionary.ParagraphsReqRefs;
                }

                switch (p.getImplementationStatus())
                {
                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented:
                        retVal.implementableCount += 1;

                        bool implemented = true;
                        if (paragraphsReqRefDictionary.ContainsKey(p))
                        {
                            List<ReqRef> implementations = paragraphsReqRefDictionary[p];
                            for (int i = 0; i < implementations.Count; i++)
                            {
                                // the implementation may be also a ReqRef
                                if (implementations[i].Enclosing is ReqRelated)
                                {
                                    ReqRelated reqRelated = implementations[i].Enclosing as ReqRelated;
                                    // Do not consider tests
                                    if (Utils.EnclosingFinder<DataDictionary.Tests.Frame>.find(reqRelated) == null)
                                    {
                                        implemented = implemented && reqRelated.ImplementationCompleted;
                                    }
                                }
                            }
                        }
                        if (implemented)
                        {
                            retVal.implementedCount += 1;
                        }
                        else
                        {
                            retVal.unImplementedCount += 1;
                        }
                        break;

                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA:
                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM:
                        retVal.implementableCount += 1;
                        retVal.unImplementedCount += 1;
                        break;

                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable:
                        retVal.notImplementable += 1;
                        break;

                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable:
                        retVal.implementableCount += 1;
                        retVal.newRevisionAvailable += 1;
                        break;
                }
            }

            // Count the tested paragraphs
            HashSet<DataDictionary.Specification.Paragraph> testedParagraphs = new HashSet<DataDictionary.Specification.Paragraph>();
            foreach (DataDictionary.Dictionary dictionary in efsSystem.Dictionaries)
            {
                foreach (DataDictionary.Specification.Paragraph p in Reports.Tests.TestsCoverageReport.CoveredRequirements(dictionary))
                {
                    testedParagraphs.Add(p);
                }
            }

            foreach (DataDictionary.Specification.Paragraph p in paragraphs)
            {
                if (testedParagraphs.Contains(p))
                {
                    retVal.testedCount += 1;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the stat message according to the list of paragraphs provided
        /// </summary>
        /// <param name="efsSystem"></param>
        /// <param name="paragraphs"></param>
        /// <param name="indicateSelected">Indicates that there are selected requirements</param>
        /// <returns></returns>
        public static string CreateStatMessage(EFSSystem efsSystem, List<DataDictionary.Specification.Paragraph> paragraphs, bool indicateSelected)
        {
            string retVal = "Statistics : ";

            ParagraphSetMetrics metrics = CreateParagraphSetMetrics(efsSystem, paragraphs);

            if (metrics.subParagraphCount > 0 && metrics.implementableCount > 0)
            {
                retVal += metrics.subParagraphCount + (indicateSelected ? " selected" : "") + " requirements, ";
                retVal += +metrics.implementableCount + " implementable (" + Math.Round(((float)metrics.implementableCount / metrics.subParagraphCount * 100), 2) + "%), ";
                retVal += metrics.implementedCount + " implemented (" + Math.Round(((float)metrics.implementedCount / metrics.implementableCount * 100), 2) + "%), ";
                retVal += +metrics.unImplementedCount + " not implemented (" + Math.Round(((float)metrics.unImplementedCount / metrics.implementableCount * 100), 2) + "%), ";
                retVal += metrics.newRevisionAvailable + " with new revision (" + Math.Round(((float)metrics.newRevisionAvailable / metrics.implementableCount * 100), 2) + "%), ";
                retVal += metrics.testedCount + " tested (" + Math.Round(((float)metrics.testedCount / metrics.implementableCount * 100), 2) + "%)";
            }
            else
            {
                retVal += "No implementable requirement selected";
            }

            return retVal;
        }
    }
}
