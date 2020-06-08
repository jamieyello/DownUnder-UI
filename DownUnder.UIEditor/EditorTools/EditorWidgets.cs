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
            Widget layout = new Widget(parent).WithAddedBehavior(new GridFormat(2, 1));

            // Project
            layout[0, 0].EmbedChildren = false;
            layout[0, 0].Add(Project());

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 2));
            layout[1, 0] = new Widget().WithAddedBehavior(new GridFormat(1, 2));

            // Property grid
            side_grid[0, 1] = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();

            // Spacing grid
            side_grid[0, 0] = new Widget().WithAddedBehavior(new SpacedListFormat());
            side_grid[0, 0].Add(new Widget());
            side_grid[0, 0].Add(new Widget());
            side_grid[0, 0].Add(new Widget());
            side_grid[0, 0].Add(new Widget());
            side_grid[0, 0].Add(new Widget());
            side_grid[0, 0].Add(new Widget());

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
