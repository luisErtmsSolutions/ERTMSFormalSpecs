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
namespace GUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using DataDictionary;
    using DataDictionary.Specification;
    using GUI.LongOperations;
    using GUI.SpecificationView;
    using Utils;
    using WeifenLuo.WinFormsUI.Docking;
    using GUI.EditorView;

    public partial class MainWindow : Form
    {
        /// <summary>
        /// The sub forms for this window
        /// </summary>
        public HashSet<Form> SubForms { get; set; }

        /// <summary>
        /// The sub IBaseForms handled in this MDI
        /// </summary>
        public HashSet<IBaseForm> SubWindows
        {
            get
            {
                HashSet<IBaseForm> retVal = new HashSet<IBaseForm>();

                foreach (Form form in SubForms)
                {
                    if (form is IBaseForm)
                    {
                        retVal.Add((IBaseForm)form);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// The editors opened in the MDI
        /// </summary>
        public HashSet<EditorView.Window> Editors
        {
            get
            {
                HashSet<EditorView.Window> retVal = new HashSet<EditorView.Window>();

                foreach (Form form in SubForms)
                {
                    if (form is EditorView.Window)
                    {
                        retVal.Add((EditorView.Window)form);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Selects the model element in all opened sub windows
        /// </summary>
        /// <param name="model"></param>
        /// <param name="getFocus">Indicates whether the focus should be given to the enclosing form</param>
        public void Select(IModelElement model, bool getFocus = false)
        {
            if (model != null)
            {
                foreach (IBaseForm iBaseForm in SubWindows)
                {
                    BaseTreeView treeView = iBaseForm.TreeView;
                    if (treeView != null)
                    {
                        BaseTreeNode node = treeView.Select(model, getFocus);
                        if (node != null)
                        {
                            Form form = iBaseForm as Form;
                            if (form != null)
                            {
                                form.Focus();
                            }
                        }
                    }
                }
            }
        }

        public void HandleSubWindowClosed(Form form)
        {
            SubForms.Remove(form);
        }

        /// <summary>
        /// Finds a  specific window in a collection of windows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class GenericWindowHandling<T>
            where T : Form, new()
        {
            /// <summary>
            /// Finds the specified element in the collection provided
            /// </summary>
            /// <param name="?"></param>
            /// <returns></returns>
            public static T find(ICollection<Form> subWindows)
            {
                T retVal = null;

                foreach (Form form in subWindows)
                {
                    retVal = form as T;
                    if (retVal != null)
                    {
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            /// Finds the specified element in the collection provided
            /// </summary>
            /// <param name="?"></param>
            /// <returns></returns>
            public static T find(ICollection<IBaseForm> subWindows)
            {
                T retVal = null;

                foreach (IBaseForm form in subWindows)
                {
                    retVal = form as T;
                    if (retVal != null)
                    {
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            /// Displays or shows the window, at the specified location
            /// </summary>
            /// <param name="window"></param>
            /// <param name="subWindow"></param>
            /// <param name="area"></param>
            /// <param name="force">Indicates that the doc area should be set before trying to open the window</param>
            public static void AddOrShow(MainWindow window, T subWindow, DockAreas area)
            {
                if (subWindow == null)
                {
                    subWindow = new T();
                    window.AddChildWindow(subWindow, area);
                }
                else
                {
                    subWindow.Show();
                }
            }
        }

        /// <summary>
        /// Provides a message view window
        /// </summary>
        public MessagesView.Window MessagesWindow
        {
            get
            {
                return GenericWindowHandling<MessagesView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a more info view window
        /// </summary>
        public MoreInfoView.Window MoreInfoWindow
        {
            get
            {
                return GenericWindowHandling<MoreInfoView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a property view window
        /// </summary>
        public PropertyView.Window PropertyWindow
        {
            get
            {
                return GenericWindowHandling<PropertyView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a requirement view window
        /// </summary>
        public RequirementsView.Window RequirementsWindow
        {
            get
            {
                return GenericWindowHandling<RequirementsView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a usage view window
        /// </summary>
        public UsageView.Window UsageWindow
        {
            get
            {
                return GenericWindowHandling<UsageView.Window>.find(SubWindows);
            }
        }


        /// <summary>
        /// Provides a data dictionary window
        /// </summary>
        public DataDictionaryView.Window DataDictionaryWindow
        {
            get
            {
                return GenericWindowHandling<DataDictionaryView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a specification window
        /// </summary>
        public SpecificationView.Window SpecificationWindow
        {
            get
            {
                return GenericWindowHandling<SpecificationView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides the history window
        /// </summary>
        public HistoryView.Window HistoryWindow
        {
            get
            {
                return GenericWindowHandling<HistoryView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a test runner window
        /// </summary>
        public TestRunnerView.Window TestWindow
        {
            get
            {
                return GenericWindowHandling<TestRunnerView.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// Provides a watch window
        /// </summary>
        public TestRunnerView.Watch.Window WatchWindow
        {
            get
            {
                return GenericWindowHandling<TestRunnerView.Watch.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// The translation window
        /// </summary>
        private TranslationRules.Window TranslationWindow
        {
            get
            {
                TranslationRules.Window retVal = null;
                DataDictionary.Dictionary dictionary = GetActiveDictionary();
                if (dictionary != null)
                {
                    retVal = new TranslationRules.Window(dictionary.TranslationDictionary);
                    AddChildWindow(retVal, DockAreas.Document);
                }
                return retVal;
            }
        }

        /// <summary>
        /// The shortcuts window
        /// </summary>
        private Shortcuts.Window ShortcutsWindow
        {
            get
            {
                return GenericWindowHandling<Shortcuts.Window>.find(SubWindows);
            }
        }

        /// <summary>
        /// The selection history window
        /// </summary>
        private SelectionHistory.Window SelectionHistoryWindow
        {
            get
            {
                return GenericWindowHandling<SelectionHistory.Window>.find(SubWindows);
            }
        }


        /// <summary>
        /// The editor window
        /// </summary>
        private EditorView.ExpressionWindow ExpressionEditorWindow
        {
            get
            {
                return GenericWindowHandling<EditorView.ExpressionWindow>.find(SubWindows);
            }
        }

        /// <summary>
        /// The comment window
        /// </summary>
        private EditorView.CommentWindow CommentEditorWindow
        {
            get
            {
                return GenericWindowHandling<EditorView.CommentWindow>.find(SubWindows);
            }
        }


        /// <summary>
        /// The thread used to synchronize node names with their model
        /// </summary>
        private class Synchronizer : GenericSynchronizationHandler<MainWindow>
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="instance"></param>
            public Synchronizer(MainWindow instance, int cycleTime)
                : base(instance, cycleTime)
            {
            }

            /// <summary>
            /// Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(MainWindow instance)
            {
                instance.Invoke((MethodInvoker)delegate
                {
                    instance.UpdateTitle();
                    foreach (EditorView.Window editor in instance.Editors)
                    {
                        if (!editor.EditorTextBoxHasFocus())
                        {
                            editor.RefreshText();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Indicates that synchronization is required
        /// </summary>
        private Synchronizer WindowSynchronizerTask { get; set; }


        /// <summary>
        /// The class that is used to update the status
        /// </summary>
        private class StatusSynchronizer : GenericSynchronizationHandler<MainWindow>
        {
            /// <summary>
            /// The model element used to update the status bar
            /// </summary>
            private IHoldsParagraphs Model { get; set; }

            /// <summary>
            /// Indicates that the model has changed
            /// </summary>
            private bool ModelChanged { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="window"></param>
            public StatusSynchronizer(MainWindow window)
                : base(window, 100)
            {
                Model = null;
                ModelChanged = false;
            }

            /// <summary>
            /// Sets the instance to display
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(IHoldsParagraphs model)
            {
                Model = model;
                ModelChanged = true;
                Instance.Invoke((MethodInvoker)delegate
                {
                    Instance.SetStatus("Computing coverage...");
                });
            }

            /// <summary>
            /// Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(MainWindow instance)
            {
                if (ModelChanged)
                {
                    ModelChanged = false;
                    Instance.Invoke((MethodInvoker)delegate
                    {
                        Instance.SetStatus("Computing coverage...");
                    });

                    List<DataDictionary.Specification.Paragraph> paragraphs = new List<DataDictionary.Specification.Paragraph>();
                    Model.GetParagraphs(paragraphs);
                    Paragraph p = Model as Paragraph;
                    if (p != null)
                    {
                        paragraphs.Add(p);
                    }

                    string message = ParagraphTreeNode.CreateStatMessage(EFSSystem.INSTANCE, paragraphs, false);
                    instance.Invoke((MethodInvoker)delegate
                    {
                        instance.SetStatus(message);
                    });
                }
            }
        }

        /// <summary>
        /// Indicates that synchronization is required
        /// </summary>
        private StatusSynchronizer StatusSynchronizerTask { get; set; }

        /// <summary>
        /// The maximum size of the history
        /// </summary>
        private const int MAX_SELECTION_HISTORY = 100;

        /// <summary>
        /// The selection history
        /// </summary>
        public List<IModelElement> SelectionHistory { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SubForms = new HashSet<Form>();
            AllowRefresh = true;
            GUIUtils.MDIWindow = this;
            GUIUtils.Graphics = CreateGraphics();
            SelectionHistory = new List<IModelElement>();

            FormClosing += new FormClosingEventHandler(MainWindow_FormClosing);
            WindowSynchronizerTask = new Synchronizer(this, 300);
            StatusSynchronizerTask = new StatusSynchronizer(this);
            KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            Refresh();
        }

        /// <summary>
        /// Handles the closing of the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool canceled = CheckSave();
            e.Cancel = canceled;
        }

        /// <summary>
        /// Handles specific key actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        Close();
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainWindow()
        {
            try
            {
                GUIUtils.Graphics.Dispose();
                GUIUtils.Graphics = null;
            }
            catch (Exception)
            {
            }
            GUIUtils.MDIWindow = null;
        }

        /// <summary>
        /// Updates the title according to the windows state
        /// </summary>
        public void UpdateTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string versionNumber = fvi.FileVersion;

            String windowTitle = "ERTMS Formal Spec Workbench (version " + versionNumber + ")";

            foreach (DataDictionary.Dictionary dictioanry in EFSSystem.Dictionaries)
            {
                windowTitle += " " + dictioanry.FilePath;
            }

            if (EFSSystem != null && EFSSystem.ShouldSave)
            {
                windowTitle += " [modified]";
            }

            Text = windowTitle;
        }

        Dictionary<Form, Rectangle> InitialRectangle = new Dictionary<Form, Rectangle>();

        /// <summary>
        /// Adds a child window to this parent MDI
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dockArea">where to place the window</param>
        /// <returns></returns>
        public void AddChildWindow(Form window, DockAreas dockArea = DockAreas.Document)
        {
            if (window != null)
            {
                InitialRectangle[window] = new Rectangle(new Point(50, 50), window.Size);

                DockContent docContent = window as DockContent;
                if (docContent != null)
                {
                    SubForms.Add(docContent);

                    docContent.DockAreas = dockArea;
                    if (dockArea == DockAreas.DockLeft)
                    {
                        docContent.Show(dockPanel, DockState.DockLeftAutoHide);
                    }
                    else if (dockArea == DockAreas.Float)
                    {
                        docContent.Show(dockPanel, DockState.Float);
                    }
                    else
                    {
                        docContent.Show(dockPanel);
                    }
                }
                else
                {
                    SubForms.Add(window);
                    window.MdiParent = this;
                    window.Show();

                    window.Activate();
                    ActivateMdiChild(window);
                }
            }
        }

        /// <summary>
        /// Ensures that a window is closed
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private void EnsureIsClosed(Form window)
        {
            if (window != null)
            {
                try
                {
                    window.Close();
                    window.MdiParent = null;
                    SubForms.Remove(window);
                    RemoveOwnedForm(window);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Closes all child windows of this MDI window
        /// </summary>
        private void CloseChildWindows()
        {
            while (SubWindows.Count > 0)
            {
                Form window = (Form)SubWindows.First();
                EnsureIsClosed(window);
            }
        }

        /// <summary>
        /// Indicates that the refresh should be performed
        /// </summary>
        public bool AllowRefresh { get; set; }

        /// <summary>
        /// Refreshes the content of the window based on the associated model
        /// (changes may have occured)
        /// </summary>
        public void RefreshModel()
        {
            if (AllowRefresh)
            {
                foreach (Form form in SubForms)
                {
                    IBaseForm baseForm = form as IBaseForm;
                    if (baseForm != null)
                    {
                        baseForm.RefreshModel();
                    }
                    form.Refresh();
                }
            }
        }

        /// <summary>
        /// Refreshes the display of the windows.
        /// No structural model change occurred.
        /// </summary>
        public override void Refresh()
        {
            try
            {
                DataDictionary.Generated.ControllersManager.DesactivateAllNotifications();
                foreach (IBaseForm form in SubWindows)
                {
                    form.Refresh();
                }
                UpdateTitle();
            }
            finally
            {
                DataDictionary.Generated.ControllersManager.ActivateAllNotifications();
            }

            base.Refresh();
        }

        #region OpenFile
        /// ------------------------------------------------------
        ///    OPEN OPERATIONS
        /// ------------------------------------------------------

        /// <summary>
        /// The efs system
        /// </summary>
        public DataDictionary.EFSSystem EFSSystem
        {
            get { return DataDictionary.EFSSystem.INSTANCE; }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open ERTMS Formal Spec file";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    HandlingSelection = true;
                    bool allowErrors = false;
                    OpenFileOperation openFileOperation = new OpenFileOperation(openFileDialog.FileName, EFSSystem, allowErrors, true);
                    openFileOperation.ExecuteUsingProgressDialog("Opening file", true);

                    // Open the windows
                    if (openFileOperation.Dictionary != null)
                    {
                        DataDictionary.Dictionary dictionary = openFileOperation.Dictionary;
                        DataDictionary.Generated.ControllersManager.DesactivateAllNotifications();

                        // Display the document views
                        // Only open the model view window if model elements are available in the opened file
                        DataDictionaryView.Window modelWindow = null;
                        if (dictionary.NameSpaces.Count > 0)
                        {
                            modelWindow = new DataDictionaryView.Window(dictionary);
                            AddChildWindow(modelWindow, DockAreas.Document);
                        }
                        GenericWindowHandling<TestRunnerView.Window>.AddOrShow(this, TestWindow, DockAreas.Document);

                        // Display the views in the left pane
                        GenericWindowHandling<SpecificationView.Window>.AddOrShow(this, SpecificationWindow, DockAreas.DockLeft);

                        // Display the views in the bottom pane
                        GenericWindowHandling<RequirementsView.Window>.AddOrShow(this, RequirementsWindow, DockAreas.DockBottom);
                        GenericWindowHandling<UsageView.Window>.AddOrShow(this, UsageWindow, DockAreas.DockBottom);

                        GenericWindowHandling<EditorView.ExpressionWindow>.AddOrShow(this, ExpressionEditorWindow, DockAreas.DockBottom);
                        ExpressionEditorWindow.Show(RequirementsWindow.Pane, DockAlignment.Right, 0.5);
                        GenericWindowHandling<EditorView.CommentWindow>.AddOrShow(this, CommentEditorWindow, DockAreas.DockBottom);
                        GenericWindowHandling<MoreInfoView.Window>.AddOrShow(this, MoreInfoWindow, DockAreas.DockBottom);
                        GenericWindowHandling<TestRunnerView.Watch.Window>.AddOrShow(this, WatchWindow, DockAreas.DockBottom);
                        MoreInfoWindow.Show();

                        // Display the views in the right pane
                        GenericWindowHandling<PropertyView.Window>.AddOrShow(this, PropertyWindow, DockAreas.DockRight);
                        GenericWindowHandling<HistoryView.Window>.AddOrShow(this, HistoryWindow, DockAreas.DockRight);
                        HistoryWindow.Show(PropertyWindow.Pane, DockAlignment.Bottom, 0.6);
                        GenericWindowHandling<Shortcuts.Window>.AddOrShow(this, ShortcutsWindow, DockAreas.DockRight);
                        ShortcutsWindow.Show(HistoryWindow.Pane, HistoryWindow);
                        GenericWindowHandling<SelectionHistory.Window>.AddOrShow(this, SelectionHistoryWindow, DockAreas.DockRight);
                        SelectionHistoryWindow.Show(ShortcutsWindow.Pane, ShortcutsWindow);

                        GenericWindowHandling<MessagesView.Window>.AddOrShow(this, MessagesWindow, DockAreas.DockRight);
                        MessagesWindow.Show(HistoryWindow.Pane, DockAlignment.Bottom, 0.3);

                        if (modelWindow != null)
                        {
                            modelWindow.Focus();
                        }

                        SetCoverageStatus(EFSSystem);
                    }
                    else if (!openFileOperation.Dialog.Canceled)
                    {
                        MessageBox.Show("Cannot open file, please see log file (GUI.Log) for more information", "Cannot open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    HandlingSelection = false;
                    DataDictionary.Generated.ControllersManager.ActivateAllNotifications();
                }

                Refresh();
            }
        }

        #endregion

        #region SaveFile
        /// ------------------------------------------------------
        ///    SAVE OPERATIONS
        /// ------------------------------------------------------

        /// <summary>
        /// Provides the dictionary on which operation should be performed
        /// </summary>
        /// <returns></returns>
        public DataDictionary.Dictionary GetActiveDictionary()
        {
            DataDictionary.Dictionary retVal = null;

            if (EFSSystem != null)
            {
                if (EFSSystem.Dictionaries.Count == 1)
                {
                    retVal = EFSSystem.Dictionaries[0];
                }
                else if (EFSSystem.Dictionaries.Count > 1)
                {
                    DictionarySelector.DictionarySelector dictionarySelector = new DictionarySelector.DictionarySelector(EFSSystem);
                    dictionarySelector.ShowDialog(this);

                    if (dictionarySelector.Selected != null)
                    {
                        retVal = dictionarySelector.Selected;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// The tooltip associated to this form
        /// </summary>
        public ToolTip ToolTip { get { return toolTip; } }


        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary activeDictionary = GetActiveDictionary();

            if (activeDictionary != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Saving EFS file " + activeDictionary.Name;
                saveFileDialog.Filter = "EFS files (*.efs)|*.efs|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    activeDictionary.FilePath = saveFileDialog.FileName;
                    SaveOperation operation = new SaveOperation(this, activeDictionary);
                    operation.ExecuteUsingProgressDialog("Saving file " + activeDictionary.FilePath, false);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                SaveOperation operation = new SaveOperation(this, dictionary);
                operation.ExecuteUsingProgressDialog("Saving file " + dictionary.Name, false);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                SaveOperation operation = new SaveOperation(this, dictionary);
                operation.ExecuteUsingProgressDialog("Saving file " + dictionary.Name, false);
            }
        }
        #endregion

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Checks if the file should be saved before closing the window
        /// </summary>
        /// <returns></returns>
        private bool CheckSave()
        {
            bool retVal = false;

            if (EFSSystem.ShouldSave)
            {
                DialogResult result = MessageBox.Show("Model has been changed, do you want to save it ?", "Model changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        SaveOperation operation = new SaveOperation(this, EFSSystem);
                        operation.ExecuteUsingProgressDialog("Saving files", false);
                        break;

                    case System.Windows.Forms.DialogResult.No:
                        break;

                    case System.Windows.Forms.DialogResult.Cancel:
                        retVal = true;
                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The rich text box currently selected
        /// </summary>
        public EditorTextBox SelectedRichTextBox { get; set; }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Undo();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Redo();
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Cut();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Copy();
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Paste();
            }
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        #region Check model

        private void checkModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckModelOperation operation = new CheckModelOperation(EFSSystem);
            operation.ExecuteUsingProgressDialog("Check model");

            MessageCounter counter = new MessageCounter(EFSSystem);
            MessageBox.Show(counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info + " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        private void implementedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckImplementation();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void implementationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.MarkUnimplementedItems();
            }
            Refresh();
        }

        private void verificationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.MarkNotVerifiedRules();
            }
            Refresh();
        }

        private void reviewedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckReview();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        /// ------------------------------------------------------
        ///    MARKS OPERATIONS
        /// ------------------------------------------------------

        private void clearMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearMarks();
        }

        /// <summary>
        /// Clears all marks from the model/spec/tests/...
        /// </summary>
        public void ClearMarks()
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
            }
            Refresh();
        }

        private void markRequirementsWhereMoreInfoIsRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckMoreInfo();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void markImplementedButNoFunctionalTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckImplementedWithNoFunctionalTest();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void markNotImplementedButImplementationExistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckNotImplementedButImplementationExists();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void markApplicableParagraphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckApplicable();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void markImplementationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                dictionary.MarkUnimplementedTests();
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();

            Refresh();
        }

        private void markNotTranslatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                dictionary.MarkNotTranslatedTests();
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();

            Refresh();
        }

        private void markNotImplementedTranslationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                dictionary.MarkNotImplementedTranslations();
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();

            Refresh();
        }

        private void markNonApplicableRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckNonApplicable();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void markSpecIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckSpecIssues();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        #region Import test database
        /// ------------------------------------------------------
        ///    IMPORT TEST DATABASE OPERATIONS
        /// ------------------------------------------------------
        private void importDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open test sequence database";
                openFileDialog.Filter = "Access Files (*.mdb)|*.mdb";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ImportTestDataBaseOperation operation = new ImportTestDataBaseOperation(openFileDialog.FileName, dictionary, ImportTestDataBaseOperation.Mode.File);
                    operation.ExecuteUsingProgressDialog("Import database");

                    // Updates the test tree view data
                    if (TestWindow != null)
                    {
                        TestWindow.TreeView.RefreshModel();
                        Refresh();
                    }
                }
            }
        }

        private void importFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
                if (selectFolderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ImportTestDataBaseOperation operation = new ImportTestDataBaseOperation(selectFolderDialog.SelectedPath, dictionary, ImportTestDataBaseOperation.Mode.Directory);
                    operation.ExecuteUsingProgressDialog("Import database directory");

                    // Updates the test tree view data
                    if (TestWindow != null)
                    {
                        TestWindow.TreeView.RefreshModel();
                        Refresh();
                    }
                }
            }
        }
        #endregion

        /// ------------------------------------------------------
        ///    CREATE REPORT OPERATIONS
        /// ------------------------------------------------------

        private void specCoverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.SpecReport aReport = new Report.SpecReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void testsCoverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.TestReport aReport = new Report.TestReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateDynamicCoverageReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.TestReport aReport = new Report.TestReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateCoverageReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.SpecReport aReport = new Report.SpecReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateSpecIssuesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.SpecIssuesReport aReport = new Report.SpecIssuesReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateDataDictionaryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.ModelReport aReport = new Report.ModelReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SearchDialog.SearchDialog dialog = new SearchDialog.SearchDialog();
            dialog.Initialise(EFSSystem);
            dialog.ShowDialog(this);
        }

        private void refreshWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure the system has been compiled
            EFSSystem efsSystem = EFSSystem.INSTANCE;
            efsSystem.Compiler.Compile_Synchronous(efsSystem.ShouldRebuild);
            efsSystem.ShouldRebuild = false;

            RefreshModel();
        }

        private void showRulePerformancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RulePerformances.RulesPerformances rulePerformances = new RulePerformances.RulesPerformances(EFSSystem);
            AddChildWindow(rulePerformances, DockAreas.Document);
        }

        /// <summary>
        /// ReInit counters in rules
        /// </summary>
        private class ResetTimeStamps : DataDictionary.Generated.Visitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="efsSystem"></param>
            public ResetTimeStamps(DataDictionary.EFSSystem efsSystem)
            {
                foreach (DataDictionary.Dictionary dictionary in efsSystem.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(DataDictionary.Generated.Rule obj, bool visitSubNodes)
            {
                DataDictionary.Rules.Rule rule = obj as DataDictionary.Rules.Rule;

                rule.ExecutionTimeInMilli = 0;
                rule.ExecutionCount = 0;

                base.visit(obj, visitSubNodes);
            }

            public override void visit(DataDictionary.Generated.Function obj, bool visitSubNodes)
            {
                DataDictionary.Functions.Function function = obj as DataDictionary.Functions.Function;

                function.ExecutionTimeInMilli = 0;
                function.ExecutionCount = 0;

                base.visit(obj);
            }

            public override void visit(DataDictionary.Generated.Frame obj, bool visitSubNodes)
            {
                // No rules in frames
            }
        }

        private void resetCountersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EFSSystem != null)
            {
                ResetTimeStamps reset = new ResetTimeStamps(EFSSystem);
            }
        }

        private void showFunctionsPerformancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsPerformances.FunctionsPerformances functionsPerformances = new FunctionsPerformances.FunctionsPerformances(EFSSystem);
            AddChildWindow(functionsPerformances, DockAreas.Document);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creates a new dictionary
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Create new dictionary. Select dictionary file location";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs";
            openFileDialog.CheckFileExists = false;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                DataDictionary.Dictionary dictionary = new DataDictionary.Dictionary();
                dictionary.FilePath = filePath;
                dictionary.Name = Path.GetFileNameWithoutExtension(filePath);
                EFSSystem.AddDictionary(dictionary);
                RefreshModel();

                // Open a data dictionary window if none is yet present
                bool found = false;
                foreach (IBaseForm form in SubWindows)
                {
                    if (form is DataDictionaryView.Window)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    AddChildWindow(new DataDictionaryView.Window(dictionary), DockAreas.Document);
                }
            }
        }

        private void markParagraphsFromNewRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();

                foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                {
                    specification.CheckNewRevision();
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void generateERTMSAcademyReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.ERTMSAcademyReport aReport = new Report.ERTMSAcademyReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void compareWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open ERTMS Formal Spec file";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                DataDictionary.Dictionary dictionary = GetActiveDictionary();

                CompareWithFileOperation operation = new CompareWithFileOperation(dictionary, openFileDialog.FileName);
                operation.ExecuteUsingProgressDialog("Compare with file");

                Refresh();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.Options optionForm = new Options.Options();
            optionForm.Setup(EFSSystem);
            optionForm.ShowDialog(this);
            optionForm.UpdateSystem(EFSSystem);
        }

        private void compareWithGitRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Compares with the active dictionary
            DataDictionary.Dictionary dictionary = GetActiveDictionary();

            // Retrieve the hash tag and the corresponding dictionary version
            VersionSelector.VersionSelector selector = new VersionSelector.VersionSelector(dictionary);
            selector.Text = "Compare current version with with repository version";
            selector.ShowDialog();
            if (selector.Selected != null)
            {
                CompareWithRepositoryOperation operation = new CompareWithRepositoryOperation(dictionary, selector.Selected);
                operation.ExecuteUsingProgressDialog("Compare with repository version " + selector.Selected.MessageShort);
                Refresh();
            }
        }

        /// <summary>
        /// Refreshes all graph views
        /// </summary>
        public void RefreshAfterStep()
        {
            foreach (Form form in SubForms)
            {
                if (form is GraphView.GraphView)
                {
                    ((GraphView.GraphView)form).RefreshAfterStep();
                }
                if (form is DataDictionaryView.Window)
                {
                    ((DataDictionaryView.Window)form).RefreshAfterStep();
                }
                if (form is StateDiagram.StateDiagramWindow)
                {
                    ((StateDiagram.StateDiagramWindow)form).RefreshAfterStep();
                }
                if (form is TestRunnerView.Window)
                {
                    ((TestRunnerView.Window)form).RefreshAfterStep();
                }
                if (form is TestRunnerView.Watch.Window)
                {
                    ((TestRunnerView.Watch.Window)form).RefreshAfterStep();
                }
            }
        }

        /// <summary>
        /// Indicates that the MDI window is currently handling a selection change
        /// </summary>
        public bool HandlingSelection { get; set; }

        /// <summary>
        /// Keeps track of a new selection
        /// </summary>
        /// <param name="selected"></param>
        public void HandleSelection(BaseTreeNode selected)
        {
            if (selected != null && selected.Model != null)
            {
                IModelElement model = selected.Model;
                if (!HandlingSelection)
                {
                    try
                    {
                        HandlingSelection = true;

                        // Messages
                        MessagesView.Window messageView = MessagesWindow;
                        if (messageView != null)
                        {
                            messageView.SetModel(model);
                        }

                        // More info
                        MoreInfoView.Window moreInfoView = MoreInfoWindow;
                        if (moreInfoView != null)
                        {
                            moreInfoView.SetModel(model as TextualExplain);
                        }

                        // Properties
                        PropertyView.Window propertyView = PropertyWindow;
                        if (propertyView != null)
                        {
                            propertyView.SetModel(selected);
                        }

                        // Related requirements
                        RequirementsView.Window requirementsView = RequirementsWindow;
                        if (requirementsView != null)
                        {
                            requirementsView.SetModel((DataDictionary.ModelElement)model);
                        }

                        // Expression editor view
                        EditorView.ExpressionWindow editorView = ExpressionEditorWindow;
                        if (editorView != null)
                        {
                            IExpressionable expressionable = model as IExpressionable;
                            if (expressionable != null)
                            {
                                editorView.setChangeHandler(new ExpressionableTextChangeHandler((DataDictionary.ModelElement)expressionable));
                            }
                            else
                            {
                                Paragraph paragraph = model as Paragraph;
                                if (paragraph != null)
                                {
                                    editorView.setChangeHandler(new ParagraphTextChangeHandler(paragraph));
                                }

                                else
                                {
                                    editorView.setChangeHandler(null);
                                }
                            }
                        }

                        // Comment editor view
                        EditorView.CommentWindow commentView = CommentEditorWindow;
                        if (commentView != null)
                        {
                            commentView.setChangeHandler(new CommentableTextChangeHandler((DataDictionary.ModelElement)(model as ICommentable)));
                        }

                        // Uages 
                        UsageView.Window usageView = UsageWindow;
                        if (usageView != null)
                        {
                            usageView.SetModel((DataDictionary.ModelElement)model);
                        }

                        // History
                        if (SelectionHistory.Count > MAX_SELECTION_HISTORY)
                        {
                            SelectionHistory.RemoveAt(SelectionHistory.Count - 1);
                        }

                        if (SelectionHistory.Count == 0 || SelectionHistory[0] != model)
                        {
                            SelectionHistory.Insert(0, model);
                            SelectionHistory.Window selectionHistoryWindow = GenericWindowHandling<SelectionHistory.Window>.find(SubForms);
                            if (selectionHistoryWindow != null)
                            {
                                selectionHistoryWindow.RefreshModel();
                            }
                        }
                    }
                    finally
                    {
                        HandlingSelection = false;
                    }
                }
            }
        }

        private void generateFunctionalAnalysisReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                Report.FunctionalAnalysisReport aReport = new Report.FunctionalAnalysisReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void dockedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockContent dockContent = SelectedForm() as DockContent;
            if (dockContent != null)
            {
                if (dockContent.DockAreas == DockAreas.Document)
                {
                    dockContent.Hide();
                    Rectangle rectangle = InitialRectangle[dockContent];
                    dockContent.DockAreas = DockAreas.Float;
                    dockContent.DockState = DockState.Float;
                    dockContent.Show(dockPanel, rectangle);
                    dockContent.ParentForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                }
            }
        }

        /// <summary>
        /// Provides the selected form
        /// </summary>
        /// <returns></returns>
        private Form SelectedForm()
        {
            Form retVal = null;

            foreach (DockContent dockContent in dockPanel.Contents)
            {
                if (dockContent.IsActivated)
                {
                    retVal = dockContent;
                    break;
                }
            }

            return retVal;
        }

        private void showSpecificationViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GenericWindowHandling<SpecificationView.Window>.AddOrShow(this, SpecificationWindow, DockAreas.DockLeft);
        }

        private void showModelViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                AddChildWindow(new DataDictionaryView.Window(dictionary), DockAreas.Document);
            }
        }

        private void showShortcutsViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GenericWindowHandling<Shortcuts.Window>.AddOrShow(this, ShortcutsWindow, DockAreas.DockRight);
        }

        private void showTestsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GenericWindowHandling<TestRunnerView.Window>.AddOrShow(this, TestWindow, DockAreas.Document);
        }

        private void showTranslationViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildWindow(TranslationWindow, DockAreas.Document);
        }

        /// <summary>
        /// Sets the status of the window
        /// </summary>
        /// <param name="statusText"></param>
        public void SetStatus(string statusText)
        {
            toolStripStatusLabel.Text = statusText;
        }

        /// <summary>
        /// Sets the default status
        /// </summary>
        public void SetCoverageStatus(IHoldsParagraphs model)
        {
            StatusSynchronizerTask.SetModel(model);
        }

        private void blameUntilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Compares with the active dictionary
            DataDictionary.Dictionary dictionary = GetActiveDictionary();
            string workingDir = Path.GetDirectoryName(dictionary.FilePath);
            string historyLocation = workingDir + Path.DirectorySeparatorChar + dictionary.Name + ".hst";

            // Retrieve the hash tag
            VersionSelector.VersionSelector selector = new VersionSelector.VersionSelector(dictionary);
            selector.Text = "Select the version up to which blame mode should be built";
            selector.ShowDialog();

            UpdateBlameInformationOperation operation = new UpdateBlameInformationOperation(dictionary, selector.Selected);
            operation.ExecuteUsingProgressDialog("Update blame information");
        }

        private void showHistoryViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HistoryWindow == null)
            {
                HistoryView.Window window = new HistoryView.Window();
                AddChildWindow(window, DockAreas.Document);
            }
        }

        private void markNotTestedButFunctionalTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataDictionary.Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
                if (dictionary.Specifications != null)
                {
                    foreach (DataDictionary.Specification.Specification specification in dictionary.Specifications)
                    {
                        specification.CheckNotTestedWithFunctionalTests();
                    }
                }
            }
            EFSSystem.INSTANCE.Markings.RegisterCurrentMarking();
            Refresh();
        }

        private void checkModelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CheckModelOperation operation = new CheckModelOperation(EFSSystem);
            operation.ExecuteUsingProgressDialog("Check model");

            MessageCounter counter = new MessageCounter(EFSSystem);
            MessageBox.Show(counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info + " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CheckDeadModelOperation operation = new CheckDeadModelOperation(EFSSystem);
            operation.ExecuteUsingProgressDialog("Check dead model");

            MessageCounter counter = new MessageCounter(EFSSystem);
            MessageBox.Show(counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info + " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showRequirementSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequirementSetDiagram.RequirementSetDiagramWindow window = new RequirementSetDiagram.RequirementSetDiagramWindow();
            GUIUtils.MDIWindow.AddChildWindow(window);

            DataDictionary.Dictionary dictionary = GUIUtils.MDIWindow.GetActiveDictionary();
            window.SetEnclosing(dictionary);
            window.Text = "Requirement sets for " + dictionary.Name;
        }

        private void showWatchViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<TestRunnerView.Watch.Window>.AddOrShow(this, WatchWindow, DockAreas.DockBottom);
        }

        private void showMessagesVoewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<MessagesView.Window>.AddOrShow(this, MessagesWindow, DockAreas.DockBottom);
        }

        private void showMoreInfoViewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<MoreInfoView.Window>.AddOrShow(this, MoreInfoWindow, DockAreas.DockBottom);
        }

        private void showProperyViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<PropertyView.Window>.AddOrShow(this, PropertyWindow, DockAreas.DockRight);
        }

        private void showRequirementViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<RequirementsView.Window>.AddOrShow(this, RequirementsWindow, DockAreas.DockBottom);
        }

        private void showUsageViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<UsageView.Window>.AddOrShow(this, UsageWindow, DockAreas.DockBottom);
        }

        private void showSelectionHistoryViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<SelectionHistory.Window>.AddOrShow(this, SelectionHistoryWindow, DockAreas.DockBottom);
        }

        private void showExpressionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<EditorView.ExpressionWindow>.AddOrShow(this, ExpressionEditorWindow, DockAreas.DockBottom);
        }

        private void showCommentEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<EditorView.CommentWindow>.AddOrShow(this, CommentEditorWindow, DockAreas.DockBottom);
        }
    }
}