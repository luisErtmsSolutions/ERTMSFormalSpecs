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

using System.Collections;
using DataDictionary.Generated;
using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;
using Function = DataDictionary.Functions.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using Procedure = DataDictionary.Functions.Procedure;
using Type = DataDictionary.Types.Type;

namespace DataDictionary
{
    public class Parameter : Generated.Parameter, ITypedElement
    {
        /// <summary>
        /// Parameter namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        /// Parameter type name
        /// </summary>
        public string TypeName
        {
            get { return getTypeName(); }
            set
            {
                type = null;
                setTypeName(value);
            }
        }

        /// <summary>
        /// Parameter type
        /// </summary>
        private Type type;

        public Type Type
        {
            get
            {
                if (type == null)
                {
                    type = EFSSystem.findType(NameSpace, TypeName);
                }
                return type;
            }
            set
            {
                TypeName = value.Name;
                type = value;
            }
        }

        /// <summary>
        /// The enclosing collection of the parameter
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                if (this.Enclosing is Function)
                {
                    return EnclosingFinder<Function>.find(this).FormalParameters;
                }
                else if (this.Enclosing is Procedure)
                {
                    return EnclosingFinder<Procedure>.find(this).FormalParameters;
                }

                return null;
            }
        }

        /// <summary>
        /// Creates an actual parameter based on this formal parameter and the value assigned to that parameter
        /// </summary>
        /// <returns></returns>
        public Actual createActual()
        {
            Actual retVal = new Actual();
            retVal.Name = Name;
            retVal.Enclosing = Enclosing;
            retVal.Parameter = this;

            return retVal;
        }

        /// <summary>
        /// Provides the mode of the parameter
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
            set { }
        }

        /// <summary>
        /// The default value
        /// </summary>
        public string Default
        {
            get { return Type.Default; }
            set { }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        public override string ToString()
        {
            string retVal = "Parameter " + Name;

            return retVal;
        }

        /// <summary>
        /// Provides the name of this model element when accessing it from the other model element (provided as parameter)
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        public override string ReferenceName(ModelElement modelElement)
        {
            string retVal = Name;

            return retVal;
        }
    }
}