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

using System.Text;
using System.Collections.Generic;
namespace DataDictionary
{
    public abstract class ModelElement : Generated.BaseModelElement
    {
        /// <summary>
        /// Provides the EFS System in which this element belongs
        /// </summary>
        public EFSSystem EFSSystem
        {
            get
            {
                return EFSSystem.INSTANCE;
            }
        }

        /// <summary>
        /// Provides the Dictionary in which this element belongs
        /// </summary>
        public Dictionary Dictionary
        {
            get
            {
                return Utils.EnclosingFinder<Dictionary>.find(this);
            }
        }

        /// <summary>
        /// Adds a new element log attached to this model element
        /// </summary>
        /// <param name="log"></param>
        public override void AddElementLog(Utils.ElementLog log)
        {
            if (!BeSilent)
            {
                Parameter enclosingParameter = Utils.EnclosingFinder<Parameter>.find(this);
                if (enclosingParameter != null)
                {
                    log.Log = "In " + FullName + ":" + log.Log;
                    enclosingParameter.AddElementLog(log);
                }
                else
                {
                    base.AddElementLog(log);
                }
            }
        }

        /// <summary>
        /// Indicates that no logging should occur
        /// </summary>
        public static bool BeSilent { get; set; }

        public void EnsureGuid()
        {
            if (string.IsNullOrEmpty(getGuid()))
            {
                setGuid(System.Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Provides the Guid of the paragraph and creates one if it is not yet set
        /// </summary>
        public virtual string Guid
        {
            get
            {
                ObjectFactory factory = (ObjectFactory)Generated.acceptor.getFactory();
                if (string.IsNullOrEmpty(getGuid()))
                {
                    EnsureGuid();
                }

                return getGuid();
            }
        }

        /// <summary>
        ///  Provides the common prefix between s1 and s2
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private string CommonPrefix(string s1, string s2)
        {
            int i = 0;
            while (i < s1.Length && i < s2.Length && s1[i] == s2[i])
            {
                i += 1;
            }

            do
            {
                i -= 1;
            } while (i >= 0 && s1[i] != '.');
            i += 1;

            return s1.Substring(0, i);
        }

        /// <summary>
        /// Provides the name of this model element when accessing it from the other model element (provided as parameter)
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        public virtual string ReferenceName(ModelElement modelElement)
        {
            string retVal = FullName;

            if (modelElement != null)
            {
                string prefix = "";

                Types.Structure structure1 = Utils.EnclosingFinder<Types.Structure>.find(this, true);
                Types.Structure structure2 = Utils.EnclosingFinder<Types.Structure>.find(modelElement, true);
                if (structure1 != null)
                {
                    if (structure2 != null)
                    {
                        prefix = CommonPrefix(structure1.FullName + ".", structure2.FullName + ".");
                    }
                    else
                    {
                        if (!(this is Types.Structure))
                        {
                            retVal = Name;
                            prefix = "";
                        }
                        else
                        {
                            retVal = FullName;
                        }
                    }
                }
                else
                {
                    Types.StateMachine stateMachine1 = Utils.EnclosingFinder<Types.StateMachine>.find(this, true);
                    Types.StateMachine stateMachine2 = Utils.EnclosingFinder<Types.StateMachine>.find(modelElement, true);
                    if (stateMachine1 != null && stateMachine2 != null)
                    {
                        prefix = CommonPrefix(stateMachine1.FullName + ".", stateMachine2.FullName + ".");
                    }
                    else
                    {
                        Types.NameSpace nameSpace1 = Utils.EnclosingFinder<Types.NameSpace>.find(this, true);
                        Types.NameSpace nameSpace2 = Utils.EnclosingFinder<Types.NameSpace>.find(modelElement, true);

                        if (nameSpace1 != null && nameSpace2 != null)
                        {
                            prefix = CommonPrefix(nameSpace1.FullName + ".", nameSpace2.FullName + ".");
                        }
                    }
                }

                if (prefix.Length < retVal.Length)
                {
                    retVal = retVal.Substring(prefix.Length);
                }
                else
                {
                    retVal = Name;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the description of the requirements related to this model element 
        /// </summary>
        /// <returns></returns>
        public virtual string RequirementDescription()
        {
            string retVal = "";

            ReqRelated reqRelated = Utils.EnclosingFinder<ReqRelated>.find(this, true);
            if (reqRelated != null)
            {
                retVal = reqRelated.RequirementDescription();
            }

            return retVal;
        }

    }

    public interface TextualExplain
    {
        /// <summary>
        /// The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        string getExplain(bool explainSubElements);
    }

    /// <summary>
    /// Utilities for RTF explain boxes
    /// </summary>
    public class TextualExplainUtilities
    {
        /// <summary>
        /// Left pads the string provided
        /// </summary>
        /// <param name="data">the data to pad</param>
        /// <param name="padlen">the size of the pad</param>
        /// <returns></returns>
        public static string Pad(string data, int padlen)
        {
            return "".PadLeft(padlen) + data;
        }

        /// <summary>
        /// Provides the expression
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Expression(ModelElement element, int padlen)
        {
            string retVal = "";

            IExpressionable expressionable = element as IExpressionable;
            if (expressionable != null)
            {
                if (string.IsNullOrEmpty(expressionable.ExpressionText))
                {
                    retVal = TextualExplainUtilities.Pad("<Undefined expression or statement>", padlen);
                }
                else
                {
                    retVal = TextualExplainUtilities.Pad(expressionable.ExpressionText, padlen);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Comments a section of text
        /// </summary>
        /// <param name="data">the data to pad</param>
        /// <param name="padlen">the size of the pad</param>
        /// <returns></returns>
        public static string Comment(string data, int padlen)
        {
            string retVal = "";

            foreach (string line in data.Split('\n'))
            {
                retVal = retVal + Pad("{\\cf11//" + line + "}\\cf1\\par ", padlen);
            }

            return retVal;
        }

        /// <summary>
        /// Comments an Icommentable
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Comment(ModelElement element, int padlen)
        {
            string retVal = "";

            ICommentable commentable = element as ICommentable;
            if (commentable != null && !string.IsNullOrEmpty(commentable.Comment))
            {
                retVal = Comment(commentable.Comment, padlen);
            }

            return retVal;
        }

        /// <summary>
        /// The name of the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Name(ModelElement element, int padlen)
        {
            string retVal = "";

            Namable namable = element as Namable;
            if (namable != null)
            {
                retVal = TextualExplainUtilities.Pad("{\\cf11 // " + namable.Name + "}\\cf1\\par", padlen);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the header of the element 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Header(ModelElement element, int padlen)
        {
            string retVal = "";

            retVal += Comment(element, padlen);
            retVal += Name(element, padlen);

            return retVal;
        }

        /// <summary>
        /// Iterates the same character a given number of times
        /// </summary>
        /// <param name="c">the character to iterate</param>
        /// <param name="padlen">the size of the expected result</param>
        /// <returns></returns>
        public static string Iterate(char c, int len)
        {
            string retVal = "";
            for (int i = 0; i < len; i++)
            {
                retVal = retVal + c;
            }
            return retVal;
        }

        /// <summary>
        /// The kind of opening brace
        /// </summary>
        private enum BraceType { command, character };

        /// <summary>
        /// Adds RTF prefixes and postfixes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encapsule(string data)
        {
            string retVal = data;

            if (!retVal.StartsWith("{\\rtf"))
            {
                // Replaces all end of lines with \par
                retVal = retVal.Replace("\n", "\\par ");

                // Replaces braces
                // Hyp : Braces are always balanced
                Stack<BraceType> braces = new Stack<BraceType>();
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    if (retVal[i] == '{')
                    {
                        if (i < retVal.Length - 4)
                        {
                            if (retVal[i + 1] == '{')
                            {
                                tmp.Append("{");
                                braces.Push(BraceType.command);
                            }
                            else if (retVal[i + 1] != '\\')
                            {
                                tmp.Append("\\{");
                                braces.Push(BraceType.character);
                            }
                            else if (retVal.Substring(i + 2, 3) == "par")
                            {
                                tmp.Append("\\{ ");
                                braces.Push(BraceType.character);
                            }
                            else
                            {
                                tmp.Append("{");
                                braces.Push(BraceType.command);
                            }
                        }
                        else
                        {
                            tmp.Append("\\{");
                            braces.Push(BraceType.character);
                        }
                    }
                    else if (retVal[i] == '}')
                    {
                        if (braces.Count > 0)
                        {
                            BraceType braceType = braces.Pop();
                            switch (braceType)
                            {
                                case BraceType.character:
                                    tmp.Append("\\}");
                                    break;

                                case BraceType.command:
                                    tmp.Append("}");
                                    break;
                            }
                        }
                        else
                        {
                            tmp.Append("\\}");
                        }
                    }
                    else
                    {
                        tmp.Append(retVal[i]);
                    }
                }
                retVal = tmp.ToString();

                // This is used to ensure that the right style is used for the text
                retVal = "{\\cf11}\\cf1" + retVal;

                // Common prefix to handle the colors
                retVal = "{\\rtf1\\ansi{\\colortbl;\\red0\\green0\\blue0;\\red0\\green0\\blue255;\\red0\\green255\\blue255;\\red0\\green255\\blue0;\\red255\\green0\\blue255;\\red255\\green0\\blue0;\\red255\\green255\\blue0;\\red255\\green255\\blue255;\\red0\\green0\\blue128;\\red0\\green128\\blue128;\\red0\\green128\\blue0;\\red128\\green0\\blue128;\\red128\\green0\\blue0;\\red128\\green128\\blue0;\\red128\\green128\\blue128;\\red192\\green192\\blue192;}" + retVal + "}";
            }

            return retVal;
        }
    }
}
