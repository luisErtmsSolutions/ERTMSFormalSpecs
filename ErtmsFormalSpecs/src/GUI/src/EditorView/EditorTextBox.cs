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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Values;
namespace GUI
{
    public partial class EditorTextBox : UserControl
    {
        /// <summary>
        /// Indicates that only types should be considered in the search
        /// </summary>
        public bool ConsiderOnlyTypes { get; set; }

        /// <summary>
        /// Indicates whether there is a pending selection in the combo box
        /// </summary>
        private bool PendingSelection { get; set; }

        /// <summary>
        /// The initial RTF data, used to initialise back the control and remove all formatting information
        /// </summary>
        private string InitialRTF { get; set; }

        /// <summary>
        /// The enclosing IBaseForm
        /// </summary>
        private IBaseForm EnclosingForm
        {
            get
            {
                Control current = Parent;
                while (current != null && !(current is IBaseForm))
                {
                    current = current.Parent;
                }

                return current as IBaseForm;
            }
        }

        private Utils.IModelElement __instance = null;

        /// <summary>
        /// Provides the instance on which this editor is based
        /// </summary>
        public Utils.IModelElement Instance
        {
            get
            {
                if (__instance == null && EnclosingForm != null)
                {
                    __instance = (Utils.IModelElement)EnclosingForm.Selected;
                }
                return __instance;
            }
            set
            {
                __instance = value;
            }
        }

