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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace GUI
{
    public class GUIUtils
    {
        /// <summary>
        /// The main window of the application
        /// </summary>
        public static MainWindow MDIWindow { get; set; }

        /// <summary>
        /// Refreshes the view according to the model element that has been changed
        /// </summary>
        /// <param name="model"></param>
        public static void RefreshViewAccordingToModel(DataDictionary.Generated.BaseModelElement model)
        {
            MDIWindow.Invoke((MethodInvoker)delegate
            {
                foreach (EditorView.Window editor in MDIWindow.Editors)
                {
                    if (editor.Instance == model)
                    {
                        editor.RefreshText();
                    }
                }
            });
        }

        /// <summary>
        /// Access to a graphics item
        /// </summary>
        public static Graphics Graphics { get; set; }

        /// --------------------------------------------------------------------
        ///   Enclosing finder
        /// --------------------------------------------------------------------
        /// <summary>
        /// Finds in an enclosing element the element whose type matches T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class EnclosingFinder<T> : Utils.IFinder
            where T : class
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public EnclosingFinder()
            {
                Utils.FinderRepository.INSTANCE.Register(this);
            }

            /// <summary>
            /// Finds an enclosing element whose type is T
            /// </summary>
            /// <param name="el"></param>
            /// <returns></returns>
            public static T find(System.Windows.Forms.Control el)
            {
                while (el != null && !(el is T))
                {
                    el = el.Parent;
                }
                return el as T;
            }

            /// <summary>
            /// Clears the cache
            /// </summary>
            public void ClearCache()
            {
                // No cache
            }
        }

        public static void ResizePropertyGridSplitter(
            PropertyGrid propertyGrid,
            int labelColumnPercentageWidth)
        {
            var width =
                propertyGrid.Width * (labelColumnPercentageWidth / 100.0);

            // Go up in hierarchy until found real property grid type.
            var realType = propertyGrid.GetType();
            while (realType != null && realType != typeof(PropertyGrid))
            {
                realType = realType.BaseType;
            }

            var gvf = realType.GetField(@"gridView",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance);
            var gv = gvf.GetValue(propertyGrid);

            var mtf = gv.GetType().GetMethod(@"MoveSplitterTo",
                BindingFlags.NonPublic |
                BindingFlags.InvokeMethod |
                BindingFlags.Instance);
            mtf.Invoke(gv, new object[] { (int)width });
        }

        /// <summary>
        /// Adjust the text size according to the display size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="font"></param>
        public static string AdjustForDisplay(Graphics graphics, string text, int width, Font font)
        {
            string retVal = text;

            if (graphics.MeasureString(text, font).Width > width)
            {
                width = (int)(width - graphics.MeasureString("...", font).Width);
                int i = text.Length;
                int step = i / 2;
                while (graphics.MeasureString(text.Substring(0, i), font).Width > width)
                {
                    i = i - step;
                    step = step / 2;
                    while (graphics.MeasureString(text.Substring(0, i), font).Width < width && step > 0)
                    {
                        i = i + step;
                    }
                }
                retVal = text.Substring(0, i) + "...";
            }

            return retVal;
        }
    }
}
