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
using System.Windows.Forms;

namespace GUI.SpecificationView
{
    public class SpecificationsTreeNode : ModelElementTreeNode<DataDictionary.Dictionary>
    {
        /// <summary>
        /// The editor
        /// </summary>
        private class SpecificationEditor : NamedEditor
        {

        }

        /// <summary>
        /// Instanciates the editor
        /// </summary>
        /// <returns></returns>
        protected override ModelElementTreeNode<DataDictionary.Dictionary>.Editor createEditor()
        {
            return new SpecificationEditor();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public SpecificationsTreeNode(DataDictionary.Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, "Specifications", true)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (DataDictionary.Specification.Specification specification in Item.Specifications)
            {
                Nodes.Add(new SpecificationTreeNode(specification, buildSubNodes));
            }
            SortSubNodes();
        }

        /// <summary>
        /// Adds a new specification to this dictionary
        /// </summary>
        /// <param name="specification"></param>
        public void AddSpecification(DataDictionary.Specification.Specification specification)
        {
            Item.appendSpecifications(specification);
            Nodes.Add(new SpecificationTreeNode(specification, true));
            RefreshNode();
        }

        public void AddSpecificationHandler(object sender, EventArgs args)
        {
            DataDictionary.Specification.Specification specification = (DataDictionary.Specification.Specification)DataDictionary.Generated.acceptor.getFactory().createSpecification();
            specification.setName("Specification" + (Item.countSpecifications() + 1));
            AddSpecification(specification);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add specification", new EventHandler(AddSpecificationHandler)));

            return retVal;
        }

        /// <summary>
        /// Update counts according to the selected chapter
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            Window window = BaseForm as Window;
            if (window != null)
            {
                GUIUtils.MDIWindow.SetCoverageStatus(Item);
            }
        }
    }
}
