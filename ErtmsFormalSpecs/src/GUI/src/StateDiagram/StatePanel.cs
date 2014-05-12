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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Constants;
using DataDictionary.Types;
using GUI.BoxArrowDiagram;
using DataDictionary.Rules;
using DataDictionary.Variables;
using Utils;

namespace GUI.StateDiagram
{
    public class StatePanel : BoxArrowPanel<State, Transition>
    {
        private System.Windows.Forms.ToolStripMenuItem addStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTransitionMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;

        /// <summary>
        /// Initializes the start menu
        /// </summary>
        public void InitializeStartMenu()
        {
            addStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addTransitionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            // 
            // addStateMenuItem
            // 
            addStateMenuItem.Name = "addStateMenuItem";
            addStateMenuItem.Size = new System.Drawing.Size(161, 22);
            addStateMenuItem.Text = "Add State";
            addStateMenuItem.Click += new System.EventHandler(addBoxMenuItem_Click);
            // 
            // addTransitionMenuItem
            // 
            addTransitionMenuItem.Name = "addTransitionMenuItem";
            addTransitionMenuItem.Size = new System.Drawing.Size(161, 22);
            addTransitionMenuItem.Text = "Add transition";
            addTransitionMenuItem.Click += new System.EventHandler(addArrowMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator.Name = "toolStripSeparator1";
            toolStripSeparator.Size = new System.Drawing.Size(158, 6);
            // 
            // toolStripMenuItem1
            // 
            deleteMenuItem.Name = "toolStripMenuItem1";
            deleteMenuItem.Size = new System.Drawing.Size(153, 22);
            deleteMenuItem.Text = "Delete selected";
            deleteMenuItem.Click += new System.EventHandler(deleteMenuItem1_Click);

            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                addStateMenuItem,
                addTransitionMenuItem,
                toolStripSeparator,
                deleteMenuItem});
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StatePanel()
            : base()
        {
            InitializeStartMenu();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public StatePanel(IContainer container)
            : base()
        {
            container.Add(this);

            InitializeStartMenu();
        }

        /// <summary>
        /// Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<State, Transition> createBox(State model)
        {
            BoxControl<State, Transition> retVal = new StateControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        /// Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<State, Transition> createArrow(Transition model)
        {
            ArrowControl<State, Transition> retVal = new TransitionControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        /// The state machine displayed by this panel
        /// </summary>
        public StateMachine StateMachine { get { return Model as StateMachine; } set { Model = value; } }

        /// <summary>
        /// The state machine variable (if any) displayed by this panel
        /// </summary>
        public IVariable StateMachineVariable { get; set; }

        /// <summary>
        /// Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<State> getBoxes()
        {
            List<State> retVal = new List<State>();

            foreach (State state in StateMachine.States)
            {
                retVal.Add(state);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<Transition> getArrows()
        {
            return StateMachine.Transitions;
        }

        private void addBoxMenuItem_Click(object sender, EventArgs e)
        {
            State state = (State)DataDictionary.Generated.acceptor.getFactory().createState();
            state.Name = "State" + (StateMachine.States.Count + 1);

            if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
            {
                DataDictionaryView.StateMachineTreeNode node = GUIUtils.MDIWindow.DataDictionaryWindow.FindNode(StateMachine) as DataDictionaryView.StateMachineTreeNode;
                if (node != null)
                {
                    node.AddState(state);
                }
                else
                {
                    StateMachine.appendStates(state);
                }
            }

            RefreshControl();
        }

        private void addArrowMenuItem_Click(object sender, EventArgs e)
        {
            if (StateMachine.States.Count > 1)
            {
                DataDictionary.ObjectFactory factory = (DataDictionary.ObjectFactory)DataDictionary.Generated.acceptor.getFactory();
                DataDictionary.Rules.Rule rule = (DataDictionary.Rules.Rule)factory.createRule();
                rule.Name = "Rule" + (StateMachine.Rules.Count + 1);

                DataDictionary.Rules.RuleCondition ruleCondition = (DataDictionary.Rules.RuleCondition)factory.createRuleCondition();
                ruleCondition.Name = "RuleCondition" + (rule.RuleConditions.Count + 1);
                rule.appendConditions(ruleCondition);

                DataDictionary.Rules.Action action = (DataDictionary.Rules.Action)factory.createAction();
                action.ExpressionText = "THIS <- " + ((State)StateMachine.States[1]).LiteralName;
                ruleCondition.appendActions(action);

                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    DataDictionaryView.StateTreeNode stateNode = GUIUtils.MDIWindow.DataDictionaryWindow.FindNode((State)StateMachine.States[0]) as DataDictionaryView.StateTreeNode;
                    if (!stateNode.SubNodesBuilt)
                    {
                        stateNode.BuildSubNodes(false);
                        stateNode.UpdateColor();
                    }
                    DataDictionaryView.RuleTreeNode ruleNode = stateNode.Rules.AddRule(rule);
                }

                RefreshControl();
                Refresh();

                ArrowControl<State, Transition> control = getArrowControl(ruleCondition);
                Select(control, false);
            }
        }

        private void deleteMenuItem1_Click(object sender, EventArgs e)
        {
            IModelElement model = null;

            if (Selected is BoxControl<State, Transition>)
            {
                model = (Selected as BoxControl<State, Transition>).Model;
            }
            else if (Selected is ArrowControl<State, Transition>)
            {
                ArrowControl<State, Transition> control = Selected as ArrowControl<State, Transition>;
                RuleCondition ruleCondition = control.Model.RuleCondition;
                Rule rule = ruleCondition.EnclosingRule;
                if (rule.countConditions() == 1)
                {
                    model = rule;
                }
                else
                {
                    model = ruleCondition;
                }

            }

            if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
            {
                BaseTreeNode node = GUIUtils.MDIWindow.DataDictionaryWindow.FindNode(model);
                if (node != null)
                {
                    node.Delete();
                }
            }
            Select(null, false);

            RefreshControl();
            Refresh();
        }
    }
}
