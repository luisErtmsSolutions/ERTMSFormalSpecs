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
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.SelectionHistory
{
    public partial class Window : BaseForm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            historyDataGridView.DoubleClick += new System.EventHandler(historyDataGridView_DoubleClick);

            Visible = false;
            Text = "Selection history view";

            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight;
            Refresh();
        }

        void historyDataGridView_DoubleClick(object sender, System.EventArgs e)
        {
            DataDictionary.ModelElement selected = null;

            if (historyDataGridView.SelectedCells.Count == 1)
            {
                selected = ((List<HistoryObject>)historyDataGridView.DataSource)[historyDataGridView.SelectedCells[0].OwningRow.Index].Reference;
            }

            if (selected != null)
            {
                int i = GUIUtils.MDIWindow.SelectionHistory.IndexOf(selected);
                while (i > 0)
                {
                    GUIUtils.MDIWindow.SelectionHistory.RemoveAt(0);
                    i -= 1;
                }

                GUIUtils.MDIWindow.Select(selected, true);
                RefreshModel();
            }
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
        /// Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            if (GUIUtils.MDIWindow != null)
            {
                List<HistoryObject> history = new List<HistoryObject>();

                foreach (Utils.IModelElement element in GUIUtils.MDIWindow.SelectionHistory)
                {
                    DataDictionary.ModelElement modelElement = element as DataDictionary.ModelElement;
                    if (modelElement != null)
                    {
                        history.Add(new HistoryObject(modelElement));
                    }
                }

                historyDataGridView.DataSource = history;
            }

            Refresh();
        }

        /// <summary>
        /// Provides the model element currently selected in this IBaseForm
        /// </summary>
        public override Utils.IModelElement Selected
        {
            get
            {
                Utils.IModelElement retVal = null;

                if (TreeView != null && TreeView.Selected != null)
                {
                    retVal = TreeView.Selected.Model;
                }

                return retVal;
            }
        }

        private class HistoryObject
        {
            /// <summary>
            /// The object that is referenced for history
            /// </summary>
            [System.ComponentModel.Browsable(false)]
            public DataDictionary.ModelElement Reference { get; private set; }

            /// <summary>
            /// The identification of the history element
            /// </summary>
            public string Model
            {
                get
                {
                    return Reference.Name;
                }
            }

            /// <summary>
            /// The type of the referenced object
            /// </summary>
            public string Type
            {
                get
                {
                    return Reference.GetType().Name;
                }
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="reference"></param>
            public HistoryObject(DataDictionary.ModelElement reference)
            {
                Reference = reference;
            }
        }
    }
}
