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

namespace GUI.TestRunnerView
{
    public partial class Window : BaseForm
    {
        public override MyPropertyGrid Properties
        {
            get { return null; }
        }

        public override EditorTextBox RequirementsTextBox
        {
            get { return null; }
        }

        public override EditorTextBox ExpressionEditorTextBox
        {
            get { return null; }
        }


        public override BaseTreeView TreeView
        {
            get { return testBrowserTreeView; }
        }

        public override ExplainTextBox ExplainTextBox
        {
            get { return null; }
        }

        /// <summary>
        /// The data dictionary for this view
        /// </summary>
        private DataDictionary.EFSSystem efsSystem;
        public DataDictionary.EFSSystem EFSSystem
        {
            get { return efsSystem; }
            private set
            {
                efsSystem = value;
                testBrowserTreeView.Root = efsSystem;
            }
        }

        /// <summary>
        /// The runner
        /// </summary>
        public DataDictionary.Tests.Runner.Runner getRunner(DataDictionary.Tests.SubSequence subSequence)
        {
            if (EFSSystem.Runner == null)
            {
                foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    if (subSequence != null)
                    {
                        EFSSystem.Runner = new DataDictionary.Tests.Runner.Runner(subSequence, false, false);
                        break;
                    }
                }
            }

            return EFSSystem.Runner;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Text = "System test view";
            Visible = false;
            EFSSystem = EFSSystem.INSTANCE;
            Refresh();
        }

        /// <summary>
        /// Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        /// Indicates that a refresh is ongoing
        /// </summary>
        private bool DoingRefresh { get; set; }

        /// <summary>
        /// Sets the current frame parameters
        /// </summary>
        /// <param name="frame"></param>
        public void setFrame(DataDictionary.Tests.Frame frame)
        {
            Invoke((MethodInvoker)delegate
            {
                frameToolStripComboBox.Text = frame.Name;
                Refresh();
            });
        }

        /// <summary>
        /// Sets the current sub sequence window parameters
        /// </summary>
        /// <param name="subSequence"></param>
        public void setSubSequence(DataDictionary.Tests.SubSequence subSequence)
        {
            Invoke((MethodInvoker)delegate
            {
                subSequenceSelectorComboBox.Text = subSequence.Name;
                setFrame(subSequence.Frame);
                Refresh();
            });
        }

        /// <summary>
        /// Refreshes the display
        /// </summary>
        public override void Refresh()
        {
            if (!DoingRefresh)
            {
                try
                {
                    DoingRefresh = true;

                    string selectedFrame = frameToolStripComboBox.Text;
                    string selectedSequence = subSequenceSelectorComboBox.Text;

                    if (EFSSystem.Runner == null)
                    {
                        toolStripTimeTextBox.Text = "0";
                        toolStripCurrentStepTextBox.Text = "<none>";
                    }
                    else
                    {
                        toolStripTimeTextBox.Text = "" + EFSSystem.Runner.Time;
                        DataDictionary.Tests.Step currentStep = EFSSystem.Runner.CurrentStep();
                        if (currentStep != null)
                        {
                            toolStripCurrentStepTextBox.Text = currentStep.Name;
                        }
                        else
                        {
                            toolStripCurrentStepTextBox.Text = "<none>";
                        }

                        if (EFSSystem.Runner.SubSequence != null)
                        {
                            Frame = EFSSystem.Runner.SubSequence.Frame;
                            selectedFrame = EFSSystem.Runner.SubSequence.Frame.Name;
                            selectedSequence = EFSSystem.Runner.SubSequence.Name;
                        }
                    }

                    testBrowserTreeView.Refresh();
                    testDescriptionTimeLineControl.Refresh();
                    testExecutionTimeLineControl.Refresh();

                    frameToolStripComboBox.Items.Clear();
                    List<string> frames = new List<string>();
                    foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
                    {
                        foreach (DataDictionary.Tests.Frame frame in dictionary.Tests)
                        {
                            frames.Add(frame.Name);
                        }
                    }
                    frames.Sort();

                    foreach (string frame in frames)
                    {
                        if (frame != null)
                        {
                            frameToolStripComboBox.Items.Add(frame);
                        }
                    }
                    frameToolStripComboBox.Text = selectedFrame;
                    frameToolStripComboBox.ToolTipText = selectedFrame;

                    if (Frame == null || frameToolStripComboBox.Text.CompareTo(Frame.Name) != 0)
                    {
                        subSequenceSelectorComboBox.Items.Clear();
                        foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
                        {
                            Frame = dictionary.findFrame(frameToolStripComboBox.Text);
                            if (Frame != null)
                            {
                                foreach (DataDictionary.Tests.SubSequence subSequence in Frame.SubSequences)
                                {
                                    subSequenceSelectorComboBox.Items.Add(subSequence.Name);
                                }
                                break;
                            }
                        }
                        if (subSequenceSelectorComboBox.Items.Count > 0)
                        {
                            subSequenceSelectorComboBox.Text = subSequenceSelectorComboBox.Items[0].ToString();
                        }
                        else
                        {
                            subSequenceSelectorComboBox.Text = "";
                        }

                        if (EFSSystem.Runner != null && EFSSystem.Runner.SubSequence != null)
                        {
                            // Only resets the runner if it is related to a test sequence
                            // TODO : Make sure this is right
                            EFSSystem.Runner = null;
                        }
                        }
                    }

                    if (EFSSystem.Runner != null && EFSSystem.Runner.SubSequence != null)
                    {
                        subSequenceSelectorComboBox.Text = EFSSystem.Runner.SubSequence.Name;
                    }

                    subSequenceSelectorComboBox.ToolTipText = subSequenceSelectorComboBox.Text;
                }
                finally
                {
                    DoingRefresh = false;
                }
            }

