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
namespace GUI.ModelDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataDictionary;
    using GUI.BoxArrowDiagram;
    using DataDictionary.Types;
    using DataDictionary.Variables;
    using DataDictionary.Functions;
    using DataDictionary.Rules;
    using System.Drawing;

    /// <summary>
    /// The panel used to display model elements (types, variables, rules, ...)
    /// </summary>
    public class ModelDiagramPanel : BoxArrowPanel<IGraphicalDisplay, ModelArrow>
    {
        private System.Windows.Forms.ToolStripMenuItem addRangeMenuItem;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelDiagramPanel()
            : base()
        {
        }

        /// <summary>
        /// Adds the menu items related to this model element
        /// </summary>
        public override void InitializeStartMenu()
        {
            if ((NameSpace != null))
            {
                // 
                // Add range
                // 
                addRangeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                addRangeMenuItem.Text = "Add range";
                addRangeMenuItem.Click += new System.EventHandler(addRangeMenuItem_Click);
            }
        }

        /// <summary>
        /// Adds a new range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRangeMenuItem_Click(object sender, EventArgs e)
        {
            Range range = (Range)DataDictionary.Generated.acceptor.getFactory().createRange();
            range.Name = "Range";
            NameSpace.appendRanges(range);
            RefreshControl();
        }

        /// <summary>
        /// The namespace for which this panel is built
        /// </summary>
        public NameSpace NameSpace { get { return Model as NameSpace; } set { Model = value; } }

        /// <summary>
        /// The dictionary for which this panel is built
        /// </summary>
        public Dictionary Dictionary { get { return Model as Dictionary; } set { Model = value; } }

        /// <summary>
        /// Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<IGraphicalDisplay, ModelArrow> createBox(IGraphicalDisplay model)
        {
            ModelControl retVal = null;

            NameSpace nameSpace = model as NameSpace;
            if (retVal == null && nameSpace != null)
            {
                retVal = new NameSpaceModelControl(nameSpace);
            }

            Variable variable = model as Variable;
            if (retVal == null && variable != null)
            {
                retVal = new VariableModelControl(variable);
            }

            Function function = model as Function;
            if (retVal == null && function != null)
            {
                retVal = new FunctionModelControl(function);
            }

            Procedure procedure = model as Procedure;
            if (procedure != null)
            {
                retVal = new ProcedureModelControl(procedure);
            }

            Range range = model as Range;
            if (range != null)
            {
                retVal = new RangeModelControl(range);
            }

            DataDictionary.Types.Enum enumeration = model as DataDictionary.Types.Enum;
            if (enumeration != null)
            {
                retVal = new EnumModelControl(enumeration);
            }

            DataDictionary.Types.Collection collection = model as DataDictionary.Types.Collection;
            if (collection != null)
            {
                retVal = new CollectionModelControl(collection);
            }

            StateMachine stateMachine = model as StateMachine;
            if (stateMachine != null)
            {
                retVal = new StateMachineModelControl(stateMachine);
            }

            Structure structure = model as Structure;
            if (structure != null)
            {
                retVal = new StructureModelControl(structure);
            }

            Rule rule = model as Rule;
            if (rule != null)
            {
                retVal = new RuleModelControl(rule);
            }

            return retVal;
        }

        /// <summary>
        /// Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<IGraphicalDisplay, ModelArrow> createArrow(ModelArrow model)
        {
            ModelArrowControl retVal = new ModelArrowControl(model);

            return retVal;
        }

        /// <summary>
        /// Provides the boxes representing the models displayed in this panel
        /// </summary>
        /// <returns></returns>
        public override List<IGraphicalDisplay> getBoxes()
        {
            List<IGraphicalDisplay> retVal = new List<IGraphicalDisplay>();

            if (NameSpace != null)
            {
                foreach (NameSpace nameSpace in NameSpace.NameSpaces)
                {
                    retVal.Add(nameSpace);
                }

                foreach (DataDictionary.Types.Type type in NameSpace.Types)
                {
                    retVal.Add(type);
                }

                foreach (DataDictionary.Variables.Variable variable in NameSpace.Variables)
                {
                    retVal.Add(variable);
                }

                foreach (DataDictionary.Functions.Function function in NameSpace.Functions)
                {
                    retVal.Add(function);
                }

                foreach (DataDictionary.Functions.Procedure procedure in NameSpace.Procedures)
                {
                    retVal.Add(procedure);
                }

                foreach (DataDictionary.Rules.Rule rule in NameSpace.Rules)
                {
                    retVal.Add(rule);
                }
            }

            if (Dictionary != null)
            {
                foreach (NameSpace nameSpace in Dictionary.NameSpaces)
                {
                    retVal.Add(nameSpace);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the arrows between the models displayed in this panel
        /// </summary>
        /// <returns></returns>
        public override List<ModelArrow> getArrows()
        {
            List<ModelArrow> retVal = new List<ModelArrow>();

            List<IGraphicalDisplay> boxes = getBoxes();
            foreach (IGraphicalDisplay item in boxes)
            {
                Variable variable = item as Variable;
                if (variable != null && variable.Type != null)
                {
                    retVal.Add(new ModelArrow(variable, variable.Type, "type", variable));
                }

                Collection collection = item as Collection;
                if (collection != null && collection.Type != null)
                {
                    retVal.Add(new ModelArrow(collection, collection.Type, "of " + collection.getMaxSize(), collection));
                }

                Structure structure = item as Structure;
                if (structure != null)
                {
                    foreach (StructureElement element in structure.Elements)
                    {
                        if (element.Type != null)
                        {
                            if (boxes.Contains(element.Type))
                            {
                                retVal.Add(new ModelArrow(structure, element.Type, element.Name, element));
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// The last model for wich a displayed position has been computed
        /// </summary>
        private IGraphicalDisplay LastComputedPositionModel = null;

        /// <summary>
        /// Reinitialises the automatic position handling
        /// </summary>
        protected override void InitPositionHandling()
        {
            LastComputedPositionModel = null;
            base.InitPositionHandling();
        }

        /// <summary>
        /// Provides the next control position.
        /// Align controls according to their type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override System.Drawing.Point GetNextPosition(IGraphicalDisplay model)
        {
            if (LastComputedPositionModel != null && model.GetType() != LastComputedPositionModel.GetType())
            {
                int Y_OFFSET = model.Height + 10;
                CurrentPosition = new Point(1, CurrentPosition.Y + Y_OFFSET);
            }
            LastComputedPositionModel = model;

            return base.GetNextPosition(model);
        }
    }
}
