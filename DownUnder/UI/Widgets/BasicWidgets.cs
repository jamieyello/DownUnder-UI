using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.DataTypes;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Widgets
{
    public static class BasicWidgets
    {
        public static Widget PropertyGrid(object obj)
        {
            Widget property_edit_widget = new Widget() { SnappingPolicy = DiagonalDirections2D.TL_TR_BL_BR };
            //var property_children = new AddPropertyEditChildren { EditObject = obj };
            //property_edit_widget.Behaviors.Add(property_children);
            //property_edit_widget.Behaviors.Add(new GridFormat(2, property_children.Properties.Length));
            return property_edit_widget;
        }

        public static Widget DropDown(IEnumerable<string> items, PopInOut pop_in_out_behavior = null)
        {
            WidgetList widgets = new WidgetList();
            foreach (string item in items) widgets.Add(new Widget().WithAddedBehavior(new DrawText() { Text = item, ConstrainAreaToText = true }));
            return DropDown(widgets, pop_in_out_behavior);
        }

        public static Widget DropDown(IEnumerable<Widget> widgets, PopInOut pop_in_out_behavior = null)
        {
            Widget dropdown = new Widget();
            dropdown.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NonScrollable;
            dropdown.MinimumSize = new Point2(1f, 1f);
            dropdown.AddRange(widgets);
            dropdown.Behaviors.Add(new GridFormat(1, widgets.Count()));
            if (pop_in_out_behavior == null) dropdown.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.95f, Directions2D.DLR), RectanglePart.Uniform(0f, Directions2D.D, 1f)) { ClosingMotion = InterpolationSettings.Faster });
            else dropdown.Behaviors.Add(pop_in_out_behavior);

            return dropdown;
        }
    }
}
