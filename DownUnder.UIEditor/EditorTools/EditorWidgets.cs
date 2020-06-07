using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Diagnostics;
using System.Reflection;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(DWindow parent)
        {
            GridFormat main_grid = new GridFormat(2, 1);
            Widget layout = new Widget(parent);

            // Main grid
            layout.Behaviors.Add(main_grid);

            // Project
            main_grid[0, 0].EmbedChildren = false;
            main_grid[0, 0].Add(Project());

            // Left grid
            GridFormat side_grid = new GridFormat(1, 2);
            Widget side_grid_layout = new Widget();
            side_grid_layout.Behaviors.Add(side_grid);
            main_grid[1, 0] = side_grid_layout;

            // Property grid
            side_grid[0, 1] = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();
            
            // Spacing grid

            return layout;
        }

        private static Widget Project()
        {
            Widget project = new Widget()
            {
                Area = new RectangleF(20, 20, 300, 200)
            };

            project.DesignerObjects.IsEditModeEnabled = true;
            project.DesignerObjects.UserResizingPolicy = Widget.UserResizePolicyType.allow;
            project.DesignerObjects.AllowedResizingDirections = Directions2D.DR;
            project.DesignerObjects.UserRepositionPolicy = Widget.UserResizePolicyType.disallow;

            return project;
        }
    }
}
