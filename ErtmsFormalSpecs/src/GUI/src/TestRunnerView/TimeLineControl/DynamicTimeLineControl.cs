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
    using DataDictionary.Tests.Runner.Events;
    using DataDictionary.Tests.Runner;
    using System.Windows.Forms;

    /// <summary>
    /// The static time line according to a TimeLine
    /// </summary>
    public class DynamicTimeLineControl : TimeLineControl
    {
        /// <summary>
        /// The time line handled by this window
        /// </summary>
        public EventTimeLine TimeLine
        {
            get
            {
                EventTimeLine retVal = null;

                Runner runner = DataDictionary.EFSSystem.INSTANCE.Runner;
                if (runner != null)
                {
                    retVal = runner.EventTimeLine;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DynamicTimeLineControl()
            : base()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("Configure filter...", new EventHandler(OpenFilter)));
            DoubleClick += new EventHandler(TimeLineControl_DoubleClick);

            FilterConfiguration = new FilterConfiguration();
        }

        /// <summary>
        /// The time line filtering configuration
        /// </summary>
        public FilterConfiguration FilterConfiguration { get; private set; }

        /// <summary>
        /// Opens the filtering dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFilter(object sender, EventArgs e)
        {
            Filtering filtering = new Filtering();
            filtering.Configure(GUIUtils.MDIWindow.EFSSystem, FilterConfiguration);
            filtering.ShowDialog(this);
            filtering.UpdateConfiguration(FilterConfiguration);
            CleanEventPositions();
            Refresh();
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
                DataDictionary.Interpreter.ExplanationPart explain = variableUpdate.Explanation;
                ExplainBox explainTextBox = new ExplainBox();
                explainTextBox.setExplanation(explain);
                GUIUtils.MDIWindow.AddChildWindow(explainTextBox);
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                DataDictionary.Tests.Expectation expectation = expect.Expectation;

                if (expectation != null)
                {
                    DataDictionary.Interpreter.ExplanationPart explain = expectation.Expression.Explain();
                    ExplainBox explainTextBox = new ExplainBox();
                    explainTextBox.setExplanation(explain);
                    GUIUtils.MDIWindow.AddChildWindow(explainTextBox);
                }
            }
        }

        /// <summary>
        /// Refreshes the view
        /// </summary>
        public override void Refresh()
        {
            if (TimeLine != null && TimeLine.Events.Count != HandledEvents)
            {
                UpdatePositionHandler();
                UpdatePanelSize();
                HandledEvents = TimeLine.Events.Count;
                base.Refresh();
            }
        }

        /// <summary>
        /// Update the information stored in the position handler according to the time line
        /// </summary>
        protected override void UpdatePositionHandler()
        {
            PositionHandler.CleanPositions();
            if (TimeLine != null)
            {
                foreach (ModelEvent evt in TimeLine.Events)
                {
                    if (FilterConfiguration.VisibleEvent(evt) || evt is SubStepActivated)
                    {
                        PositionHandler.RegisterEvent(evt);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the size of the panel according to the number of events to handle
        /// </summary>
        protected override void UpdatePanelSize()
        {
            base.UpdatePanelSize();
            ScrollControlIntoView(AutoScrollEnabler);
        }
    }
}
