using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;

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
    }
}
