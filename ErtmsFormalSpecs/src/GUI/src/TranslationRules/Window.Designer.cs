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
namespace GUI.TranslationRules
{
    partial class Window
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.testBrowserStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.nextErrortoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextWarningToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.translationTreeView = new GUI.TranslationRules.TranslationTreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid = new GUI.MyPropertyGrid();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.sourceTextBox = new GUI.EditorTextBox();
            this.explainTextBox = new GUI.ExplainTextBox();
            this.expressionEditorTextBox = new GUI.EditorTextBox();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.testBrowserStatusLabel,
            this.toolStripSeparator2,
            this.nextErrortoolStripButton,
            this.nextWarningToolStripButton,
            this.nextInfoToolStripButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(926, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // testBrowserStatusLabel
            // 
            this.testBrowserStatusLabel.Name = "testBrowserStatusLabel";
            this.testBrowserStatusLabel.Size = new System.Drawing.Size(86, 22);
            this.testBrowserStatusLabel.Text = "toolStripLabel1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // nextErrortoolStripButton
            // 
            this.nextErrortoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextErrortoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextErrortoolStripButton.Image")));
            this.nextErrortoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextErrortoolStripButton.Name = "nextErrortoolStripButton";
            this.nextErrortoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextErrortoolStripButton.Text = "Next error";
            this.nextErrortoolStripButton.Click += new System.EventHandler(this.nextErrortoolStripButton_Click);
            // 
            // nextWarningToolStripButton
            // 
            this.nextWarningToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextWarningToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextWarningToolStripButton.Image")));
            this.nextWarningToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextWarningToolStripButton.Name = "nextWarningToolStripButton";
            this.nextWarningToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextWarningToolStripButton.Text = "Next warning";
            this.nextWarningToolStripButton.Click += new System.EventHandler(this.nextWarningToolStripButton_Click);
            // 
            // nextInfoToolStripButton
            // 
            this.nextInfoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextInfoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextInfoToolStripButton.Image")));
            this.nextInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextInfoToolStripButton.Name = "nextInfoToolStripButton";
            this.nextInfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextInfoToolStripButton.Text = "Next info";
            this.nextInfoToolStripButton.Click += new System.EventHandler(this.nextInfoToolStripButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.translationTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(926, 548);
            this.splitContainer1.SplitterDistance = 306;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // translationTreeView
            // 
            this.translationTreeView.AllowDrop = true;
            this.translationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.translationTreeView.HideSelection = false;
            this.translationTreeView.ImageIndex = 0;
            this.translationTreeView.LabelEdit = true;
            this.translationTreeView.Location = new System.Drawing.Point(0, 0);
            this.translationTreeView.Name = "translationTreeView";
            this.translationTreeView.Root = null;
            this.translationTreeView.Selected = null;
            this.translationTreeView.SelectedImageIndex = 0;
            this.translationTreeView.Size = new System.Drawing.Size(306, 548);
            this.translationTreeView.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(617, 548);
            this.splitContainer2.SplitterDistance = 216;
            this.splitContainer2.TabIndex = 1;
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowDrop = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(617, 216);
            this.propertyGrid.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.sourceTextBox);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.explainTextBox);
            this.splitContainer3.Panel2.Controls.Add(this.expressionEditorTextBox);
            this.splitContainer3.Size = new System.Drawing.Size(617, 328);
            this.splitContainer3.SplitterDistance = 300;
            this.splitContainer3.TabIndex = 0;
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.AutoComplete = true;
            this.sourceTextBox.BackColor = System.Drawing.Color.Transparent;
            this.sourceTextBox.ConsiderOnlyTypes = false;
            this.sourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceTextBox.Instance = null;
            this.sourceTextBox.Location = new System.Drawing.Point(0, 0);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.ReadOnly = false;
            this.sourceTextBox.Rtf = resources.GetString("sourceTextBox.Rtf");
            this.sourceTextBox.Size = new System.Drawing.Size(300, 328);
            this.sourceTextBox.TabIndex = 0;
            // 
            // explainTextBox
            // 
            this.explainTextBox.AutoComplete = true;
            this.explainTextBox.BackColor = System.Drawing.Color.Transparent;
            this.explainTextBox.ConsiderOnlyTypes = false;
            this.explainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explainTextBox.Instance = null;
            this.explainTextBox.Location = new System.Drawing.Point(0, 0);
            this.explainTextBox.Name = "explainTextBox";
            this.explainTextBox.ReadOnly = false;
            this.explainTextBox.Rtf = resources.GetString("explainTextBox.Rtf");
            this.explainTextBox.Size = new System.Drawing.Size(313, 328);
            this.explainTextBox.TabIndex = 1;
            // 
            // expressionEditorTextBox
            // 
            this.expressionEditorTextBox.AutoComplete = true;
            this.expressionEditorTextBox.BackColor = System.Drawing.Color.Transparent;
            this.expressionEditorTextBox.ConsiderOnlyTypes = false;
            this.expressionEditorTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expressionEditorTextBox.Instance = null;
            this.expressionEditorTextBox.Location = new System.Drawing.Point(0, 0);
            this.expressionEditorTextBox.Name = "expressionEditorTextBox";
            this.expressionEditorTextBox.ReadOnly = false;
            this.expressionEditorTextBox.Rtf = resources.GetString("expressionEditorTextBox.Rtf");
            this.expressionEditorTextBox.Size = new System.Drawing.Size(313, 328);
            this.expressionEditorTextBox.TabIndex = 0;
            this.expressionEditorTextBox.Visible = false;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 573);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.Text = "Translation rules";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel testBrowserStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private TranslationTreeView translationTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private MyPropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripButton nextErrortoolStripButton;
        private System.Windows.Forms.ToolStripButton nextWarningToolStripButton;
        private System.Windows.Forms.ToolStripButton nextInfoToolStripButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private EditorTextBox sourceTextBox;
        private EditorTextBox expressionEditorTextBox;
        private ExplainTextBox explainTextBox;

    }
}