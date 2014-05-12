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
namespace GUI.ModelDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataDictionary;
    using DataDictionary.Variables;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The boxes that represent a variable
    /// </summary>
    public class VariableModelControl : ModelControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VariableModelControl(Variable model)
            : base(model)
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            NORMAL_COLOR = Color.LightBlue;

            MouseClick += new MouseEventHandler(HandleMouseClick);
        }

        /// <summary>
        /// Handles a simple click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Panel.Select(this, true);
            }
        }

        /// <summary>
        /// The name of the kind of model
        /// </summary>
        public override string ModelName { get { return "Variable"; } }
    }
}
