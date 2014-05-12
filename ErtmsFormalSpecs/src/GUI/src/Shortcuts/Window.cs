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

namespace GUI.Shortcuts
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

            Visible = false;
            shortcutTreeView.Root = DataDictionary.EFSSystem.INSTANCE;
            Text = "Shortcuts view";

            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight;
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
        /// Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            shortcutTreeView.RefreshModel();
            Refresh();
        }

        public override BaseTreeView TreeView
        {
            get { return shortcutTreeView; }
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
    }
}