        /// <summary>
        /// Provides the EFSSystem 
        /// </summary>
        private EFSSystem EFSSystem
        {
            get
            {
                EFSSystem retVal = null;

                DataDictionary.ModelElement modelElement = Instance as DataDictionary.ModelElement;
                if (modelElement != null)
                {
                    retVal = modelElement.EFSSystem;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Indicates that autocompletion is active for the text box
        /// </summary>
        public bool AutoComplete { get; set; }

        /// <summary>
        /// A clean and empty RTF text
        /// </summary>
        private string CleanText { get; set; }

        /// <summary>
        /// Indicates that a mouse mouve event hides the explanation
        /// </summary>
        private bool ConsiderMouseMoveToCloseExplanation { get; set; }

        /// <summary>
        /// The location of the mouse
        /// </summary>
        private Point MouseLocation { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EditorTextBox()
        {
            InitializeComponent();
            InitialRTF = EditionTextBox.Rtf;

            AutoComplete = true;

            EditionTextBox.AllowDrop = true;
            EditionTextBox.DragDrop += new DragEventHandler(Editor_DragDropHandler);
            EditionTextBox.KeyUp += new KeyEventHandler(Editor_KeyUp);
            EditionTextBox.KeyPress += new KeyPressEventHandler(Editor_KeyPress);
            EditionTextBox.MouseUp += new MouseEventHandler(EditionTextBox_MouseClick);
            EditionTextBox.ShortcutsEnabled = true;
            EditionTextBox.MouseMove += new MouseEventHandler(EditionTextBox_MouseMove);

            EditionTextBox.GotFocus += new EventHandler(Editor_GotFocus);
            EditionTextBox.LostFocus += new EventHandler(Editor_LostFocus);
            SelectionComboBox.LostFocus += new EventHandler(SelectionComboBox_LostFocus);
            SelectionComboBox.KeyUp += new KeyEventHandler(SelectionComboBox_KeyUp);
            SelectionComboBox.SelectedValueChanged += new EventHandler(SelectionComboBox_SelectedValueChanged);
            SelectionComboBox.DropDownStyleChanged += new EventHandler(SelectionComboBox_DropDownStyleChanged);
            SelectionComboBox.LocationChanged += new EventHandler(SelectionComboBox_LocationChanged);

            CleanText = explainRichTextBox.Rtf;
        }

        /// <summary>
        /// Displays the help associated to a location in the text box
        /// </summary>
        /// <param name="location"></param>
        private void DisplayHelp(Point location)
        {
            List<Utils.INamable> instances = GetInstances(location);

            if (instances.Count > 0)
            {
                location.Offset(10, 10);
                bool considerMouseMove = true;
                ExplainAndShow(instances, location, considerMouseMove);
            }
            else
            {
                contextMenuStrip1.Show(PointToScreen(MouseLocation));
            }
        }

        /// <summary>
        /// Provides the instances related to a location in the textbox
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<Utils.INamable> GetInstances(Point location)
        {
            List<Utils.INamable> retVal = new List<Utils.INamable>();

            if (Instance != null)
            {
                int index = EditionTextBox.GetCharIndexFromPosition(location);

                int start = index;
                while (start > 0 && ValidIdentifierCharacter(EditionTextBox.Text[start]))
                {
                    start -= 1;
                }
                if (start > 0)
                {
                    start += 1;
                }

                int end = index;
                while (end < EditionTextBox.Text.Length && ValidIdentifierCharacter(EditionTextBox.Text[end]) && EditionTextBox.Text[end] != '.')
                {
                    end += 1;
                }
                if (end < EditionTextBox.Text.Length)
                {
                    end -= 1;
                }

                if (start < end)
                {
                    string identifier = EditionTextBox.Text.Substring(start, Math.Min(end - start + 1, EditionTextBox.Text.Length - start));
                    Expression expression = EFSSystem.Parser.Expression(Instance as ModelElement, identifier, AllMatches.INSTANCE, true, null, true);
                    if (expression != null)
                    {
                        if (expression.Ref != null)
                        {
                            retVal.Add(expression.Ref);
                        }
                        else
                        {
                            bool last = end == EditionTextBox.Text.Length || EditionTextBox.Text[end] != '.';
                            ReturnValue returnValue = expression.getReferences(Instance, AllMatches.INSTANCE, last);
                            foreach (ReturnValueElement element in returnValue.Values)
                            {
                                retVal.Add(element.Value);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Takes case of the mouse move to hide the explain text box, if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditionTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation = e.Location;
            if (ConsiderMouseMoveToCloseExplanation && explainRichTextBox.Visible)
            {
                Rectangle rectangle = explainRichTextBox.DisplayRectangle;
                rectangle.Location = explainRichTextBox.Location;
                rectangle.Inflate(20, 20);
                if (!rectangle.Contains(e.Location))
                {
                    explainRichTextBox.Hide();
                    ConsiderMouseMoveToCloseExplanation = false;
                }
            }
        }

        /// <summary>
        /// Indicates that the character belongs to a fully qualified identifier
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool ValidIdentifierCharacter(char c)
        {
            bool retVal = Char.IsLetterOrDigit(c) || c == '_' || c == '.';

            return retVal;
        }

        void EditionTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MouseLocation = e.Location;
                DisplayHelp(e.Location);
            }

            if (Control.ModifierKeys == Keys.Control)
            {
                List<Utils.INamable> instances = GetInstances(e.Location);
                if (instances.Count == 1)
                {
                    MainWindow mdiWindow = GUIUtils.MDIWindow;
                    if (mdiWindow != null)
                    {
                        mdiWindow.Select(instances[0] as Utils.ModelElement, true);
                    }
                }
            }
        }

        void SelectionComboBox_LocationChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference)SelectionComboBox.SelectedItem, Point.Empty);
        }

        void SelectionComboBox_DropDownStyleChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference)SelectionComboBox.SelectedItem, Point.Empty);
        }

        void SelectionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference)SelectionComboBox.SelectedItem, Point.Empty);
        }

        /// <summary>
        /// Explains a reference and shows the associated textbox
        /// </summary>
        /// <param name="reference">The object reference to explain</param>
        /// <param name="location">The location where the explain box should be displayed. If empty is displayed, the location is computed based on the combo box location</param>
        /// <param name="sensibleToMouseMove">Indicates that the explain box should be closed when the mouse moves</param>
        private void ExplainAndShowReference(ObjectReference reference, Point location, bool sensibleToMouseMove = false)
        {
            if (reference != null)
            {
                ExplainAndShow(reference.Models, location, sensibleToMouseMove);
            }
        }

        /// <summary>
        /// Explains a list of namables and shows the associated textbox
        /// </summary>
        /// <param name="namable">The namable to explain</param>
        /// <param name="location">The location where the explain box should be displayed. If empty is displayed, the location is computed based on the combo box location</param>
        /// <param name="sensibleToMouseMove">Indicates that the explain box should be closed when the mouse moves</param>
        private void ExplainAndShow(List<Utils.INamable> namables, Point location, bool sensibleToMouseMove)
        {
            explainRichTextBox.Rtf = CleanText;
            if (namables != null)
            {
                string data = "";
                foreach (Utils.INamable namable in namables)
                {
                    data += "{\\b " + namable.GetType().Name + "} " + namable.Name + "\\par ";
                    ICommentable commentable = namable as ICommentable;
                    if (commentable != null)
                    {
                        if (String.IsNullOrEmpty(commentable.Comment))
                        {
                            data += "{\\i No description available }";
                        }
                        else
                        {
                            data += commentable.Comment;
                        }
                    }
                    data += "\\par\\par";
                }

                explainRichTextBox.Rtf = TextualExplainUtilities.Encapsule(data);

                if (location == Point.Empty)
                {
                    if (SelectionComboBox.DroppedDown)
                    {
                        explainRichTextBox.Location = new Point(
                            SelectionComboBox.Location.X + SelectionComboBox.Size.Width,
                            SelectionComboBox.Location.Y + SelectionComboBox.Size.Height
                        );
                    }
                    else
                    {
                        explainRichTextBox.Location = new Point(
                            SelectionComboBox.Location.X,
                            SelectionComboBox.Location.Y + SelectionComboBox.Size.Height
                        );
                    }
                }
                else
                {
                    explainRichTextBox.Location = new Point(
                        Math.Min(location.X, EditionTextBox.Size.Width - explainRichTextBox.Size.Width),
                        Math.Min(location.Y, EditionTextBox.Size.Height - explainRichTextBox.Size.Height));
                }

                ConsiderMouseMoveToCloseExplanation = sensibleToMouseMove;
                explainRichTextBox.Show();
                EditionTextBox.SendToBack();
            }
        }

        /// <summary>
        /// The text box
        /// </summary>
        public RichTextBox TextBox { get { return EditionTextBox; } }

        public void Copy()
        {
            EditionTextBox.Copy();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        public void Cut()
        {
            EditionTextBox.Cut();
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        public void Paste()
        {
            EditionTextBox.Paste();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        public void Undo()
        {
            EditionTextBox.Undo();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        public void Redo()
        {
            EditionTextBox.Redo();
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        /// <summary>
        /// Provides the list of available elements which match the prefix provided
        /// </summary>
        /// <param name="element">The element in which the possibilities should be found</param>
        /// <param name="searchOptions"></param>
        /// <returns></returns>
        private HashSet<ObjectReference> GetPossibilities(Utils.IModelElement element, SearchOptions searchOptions)
        {
            HashSet<ObjectReference> retVal = new HashSet<ObjectReference>();

            while (element != null)
            {
                bool type = false;

                Utils.ISubDeclarator subDeclarator = element as Utils.ISubDeclarator;
                if (subDeclarator == null)
                {
                    DataDictionary.Types.ITypedElement typedElement = element as DataDictionary.Types.ITypedElement;
                    if (typedElement != null)
                    {
                        type = true;
                        subDeclarator = typedElement.Type as Utils.ISubDeclarator;
                    }
                }

                DataDictionary.Variables.IVariable variable = subDeclarator as DataDictionary.Variables.IVariable;
                if (variable != null)
                {
                    type = true;
                    subDeclarator = variable.Type as Utils.ISubDeclarator;
                }

                if (subDeclarator != null)
                {
                    subDeclarator.InitDeclaredElements();
                    foreach (KeyValuePair<string, List<Utils.INamable>> pair in subDeclarator.DeclaredElements)
                    {
                        string subElem = pair.Key;
                        if (subElem.StartsWith(searchOptions.Prefix))
                        {
                            if (searchOptions.EnclosingName != null)
                            {
                                foreach (Utils.INamable namable in subDeclarator.DeclaredElements[subElem])
                                {
                                    if (namable.FullName.EndsWith(searchOptions.EnclosingName + "." + subElem) || type || subDeclarator is StructureElement)
                                    {
                                        if (ConsiderOnlyTypes)
                                        {
                                            if (namable is DataDictionary.Types.Type || namable is DataDictionary.Types.NameSpace)
                                            {
                                                if (!(namable is DataDictionary.Functions.Function))
                                                {
                                                    retVal.Add(new ObjectReference(pair.Key, pair.Value));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retVal.Add(new ObjectReference(pair.Key, pair.Value));
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                retVal.Add(new ObjectReference(pair.Key, pair.Value));
                            }
                        }
                    }
                }

                if (searchOptions.ConsiderEnclosing)
                {
                    element = element.Enclosing as Utils.IModelElement;
                }
                else
                {
                    element = null;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the current prefix, according to the selection position
        /// </summary>
        /// <returns></returns>
        private string CurrentPrefix()
        {
            string retVal = "";

            int end = EditionTextBox.SelectionStart - 1;
            while (end >= EditionTextBox.Text.Length)
            {
                end = end - 1;
            }

            while (end >= 0 && Char.IsSeparator(EditionTextBox.Text[end]))
            {
                end = end - 1;
            }

            int parenthesis = 0;
            int start = end;
            while (start >= 0)
            {
                char current = EditionTextBox.Text[start];

                if (parenthesis == 0)
                {
                    if (Char.IsLetterOrDigit(current) || current == '.')
                    {
                        // Continue on
                    }
                    else if (current == ')')
                    {
                        parenthesis += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (current == '(')
                {
                    parenthesis -= 1;
                }

                start = start - 1;
            }
            start = start + 1;

            if (start <= end)
            {
                retVal = EditionTextBox.Text.Substring(start, end - start + 1);
            }
            return retVal;
        }

        /// <summary>
        /// A reference to an object, displayed in the combo box
        /// </summary>
        private class ObjectReference : IComparable<ObjectReference>
        {
            /// <summary>
            /// The display name of the object reference
            /// </summary>
            public string DisplayName { get; private set; }

            /// <summary>
            /// The model elements referenced by this object reference
            /// </summary>
            public List<Utils.INamable> Models { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="models"></param>
            /// <param name="name"></param>
            public ObjectReference(string name, List<Utils.INamable> models)
            {
                DisplayName = name;
                Models = models;
            }

            public override string ToString()
            {
                return DisplayName;
            }

            // Summary:
            //     Compares the current object with another object of the same type.
            //
            // Parameters:
            //   other:
            //     An object to compare with this object.
            //
            // Returns:
            //     A value that indicates the relative order of the objects being compared.
            //     The return value has the following meanings: Value Meaning Less than zero
            //     This object is less than the other parameter.Zero This object is equal to
            //     other. Greater than zero This object is greater than other.
            public int CompareTo(ObjectReference other)
            {
                return DisplayName.CompareTo(other.DisplayName);
            }
        }

        /// <summary>
        /// Code templates
        /// </summary>
        private static string[] TEMPLATES = new string[] {
            ForAllExpression.OPERATOR + " <collection> | <condition> ", 
            ThereIsExpression.OPERATOR + " <collection> | <condition> ",
            FirstExpression.OPERATOR + " <collection> | <condition>", 
            LastExpression.OPERATOR + " <collection> | <condition>", 
            CountExpression.OPERATOR + " <collection> | <condition>", 
            MapExpression.OPERATOR + " <collection> | <condition> USING <map_expression>",
            SumExpression.OPERATOR + " <collection> | <condition> USING <map_expression>", 
            ReduceExpression.OPERATOR + " <collection> | <condition> USING <map_expression> INITIAL_VALUE <expression>" 
        };

        /// <summary>
        /// Provides the list of model elements which correspond to the prefix given
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private List<ObjectReference> AllChoices(string text, out string prefix)
        {
            List<ObjectReference> retVal = new List<ObjectReference>();

            // EFSSystem.INSTANCE.Compiler.Compile_Synchronous(false, true);
            SearchOptions searchOptions = BuildSearchOptions(text);
            prefix = searchOptions.Prefix;

            int value;
            bool isANumber = false;
            if (!string.IsNullOrEmpty(searchOptions.EnclosingName))
            {
                isANumber = int.TryParse(searchOptions.EnclosingName.Substring(0, searchOptions.EnclosingName.Length - 1), out value);
            }

            if (!isANumber)
            {
                // Handles references to model elements
                foreach (Utils.INamable namable in searchOptions.Instances)
                {
                    retVal.AddRange(GetPossibilities((Utils.IModelElement)namable, searchOptions));
                }

                // Handles code templates
                if (searchOptions.ConsiderTemplates)
                {
                    foreach (string template in TEMPLATES)
                    {
                        if (template.StartsWith(prefix))
                        {
                            retVal.Add(new ObjectReference(template, new List<Utils.INamable>()));
                        }
                    }
                }
                retVal.Sort();
            }

            return retVal;
        }


        /// <summary>
        /// The search options to be used
        /// </summary>
        private class SearchOptions
        {
            /// <summary>
            /// Indicates that templates should be taken into consideration
            /// </summary>
            public bool ConsiderTemplates { get; set; }

            /// <summary>
            /// Indicates that the enclosing elements should be taken into consideration
            /// </summary>
            public bool ConsiderEnclosing { get; set; }

            /// <summary>
            /// The name of the enclosing element
            /// </summary>
            public string EnclosingName { get; set; }

            /// <summary>
            /// The prefix of the element to be found
            /// </summary>
            public string Prefix { get; set; }

            /// <summary>
            /// The list of instances on which the search should occur
            /// </summary>
            public List<Utils.INamable> Instances { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public SearchOptions()
            {
                ConsiderTemplates = true;
                ConsiderEnclosing = true;
                EnclosingName = "";
                Prefix = "";
                Instances = new List<Utils.INamable>();
            }
        }

        /// <summary>
        /// Provides the search options according to the text provided 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private SearchOptions BuildSearchOptions(string text)
        {
            SearchOptions retVal = new SearchOptions();

            int lastDot = text.LastIndexOf('.');
            if (lastDot > 0)
            {
                retVal.EnclosingName = text.Substring(0, lastDot);
                retVal.ConsiderTemplates = string.IsNullOrEmpty(retVal.EnclosingName);
                if (text.StartsWith("X."))
                {
                    // Computes the location of the IN keyword
                    int start = EditionTextBox.SelectionStart;
                    bool found = false;
                    while (start > 1 && !found)
                    {
                        found = EditionTextBox.Text[start - 1] == 'I' && EditionTextBox.Text[start] == 'N';
                        start -= 1;
                    }

                    if (found)
                    {
                        start += 2;
                        while (start < EditionTextBox.SelectionStart && Char.IsSeparator(EditionTextBox.Text[start]))
                        {
                            start += 1;
                        }

                        // Computes the location of the end of the list identification
                        found = false;
                        int len = 0;
                        while (start + len < EditionTextBox.SelectionStart && !found)
                        {
                            found = Char.IsSeparator(EditionTextBox.Text[start + len]);
                            len += 1;
                        }


                        // Create a fake foreach expression to hold the list expression and the current expression
                        ModelElement modelElement = Instance as ModelElement;
                        if (modelElement != null)
                        {
                            Expression listExpression = EFSSystem.Parser.Expression(modelElement, EditionTextBox.Text.Substring(start, len), IsVariableOrValue.INSTANCE, false);
                            Expression currentExpression = EFSSystem.Parser.Expression(modelElement, retVal.EnclosingName, AllMatches.INSTANCE, false);
                            Expression foreachExpression = new ForAllExpression(modelElement, modelElement, listExpression, currentExpression, -1, -1);
                            foreachExpression.SemanticAnalysis();
                            if (currentExpression.Ref != null)
                            {
                                retVal.Instances.Add(currentExpression.Ref);
                            }
                        }
                    }
                }
                else if (retVal.EnclosingName.Contains('('))
                {
                    // Is this a function call ? 
                    int parentIndex = retVal.EnclosingName.IndexOf('(');
                    string functionName = retVal.EnclosingName.Substring(0, parentIndex);

                    Expression expression = EFSSystem.Parser.Expression(Instance as ModelElement, functionName, AllMatches.INSTANCE);
                    DataDictionary.Functions.Function function = expression.Ref as DataDictionary.Functions.Function;
                    if (function != null)
                    {
                        retVal.Instances.Add(function.ReturnType);
                        retVal.EnclosingName = "";
                        retVal.ConsiderTemplates = false;
                        retVal.ConsiderEnclosing = false;
                    }
                }
                else
                {
                    Expression expression = EFSSystem.Parser.Expression(Instance as ModelElement, retVal.EnclosingName, AllMatches.INSTANCE);

                    if (expression != null)
                    {
                        if (expression.Ref != null)
                        {
                            retVal.Instances.Add(expression.Ref);
                        }
                        else
                        {
                            foreach (ReturnValueElement element in expression.getReferences(null, AllMatches.INSTANCE, false).Values)
                            {
                                retVal.Instances.Add(element.Value);
                            }
                        }
                    }
                    else
                    {
                        retVal.Instances.Add(Instance);
                    }
                }

                retVal.Prefix = text.Substring(lastDot + 1);
            }
            else
            {
                retVal.Instances.Add(Instance);
                retVal.EnclosingName = null;
                retVal.Prefix = text;
                retVal.ConsiderTemplates = true;
            }

            return retVal;
        }

        /// <summary>
        /// Displays the combo box if required and updates the edotor's text
        /// </summary>
        private void DisplayComboBox()
        {
            string prefix = CurrentPrefix().Trim();
            List<ObjectReference> allChoices = AllChoices(prefix, out prefix);

            if (prefix.Length <= EditionTextBox.SelectionStart)
            {
                EditionTextBox.Select(EditionTextBox.SelectionStart - prefix.Length, prefix.Length);
                if (allChoices.Count == 1)
                {
                    EditionTextBox.SelectedText = allChoices[0].DisplayName;
                }
                else if (allChoices.Count > 1)
                {
                    SelectionComboBox.Items.Clear();
                    foreach (ObjectReference choice in allChoices)
                    {
                        SelectionComboBox.Items.Add(choice);
                    }
                    if (prefix != null && prefix.Length > 0)
                    {
                        SelectionComboBox.Text = prefix;
                    }
                    else
                    {
                        SelectionComboBox.Text = allChoices[0].DisplayName;
                    }

                    // Try to compute the combo box location
                    // TODO : Hypothesis. The first displayed line is the first line of the text
                    int line = 1;
                    string lineData = "";
                    for (int i = 0; i < EditionTextBox.SelectionStart; i++)
                    {
                        switch (EditionTextBox.Text[i])
                        {
                            case '\n':
                                line += 1;
                                lineData = "";
                                break;

                            default:
                                lineData += EditionTextBox.Text[i];
                                break;
                        }
                    }

                    SizeF size = GUIUtils.Graphics.MeasureString(lineData, EditionTextBox.Font);
                    int x = Math.Min((int)size.Width, Location.X + Size.Width - SelectionComboBox.Width);
                    int y = (line - 1) * EditionTextBox.Font.Height + 5;
                    Point comboBoxLocation = new Point(x, y);
                    SelectionComboBox.Location = comboBoxLocation;
                    PendingSelection = true;
                    EditionTextBox.SendToBack();
                    SelectionComboBox.Show();
                    SelectionComboBox.Focus();
                }
            }
        }

        void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (AutoComplete)
                {
                    if (e.Control == true)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Space:
                                // Remove the space that has just been added
                                EditionTextBox.Select(EditionTextBox.SelectionStart - 1, 1);
                                EditionTextBox.SelectedText = "";
                                DisplayComboBox();
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!e.Handled)
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            EditionTextBox.SelectAll();
                            e.Handled = true;
                            break;

                        case Keys.C:
                            EditionTextBox.Copy();
                            e.Handled = true;
                            break;
                    }
                }
            }
        }

        void Editor_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (AutoComplete)
                {
                    switch (e.KeyChar)
                    {
                        case '.':
                            EditionTextBox.SelectedText = e.KeyChar.ToString();
                            e.Handled = true;
                            DisplayComboBox();
                            break;

                        case '{':
                            Expression structureTypeExpression = EFSSystem.Parser.Expression(Instance as ModelElement, CurrentPrefix().Trim(), IsStructure.INSTANCE);
                            if (structureTypeExpression != null)
                            {
                                DataDictionary.Types.Structure structure = structureTypeExpression.Ref as DataDictionary.Types.Structure;
                                if (structure != null)
                                {
                                    StringBuilder builder = new StringBuilder("{\n");
                                    createDefaultStructureValue(builder, structure, false);
                                    EditionTextBox.SelectedText = builder.ToString();
                                    e.Handled = true;
                                }
                            }
                            break;

                        case '(':
                            Expression callableExpression = EFSSystem.Parser.Expression(Instance as ModelElement, CurrentPrefix().Trim(), IsCallable.INSTANCE);
                            if (callableExpression != null)
                            {
                                DataDictionary.Interpreter.ICallable callable = callableExpression.Ref as DataDictionary.Interpreter.ICallable;
                                if (callable != null)
                                {
                                    StringBuilder builder = new StringBuilder();
                                    createCallableParameters(builder, callable);
                                    EditionTextBox.SelectedText = builder.ToString();
                                    e.Handled = true;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ConfirmComboBoxSelection()
        {
            if (PendingSelection)
            {
                EditionTextBox.SelectedText = SelectionComboBox.Text;
                EditionTextBox.SelectionStart = EditionTextBox.SelectionStart;
                SelectionComboBox.Text = "";
                SelectionComboBox.Items.Clear();
                SelectionComboBox.Hide();
                explainRichTextBox.Hide();
                PendingSelection = false;
            }
        }

        private void AbordComboBoxSelection()
        {
            if (PendingSelection)
            {
                PendingSelection = false;
                SelectionComboBox.Text = "";
                SelectionComboBox.Items.Clear();
                SelectionComboBox.Hide();
                explainRichTextBox.Hide();
            }
        }

        void SelectionComboBox_LostFocus(object sender, EventArgs e)
        {
            ConfirmComboBoxSelection();
        }

        void SelectionComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    ConfirmComboBoxSelection();
                    break;

                case Keys.Escape:
                    AbordComboBoxSelection();
                    break;

                default:
                    break;
            }
        }

        void Editor_LostFocus(object sender, EventArgs e)
        {
            if (GUIUtils.MDIWindow != null)
            {
                GUIUtils.MDIWindow.SelectedRichTextBox = null;
            }
        }

        void Editor_GotFocus(object sender, EventArgs e)
        {
            if (GUIUtils.MDIWindow != null)
            {
                GUIUtils.MDIWindow.SelectedRichTextBox = this;
            }
        }

        /// <summary>
        /// Called when the drop operation is performed on this text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Editor_DragDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                BaseTreeNode SourceNode = (BaseTreeNode)e.Data.GetData("WindowsForms10PersistentObject");
                if (SourceNode != null)
                {
                    DataDictionaryView.VariableTreeNode variableNode = SourceNode as DataDictionaryView.VariableTreeNode;
                    if (variableNode != null)
                    {
                        StringBuilder text = new StringBuilder();
                        text.Append(StripUseless(SourceNode.Model.FullName, Instance) + " <- ");

                        DataDictionary.Variables.Variable variable = variableNode.Item;
                        DataDictionary.Types.Structure structure = variable.Type as DataDictionary.Types.Structure;
                        if (structure != null)
                        {
                            createDefaultStructureValue(text, structure);
                        }
                        else
                        {
                            text.Append(variable.DefaultValue.FullName);
                        }
                        EditionTextBox.SelectedText = text.ToString();
                    }
                    else
                    {
                        DataDictionaryView.StructureTreeNode structureTreeNode = SourceNode as DataDictionaryView.StructureTreeNode;
                        if (structureTreeNode != null)
                        {
                            StringBuilder text = new StringBuilder();

                            DataDictionary.Types.Structure structure = structureTreeNode.Item;
                            createDefaultStructureValue(text, structure);
                            EditionTextBox.SelectedText = text.ToString();
                        }
                        else
                        {
                            EditionTextBox.SelectedText = StripUseless(SourceNode.Model.FullName, Instance);
                        }
                    }
                }
            }
        }

        private void createDefaultStructureValue(StringBuilder text, DataDictionary.Types.Structure structure, bool displayStructureName = true)
        {
            if (displayStructureName)
            {
                text.Append(StripUseless(structure.FullName, Instance) + "{\n");
            }

            bool first = true;
            foreach (DataDictionary.Types.StructureElement element in structure.Elements)
            {
                if (!first)
                {
                    text.Append(",\n");
                }
                insertElement(element, text, 4);
                first = false;
            }
            text.Append("\n}");
        }

        private void createCallableParameters(StringBuilder text, DataDictionary.Interpreter.ICallable callable)
        {
            if (callable.FormalParameters.Count > 0)
            {
                text.Append("(\n");
                bool first = true;
                foreach (DataDictionary.Parameter parameter in callable.FormalParameters)
                {
                    if (!first)
                    {
                        text.Append(",\n");
                    }
                    text.Append(TextualExplainUtilities.Pad(parameter.Name + "=>" + parameter.Type.Default, 4));
                    first = false;
                }
                text.Append("\n)");
            }
            else
            {
                text.Append("()");
            }
        }

        private void insertElement(DataDictionary.Types.ITypedElement element, StringBuilder text, int indent)
        {
            text.Append(TextualExplainUtilities.Pad(element.Name + " => ", indent));
            DataDictionary.Types.Structure structure = element.Type as DataDictionary.Types.Structure;
            if (structure != null)
            {
                indent = indent + 4;
                text.Append(StripUseless(structure.FullName, Instance) + "{\n");
                bool first = true;
                foreach (DataDictionary.Types.StructureElement subElement in structure.Elements)
                {
                    if (!first)
                    {
                        text.Append(",\n");
                    }
                    insertElement(subElement, text, indent);
                    first = false;
                }
                indent -= 4;
                text.Append("\n" + TextualExplainUtilities.Pad("}", indent));
            }
            else
            {
                DataDictionary.Values.IValue value = null;
                if (element.Default == null || element.Default.Length == 0)
                {
                    if (element.Type != null && element.Type.DefaultValue != null)
                    {
                        value = element.Type.DefaultValue;
                    }
                }
                else
                {
                    if (element.Type != null)
                    {
                        value = element.Type.getValue(element.Default);
                    }
                }

                if (value != null)
                {
                    text.Append(value.FullName);
                }
            }
        }

        /// <summary>
        /// The prefix for the Default namespace
        /// </summary>
        private static string DEFAULT_PREFIX = "Default.";

        /// <summary>
        /// Removes useless prefixes from the string provided
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string StripUseless(string fullName, Utils.IModelElement model)
        {
            string retVal = fullName;

            if (model != null)
            {
                char[] tmp = fullName.ToArray();
                char[] modelName = model.FullName.ToArray();

                int i = 0;
                while (i < tmp.Length && i < modelName.Length)
                {
                    if (tmp[i] != modelName[i])
                    {
                        break;
                    }
                    i += 1;
                }

                retVal = retVal.Substring(i);
                if (Utils.Utils.isEmpty(retVal))
                {
                    retVal = model.Name;
                }
            }

            if (retVal.StartsWith(DEFAULT_PREFIX))
            {
                retVal = retVal.Substring(DEFAULT_PREFIX.Length);
            }

            return retVal;
        }

        private string lastRtf = "";
        public string Rtf
        {
            get
            {
                return EditionTextBox.Rtf;
            }
            set
            {
                if (value != lastRtf)
                {
                    lastRtf = value;
                    EditionTextBox.Rtf = InitialRTF;
                    EditionTextBox.Rtf = TextualExplainUtilities.Encapsule(value);
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return EditionTextBox.ReadOnly;
            }
            set
            {
                EditionTextBox.ReadOnly = value;
            }
        }

        private string LastText = "";
        public override string Text
        {
            get
            {
                return EditionTextBox.Text;
            }
            set
            {
                if (value != EditionTextBox.Text)
                {
                    LastText = value;
                    EditionTextBox.Rtf = InitialRTF;
                    EditionTextBox.Text = value.Trim();
                }
            }
        }


        /// <summary>
        /// Edits a structure value expression and provides the edited expression after user has performed his changes
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private StructExpression EditStructureExpression(StructExpression expression)
        {
            StructExpression retVal = expression;

            if (expression != null)
            {
                bool silentMode = ModelElement.BeSilent;
                try
                {
                    ModelElement.BeSilent = true;

                    InterpretationContext context = new InterpretationContext(Instance);
                    context.UseDefaultValue = false;
                    StructureValue value = expression.GetValue(context) as StructureValue;
                    if (value != null)
                    {
                        StructureValueEditor.Window window = new StructureValueEditor.Window();
                        window.SetModel(value);
                        window.ShowDialog();

                        string newExpression = value.ToExpressionWithDefault();
                        bool doSemanticalAnalysis = true;
                        bool silent = true;
                        retVal = EFSSystem.INSTANCE.Parser.Expression((ModelElement)Instance, newExpression, AllMatches.INSTANCE, doSemanticalAnalysis, null, silent) as StructExpression;
                    }
                }
                finally
                {
                    ModelElement.BeSilent = silentMode;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Browses through the expression to find the structure value to edit
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression VisitExpression(Expression expression)
        {
            Expression retVal = expression;

            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                binaryExpression.Left = VisitExpression(binaryExpression.Left);
                binaryExpression.Right = VisitExpression(binaryExpression.Right);
            }

            UnaryExpression unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                if (unaryExpression.Expression != null)
                {
                    unaryExpression.Expression = VisitExpression(unaryExpression.Expression);
                }
            }

            StructExpression structExpression = expression as StructExpression;
            if (structExpression != null)
            {
                retVal = EditStructureExpression(structExpression);
            }

            return retVal;
        }

        /// <summary>
        /// Browses through the statement to find the structures to edit
        /// </summary>
        /// <param name="statement"></param>
        private Statement VisitStatement(Statement statement)
        {
            Statement retVal = statement;

            VariableUpdateStatement variableUpdateStatement = statement as VariableUpdateStatement;
            if (variableUpdateStatement != null)
            {
                variableUpdateStatement.Expression = VisitExpression(variableUpdateStatement.Expression);
            }

            return retVal;
        }

        private void openStructureEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool doSemanticalAnalysis = true;
            bool silent = true;
            Expression expression = EFSSystem.INSTANCE.Parser.Expression(Instance as ModelElement, EditionTextBox.Text, AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
            if (expression != null)
            {
                expression = VisitExpression(expression);
                EditionTextBox.Text = expression.ToString();
            }

            Statement statement = EFSSystem.INSTANCE.Parser.Statement(Instance as ModelElement, EditionTextBox.Text, silent);
            if (statement != null)
            {
                statement = VisitStatement(statement);
                EditionTextBox.Text = statement.ToString();
            }
        }
    }
}
