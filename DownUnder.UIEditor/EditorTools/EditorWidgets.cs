using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(DWindow parent)
        {
            GridFormat main_grid = new GridFormat(2, 1);
            Widget layout = new Widget(parent);
            layout.Behaviors.Add(main_grid);

            GridFormat side_grid = new GridFormat(1, 2);
            Widget side_grid_layout = new Widget();
            side_grid_layout.Behaviors.Add(side_grid);
            main_grid[1, 0] = side_grid_layout;

            Widget property_grid_container = new Widget();
            Widget inner_property_edit_widget = new Widget() { SnappingPolicy = DiagonalDirections2D.TL_TR_BL_BR };
            //side_grid[0, 1] = 

            side_grid[0, 1] = property_grid_container;
            inner_property_edit_widget.Behaviors.Add(new AddPropertyEditChildren { EditObject = new RectangleF() });
            inner_property_edit_widget.Behaviors.Add(new GridFormat(2, inner_property_edit_widget.Behaviors.Get<AddPropertyEditChildren>().Properties.Length));
            property_grid_container.Add(inner_property_edit_widget);

            return layout;
        }
    }
}