            base.Refresh();
        }

        /// <summary>
        /// Step once
        /// </summary>
        public void StepOnce()
        {
            CheckRunner();
            if (EFSSystem.Runner != null)
            {
                EFSSystem.Runner.RunUntilTime(EFSSystem.Runner.Time + EFSSystem.Runner.Step);
                GUIUtils.MDIWindow.RefreshAfterStep();
            }
        }

        private void stepOnce_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = testExecutionTabPage;
            StepOnce();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                EFSSystem.Runner.EndExecution();
                EFSSystem.Runner = null;
            }
            Clear();
            GUIUtils.MDIWindow.RefreshAfterStep();
            tabControl1.SelectedTab = testExecutionTabPage;
        }

        public void Clear()
        {
            CheckRunner();

            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
            }
        }

        /// <summary>
        /// Ensures that the runner is not empty
        /// </summary>
        private void CheckRunner()
        {
            if (EFSSystem.Runner == null)
            {
                if (Frame != null)
                {
                    DataDictionary.Tests.SubSequence subSequence = Frame.findSubSequence(subSequenceSelectorComboBox.Text);
                    if (subSequence != null)
                    {
                        EFSSystem.Runner = new DataDictionary.Tests.Runner.Runner(subSequence, true, false);
                    }
                }
                else
                {
                    EFSSystem.Runner = GUI.IPCInterface.EFSService.INSTANCE.Runner;
                }
            }
        }

        private void rewindButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = testExecutionTabPage;
            StepBack();
        }

        public void StepBack()
        {
            CheckRunner();
            if (EFSSystem.Runner != null)
            {
                EFSSystem.Runner.StepBack();
                GUIUtils.MDIWindow.RefreshAfterStep();
            }
        }

        private void testCaseSelectorComboBox_SelectionChanged(object sender, EventArgs e)
        {
            DataDictionary.Tests.Runner.Runner runner = EFSSystem.Runner;
            if (runner != null && (runner.SubSequence == null || runner.SubSequence.Name.CompareTo(subSequenceSelectorComboBox.Text) != 0))
            {
                EFSSystem.Runner = null;
            }
            Refresh();
        }

        /// <summary>
        /// Refreshes the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            testBrowserTreeView.RefreshModel();
        }

        /// <summary>
        /// Selects the current step by clicking on the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                DataDictionary.Tests.Step step = EFSSystem.Runner.CurrentStep();
                if (step != null)
                {
                    GUIUtils.MDIWindow.Select(step);
                }
            }
        }

        /// <summary>
        /// Selects the current test sequence by clicking on the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                DataDictionary.Tests.SubSequence subSequence = EFSSystem.Runner.SubSequence;
                if (subSequence != null)
                {
                    GUIUtils.MDIWindow.Select(subSequence);
                }
            }
        }

        /// <summary>
        /// Selects the next node where info message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextInfoToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(Utils.ElementLog.LevelEnum.Info);
        }

        /// <summary>
        /// Selects the next node where warning message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextWarningToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(Utils.ElementLog.LevelEnum.Warning);
        }

        /// <summary>
        /// Selects the next node where error message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextErrortoolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(Utils.ElementLog.LevelEnum.Error);
        }

        /// <summary>
        /// The frame currently selected
        /// </summary>
        private DataDictionary.Tests.Frame Frame { get; set; }

        private void frameSelectorComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (Frame == null || Frame.Name.CompareTo(frameToolStripComboBox.Text) != 0)
            {
                EFSSystem.Runner = null;
            }
            Refresh();
        }

        public void RefreshAfterStep()
        {
            Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!EFSSystem.INSTANCE.Markings.selectPreviousMarking())
            {
                MessageBox.Show("No more marking to show", "No more markings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!EFSSystem.INSTANCE.Markings.selectNextMarking())
            {
                MessageBox.Show("No more marking to show", "No more markings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
