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

using System.Windows.Forms;
using DataDictionary.Interpreter;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.TestRunnerView
{
    public partial class ExplainBox : DockContent
    {
        /// <summary>
        /// A node of the tree
        /// </summary>
        private class ExplainTreeNode : TreeNode
        {
            /// <summary>
            /// The explanation which corresponds to this node
            /// </summary>
            public ExplanationPart Explanation { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="explanation"></param>
            public ExplainTreeNode(ExplanationPart explanation)
            {
                Explanation = explanation;

                if (explanation != null)
                {
                    Text = explanation.Message;
                }
            }

            /// <summary>
            /// Provides the explain box in which this node lies
            /// </summary>
            private ExplainBox ExplainBox
            {
                get
                {
                    return GUIUtils.EnclosingFinder<ExplainBox>.find(TreeView);
                    ;
                }
            }

            /// <summary>
            /// Selects the corresponding model element
            /// </summary>
            public void SelectModel(bool selectModel)
            {
                if (Explanation != null && Explanation.Element != null)
                {
                    if (selectModel)
                    {
                        GUIUtils.MDIWindow.Select(Explanation.Element, true);
                    }
                    ExplainBox.explainRichTextBox.Instance = Explanation;
                    ExplainBox.explainRichTextBox.Text = Explanation.Message;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExplainBox()
        {
            InitializeComponent();

            explainTreeView.AfterSelect += new TreeViewEventHandler(explainTreeView_AfterSelect);
            explainTreeView.BeforeExpand += new TreeViewCancelEventHandler(explainTreeView_BeforeExpand);
        }

        private void explainTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ExplainTreeNode node = e.Node as ExplainTreeNode;
            foreach (ExplainTreeNode subNode in node.Nodes)
            {
                subNode.Nodes.Clear();
                innerSetExplanation(subNode.Explanation, subNode, 1);
            }
        }

        /// <summary>
        /// Sets the node, and its subnode according to the content of the explanation
        /// </summary>
        /// <param name="part"></param>
        /// <param name="node"></param>
        /// <param param name="level">the level in which the explanation is inserted</param>
        private void innerSetExplanation(ExplanationPart part, ExplainTreeNode node, int level)
        {
            if (part != null)
            {
                foreach (ExplanationPart subPart in part.SubExplanations)
                {
                    if (level < 2)
                    {
                        ExplainTreeNode subNode = new ExplainTreeNode(subPart);
                        innerSetExplanation(subPart, subNode, level + 1);
                        node.Nodes.Add(subNode);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the explanation for this explain box
        /// </summary>
        /// <param name="explanation"></param>
        public void setExplanation(ExplanationPart explanation)
        {
            explainTreeView.Nodes.Clear();
            if (explanation != null)
            {
                ExplainTreeNode node = new ExplainTreeNode(explanation);
                innerSetExplanation(explanation, node, 0);
                explainTreeView.Nodes.Add(node);
            }
        }

        /// <summary>
        /// Handles the selection of an element of the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void explainTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ExplainTreeNode node = explainTreeView.SelectedNode as ExplainTreeNode;
            if (node != null)
            {
                node.SelectModel(ModifierKeys == Keys.Control);
            }
        }
    }
}