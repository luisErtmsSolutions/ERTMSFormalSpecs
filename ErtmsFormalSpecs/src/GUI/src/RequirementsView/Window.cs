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

namespace GUI.RequirementsView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        /// The editor used to edit these properties
        /// </summary>
        private DataDictionary.ModelElement Model { get; set; }

        /// <summary>
        /// The empty RTF
        /// </summary>
        private string EmptyRTF { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom;
            richTextBox.Enabled = true;
            richTextBox.ReadOnly = true;

            EmptyRTF = richTextBox.Rtf;
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
        public void SetModel(DataDictionary.ModelElement model)
        {
            Model = model;
            RefreshModel();
        }

        /// <summary>
        /// Refreshes the displayed messages according to the window model
        /// </summary>
        public override void RefreshModel()
        {
            if (Model != null)
            {
                richTextBox.Text = Model.RequirementDescription();
            }
            else
            {
                richTextBox.Rtf = EmptyRTF;
            }
            Refresh();
        }
    }
}
