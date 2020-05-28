using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;

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

            AddPropertyEditChildren property_edit = new AddPropertyEditChildren();
            Widget property_grid_container = new Widget();
            Widget inner_property_edit_widget = new Widget(parent);
            side_grid[0, 1] = 

            return layout;
        }
    }
}
