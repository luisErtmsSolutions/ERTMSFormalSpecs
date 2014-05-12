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
using System.Collections.Generic;
using DataDictionary.Interpreter;
using System;

namespace DataDictionary.Tests
{
    public class Frame : Generated.Frame, IExpressionable, ICommentable
    {
        /// <summary>
        /// The frame sub sequences
        /// </summary>
        public System.Collections.ArrayList SubSequences
        {
            get
            {
                if (allSubSequences() == null)
                {
                    setAllSubSequences(new System.Collections.ArrayList());
                }
                return allSubSequences();
            }
        }

        /// <summary>
        /// Executes the test cases for this test sequence
        /// </summary>
        /// <param name="runner">The runner used to execute the tests</param>
        /// <returns>the number of failed sub sequences</returns>
        public int ExecuteAllTests()
        {
            int retVal = 0;

            try
            {
                foreach (DataDictionary.Tests.SubSequence subSequence in SubSequences)
                {
                    EFSSystem.Runner = new Runner.Runner(subSequence, false, false);
                    int testCasesFailed = subSequence.ExecuteAllTestCases(EFSSystem.Runner);
                    if (testCasesFailed > 0)
                    {
                        subSequence.AddError("Execution failed");
                        retVal += 1;
                    }
                }
            }
            finally
            {
                EFSSystem.Runner = null;
            }

            return retVal;
        }

        public override System.Collections.ArrayList EnclosingCollection
        {
            get { return Dictionary.Tests; }
        }

        /// <summary>
        /// �Provides the test cases for this test frame
        /// </summary>
        /// <param name="testCases"></param>
        public void FillTestCases(List<TestCase> testCases)
        {
            foreach (SubSequence subSequence in SubSequences)
            {
                subSequence.FillTestCases(testCases);
            }
        }

        /// <summary>
        /// Provides the list of sub sequences for this frame
        /// </summary>
        /// <param name="retVal"></param>
        public void FillSubSequences(List<SubSequence> retVal)
        {
            foreach (SubSequence subSequence in SubSequences)
            {
                retVal.Add(subSequence);
            }
        }

        /// <summary>
        /// Provides the sub sequence whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SubSequence findSubSequence(string name)
        {
            return (SubSequence)Utils.INamableUtils.findByName(name, SubSequences);
        }

        /// <summary>
        /// Translates the frame according to the translation dictionary provided
        /// </summary>
        /// <param name="translationDictionary"></param>
        public void Translate(Translations.TranslationDictionary translationDictionary)
        {
            foreach (SubSequence subSequence in SubSequences)
            {
                subSequence.Translate(translationDictionary);
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            {
                SubSequence item = element as SubSequence;
                if (item != null)
                {
                    appendSubSequences(item);
                }
            }
        }

        /// <summary>
        /// Provides the cycle time value
        /// </summary>
        private Expression __cycleTime = null;
        public Expression CycleDuration
        {
            get
            {
                if (__cycleTime == null)
                {
                    try
                    {
                        __cycleTime = EFSSystem.Parser.Expression(this, getCycleDuration());
                    }
                    catch (Exception e)
                    {
                        AddError("Invalid cycle type, 100 ms assumed");
                        __cycleTime = EFSSystem.Parser.Expression(this, "100");
                    }
                }

                return __cycleTime;
            }
            set
            {
                __cycleTime = null;
            }
        }

        /// <summary>
        /// The expression text for this expressionable
        /// </summary>
        public override string ExpressionText
        {
            get
            {
                return getCycleDuration();
            }
            set
            {
                CycleDuration = null;
                setCycleDuration(value);
            }
        }

        /// <summary>
        /// The corresponding expression tree
        /// </summary>
        public Interpreter.InterpreterTreeNode Tree { get { return CycleDuration; } }

        /// <summary>
        /// Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            CycleDuration = null;
        }

        /// <summary>
        /// Creates the tree according to the expression text
        /// </summary>
        public Interpreter.InterpreterTreeNode Compile()
        {
            return CycleDuration;
        }


        /// <summary>
        /// Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = false;

            Expression tree = EFSSystem.Parser.Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
        }

        /// <summary>
        /// Creates the frame and sets its default values
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Frame createDefault(string name)
        {
            Frame retVal = (Frame)DataDictionary.Generated.acceptor.getFactory().createFrame();
            retVal.Name = name;
            retVal.setCycleDuration("0.1");
            retVal.appendSubSequences(SubSequence.createDefault("Sequence1"));

            return retVal;
        }

        /// <summary>
        /// The frameref which instanciated this frame
        /// </summary>
        public FrameRef FrameRef { get; set; }

        /// <summary>
        /// The comment related to this element
        /// </summary>
        public string Comment
        {
            get { return getComment(); }
            set { setComment(value); }
        }

    }
}
