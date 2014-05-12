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
namespace GUI.TestRunnerView.TimeLineControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataDictionary.Tests;
    using DataDictionary.Tests.Runner.Events;
    using System.Windows.Forms;
    using DataDictionary;
    using Utils;
    using System.Collections;
    using DataDictionary.Interpreter.Filter;
    using DataDictionary.Interpreter;
    using DataDictionary.Values;
    using GUI.EditorView;

    /// <summary>
    /// The static time line according to a test case
    /// </summary>
    public class StaticTimeLineControl : TimeLineControl
    {
        /// <summary>
        /// The test case
        /// </summary>
        private TestCase __testCase;

        /// <summary>
        /// The test case for which this time line is built
        /// </summary>
        public TestCase TestCase { get { return __testCase; } set { __testCase = value; CleanEventPositions(); } }

        /// <summary>
        /// Constructor
        /// </summary>
        public StaticTimeLineControl()
            : base()
        {
            AllowDrop = true;
            DragEnter += new DragEventHandler(TimeLineControl_DragEnter);
            DragDrop += new DragEventHandler(TimeLineControl_DragDrop);

            DoubleClick += new EventHandler(TimeLineControl_DoubleClick);
            MouseDown += new MouseEventHandler(StaticTimeLineControl_MouseDown);
            TestCase = null;
        }

        private const int CTRL = 8;

        /// <summary>
        /// Changes the cursor according to the modifier key when a drag & drop operation is performed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeLineControl_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.KeyState & CTRL) != 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Handles a drop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeLineControl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                BaseTreeNode SourceNode = (BaseTreeNode)e.Data.GetData("WindowsForms10PersistentObject");
                if (SourceNode != null)
                {
                    DataDictionaryView.VariableTreeNode variableNode = SourceNode as DataDictionaryView.VariableTreeNode;
                    if (variableNode != null)
                    {
                        SubStep subStep = SubStepRelatedToMousePosition();
                        if (subStep != null)
                        {
                            // Create the default value
                            IValue value = null;
                            Expression expression = null;
                            string defaultValue = variableNode.Item.GetDefaultValueText();
                            if (defaultValue != null)
                            {
                                bool doSemanticalAnalysis = true;
                                bool silent = true;
                                expression = EFSSystem.INSTANCE.Parser.Expression(variableNode.Item, defaultValue, AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
                            }

                            if (expression != null)
                            {
                                InterpretationContext context = new InterpretationContext();
                                context.UseDefaultValue = false;
                                value = expression.GetValue(context);
                            }

                            if (value == null || value is EmptyValue)
                            {
                                DataDictionary.Types.Structure structureType = variableNode.Item.Type as DataDictionary.Types.Structure;
                                if (structureType != null)
                                {
                                    bool setDefaultValue = false;
                                    value = new StructureValue(structureType, setDefaultValue);
                                }
                            }

                            // Create the action or the expectation according to the keyboard modifier keys
                            if (value != null)
                            {
                                if ((e.KeyState & CTRL) != 0)
                                {
                                    DataDictionary.Tests.Expectation expectation = (DataDictionary.Tests.Expectation)DataDictionary.Generated.acceptor.getFactory().createExpectation();
                                    expectation.ExpressionText = variableNode.Item.FullName + " == " + value.FullName;
                                    subStep.appendExpectations(expectation);
                                }
                                else
                                {
                                    DataDictionary.Rules.Action action = (DataDictionary.Rules.Action)DataDictionary.Generated.acceptor.getFactory().createAction();
                                    action.ExpressionText = variableNode.Item.FullName + " <- " + value.FullName;
                                    subStep.appendActions(action);
                                }
                                RefreshTimeLineAndWindow();
                            }
                            else
                            {
                                MessageBox.Show("Cannot evaluate variable default value", "Cannot create event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The sub step relative to the mouse position
        /// </summary>
        /// <returns></returns>
        private SubStep SubStepRelatedToMousePosition()
        {
            SubStep retVal = null;

            ModelEvent evt = GetEventUnderMouse();
            if (evt != null && evt.Instance != null)
            {
                retVal = EnclosingFinder<SubStep>.find(evt.Instance as IEnclosed, true);
                if (retVal == null)
                {
                    Step step = evt.Instance as Step;
                    if (step != null && step.SubSteps.Count > 0)
                    {
                        retVal = (SubStep)step.SubSteps[step.SubSteps.Count - 1];
                    }
                }
            }

            if (retVal == null && TestCase.Steps.Count > 0)
            {
                Step step = (Step)TestCase.Steps[TestCase.Steps.Count - 1];
                if (step.SubSteps.Count > 0)
                {
                    retVal = (SubStep)step.SubSteps[step.SubSteps.Count - 1];
                }
            }

            return retVal;
        }

        /// <summary>
        /// Refreshes the time line and the enclosing window
        /// </summary>
        private void RefreshTimeLineAndWindow()
        {
            HandledEvents = -1;
            Window.RefreshModel();
            Window.Refresh();
        }

        /// <summary>
        /// A base menu item element
        /// </summary>
        private class BaseToolStripButton : MenuItem
        {
            /// <summary>
            /// The selected event, if any
            /// </summary>
            protected ModelEvent Selected { get; private set; }

            /// <summary>
            /// The test case in which the action occurs
            /// </summary>
            protected TestCase TestCase { get; private set; }

            /// <summary>
            /// The time line control for which this menu item is built
            /// </summary>
            private StaticTimeLineControl TimeLineControl { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            /// <param name="caption"></param>
            public BaseToolStripButton(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase, string caption)
                : base(caption)
            {
                TimeLineControl = timeLineControl;
                Selected = modelEvent;
                TestCase = testCase;
            }

            /// <summary>
            /// Provides the step enclosing the selected event
            /// </summary>
            public Step Step
            {
                get
                {
                    Step retVal = null;

                    if (Selected != null)
                    {
                        retVal = EnclosingFinder<Step>.find(Selected.Instance as IEnclosed, true); ;
                    }

                    return retVal;
                }
            }

            /// <summary>
            /// Provides the substep enclosing the selected event
            /// </summary>
            public SubStep SubStep
            {
                get
                {
                    SubStep retVal = null;

                    if (Selected != null)
                    {
                        retVal = EnclosingFinder<SubStep>.find(Selected.Instance as IEnclosed, true); ;
                    }

                    return retVal;
                }
            }

            /// <summary>
            /// Refreshes the time line after click action has been performed
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                TimeLineControl.RefreshTimeLineAndWindow();
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Deletes the current element
        /// </summary>
        private class DeleteMenuItem : BaseToolStripButton
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            public DeleteMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase)
                : base(timeLineControl, modelEvent, testCase, "Delete")
            {
                Enabled = modelEvent != null;
            }

            /// <summary>
            /// Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (Selected != null && Selected.Instance != null)
                {
                    IModelElement model = Selected.Instance as IModelElement;
                    ArrayList collection = model.EnclosingCollection;
                    if (collection != null)
                    {
                        collection.Remove(model);
                    }
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds a step after the selected step
        /// </summary>
        private class AddStepMenuItem : BaseToolStripButton
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            public AddStepMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase)
                : base(timeLineControl, modelEvent, testCase, "Add step")
            {
            }

            /// <summary>
            /// Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                Step newStep = (Step)DataDictionary.Generated.acceptor.getFactory().createStep();
                newStep.Enclosing = TestCase;

                SubStep subStep = (SubStep)DataDictionary.Generated.acceptor.getFactory().createSubStep();
                subStep.Name = "Substep 1";
                newStep.appendSubSteps(subStep);

                if (Step != null)
                {
                    newStep.Name = "NewStep";
                    int index = TestCase.Steps.IndexOf(Step);
                    TestCase.Steps.Insert(index + 1, newStep);
                }
                else
                {
                    newStep.Name = "Step " + (TestCase.Steps.Count + 1);
                    TestCase.Steps.Add(newStep);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds a step after the selected step
        /// </summary>
        private class AddSubStepMenuItem : BaseToolStripButton
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            public AddSubStepMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase)
                : base(timeLineControl, modelEvent, testCase, "Add substep")
            {
                Enabled = Step != null;
            }

            /// <summary>
            /// Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                SubStep newSubStep = (SubStep)DataDictionary.Generated.acceptor.getFactory().createSubStep();
                newSubStep.Enclosing = Step;

                if (SubStep != null)
                {
                    newSubStep.Name = "NewSubStep";
                    int index = Step.SubSteps.IndexOf(SubStep);
                    Step.SubSteps.Insert(index + 1, newSubStep);
                }
                else
                {
                    newSubStep.Name = "SubStep " + (Step.SubSteps.Count + 1);
                    Step.SubSteps.Add(newSubStep);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds a step after the selected step
        /// </summary>
        private class AddChangeMenuItem : BaseToolStripButton
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            public AddChangeMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase)
                : base(timeLineControl, modelEvent, testCase, "Add change")
            {
                Enabled = SubStep != null;
            }

            /// <summary>
            /// Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (SubStep != null)
                {
                    DataDictionary.Rules.Action action = (DataDictionary.Rules.Action)DataDictionary.Generated.acceptor.getFactory().createAction();
                    action.Name = "";
                    SubStep.appendActions(action);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds a step after the selected step
        /// </summary>
        private class AddExpectationMenuItem : BaseToolStripButton
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="testCase"></param>
            public AddExpectationMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, TestCase testCase)
                : base(timeLineControl, modelEvent, testCase, "Add expectation")
            {
                Enabled = SubStep != null;
            }

            /// <summary>
            /// Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (SubStep != null)
                {
                    DataDictionary.Tests.Expectation expectation = (DataDictionary.Tests.Expectation)DataDictionary.Generated.acceptor.getFactory().createExpectation();
                    expectation.Name = "";
                    SubStep.appendExpectations(expectation);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Updates the contextual menu according to the position of the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StaticTimeLineControl_MouseDown(object sender, MouseEventArgs e)
        {
            ContextMenu = new ContextMenu();

            ModelEvent evt = GetEventUnderMouse();
            ContextMenu.MenuItems.Add(new AddStepMenuItem(this, evt, TestCase));
            ContextMenu.MenuItems.Add(new AddSubStepMenuItem(this, evt, TestCase));
            ContextMenu.MenuItems.Add(new AddChangeMenuItem(this, evt, TestCase));
            ContextMenu.MenuItems.Add(new AddExpectationMenuItem(this, evt, TestCase));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new DeleteMenuItem(this, evt, TestCase));
        }

        /// <summary>
        /// Sets the string value into the right property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        private class TimeLineExpressionableTextChangeHandler : ExpressionableTextChangeHandler
        {
            /// <summary>
            /// The time line control
            /// </summary>
            private StaticTimeLineControl TimeLine { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="instance"></param>
            public TimeLineExpressionableTextChangeHandler(StaticTimeLineControl timeLine, IExpressionable instance)
                : base(instance as DataDictionary.ModelElement)
            {
                TimeLine = timeLine;
            }

            /// <summary>
            /// The way text is set back in the instance
            /// </summary>
            /// <returns></returns>
            public override void SetText(string text)
            {
                base.SetText(text);
                TimeLine.Refresh();
            }
        }

        /// <summary>
        /// Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeLineControl_DoubleClick(object sender, EventArgs e)
        {
            ModelEvent evt = GetEventUnderMouse();

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                EditorView.Window form = new EditorView.Window();
                TimeLineExpressionableTextChangeHandler handler = new TimeLineExpressionableTextChangeHandler(this, variableUpdate.Action);
                form.setChangeHandler(handler);
                GUIUtils.MDIWindow.AddChildWindow(form, WeifenLuo.WinFormsUI.Docking.DockAreas.Float);
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                EditorView.Window form = new EditorView.Window();
                TimeLineExpressionableTextChangeHandler handler = new TimeLineExpressionableTextChangeHandler(this, expect.Expectation);
                form.setChangeHandler(handler);
                GUIUtils.MDIWindow.AddChildWindow(form, WeifenLuo.WinFormsUI.Docking.DockAreas.Float);
            }
        }
        /// <summary>
        /// Refreshes the view according to the test case
        /// </summary>
        public override void Refresh()
        {
            if (TestCase != null && TestCase.ActionCount != HandledEvents)
            {
                UpdatePositionHandler();
                UpdatePanelSize();
                HandledEvents = TestCase.ActionCount;
            }
            base.Refresh();
        }

        /// <summary>
        /// Update the information stored in the position handler according to the test case
        /// </summary>
        protected override void UpdatePositionHandler()
        {
            PositionHandler.CleanPositions();
            if (TestCase != null)
            {
                double currentTime = 0.0;
                foreach (Step step in TestCase.Steps)
                {
                    if (step.SubSteps.Count > 0)
                    {
                        foreach (SubStep subStep in step.SubSteps)
                        {
                            SubStepActivated subStepActivated = new SubStepActivated(subStep, null);
                            subStepActivated.Time = currentTime;
                            PositionHandler.RegisterEvent(subStepActivated);
                            foreach (DataDictionary.Rules.Action action in subStep.Actions)
                            {
                                VariableUpdate variableUpdate = new VariableUpdate(action, action, null);
                                PositionHandler.RegisterEvent(variableUpdate);
                            }
                            foreach (Expectation expectation in subStep.Expectations)
                            {
                                Expect expect = new Expect(expectation, null);
                                PositionHandler.RegisterEvent(expect);
                            }

                            currentTime += 1;
                        }
                    }
                    else
                    {
                        StepActivation stepActivated = new StepActivation(step);
                        stepActivated.Time = currentTime;
                        PositionHandler.RegisterEvent(stepActivated);
                        currentTime += 1;
                    }
                }
            }
        }

    }
}
