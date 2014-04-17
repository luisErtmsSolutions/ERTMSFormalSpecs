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
namespace GUI.IPCInterface.Values
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [DataContract]
    public class StructureValue : Value
    {
        /// <summary>
        /// The actual value
        /// </summary>
        [DataMember]
        public Dictionary<string, Value> Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public StructureValue(Dictionary<string, Value> value)
        {
            Value = value;
        }

        /// <summary>
        /// Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            string retVal = "{";

            foreach (KeyValuePair<string, Value> item in Value)
            {
                if (retVal.Length != 1)
                {
                    retVal += ", ";
                }

                retVal += item.Key + " => " + item.Value.ToString();
            }

            retVal += "}";

            return retVal;
        }

        /// <summary>
        /// Converts the value provided as an EFS value
        /// </summary>
        /// <returns></returns>
        public override DataDictionary.Values.IValue convertBack(DataDictionary.Types.Type type)
        {
            DataDictionary.Values.StructureValue retVal = null;

            DataDictionary.Types.Structure structureType = type as DataDictionary.Types.Structure;
            if (structureType != null)
            {
                retVal = new DataDictionary.Values.StructureValue(structureType);

                foreach (KeyValuePair<string, Value> pair in Value)
                {
                    DataDictionary.Types.StructureElement element = structureType.findStructureElement(pair.Key);
                    if (element != null)
                    {
                        DataDictionary.Variables.Variable variable = (DataDictionary.Variables.Variable)DataDictionary.Generated.acceptor.getFactory().createVariable();
                        variable.Name = element.Name;
                        variable.Value = pair.Value.convertBack(element.Type);
                        retVal.set(variable);
                    }
                    else
                    {
                        throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot find element named " + element.Name + " in structure " + structureType.FullName));
                    }
                }
            }

            CheckReturnValue(retVal, type);
            return retVal;
        }
    }
}
