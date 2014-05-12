﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;
using DataDictionary;

namespace GUI.PropertyView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        /// The editor used to edit these properties
        /// </summary>
        private BaseTreeNode.BaseEditor Editor { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            ResizeDescriptionArea(propertyGrid, 0);

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight;
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
        /// Sets the model element for which messages should be displayed
        /// </summary>
        /// <param name="editor"></param>
        public void SetModel(BaseTreeNode node)
        {
            if (node != null && node.NodeEditor != null)
            {
                Editor = node.NodeEditor;
                RefreshModel();
            }
        }

        /// <summary>
        /// Refreshes the displayed messages according to the window model
        /// </summary>
        public override void RefreshModel()
        {
            propertyGrid.SelectedObject = Editor;
            Refresh();
        }
    }
}
