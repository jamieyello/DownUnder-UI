using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(out EditorObjects editor_objects)
        {
            Widget bordered_container = new Widget();
            bordered_container.Behaviors.Add(new BorderFormat(), out var border_format);
            Widget layout = new Widget().WithAddedBehavior(new GridFormat(2, 1));
            border_format.Center = layout;
            border_format.TopBorder = new Widget();
            border_format.TopBorder.Height = 50;

            // Project
            layout[0, 0].EmbedChildren = false;
            Widget project = Project();
            layout[0, 0].Add(project);

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 2));
            layout[1, 0] = side_grid;
            layout[1, 0].UserResizePolicy = Widget.UserResizePolicyType.allow;
            layout[1, 0].AllowedResizingDirections = Directions2D.L;

            // Property grid
            side_grid[0, 1] = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();

            //Spacing grid
            Widget behaviors_container = new Widget();
            side_grid[0, 0] = behaviors_container;
            behaviors_container.UserResizePolicy = Widget.UserResizePolicyType.allow;
            behaviors_container.AllowedResizingDirections = Directions2D.D;
            behaviors_container.Behaviors.Add(new BorderFormat(), out var behaviors_border);
            behaviors_border.TopBorder = new Widget().WithAddedBehavior(new DrawText() { Text = "Behaviors" });

            Widget behaviors_list = new Widget().WithAddedBehavior(new SpacedListFormat());
            behaviors_border.Center = behaviors_list;

            behaviors_list.Add(new DrawText().EditorWidgetRepresentation());

            //Widget button = new Widget().WithAddedBehavior(new DragableOutlineAnimation());
            //button.Size = new Point2(100, 100);

            //behaviors_list.Add(((Widget)button.Clone()).WithAddedBehavior(new DragAndDropSource() { DragObject = new Widget() { Size = new Point2(50, 50) } }));
            //behaviors_list.Add((Widget)button.Clone());
            //behaviors_list.Add((Widget)button.Clone());
            //behaviors_list.Add((Widget)button.Clone());
            //behaviors_list.Add((Widget)button.Clone());
            //behaviors_list.Add((Widget)button.Clone());

            editor_objects = new EditorObjects();
            editor_objects.project = project;
            editor_objects.property_grid_container = side_grid[0, 1];
            editor_objects.behaviors_list = behaviors_list;

            return bordered_container;
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
