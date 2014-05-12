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
using System.Drawing.Design;

namespace GUI.TestRunnerView
{
    public class StepTreeNode : ModelElementTreeNode<DataDictionary.Tests.Step>
    {
        /// <summary>
        /// The value editor
        /// </summary>
        private class ItemEditor : CommentableEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            /// The step name
            /// </summary>
            [Category("Description")]
            public override string Name
            {
                get
                {
                    string retVal = Item.Name;

                    if (Item.getTCS_Order() != 0)
                    {
                        retVal = "Step " + Item.getTCS_Order() + ": " + Item.getDescription();
                    }

                    return retVal;
                }
            }

            /// <summary>
            /// The step description
            /// </summary>
            [Category("Description")]
            public string Description
            {
                get { return Item.getDescription(); }
                set { Item.setDescription(value); }
            }

            /// <summary>
            /// The step order number
            /// </summary>
            [Category("Subset76")]
            public int Order
            {
                get { return Item.getTCS_Order(); }
                set { Item.setTCS_Order(value); }
            }

            /// <summary>
            /// The step distance
            /// </summary>
            [Category("Subset76")]
            public int Distance
            {
                get { return Item.getDistance(); }
                set { Item.setDistance(value); }
            }

            /// <summary>
            /// The step I/O mode
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_IO InputOutput
            {
                get { return Item.getIO(); }
                set { Item.setIO(value); }
            }

            /// <summary>
            /// The step Interface
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_INTERFACE Interface
            {
                get { return Item.getInterface(); }
                set { Item.setInterface(value); }
            }

            /// <summary>
            /// The step level in
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_LEVEL TestLevelIn
            {
                get { return Item.getLevelIN(); }
                set { Item.setLevelIN(value); }
            }

            /// <summary>
            /// The step level out
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_LEVEL TestLevelOut
            {
                get { return Item.getLevelOUT(); }
                set { Item.setLevelOUT(value); }
            }

            /// <summary>
            /// The step mode in
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_MODE TestModeIn
            {
                get { return Item.getModeIN(); }
                set { Item.setModeIN(value); }
            }

            /// <summary>
            /// The step mode out
            /// </summary>
            [Category("Subset76")]
            public DataDictionary.Generated.acceptor.ST_MODE TestModeOut
            {
                get { return Item.getModeOUT(); }
                set { Item.setModeOUT(value); }
            }

            /// <summary>
            /// The step is translated or not
            /// </summary>
            [Category("Subset76")]
            public bool TranslationRequired
            {
                get { return Item.getTranslationRequired(); }
                set { Item.setTranslationRequired(value); }
            }

            /// <summary>
            /// The step is translated or not
            /// </summary>
            [Category("Subset76")]
            public bool Translated
            {
                get { return Item.getTranslated(); }
                set { Item.setTranslated(value); }
            }

            /// <summary>
            /// The item user comment
            /// </summary>
            [Category("Subset76")]
            public string UserComment
            {
                get { return Item.getUserComment(); }
                set { Item.setUserComment(value); }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public StepTreeNode(DataDictionary.Tests.Step item, bool buildSubNodes)
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

            foreach (DataDictionary.Tests.SubStep subStep in Item.SubSteps)
            {
                Nodes.Add(new SubStepTreeNode(subStep, buildSubNodes));
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
        /// Translates the corresponding step, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            Utils.FinderRepository.INSTANCE.ClearCache();
            Item.Translate(Item.Dictionary.TranslationDictionary);
            GUIUtils.MDIWindow.RefreshModel();
        }

        /// <summary>
        /// Ensures that the runner corresponds to test case
        /// </summary>
        private void CheckRunner()
        {
            Window window = BaseForm as Window;
            if (window != null && window.EFSSystem.Runner != null && window.EFSSystem.Runner.SubSequence != Item.SubSequence)
            {
                window.Clear();
            }
        }

        /// <summary>
        /// Adds a step after this one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddSubStepHandler(object sender, EventArgs args)
        {
            DataDictionary.Tests.SubStep subStep = (DataDictionary.Tests.SubStep)DataDictionary.Generated.acceptor.getFactory().createSubStep();
            subStep.Name = "Sub-step" + (Nodes.Count + 1);
            subStep.Enclosing = Item;
            createSubStep(subStep);
        }

        /// <summary>
        /// Creates a new sub-step
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public SubStepTreeNode createSubStep(DataDictionary.Tests.SubStep subStep)
        {
            SubStepTreeNode retVal = new SubStepTreeNode(subStep, true);

            Item.appendSubSteps(subStep);
            Nodes.Add(retVal);

            return retVal;
        }

        /// <summary>
        /// Handles a run event on this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            CheckRunner();

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.setSubSequence(Item.SubSequence);
                DataDictionary.Tests.Runner.Runner runner = window.getRunner(Item.TestCase.SubSequence);

                runner.RunUntilStep(Item);
                foreach (DataDictionary.Tests.SubStep subStep in Item.SubSteps)
                {
                    runner.SetupSubStep(subStep);
                    if (!subStep.getSkipEngine())
                    {
                        runner.Cycle();
                    }
                }

                window.RefreshAfterStep();
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
            }
        }

        /// <summary>
        /// Handles a run event on this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunForExpectationsHandler(object sender, EventArgs args)
        {
            CheckRunner();

            Window window = BaseForm as Window;
            if (window != null)
            {
                DataDictionary.Tests.Runner.Runner runner = window.getRunner(Item.TestCase.SubSequence);

                SynchronizerList.SuspendSynchronization();
                runner.RunUntilStep(Item);
                foreach (DataDictionary.Tests.SubStep subStep in Item.SubSteps)
                {
                    runner.SetupSubStep(subStep);
                    if (!subStep.getSkipEngine())
                    {
                        runner.RunForBlockingExpectations(true);
                    }
                }
                SynchronizerList.ResumeSynchronization();
            }
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add sub-step", new EventHandler(AddSubStepHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("Apply translation rules", new EventHandler(TranslateHandler)));
            retVal.Insert(7, new MenuItem("-"));
            retVal.Insert(8, new MenuItem("Run once", new EventHandler(RunHandler)));
            retVal.Insert(9, new MenuItem("Run until expectation reached", new EventHandler(RunForExpectationsHandler)));
            retVal.Insert(10, new MenuItem("-"));

            return retVal;
        }

        /// <summary>
        /// Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            if (SourceNode is SubStepTreeNode)
            {
                SubStepTreeNode subStep = SourceNode as SubStepTreeNode;

                subStep.Delete();

                createSubStep(subStep.Item);
            }
        }
    }
}
