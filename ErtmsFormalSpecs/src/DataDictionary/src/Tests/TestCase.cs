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

namespace DataDictionary.Tests
{
    public class TestCase : Generated.TestCase, TextualExplain, ICommentable
    {
        public System.Collections.ArrayList Steps
        {
            get
            {
                if (allSteps() == null)
                {
                    setAllSteps(new System.Collections.ArrayList());
                }
                return allSteps();
            }
        }

        public override string Name
        {
            get
            {
                string retVal = base.Name;

                if (Utils.Utils.isEmpty(retVal))
                {
                    retVal = "Feature " + getFeature() + ", test case " + getCase();
                }

                return retVal;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Provides the sub sequence for this step
        /// </summary>
        public SubSequence SubSequence
        {
            get { return Utils.EnclosingFinder<SubSequence>.find(this); }
        }


        public override System.Collections.ArrayList EnclosingCollection
        {
            get { return SubSequence.TestCases; }
        }

        /// <summary>
        /// Translates the current step, according to the translation dictionary
        /// </summary>
        /// <param name="translationDictionary"></param>
        public void Translate(Translations.TranslationDictionary translationDictionary)
        {
            foreach (Step step in Steps)
            {
                step.Translate(translationDictionary);
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            {
                Step item = element as Step;
                if (item != null)
                {
                    appendSteps(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        /// Fills the actual test case with steps of another test case
        /// </summary>
        /// <param name="oldTestCase"></param>
        public void Merge(TestCase aTestCase)
        {
            if (Steps.Count != aTestCase.Steps.Count)
            {
                throw new Exception("The number of steps of " + Name + " changed");
            }
            else
            {
                int cnt = 0;
                foreach (Step step in Steps)
                {
                    Step oldStep = aTestCase.Steps[cnt] as Step;
                    if (aTestCase != null)
                    {
                        if (step.getTCS_Order() == oldStep.getTCS_Order())
                        {
                            step.Merge(oldStep);
                        }
                        else
                        {
                            throw new Exception("The new version of the test case " + Name + " contains the step " + step.Name + " instead of " + oldStep.Name);
                        }
                    }
                    cnt++;
                }
            }
        }

        /// <summary>
        /// Creates a test case and sets its default values
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TestCase createDefault(string name)
        {
            TestCase retVal = (TestCase)DataDictionary.Generated.acceptor.getFactory().createTestCase();
            retVal.Name = name;

            retVal.appendSteps(Step.createDefault("Step1"));

            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the test case's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel, bool explainSubElements)
        {
            string retVal = TextualExplainUtilities.Header(this, indentLevel);

            foreach (Step step in Steps)
            {
                retVal += step.getExplain(indentLevel + 2, explainSubElements) + "\\par";

            }
            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the test case's behaviour
        /// </summary>

        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = "";

            retVal = getExplain(0, explainSubElements);

            return retVal;
        }

        /// <summary>
        /// Provides the number of actions & expectations for this test case
        /// </summary>
        public int ActionCount
        {
            get
            {
                int retVal = 0;

                foreach (Step step in Steps)
                {
                    foreach (SubStep subStep in step.SubSteps)
                    {
                        retVal += subStep.Actions.Count + subStep.Expectations.Count;
                    }
                }

                return retVal;
            }
        }
    }
}
