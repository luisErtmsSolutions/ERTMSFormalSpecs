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
using DataDictionary.Specification;

namespace GUI.SpecificationView
{
    public class ChapterTreeNode : ModelElementTreeNode<DataDictionary.Specification.Chapter>
    {
        /// <summary>
        /// The value editor
        /// </summary>
        private class ItemEditor : Editor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            public string Identifier
            {
                get { return Item.getId(); }
                set
                {
                    Item.setId(value);
                    RefreshNode();
                }
            }

            [Category("Description")]
            public string Name
            {
                get { return Item.Name; }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public ChapterTreeNode(DataDictionary.Specification.Chapter item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (DataDictionary.Specification.Paragraph paragraph in Item.Paragraphs)
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
        /// Adds a new paragraph to this chapter
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
            paragraph.FullId = Item.getId() + "." + (Item.countParagraphs() + 1);
            paragraph.Text = "";
            AddParagraph(paragraph);
        }

        public void ChangeRequirementToNoteHandler(object sender, EventArgs args)
        {
            foreach (DataDictionary.Specification.Paragraph paragraph in Item.Paragraphs)
            {
                paragraph.ChangeType(DataDictionary.Generated.acceptor.Paragraph_type.aREQUIREMENT, DataDictionary.Generated.acceptor.Paragraph_type.aNOTE);
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
                if (MessageBox.Show("Are you sure you want to move the corresponding paragraph?", "Move paragraph", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ParagraphTreeNode paragraphTreeNode = (ParagraphTreeNode)SourceNode;

                    DataDictionary.Specification.Paragraph paragraph = paragraphTreeNode.Item;
                    paragraphTreeNode.Delete();
                    AddParagraph(paragraph);
                }
            }
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
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add paragraph", new EventHandler(AddParagraphHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Change 'Requirement' to 'Note'", new EventHandler(ChangeRequirementToNoteHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));

            MenuItem recursiveActions = retVal.Find(x => x.Text.StartsWith("Recursive"));
            if (recursiveActions != null)
            {
                recursiveActions.MenuItems.Add(new MenuItem("-"));
                recursiveActions.MenuItems.Add(new MenuItem("Remove requirement sets", new EventHandler(RemoveRequirementSets)));
            }

            return retVal;
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
                GUIUtils.MDIWindow.SetCoverageStatus(Item);
            }
        }
    }
}
