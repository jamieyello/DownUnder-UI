using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.DataTypes;
using DownUnder.UI.Widgets.Behaviors.Examples;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UIEditor.EditorTools.Actions;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using static DownUnder.UI.Widgets.Widget;
using ScrollBar = DownUnder.UI.Widgets.Behaviors.Functional.ScrollBar;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(out EditorObjects editor_objects)
        {
            Widget bordered_container = new Widget() { Size = new Point2(60, 60) };
            bordered_container.Behaviors.Add(new BorderFormat(), out var border_format);
            Widget layout = new Widget().WithAddedBehavior(new GridFormat(2, 1));
            border_format.Center = layout;

            AutoDictionary<string, AutoDictionary<string, DropDownEntry>> file_bar_entries = new AutoDictionary<string, AutoDictionary<string, DropDownEntry>>();

            file_bar_entries["File"]["New"].ClickAction = null;
            file_bar_entries["File"]["Save"].ClickAction = new SaveProject();
            file_bar_entries["File"]["Load"].ClickAction = new LoadProject();
            file_bar_entries["Edit"]["Undo"].ClickAction = null;
            file_bar_entries["Edit"]["Redo"].ClickAction = null;
            file_bar_entries["View"]["Behavior Browser"].ClickAction = null;
            file_bar_entries["View"]["Action Browser"].ClickAction = null;

            border_format.TopBorder = BasicWidgets.FileBar(file_bar_entries);

            // Project
            layout[0, 0].EmbedChildren = false;
            layout[0, 0].ChangeColorOnMouseOver = false;

            Widget project = Project();
            layout[0, 0].Add(project);

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 3));
            layout[1, 0] = side_grid;
            side_grid.UserResizePolicy = UserResizePolicyType.allow;
            side_grid.AllowedResizingDirections = Directions2D.L;

            // Property grid
            Widget property_grid_container = BasicWidgets.PropertyGrid(new RectangleF()).SendToContainer();
            property_grid_container.Behaviors.Add(ShadingBehavior.SubtleBlue);
            side_grid[0, 2] = property_grid_container;

            // Behaviors dock
            Widget behaviors_container = new Widget();
            side_grid[0, 0] = behaviors_container;
            behaviors_container.UserResizePolicy = Widget.UserResizePolicyType.allow;
            behaviors_container.AllowedResizingDirections = Directions2D.D;
            behaviors_container.Behaviors.Add(new BorderFormat(), out var behaviors_border);

            behaviors_border.TopBorder = new Widget().WithAddedBehavior(new DrawText() { Text = "Behaviors" });
            behaviors_border.TopBorder.WidgetRole = WidgetRoleType.header_widget;
            behaviors_border.TopBorder.Behaviors.Add(new ShadingBehavior()
            {
                UseWidgetOutlineColor = true
                , ShadeVisibility = 1f
            }); ;

            Widget behaviors_list = new Widget().WithAddedBehavior(new SpacedListFormat());
            behaviors_border.Center = behaviors_list;
            behaviors_list.ChangeColorOnMouseOver = false;

            behaviors_list.Add(new DrawText().EditorWidgetRepresentation());

            // Widgets dock
            Widget widgets_container = new Widget();
            widgets_container.UserResizePolicy = UserResizePolicyType.allow;
            widgets_container.AllowedResizingDirections = Directions2D.D;
            BorderFormat widgets_border = new BorderFormat();
            widgets_container.Behaviors.Add(widgets_border);
            
            widgets_border.TopBorder = new Widget();
            widgets_border.TopBorder.Behaviors.Add(new DrawText() { Text = "Widgets" });
            widgets_border.TopBorder.WidgetRole = WidgetRoleType.header_widget;
            widgets_border.TopBorder.Behaviors.Add(new ShadingBehavior() { UseWidgetOutlineColor = true });

            Widget spaced_list = new Widget();
            spaced_list.ChangeColorOnMouseOver = false;
            widgets_border.Center = spaced_list;
            SpacedListFormat widgets_list_format = new SpacedListFormat();
            spaced_list.Behaviors.Add(widgets_list_format);

            Widget add_widget_button = new Widget() {
                Size = new Point2(100, 100)
                , DrawOutline = false
            };
            add_widget_button.Behaviors.Add(new ShadingBehavior() 
            { 
                UseWidgetOutlineColor = true 
                , BorderWidth = 10f
                , BorderVisibility = 0f
            });
            add_widget_button.Behaviors.Add(new DrawText() {
                Text = "+ New Widget", 
                TextPositioning = DrawText.TextPositioningPolicy.center
            });
            add_widget_button.OnClick += (obj, args) => {
                Widget dock_widget = DockWidget();
                dock_widget.Position = spaced_list[spaced_list.Count - 1].Position;
                spaced_list.Insert(spaced_list.Count - 1, dock_widget);
            };

            widgets_border.Center.Add(add_widget_button);
            side_grid[0, 1] = widgets_container;

            editor_objects = new EditorObjects();
            editor_objects.project = project;
            editor_objects.property_grid_container = property_grid_container;
            editor_objects.behaviors_list = behaviors_list;

            bordered_container.GroupBehaviors.AddPolicy(new GroupBehaviorPolicy() { Behavior = new ScrollBar() });
            bordered_container.IsCloningSupported = false;

            property_grid_container.Behaviors.Add(new RotatingCubeExample());

            return bordered_container;
        }

        private static Widget DockWidget()
        {
            Widget widget = new Widget() { Size = new Point2(100, 100) };

            Widget represented_widget = new Widget();
            widget.Behaviors.Add(new DragAndDropSource() { DragObject = represented_widget });
            widget.Behaviors.Add(new DragableOutlineAnimation());

            widget.Behaviors.Add(new DrawText() { Text = represented_widget.Name, TextPositioning = DrawText.TextPositioningPolicy.center }, out var draw_text);
            represented_widget.OnRename += (sender, args) => { draw_text.Text = represented_widget.Name; };

            return widget;
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
