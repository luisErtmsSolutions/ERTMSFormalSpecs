﻿// ------------------------------------------------------------------------------
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
using BrightIdeasSoftware;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Tests;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using GUI.DataDictionaryView;
using Utils;
using Action = DataDictionary.Rules.Action;
using ModelElement = DataDictionary.ModelElement;
using Type = DataDictionary.Types.Type;
using Window = GUI.StructureValueEditor.Window;

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

        private object __instance = null;

        /// <summary>
        /// Provides the instance on which this editor is based
        /// </summary>
        public object Instance
        {
            get
            {
                if (__instance == null && EnclosingForm != null)
                {
                    __instance = (IModelElement) EnclosingForm.Selected;
                }
                return __instance;
            }
            set { __instance = value; }
        }

        /// <summary>
        /// The instance viewed as a model element
        /// </summary>
        public IModelElement Model
        {
            get { return Instance as IModelElement; }
        }


        /// <summary>
        /// Provides the EFSSystem 
        /// </summary>
        private EFSSystem EFSSystem
        {
            get
            {
                EFSSystem retVal = null;

                ModelElement modelElement = Instance as ModelElement;
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
            List<INamable> instances = GetInstances(location);

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
        private List<INamable> GetInstances(Point location)
        {
            List<INamable> retVal = new List<INamable>();

            if (Model != null)
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
                            ReturnValue returnValue = expression.getReferences(Model, AllMatches.INSTANCE, last);
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
        private void EditionTextBox_MouseMove(object sender, MouseEventArgs e)
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

        private void EditionTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MouseLocation = e.Location;
                DisplayHelp(e.Location);
            }

            if (ModifierKeys == Keys.Control)
            {
                List<INamable> instances = GetInstances(e.Location);
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

        private void SelectionComboBox_LocationChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
        }

        private void SelectionComboBox_DropDownStyleChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
        }

        private void SelectionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
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
        private void ExplainAndShow(List<INamable> namables, Point location, bool sensibleToMouseMove)
        {
            explainRichTextBox.Rtf = CleanText;
            if (namables != null)
            {
                string data = "";
                foreach (INamable namable in namables)
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
        public RichTextBox TextBox
        {
            get { return EditionTextBox; }
        }

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
        private HashSet<ObjectReference> GetPossibilities(IModelElement element, SearchOptions searchOptions)
        {
            HashSet<ObjectReference> retVal = new HashSet<ObjectReference>();

            while (element != null)
            {
                bool type = false;

                type = ConsiderElement(element, searchOptions, retVal, type);

                if (searchOptions.ConsiderEnclosing)
                {
                    element = element.Enclosing as IModelElement;
                }
                else
                {
                    element = null;
                }
            }

            // Considers all dictionaries in the system to find the expected element
            if (searchOptions.ConsiderEnclosing)
            {
                foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    ConsiderElement(dictionary, searchOptions, retVal, false);
                }
            }

            return retVal;
        }

        private bool ConsiderElement(IModelElement element, SearchOptions searchOptions, HashSet<ObjectReference> retVal, bool type)
        {
            ISubDeclarator subDeclarator = element as ISubDeclarator;
            if (subDeclarator == null)
            {
                ITypedElement typedElement = element as ITypedElement;
                if (typedElement != null)
                {
                    type = true;
                    subDeclarator = typedElement.Type as ISubDeclarator;
                }
            }

            IVariable variable = subDeclarator as IVariable;
            if (variable != null)
            {
                type = true;
                subDeclarator = variable.Type as ISubDeclarator;
            }

            if (subDeclarator != null)
            {
                subDeclarator.InitDeclaredElements();
                foreach (KeyValuePair<string, List<INamable>> pair in subDeclarator.DeclaredElements)
                {
                    string subElem = pair.Key;
                    if (subElem.StartsWith(searchOptions.Prefix))
                    {
                        if (searchOptions.EnclosingName != null)
                        {
                            foreach (INamable namable in subDeclarator.DeclaredElements[subElem])
                            {
                                if (namable.FullName.EndsWith(searchOptions.EnclosingName + "." + subElem) || type || subDeclarator is StructureElement)
                                {
                                    if (ConsiderOnlyTypes)
                                    {
                                        if (namable is Type || namable is NameSpace)
                                        {
                                            if (!(namable is Function))
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

            return type;
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
                    if (Char.IsLetterOrDigit(current) || current == '.' || current == '_' || current == '%')
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
            public List<INamable> Models { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="models"></param>
            /// <param name="name"></param>
            public ObjectReference(string name, List<INamable> models)
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
        private static string[] TEMPLATES = new string[]
        {
            ForAllExpression.OPERATOR + " X IN <collection> | <condition> ",
            ThereIsExpression.OPERATOR + " X IN <collection> | <condition> ",
            FirstExpression.OPERATOR + " X IN <collection> | <condition>",
            LastExpression.OPERATOR + " X IN <collection> | <condition>",
            CountExpression.OPERATOR + " X IN <collection> | <condition>",
            MapExpression.OPERATOR + " <collection> | <condition> USING X IN <map_expression>",
            SumExpression.OPERATOR + " <collection> | <condition> USING X IN <map_expression>",
            ReduceExpression.OPERATOR + " <collection> | <condition> USING X IN <map_expression> INITIAL_VALUE <expression>",
            "LET <variable> <- <expression> IN <expression>",
            "STABILIZE <expression> INITIAL_VALUE <expression> STOP_CONDITION <condition>",
            "APPLY <statement> ON <collection> | <condition>",
            "INSERT <expression> IN <collection> WHEN FULL REPLACE <condition>",
            "REMOVE [FIRST|LAST|ALL] <condition> IN <collection>",
            "REPLACE <condition> IN <collection> BY <expression>",
            "%D_LRBG",
            "%Level",
            "%Mode",
            "%NID_LRBG",
            "%Q_DIRLRBG",
            "%Q_DIRTRAIN",
            "%Q_DLRBG",
            "%RBC_ID",
            "%RBCPhone",
            "%Step_Distance",
            "%Step_LevelIN",
            "%Step_LevelOUT",
            "%Step_ModeIN",
            "%Step_ModeOUT",
            "%Step_Messages_0",
            "%Step_Messages_1",
            "%Step_Messages_2",
            "%Step_Messages_3",
            "%Step_Messages_4",
            "%Step_Messages_5",
            "%Step_Messages_6",
            "%Step_Messages_7",
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
                foreach (INamable namable in searchOptions.Instances)
                {
                    retVal.AddRange(GetPossibilities((IModelElement) namable, searchOptions));
                }

                // Handles code templates
                if (searchOptions.ConsiderTemplates)
                {
                    foreach (string template in TEMPLATES)
                    {
                        if (template.StartsWith(prefix))
                        {
                            retVal.Add(new ObjectReference(template, new List<INamable>()));
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
            public List<INamable> Instances { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public SearchOptions()
            {
                ConsiderTemplates = true;
                ConsiderEnclosing = true;
                EnclosingName = "";
                Prefix = "";
                Instances = new List<INamable>();
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
                int previousDot = text.Substring(0, lastDot).LastIndexOf('.');

                // Default values for search options
                retVal.EnclosingName = text.Substring(0, lastDot);
                retVal.ConsiderTemplates = string.IsNullOrEmpty(retVal.EnclosingName);

                // Specific cases
                if (text.StartsWith("THIS."))
                {
                    retVal.EnclosingName = "";
                    retVal.ConsiderTemplates = false;
                    retVal.ConsiderEnclosing = false;
                    ModelElement modelElement = Instance as ModelElement;
                    while (modelElement != null)
                    {
                        Structure structure = modelElement as Structure;
                        if (structure != null)
                        {
                            retVal.Instances.Add(structure);
                            modelElement = null;
                        }

                        if (modelElement != null)
                        {
                            modelElement = modelElement.Enclosing as ModelElement;
                        }
                    }
                }
                else if (text.StartsWith("X."))
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
                            Expression listExpression = EFSSystem.Parser.Expression(modelElement, EditionTextBox.Text.Substring(start, len), IsVariableOrValue.INSTANCE, false, null, true);
                            Expression currentExpression = EFSSystem.Parser.Expression(modelElement, retVal.EnclosingName, AllMatches.INSTANCE, false, null, true);
                            Expression foreachExpression = new ForAllExpression(modelElement, modelElement, listExpression, "X", currentExpression, -1, -1);
                            foreachExpression.SemanticAnalysis();
                            if (currentExpression.Ref != null)
                            {
                                retVal.Instances.Add(currentExpression.Ref);
                            }
                        }
                    }
                }
                else if (retVal.EnclosingName.LastIndexOf('(') > previousDot)
                {
                    retVal.ConsiderTemplates = false;
                    retVal.ConsiderEnclosing = false;

                    // Is this a function call ? 
                    int parentIndex = retVal.EnclosingName.LastIndexOf('(');
                    string functionName = retVal.EnclosingName.Substring(0, parentIndex);

                    Expression expression = EFSSystem.Parser.Expression(Instance as ModelElement, functionName, AllMatches.INSTANCE, true, null, true);
                    Function function = expression.Ref as Function;
                    if (function != null)
                    {
                        retVal.Instances.Add(function.ReturnType);
                        retVal.EnclosingName = "";
                    }
                }
                else
                {
                    Expression expression = EFSSystem.Parser.Expression(Instance as ModelElement, retVal.EnclosingName, AllMatches.INSTANCE, true, null, true);

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
                        retVal.Instances.Add(Model);
                    }
                }

                retVal.Prefix = text.Substring(lastDot + 1);
            }
            else
            {
                retVal.Instances.Add(Model);
                retVal.EnclosingName = null;
                retVal.Prefix = text;
                retVal.ConsiderTemplates = true;
            }

            return retVal;
        }

        private int selectionStart = 0;
        private int selectionLength = 0;

        /// <summary>
        /// Displays the combo box if required and updates the edotor's text
        /// </summary>
        private void DisplayComboBox()
        {
            string prefix = CurrentPrefix().Trim();
            List<ObjectReference> allChoices = AllChoices(prefix, out prefix);

            if (prefix.Length <= EditionTextBox.SelectionStart)
            {
                // It seems that selection start and length may be lost when losing the focus. 
                // Store them to be able to reapply them 
                selectionStart = EditionTextBox.SelectionStart - prefix.Length;
                selectionLength = prefix.Length;
                EditionTextBox.Select(selectionStart, selectionLength);
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
                    int x = Math.Min((int) size.Width, Location.X + Size.Width - SelectionComboBox.Width);
                    int y = (line - 1)*EditionTextBox.Font.Height + 5;
                    Point comboBoxLocation = new Point(x, y);
                    SelectionComboBox.Location = comboBoxLocation;
                    PendingSelection = true;
                    EditionTextBox.SendToBack();
                    SelectionComboBox.Show();
                    SelectionComboBox.Focus();
                }
            }
        }

        private void Editor_KeyUp(object sender, KeyEventArgs e)
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

        private void Editor_KeyPress(object sender, KeyPressEventArgs e)
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
                            Expression structureTypeExpression = EFSSystem.Parser.Expression(Instance as ModelElement, CurrentPrefix().Trim(), IsStructure.INSTANCE, true, null, true);
                            if (structureTypeExpression != null)
                            {
                                Structure structure = structureTypeExpression.Ref as Structure;
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
                            Expression callableExpression = EFSSystem.Parser.Expression(Instance as ModelElement, CurrentPrefix().Trim(), IsCallable.INSTANCE, true, null, true);
                            if (callableExpression != null)
                            {
                                ICallable callable = callableExpression.Ref as ICallable;
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
                PendingSelection = false;
                EditionTextBox.Select(selectionStart, selectionLength);

                EditionTextBox.SelectedText = SelectionComboBox.Text;
                EditionTextBox.SelectionStart = EditionTextBox.SelectionStart;
                SelectionComboBox.Text = "";
                SelectionComboBox.Items.Clear();
                SelectionComboBox.Hide();
                explainRichTextBox.Hide();
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

        private void SelectionComboBox_LostFocus(object sender, EventArgs e)
        {
            ConfirmComboBoxSelection();
        }

        private void SelectionComboBox_KeyUp(object sender, KeyEventArgs e)
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

        private void Editor_LostFocus(object sender, EventArgs e)
        {
            if (GUIUtils.MDIWindow != null)
            {
                GUIUtils.MDIWindow.SelectedRichTextBox = null;
            }
        }

        private void Editor_GotFocus(object sender, EventArgs e)
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
        private void Editor_DragDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null)
                {
                    VariableTreeNode variableNode = sourceNode as VariableTreeNode;
                    if (variableNode != null)
                    {
                        EditionTextBox.SelectedText = setVariable(variableNode.Item);
                    }
                    else
                    {
                        StructureTreeNode structureTreeNode = sourceNode as StructureTreeNode;
                        if (structureTreeNode != null)
                        {
                            StringBuilder text = new StringBuilder();

                            Structure structure = structureTreeNode.Item;
                            createDefaultStructureValue(text, structure);
                            EditionTextBox.SelectedText = text.ToString();
                        }
                        else
                        {
                            EditionTextBox.SelectedText = StripUseless(sourceNode.Model.FullName, writingContext());
                        }
                    }
                }

                OLVListItem item = data as OLVListItem;
                if (item != null)
                {
                    Variable variable = item.RowObject as Variable;
                    if (variable != null)
                    {
                        EditionTextBox.SelectedText = setVariable(variable);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the variable in the editor
        /// </summary>
        /// <param name="variable"></param>
        private string setVariable(Variable variable)
        {
            StringBuilder text = new StringBuilder();

            text.Append(StripUseless(variable.FullName, writingContext()) + " <- ");
            Structure structure = variable.Type as Structure;
            if (structure != null)
            {
                createDefaultStructureValue(text, structure);
            }
            else
            {
                text.Append(variable.DefaultValue.FullName);
            }

            return text.ToString();
        }

        private void createDefaultStructureValue(StringBuilder text, Structure structure, bool displayStructureName = true)
        {
            if (displayStructureName)
            {
                text.Append(StripUseless(structure.FullName, writingContext()) + "{\n");
            }

            bool first = true;
            foreach (StructureElement element in structure.Elements)
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

        private void createCallableParameters(StringBuilder text, ICallable callable)
        {
            if (callable.FormalParameters.Count > 0)
            {
                text.Append("(\n");
                bool first = true;
                foreach (Parameter parameter in callable.FormalParameters)
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

        private void insertElement(ITypedElement element, StringBuilder text, int indent)
        {
            text.Append(TextualExplainUtilities.Pad(element.Name + " => ", indent));
            Structure structure = element.Type as Structure;
            if (structure != null)
            {
                indent = indent + 4;
                text.Append(StripUseless(structure.FullName, writingContext()) + "{\n");
                bool first = true;
                foreach (StructureElement subElement in structure.Elements)
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
                IValue value = null;
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
        /// Provides the writing context of this edition
        /// </summary>
        /// <returns></returns>
        private IModelElement writingContext()
        {
            IModelElement retVal = Model;

            if (retVal is Action || retVal is Expectation)
            {
                retVal = retVal.Enclosing as IModelElement;
            }

            return retVal;
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
        private string StripUseless(string fullName, IModelElement model)
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
            get { return EditionTextBox.Rtf; }
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
            get { return EditionTextBox.ReadOnly; }
            set { EditionTextBox.ReadOnly = value; }
        }

        private string LastText = "";

        public override string Text
        {
            get { return EditionTextBox.Text; }
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
        /// Edits a value expression and provides the edited expression after user has performed his changes
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression EditExpression(Expression expression)
        {
            Expression retVal = expression;

            if (expression != null)
            {
                bool silentMode = ModelElement.BeSilent;
                try
                {
                    ModelElement.BeSilent = true;

                    InterpretationContext context = new InterpretationContext(Model);
                    context.UseDefaultValue = false;
                    IValue value = expression.GetValue(context, null);
                    if (value != null)
                    {
                        Window window = new Window();
                        window.SetModel(value);
                        window.ShowDialog();

                        string newExpression = value.ToExpressionWithDefault();
                        const bool doSemanticalAnalysis = true;
                        const bool silent = true;
                        retVal = EFSSystem.INSTANCE.Parser.Expression(expression.Root, newExpression, AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
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
        /// Browses through the expression to find the value to edit
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
                else if (unaryExpression.Term != null)
                {
                    visitTerm(unaryExpression.Term);
                }
            }

            StructExpression structExpression = expression as StructExpression;
            if (structExpression != null)
            {
                retVal = EditExpression(structExpression);
            }

            Call call = expression as Call;
            if (call != null)
            {
                foreach (Expression subExpression in call.AllParameters)
                {
                    VisitExpression(subExpression);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Browse through the Term to find the value to edit
        /// </summary>
        /// <param name="term"></param>
        private void visitTerm(Term term)
        {
            if (term.LiteralValue != null)
            {
                term.LiteralValue = EditExpression(term.LiteralValue);
            }
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
            bool dialogShown = false;
            ExplanationPart part = Instance as ExplanationPart;
            if (part != null)
            {
                IValue value = part.Namable as IValue;
                if (value != null)
                {
                    Window window = new Window();
                    window.SetModel(value);
                    window.ShowDialog();
                    dialogShown = true;
                }

                Action action = part.Element as Action;
                if (!dialogShown && action != null)
                {
                    VisitStatement(action.Statement);
                    dialogShown = true;
                }

                Expectation expectation = part.Element as Expectation;
                if (!dialogShown && expectation != null)
                {
                    VisitExpression(expectation.Expression);
                    dialogShown = true;
                }
            }

            if (!dialogShown)
            {
                const bool doSemanticalAnalysis = true;
                const bool silent = true;
                ModelElement root = Instance as ModelElement;
                if (root == null)
                {
                    root = EFSSystem.INSTANCE.Dictionaries[0];
                }

                string text = EditionTextBox.Text;
                Expression expression = EFSSystem.INSTANCE.Parser.Expression(root, text, AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
                if (expression != null)
                {
                    expression = VisitExpression(expression);
                    EditionTextBox.Text = expression.ToString();
                }

                Statement statement = EFSSystem.INSTANCE.Parser.Statement(root, text, silent);
                if (statement != null)
                {
                    statement = VisitStatement(statement);
                    EditionTextBox.Text = statement.ToString();
                }
            }
        }
    }
}