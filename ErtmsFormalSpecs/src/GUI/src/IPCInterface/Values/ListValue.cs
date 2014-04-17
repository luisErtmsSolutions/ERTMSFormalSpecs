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
    public class ListValue : Value
    {
        /// <summary>
        /// The actual value
        /// </summary>
        [DataMember]
        public List<Value> Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public ListValue(List<Value> value)
        {
            Value = value;
        }

        /// <summary>
        /// Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            string retVal = "[";

            foreach (Value item in Value)
            {
                if (retVal.Length != 1)
                {
                    retVal += ", ";
                }

                retVal += item.ToString();
            }

            retVal += "]";

            return retVal;
        }

        /// <summary>
        /// Converts the value provided as an EFS value
        /// </summary>
        /// <returns></returns>
        public override DataDictionary.Values.IValue convertBack(DataDictionary.Types.Type type)
        {
            DataDictionary.Values.IValue retVal = null;

            DataDictionary.Types.Collection collectionType = type as DataDictionary.Types.Collection;
            if (collectionType != null)
            {
                List<DataDictionary.Values.IValue> values = new List<DataDictionary.Values.IValue>();

                int i = 0;
                foreach (Value item in Value)
                {
                    i += 1;
                    values.Add(item.convertBack(collectionType.Type));
                }

                retVal = new DataDictionary.Values.ListValue(collectionType, values);
            }

            CheckReturnValue(retVal, type);
            return retVal;
        }
    }
}
