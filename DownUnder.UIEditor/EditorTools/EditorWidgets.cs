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
            //Widget bordered_container = new Widget();
            //bordered_container.Behaviors.Add(new BorderFormat(), out var border_format);
            Widget layout = new Widget().WithAddedBehavior(new GridFormat(2, 1));
            //border_format.Center = layout;
            //border_format.TopBorder = new Widget();
            //border_format.TopBorder.Height = 50;

            // Project
            layout[0, 0].EmbedChildren = false;
            layout[0, 0].Add(Project());

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 2));
            layout[1, 0] = side_grid;
            layout[1, 0].UserResizePolicy = Widget.UserResizePolicyType.allow;
            layout[1, 0].AllowedResizingDirections = Directions2D.L;

            // Property grid
            side_grid[0, 1] = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();

            // Spacing grid
            side_grid[0, 0] = new Widget().WithAddedBehavior(new SpacedListFormat());

            Widget button = new Widget().WithAddedBehavior(new DragableOutlineAnimation());
            button.Size = new Point2(100, 100);

            side_grid[0, 0].Add(((Widget)button.Clone()).WithAddedBehavior(new DragAndDropSource() { DragObject = new Widget() { Size = new Point2(50, 50) } }));
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].Add((Widget)button.Clone());
            side_grid[0, 0].UserResizePolicy = Widget.UserResizePolicyType.allow;
            side_grid[0, 0].AllowedResizingDirections = Directions2D.D;

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
