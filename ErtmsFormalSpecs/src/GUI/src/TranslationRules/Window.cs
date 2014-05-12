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
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using DataDictionary;

namespace GUI.TranslationRules
{
    public partial class Window : BaseForm
    {
        public override MyPropertyGrid Properties
        {
            get { return propertyGrid; }
        }

        public override EditorTextBox RequirementsTextBox
        {
            get { return sourceTextBox; }
        }

        public override EditorTextBox ExpressionEditorTextBox
        {
            get { return expressionEditorTextBox; }
        }

        public override ExplainTextBox ExplainTextBox
        {
            get { return explainTextBox; }
        }

        public override BaseTreeView TreeView
        {
            get { return translationTreeView; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window(DataDictionary.Tests.Translations.TranslationDictionary dictionary)
        {
            InitializeComponent();

            sourceTextBox.AutoComplete = false;
            explainTextBox.AutoComplete = false;

            sourceTextBox.ReadOnly = true;
            explainTextBox.ReadOnly = true;

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            expressionEditorTextBox.TextBox.TextChanged += new EventHandler(TextBox_TextChanged);
            Visible = false;
            translationTreeView.Root = dictionary;
            Text = dictionary.Dictionary.Name + " test translation view";

            ResizeDescriptionArea(propertyGrid, 20);

            Refresh();
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            IExpressionable expressionable = Selected as IExpressionable;
            if (expressionable != null && expressionable == expressionEditorTextBox.Instance)
            {
                expressionable.ExpressionText = expressionEditorTextBox.TextBox.Text;
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
        /// Refreshes the display
        /// </summary>
        override public void Refresh()
        {
            translationTreeView.Refresh();

            testBrowserStatusLabel.Text = translationTreeView.Root.TranslationsCount + " translation rule(s) loaded";
            base.Refresh();
        }

        /// <summary>
        /// Clears messages for the element stored in the tree view in the window
        /// </summary>
        public void Clear()
        {
            translationTreeView.ClearMessages();
            GUIUtils.MDIWindow.Refresh();
        }

        /// <summary>
        /// Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            translationTreeView.RefreshModel();
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
        /// Selects the next node where warning message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextWarningToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(Utils.ElementLog.LevelEnum.Warning);
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
    }
}
