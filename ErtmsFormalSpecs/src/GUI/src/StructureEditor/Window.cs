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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataDictionary.Values;
using System.Collections;
using DataDictionary.Variables;

namespace GUI.StructureValueEditor
{
    public partial class Window : BaseForm
    {
        /// <summary>
        /// The struture value to edit
        /// </summary>
        private StructureValue Model { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            // The text to get for each column
            structureTreeListView.GetColumn(0).AspectGetter = CustomizeTreeView.FieldColumnStringonizer;
            structureTreeListView.GetColumn(1).AspectGetter = CustomizeTreeView.ValueColumnStringonizer;
            structureTreeListView.GetColumn(2).AspectGetter = CustomizeTreeView.DescriptionColumnStringonizer;
            structureTreeListView.FormatCell += new EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(CustomizeTreeView.FormatCell);

            // Tree structure
            structureTreeListView.CanExpandGetter = CustomizeTreeView.HasChildren;
            structureTreeListView.ChildrenGetter = CustomizeTreeView.GetChildren;

            // Contextual menu
            structureTreeListView.CellRightClick += new EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(CustomizeTreeView.CreateContextualMenu);

            // Edition
            structureTreeListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(CustomizeTreeView.HandleCellEditStarting);
            structureTreeListView.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(CustomizeTreeView.HandleCellEditValidating);
            structureTreeListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(CustomizeTreeView.HandleCellEditFinishing);
        }

        /// <summary>
        /// Sets the model for this tree view
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(StructureValue model)
        {
            Model = model;

            List<StructureValue> ObjectModel = new List<StructureValue>();
            ObjectModel.Add(Model);
            structureTreeListView.SetObjects(ObjectModel);
        }
    }
}
