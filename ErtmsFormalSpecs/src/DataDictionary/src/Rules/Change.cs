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
namespace DataDictionary.Rules
{
    /// <summary>
    /// Records a change event
    /// </summary>
    public class Change
    {
        /// <summary>
        /// The Logger
        /// </summary>
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Indicates whether the change has already been applied
        /// </summary>
        public bool Applied { get; private set; }

        /// <summary>
        /// The variable affected by the change
        /// </summary>
        public Variables.IVariable Variable { get; private set; }

        /// <summary>
        /// The value the variable had before the change
        /// </summary>
        public Values.IValue PreviousValue { get; private set; }

        /// <summary>
        /// The new value affected by the change
        /// </summary>
        public Values.IValue NewValue { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="previousValue"></param>
        /// <param name="newValue"></param>
        public Change(Variables.IVariable variable, Values.IValue previousValue, Values.IValue newValue)
        {
            Variable = variable;
            PreviousValue = previousValue;
            NewValue = newValue;
            Applied = false;
        }

        /// <summary>
        /// Applies the change if it has not yet been applied
        /// </summary>
        /// <param name="runner"></param>
        public void Apply(Tests.Runner.Runner runner)
        {
            if (!Applied)
            {
                if (runner.LogEvents)
                {
                    Log.Info(Variable.FullName + "<-" + NewValue.LiteralName);
                }
                Variable.Value = NewValue;
                Applied = true;
            }
        }

        /// <summary>
        /// Rolls back the change
        /// </summary>
        public void RollBack()
        {
            if (Applied)
            {
                Variable.Value = PreviousValue;
                Applied = false;
            }
        }
    }

    /// <summary>
    /// Holds a list of changes
    /// </summary>
    public class ChangeList
    {
        /// <summary>
        /// The changes stored in this change list
        /// </summary>
        List<Change> Changes { get; set; }

        /// <summary>
        /// Consdtructor
        /// </summary>
        public ChangeList()
        {
            Changes = new List<Change>();
        }

        /// <summary>
        /// Adds a change to the list of changes
        /// </summary>
        /// <param name="change">The change to add</param>
        /// <param name="apply">Indicates whether the change should be applied immediately</param>
        /// <param name="runner"></param>
        public void Add(Change change, bool apply, Tests.Runner.Runner runner)
        {
            Changes.Add(change);
            if (apply)
            {
                // BUG: This is the case for procedure calls.
                // In this case, computing the next changes induced by the procedure must be based on this changes.
                // However, this contradicts a invariant : the state of the system does not change as long as all changes have not been computed
                // To fix this, changes should be unapplied at the end of the procedure call change evaluation to be applied back 
                // during the activation application.
                change.Apply(runner);
            }
        }

        /// <summary>
        /// Ensures that all changes have a correct value
        /// </summary>
        /// <param name="element"></param>
        public void CheckChanges(ModelElement element)
        {
            foreach (Change change in Changes)
            {
                if (change.NewValue == null)
                {
                    element.AddError(change.Variable.FullName + " <- <cannot evaluate value>");
                }
            }
        }

        /// <summary>
        /// Apply all changes
        /// </summary>
        /// <param name="runner"></param>
        public void Apply(Tests.Runner.Runner runner)
        {
            foreach (DataDictionary.Rules.Change change in Changes)
            {
                change.Apply(runner);
            }
        }

        /// <summary>
        /// Roll back all changes in the list
        /// </summary>
        public void RollBack()
        {
            for (int i = Changes.Count - 1; i >= 0; i--)
            {
                Changes[i].RollBack();
            }
        }

        /// <summary>
        /// Indicates that the change list modifies the variable provided as parameter
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool ImpactVariable(Variables.IVariable variable)
        {
            bool retVal = false;

            foreach (Change change in Changes)
            {
                if (change.Variable == variable)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        public override string ToString()
        {
            string retVal = "";

            foreach (Change change in Changes)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += "\n";
                }
                retVal += change.Variable.FullName + " <- " + change.NewValue.LiteralName;
            }

            return retVal;
        }
    }
}
