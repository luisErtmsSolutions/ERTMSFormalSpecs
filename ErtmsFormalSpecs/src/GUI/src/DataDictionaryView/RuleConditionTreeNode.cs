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
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary.Rules;
using Action = DataDictionary.Rules.Action;

namespace GUI.DataDictionaryView
{
    public class RuleConditionTreeNode : ReqRelatedTreeNode<RuleCondition>
    {
        private class ItemEditor : ReqRelatedEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        private RulePreConditionsTreeNode PreConditions;
        private ActionsTreeNode Actions;
        private SubRulesTreeNode SubRules;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public RuleConditionTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public RuleConditionTreeNode(RuleCondition item, bool buildSubNodes, string name, bool isFolder = false, bool addRequirements = true)
            : base(item, buildSubNodes, name, false, addRequirements)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            PreConditions = new RulePreConditionsTreeNode(Item, buildSubNodes);
            Nodes.Add(PreConditions);

            Actions = new ActionsTreeNode(Item, buildSubNodes);
            Nodes.Add(Actions);

            SubRules = new SubRulesTreeNode(Item, buildSubNodes);
            Nodes.Add(SubRules);
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        /// Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is ActionTreeNode)
            {
                Actions.AcceptDrop(SourceNode);
            }
            else if (SourceNode is PreConditionTreeNode)
            {
                PreConditions.AcceptDrop(SourceNode);
            }
            else if (SourceNode is RuleTreeNode)
            {
                SubRules.AcceptDrop(SourceNode);
            }
        }

        /// <summary>
        /// Adds a precondition
        /// </summary>
        public void AddPreConditionHandler(object sender, EventArgs args)
        {
            PreConditions.AddHandler(sender, args);
        }

        /// <summary>
        /// Adds a precondition
        /// </summary>
        /// <param name="preCondition"></param>
        /// <returns></returns>
        public virtual PreConditionTreeNode AddPreCondition(PreCondition preCondition)
        {
            return PreConditions.AddPreCondition(preCondition);
        }

        /// <summary>
        /// Adds an action
        /// </summary>
        public void AddActionHandler(object sender, EventArgs args)
        {
            Actions.AddHandler(sender, args);
        }

        /// <summary>
        /// Adds an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual ActionTreeNode AddAction(Action action)
        {
            return Actions.AddAction(action);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Pre-condition", new EventHandler(AddPreConditionHandler)));
            newItem.MenuItems.Add(new MenuItem("Action", new EventHandler(AddActionHandler)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}