using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets
{
    public static class BasicWidgets
    {
        public static Widget PropertyGrid(object obj)
        {
            Widget property_edit_widget = new Widget() { SnappingPolicy = DiagonalDirections2D.TL_TR_BL_BR };
            var property_children = new AddPropertyEditChildren { EditObject = obj };
            property_edit_widget.Behaviors.Add(property_children);
            property_edit_widget.Behaviors.Add(new GridFormat(2, property_children.Properties.Length));
            return property_edit_widget;
        }

        public static Widget DropDown(WidgetList widgets)
        {
            Widget dropdown = new Widget() { 
                Size = new Point2(100,100) 
            }.WithAddedBehavior(new PopInOut());

            //if (widgets == null) throw new System.Exception($"Parameter '{nameof(widgets)}' cannot be null.");
            //if (widgets.Count == 0) throw new System.Exception($"Parameter '{nameof(widgets)}' cannot be empty.");

            //Widget dropdown = new Widget();
            //dropdown.MinimumHeight = 1f;
            //dropdown.IsCloningSupported = false;
            //dropdown.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NonScrollable;
            //dropdown.Behaviors.Add(new GridFormat(1, widgets.Count), out var grid);
            //for (int i = 0; i < widgets.Count; i++) grid[0, i] = widgets[i];

            //widgets[widgets.Count - 1].OnGraphicsInitialized += (sender, args) =>
            //{
            //    dropdown.Width = dropdown[0].Width;
            //    dropdown.Actions.Add(new PropertyTransitionAction<float>(nameof(Widget.Height), widgets.CombinedHeight, InterpolationSettings.Faster));
            //};

            //dropdown.OnClickOff += (s, a) =>
            //{
            //    dropdown.Actions.Add(new PropertyTransitionAction<float>(nameof(Widget.Height), 0f, InterpolationSettings.Fast), out var close);
            //    close.OnCompletion += (s, a) => dropdown.Delete();
            //};

            return dropdown;
        }
    }
}
