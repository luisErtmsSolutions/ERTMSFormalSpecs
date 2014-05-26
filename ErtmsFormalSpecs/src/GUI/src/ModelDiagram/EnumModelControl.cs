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
    using DataDictionary.Types;

    /// <summary>
    /// The boxes that represent an enumeration
    /// </summary>
    public class EnumModelControl : TypeModelControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnumModelControl(DataDictionary.Types.Enum model)
            : base(model)
        {
        }

        /// <summary>
        /// The name of the kind of type
        /// </summary>
        public override string ModelName { get { return "Enumeration"; } }
    }
}