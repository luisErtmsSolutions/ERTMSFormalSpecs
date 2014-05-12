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
using DataDictionary;
using DataDictionary.Tests;

namespace GUI.TestRunnerView
{
    public class SubSequenceTreeNode : ModelElementTreeNode<DataDictionary.Tests.SubSequence>
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
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public SubSequenceTreeNode(DataDictionary.Tests.SubSequence item, bool buildSubNodes)
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

            foreach (DataDictionary.Tests.TestCase testCase in Item.TestCases)
            {
                Nodes.Add(new TestCaseTreeNode(testCase, buildSubNodes));
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
        /// Creates a new test case (e.g. feature + test case)
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public TestCaseTreeNode createTestCase(DataDictionary.Tests.TestCase testCase)
        {
            TestCaseTreeNode retVal = new TestCaseTreeNode(testCase, true);

            Item.appendTestCases(testCase);
            Nodes.Add(retVal);

            return retVal;
        }

        #region Apply translation rules
        private class ApplyTranslationRulesHandler : Utils.ProgressHandler
        {
            /// <summary>
            /// The subsequence on which the translation rules should be applied
            /// </summary>
            private SubSequence SubSequence { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="subSequence"></param>
            public ApplyTranslationRulesHandler(SubSequence subSequence)
            {
                SubSequence = subSequence;
            }

            /// <summary>
            /// Generates the file in the background thread
            /// </summary>
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                Utils.FinderRepository.INSTANCE.ClearCache();
                SubSequence.Translate(SubSequence.Dictionary.TranslationDictionary);
            }
        }
        #endregion

        /// <summary>
        /// Translates the corresponding sub sequence, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            ApplyTranslationRulesHandler applyTranslationRulesHandler = new ApplyTranslationRulesHandler(Item);
            ProgressDialog progress = new ProgressDialog("Applying translation rules", applyTranslationRulesHandler);
            progress.ShowDialog(GUIUtils.MDIWindow);
            GUIUtils.MDIWindow.RefreshModel();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionary.Tests.TestCase testCase = DataDictionary.Tests.TestCase.createDefault("Test case" + (Item.TestCases.Count + 1));
            testCase.Enclosing = Item;

            createTestCase(testCase);
        }

        /// <summary>
        /// Provides the EFS System in which this element belongs
        /// </summary>
        public DataDictionary.EFSSystem EFSSystem
        {
            get
            {
                return Utils.EnclosingFinder<DataDictionary.EFSSystem>.find(Item);
            }
        }

        #region Execute tests
        private class ExecuteTestsHandler : LongOperations.BaseLongOperation
        {
            /// <summary>
            /// The window for which theses tests should be executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            /// The subsequence which should be executed
            /// </summary>
            private SubSequence SubSequence { get; set; }

            /// <summary>
            /// The EFS system 
            /// </summary>
            private EFSSystem EFSSystem { get { return SubSequence.EFSSystem; } }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="subSequence"></param>
            public ExecuteTestsHandler(Window window, SubSequence subSequence)
            {
                Window = window;
                SubSequence = subSequence;
            }

            /// <summary>
            /// Executes the tests in the background thread
            /// </summary>
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                if (Window != null)
                {
                    SynchronizerList.SuspendSynchronization();
                    Window.setSubSequence(SubSequence);
                    EFSSystem.Runner = new DataDictionary.Tests.Runner.Runner(SubSequence, true, false);
                    EFSSystem.Runner.RunUntilStep(null);
                    SynchronizerList.ResumeSynchronization();
                }
            }
        }

        /// <summary>
        /// Handles a run event on this test case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            ClearMessages();
            ExecuteTestsHandler executeTestHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            executeTestHandler.ExecuteUsingProgressDialog("Executing test steps");

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
            }
        }
        #endregion

        /// <summary>
        /// Handles a run event on sub sequence case and creates the associated report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReportHandler(object sender, EventArgs args)
        {
            Report.TestReport aReport = new Report.TestReport(Item);
            aReport.Show();
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add test case", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("Apply translation rules", new EventHandler(TranslateHandler)));
            retVal.Insert(7, new MenuItem("-"));
            retVal.Insert(8, new MenuItem("Execute", new EventHandler(RunHandler)));
            retVal.Insert(9, new MenuItem("Create report", new EventHandler(ReportHandler)));
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
            if (SourceNode is TestCaseTreeNode)
            {
                TestCaseTreeNode testCase = SourceNode as TestCaseTreeNode;
                testCase.Delete();

                createTestCase(testCase.Item);
            }
        }
    }
}
