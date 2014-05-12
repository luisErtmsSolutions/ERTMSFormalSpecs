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
namespace GUI.StructureValueEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using DataDictionary;
    using DataDictionary.Values;
    using DataDictionary.Variables;
    using DataDictionary.Types;
    using BrightIdeasSoftware;
    using System.Windows.Forms;
    using System.Drawing;

    /// <summary>
    /// Customize the tree view according to the model element to display
    /// </summary>
    public static class CustomizeTreeView
    {
        /// <summary>
        /// Dereferences the variables
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object DerefVariable(object obj)
        {
            object retVal = obj;

            Variable variable = retVal as Variable;
            if (variable != null)
            {
                retVal = variable.Value;
            }

            return retVal;
        }

        #region Stringonizer
        /// <summary>
        /// Provides the field column string value for the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FieldColumnStringonizer(object obj)
        {
            string retVal = "";

            StructureValue structureValue = obj as StructureValue;
            if (structureValue != null)
            {
                ListValue enclosingListValue = structureValue.Enclosing as ListValue;
                if (enclosingListValue != null)
                {
                    int index = enclosingListValue.Val.IndexOf(structureValue) + 1;
                    Variable enclosingListVariable = (Variable)enclosingListValue.Enclosing;
                    retVal = enclosingListVariable.Name + "[" + index + "]";
                }
                else
                {
                    retVal = structureValue.Type.Name;
                }
            }

            Variable variable = obj as Variable;
            if (variable != null)
            {
                retVal = variable.Name;
            }

            return retVal;
        }

        /// <summary>
        /// The default indicator
        /// </summary>
        private const string DEFAULT = "<default>";

        /// <summary>
        /// Provides the value column string value for the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ValueColumnStringonizer(object obj)
        {
            string retVal = "";

            IVariable variable = obj as IVariable;
            obj = DerefVariable(obj);

            IValue value = obj as IValue;
            StructureValue structureValue = obj as StructureValue;
            ListValue listValue = obj as ListValue;
            if (structureValue != null)
            {
                retVal = "";
            }
            else if (listValue != null)
            {
                retVal = "";
            }
            else
            {
                if (IsDefaultValue(variable, value))
                {
                    retVal = DEFAULT;
                }
                else
                {
                    retVal = value.Name;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether the value provided is the default value for that variable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsDefaultValue(IVariable variable, IValue value)
        {
            bool retVal = value is DefaultValue;

            return retVal;
        }

        /// <summary>
        /// Provides the string value for the description columb according to the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DescriptionColumnStringonizer(object obj)
        {
            string retVal = "";

            Variable variable = obj as Variable;
            if (variable != null)
            {
                if (string.IsNullOrEmpty(variable.Comment))
                {
                    retVal = variable.Type.Comment;
                }
                else
                {
                    retVal = variable.Comment;
                }
            }
            else
            {
                Value value = obj as Value;
                if (value != null)
                {
                    retVal = value.Type.Comment;
                }
            }

            return retVal;
        }

        public static void FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column.Index == 1)
            {
                Value value = DerefVariable(e.Model) as Value;
                if (value != null)
                {
                    if (value is DefaultValue)
                    {
                        e.SubItem.ForeColor = Color.Blue;
                    }
                }
            }
        }
        #endregion

        #region Tree Structure
        public static bool HasChildren(object obj)
        {
            bool retVal = false;

            obj = DerefVariable(obj);

            StructureValue structureValue = obj as StructureValue;
            if (structureValue != null)
            {
                foreach (Variable subVariable in structureValue.Val.Values)
                {
                    if (subVariable.Type is Structure)
                    {
                        if (!(subVariable.Value is DefaultValue))
                        {
                            if (subVariable.Value != EFSSystem.INSTANCE.EmptyValue)
                            {
                                retVal = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            ListValue listValue = obj as ListValue;
            if (listValue != null)
            {
                retVal = listValue.ElementCount > 0;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the children of a specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable GetChildren(object obj)
        {
            IEnumerable retVal = null;

            obj = DerefVariable(obj);

            StructureValue structureValue = obj as StructureValue;
            if (structureValue != null)
            {
                ArrayList list = new ArrayList();
                foreach (Variable subVariable in structureValue.Val.Values)
                {
                    if (subVariable.Value is DefaultValue)
                    {
                        if (subVariable.Type is Structure)
                        {
                            // Don't add it, it shall be handled by the contextual menus
                        }
                        else
                        {
                            list.Add(subVariable);
                        }
                    }
                    else
                    {
                        if (subVariable.Value != EFSSystem.INSTANCE.EmptyValue)
                        {
                            list.Add(subVariable);
                        }
                    }
                }
                retVal = list;
            }

            ListValue listValue = obj as ListValue;
            if (listValue != null)
            {
                ArrayList list = new ArrayList();
                foreach (Value element in listValue.Val)
                {
                    if (element != EFSSystem.INSTANCE.EmptyValue)
                    {
                        list.Add(element);
                    }
                }
                retVal = list;
            }

            return retVal;
        }
        #endregion

        #region Contextual menu
        /// <summary>
        /// Creates a variable according to the structure element provided
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Variable CreateVariable(StructureElement element)
        {
            Structure elementStructureType = (Structure)element.Type;
            StructureValue subValue = new StructureValue(elementStructureType, false);

            Variable retVal = (Variable)DataDictionary.Generated.acceptor.getFactory().createVariable();
            retVal.Name = element.Name;
            retVal.Type = element.Type;
            retVal.Value = subValue;
            return retVal;
        }

        /// <summary>
        /// The base toolstrip button. Handles synchronization with the object list view
        /// </summary>
        private class BaseToolStripButton : ToolStripButton
        {
            /// <summary>
            /// The arguments used to launch the tool strip
            /// </summary>
            protected CellRightClickEventArgs Args { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="text"></param>
            protected BaseToolStripButton(CellRightClickEventArgs args, string text)
                : base(text)
            {
                Args = args;
                if (Text.Length < 10)
                {
                    Width = Text.Length * 8;
                }
            }

            /// <summary>
            /// Synchronises the list view
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                try
                {
                    Args.ListView.RefreshObject(Args.Model);
                }
                catch (Exception)
                {
                }

                base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds a value in a list
        /// </summary>
        private class ToolStripAddValueInList : BaseToolStripButton
        {
            /// <summary>
            /// The variable that holds the list value
            /// </summary>
            private Variable Variable { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="variable"></param>
            public ToolStripAddValueInList(CellRightClickEventArgs args, Variable variable)
                : base(args, "Add entry")
            {
                Variable = variable;
            }

            /// <summary>
            /// Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Collection collectionType = (Collection)Variable.Type;
                Structure structureType = (Structure)collectionType.Type;
                StructureValue element = new StructureValue(structureType, false);

                if (structureType.Elements.Count == 1)
                {
                    StructureElement subElement = (StructureElement)structureType.Elements[0];
                    Structure subElementStructureType = subElement.Type as Structure;
                    if (subElementStructureType != null)
                    {
                        Variable subVariable = CreateVariable(subElement);
                        element.set(subVariable);
                    }
                }

                Variable.Value = Variable.Value.RightSide(Variable, false, true) as ListValue;
                ListValue value = Variable.Value as ListValue;
                if (value != null)
                {
                    for (int i = 0; i < value.Val.Count; i++)
                    {
                        if (value.Val[i] == EFSSystem.INSTANCE.EmptyValue)
                        {
                            value.Val[i] = element;
                            element.Enclosing = Variable.Value;
                            break;
                        }
                    }
                }

                base.OnClick(e);
            }
        }

        /// <summary>
        /// Removes a value from a list
        /// </summary>
        private class ToolStripRemoveListEntry : BaseToolStripButton
        {
            /// <summary>
            /// The value that should be updated
            /// </summary>
            private ListValue Value { get; set; }

            /// <summary>
            /// The entry to remove from the list value
            /// </summary>
            private Value Entry { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="value"></param>
            /// <param name="element"></param>
            /// <param name="displayName"></param>
            public ToolStripRemoveListEntry(CellRightClickEventArgs args, ListValue value, Value entry)
                : base(args, "Remove")
            {
                Value = value;
                Entry = entry;
            }

            /// <summary>
            /// Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                for (int i = 0; i < Value.Val.Count; i++)
                {
                    if (Value.Val[i] == Entry)
                    {
                        Value.Val[i] = EFSSystem.INSTANCE.EmptyValue;
                    }
                }
                Args.ListView.RefreshObject(Value.Enclosing);
                // base.OnClick(e);
            }
        }

        /// <summary>
        /// Adds an element in a structure value
        /// </summary>
        private class ToolStripAddStructureMember : BaseToolStripButton
        {
            /// <summary>
            /// The value that should be updated
            /// </summary>
            private StructureValue Value { get; set; }

            /// <summary>
            /// The element on which the action should be performed
            /// </summary>
            private StructureElement Element { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="value"></param>
            /// <param name="element"></param>
            public ToolStripAddStructureMember(CellRightClickEventArgs args, StructureValue value, StructureElement element)
                : base(args, "Add " + element.Name)
            {
                Value = value;
                Element = element;
            }

            /// <summary>
            /// Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Structure elementStructureType = (Structure)Element.Type;
                StructureValue subValue = new StructureValue(elementStructureType, false);
                Variable subVariable = (Variable)DataDictionary.Generated.acceptor.getFactory().createVariable();
                subVariable.Name = Element.Name;
                subVariable.Type = Element.Type;
                subVariable.Value = subValue;
                Value.set(subVariable);

                base.OnClick(e);
            }
        }

        /// <summary>
        /// Removes an element from a structure value
        /// </summary>
        private class ToolStripRemoveStructureMember : BaseToolStripButton
        {
            /// <summary>
            /// The variable to remove
            /// </summary>
            private Variable Variable { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="variable"></param>
            public ToolStripRemoveStructureMember(CellRightClickEventArgs args, Variable variable)
                : base(args, "Remove")
            {
                Variable = variable;
            }

            /// <summary>
            /// Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Variable.Value = EFSSystem.INSTANCE.EmptyValue;

                Args.ListView.RefreshObject(Variable.Enclosing);
            }
        }

        public static void CreateContextualMenu(object obj, CellRightClickEventArgs args)
        {
            ContextMenuStrip menuStrip = new ContextMenuStrip();
            List<BaseToolStripButton> items = new List<BaseToolStripButton>();

            Variable enclosingVariable = args.Model as Variable;
            obj = DerefVariable(args.Model);

            StructureValue structureValue = obj as StructureValue;
            if (structureValue != null)
            {
                Structure structureType = (Structure)structureValue.Type;
                foreach (StructureElement element in structureType.Elements)
                {
                    if (element.Type is Structure)
                    {
                        Variable subVariable = null;

                        Utils.INamable tmp = null;
                        if (structureValue.Val.TryGetValue(element.Name, out tmp))
                        {
                            subVariable = tmp as Variable;
                        }

                        if (subVariable == null || subVariable.Value == EFSSystem.INSTANCE.EmptyValue || subVariable.Value is DefaultValue)
                        {
                            items.Add(new ToolStripAddStructureMember(args, structureValue, element));
                        }
                    }
                }

                if (enclosingVariable != null)
                {
                    StructureValue enclosingStructureValue = enclosingVariable.Enclosing as StructureValue;
                    if (enclosingStructureValue != null)
                    {
                        items.Add(new ToolStripRemoveStructureMember(args, enclosingVariable));
                    }
                }

                ListValue enclosingListValue = structureValue.Enclosing as ListValue;
                if (enclosingListValue != null)
                {
                    items.Add(new ToolStripRemoveListEntry(args, enclosingListValue, structureValue));
                }
            }

            ListValue listValue = obj as ListValue;
            if (listValue != null)
            {
                if (enclosingVariable != null)
                {
                    Collection collection = (Collection)enclosingVariable.Type;
                    if (listValue.ElementCount < collection.getMaxSize())
                    {
                        items.Add(new ToolStripAddValueInList(args, enclosingVariable));
                    }
                }
            }
            items.Sort(delegate(BaseToolStripButton b1, BaseToolStripButton b2)
            {
                return b1.Text.CompareTo(b2.Text);
            });
            foreach (BaseToolStripButton menuItem in items)
            {
                menuStrip.Items.Add(menuItem);
            }

            args.MenuStrip = menuStrip;
        }
        #endregion

        #region Edition
        public static void HandleCellEditStarting(object sender, CellEditEventArgs e)
        {
            Variable variable = e.RowObject as Variable;
            if (variable != null)
            {
                DataDictionary.Types.Enum enumType = variable.Type as DataDictionary.Types.Enum;
                if (enumType != null)
                {
                    ComboBox control = new ComboBox();
                    control.Bounds = e.CellBounds;
                    control.Font = ((ObjectListView)sender).Font;
                    control.DropDownStyle = ComboBoxStyle.DropDownList;
                    control.Text = (string)e.Value;
                    foreach (DataDictionary.Constants.EnumValue enumValue in enumType.Values)
                    {
                        control.Items.Add(enumValue.Name);
                    }
                    control.Items.Add(DEFAULT);
                    control.Text = variable.Value.Name;
                    e.Control = control;
                }

                DataDictionary.Types.Range rangeType = variable.Type as DataDictionary.Types.Range;
                if (rangeType != null)
                {
                    ComboBox control = new ComboBox();
                    control.Bounds = e.CellBounds;
                    control.Font = ((ObjectListView)sender).Font;
                    control.DropDownStyle = ComboBoxStyle.DropDown;
                    control.Text = (string)e.Value;
                    foreach (DataDictionary.Constants.EnumValue enumValue in rangeType.SpecialValues)
                    {
                        control.Items.Add(enumValue.Name);
                    }
                    control.Items.Add(DEFAULT);
                    control.Text = variable.Value.Name;
                    e.Control = control;
                }

                Structure structure = variable.Type as Structure;
                if (structure != null)
                {
                    e.Cancel = true;
                }

                Collection collection = variable.Type as Collection;
                if (collection != null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public static void HandleCellEditValidating(object sender, CellEditEventArgs e)
        {
            string text = e.Control.Text;

            Variable variable = e.RowObject as Variable;
            if (variable != null)
            {
                if (DEFAULT != text)
                {
                    if (variable.Type.getValue(text) == null)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        public static void HandleCellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (!e.Cancel)
            {
                Variable variable = e.RowObject as Variable;

                string text = e.Control.Text;
                if (DEFAULT == text)
                {
                    variable.Value = new DefaultValue(variable);
                }
                else
                {
                    if (variable != null)
                    {
                        variable.Value = variable.Type.getValue(text);
                    }
                }
            }
        }
        #endregion
    }
}
