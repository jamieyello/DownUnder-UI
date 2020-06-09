using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor()
        {
            Widget layout = new Widget().WithAddedBehavior(new GridFormat(2, 1));

            // Project
            layout[0, 0].EmbedChildren = false;
            layout[0, 0].Add(Project());

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 2));
            layout[1, 0] = side_grid;

            // Property grid
            side_grid[0, 1] = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();

            // Spacing grid
            side_grid[0, 0] = new Widget().WithAddedBehavior(new SpacedListFormat());

            Widget button = new Widget().WithAddedBehavior(new DragableOutlineAnimation());
            button.Size = new Point2(100, 100);

            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());

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
