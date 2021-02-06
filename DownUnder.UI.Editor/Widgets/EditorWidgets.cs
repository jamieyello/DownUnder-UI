using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Examples;
using DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UIEditor.Behaviors;
using DownUnder.UIEditor.CodeGeneration;
using DownUnder.UIEditor.DataTypes;
using DownUnder.Utilities;
using MonoGame.Extended;
using System;
using System.Diagnostics;
using static DownUnder.UI.Widgets.DataTypes.GeneralVisualSettings;
using static DownUnder.UI.Widgets.Widget;

namespace DownUnder.UIEditor.Widgets
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(string[] args)
        {
            UIEditorStuff editorStuff = new UIEditorStuff(args);

            Widget bordered_container = new Widget() { Size = new Point2(60, 60) };
            bordered_container.Behaviors.Add(new BorderFormat(), out var border_format);
            Widget layout = new Widget().WithAddedBehavior(new GridFormat(2, 1));
            border_format.Center = layout;

            AutoDictionary<string, AutoDictionary<string, DropDownEntry>> file_bar_entries = new AutoDictionary<string, AutoDictionary<string, DropDownEntry>>();

            file_bar_entries["File"]["New"].ClickAction = null;
            file_bar_entries["File"]["Save"].ClickAction = new DirectAction(() => editorStuff.SaveTT());
            file_bar_entries["File"]["Load"].ClickAction = new DirectAction(() =>
            {
                if (editorStuff.Load())
                {
                    if (editorStuff.WorkingProject == null) editorStuff.WorkingProject = Project();
                    layout[0, 0][0] = editorStuff.WorkingProject;

                }
            });
            file_bar_entries["Edit"]["Undo"].ClickAction = null;
            file_bar_entries["Edit"]["Redo"].ClickAction = null;
            file_bar_entries["View"]["Behavior Browser"].ClickAction = null;
            file_bar_entries["View"]["Action Browser"].ClickAction = null;

            border_format.TopBorder = CommonWidgets.FileBar(file_bar_entries);

            Widget cube = new Widget() { Size = new Point2(32, 22) };
            cube.VisualSettings.DrawOutline = false;
            cube.Behaviors.Add(new CubeRotation() { Rotation = new Microsoft.Xna.Framework.Vector3(0.001f, 0.001f, 0.001f) });
            cube.Behaviors.Add(new SpinOnHoverOnOff());

            border_format.TopBorder.Insert(0, cube);

            // Project
            layout[0, 0].EmbedChildren = false;
            layout[0, 0].VisualSettings.ChangeColorOnMouseOver = false;
            layout[0, 0].VisualSettings.VisualRole = VisualRoleType.flashy_background;
            if (editorStuff.WorkingProject == null) editorStuff.WorkingProject = Project();

            layout[0, 0].Add(editorStuff.WorkingProject);

            // Left grid
            Widget side_grid = new Widget().WithAddedBehavior(new GridFormat(1, 3));
            layout[1, 0] = side_grid;
            side_grid.UserResizePolicy = UserResizePolicyType.allow;
            side_grid.AllowedResizingDirections = Directions2D.L;

            // Property grid
            Widget property_grid_container = side_grid[0, 2] = CommonWidgets.PropertyGrid(new Widget()).SendToContainer(false, true);

            // Behaviors dock
            Widget behaviors_container = new Widget();
            side_grid[0, 0] = behaviors_container;
            behaviors_container.UserResizePolicy = UserResizePolicyType.allow;
            behaviors_container.AllowedResizingDirections = Directions2D.D;
            behaviors_container.Behaviors.Add(new BorderFormat(), out var behaviors_border);

            behaviors_border.TopBorder = new Widget().WithAddedBehavior(new DrawText() { Text = "Behaviors" });
            behaviors_border.TopBorder.VisualSettings.VisualRole = VisualRoleType.header_widget;
            behaviors_border.TopBorder.Behaviors.Add(new ShadingBehavior()
            {
                UseWidgetOutlineColor = true,
                ShadeVisibility = 1f
            });

            behaviors_border.Center = WidgetBehavior.BehaviorDisplay(new Type[]
            {
                typeof(Draw3DCube),
                typeof(CubeRotation),
                typeof(ShadingBehavior),
                typeof(DrawText),
                typeof(SpacedListFormat),
                typeof(DragOffOutline),
                typeof(CenterContent),
                typeof(BlurBackground),
                typeof(MouseGlow),
            });

            // Widgets dock
            Widget widgets_container = new Widget();
            widgets_container.UserResizePolicy = UserResizePolicyType.allow;
            widgets_container.AllowedResizingDirections = Directions2D.D;
            BorderFormat widgets_border = new BorderFormat();
            widgets_container.Behaviors.Add(widgets_border);

            widgets_border.TopBorder = new Widget();
            widgets_border.TopBorder.Behaviors.Add(new DrawText() { Text = "Widgets" });
            widgets_border.TopBorder.VisualSettings.VisualRole = VisualRoleType.header_widget;
            widgets_border.TopBorder.Behaviors.Add(new ShadingBehavior() { UseWidgetOutlineColor = true });

            Widget spaced_list = new Widget();
            spaced_list.VisualSettings.ChangeColorOnMouseOver = false;
            widgets_border.Center = spaced_list;
            SpacedListFormat widgets_list_format = new SpacedListFormat();
            spaced_list.Behaviors.Add(widgets_list_format);

            Widget add_widget_button = new Widget(0, 0, 100, 100);
            add_widget_button.VisualSettings.DrawOutline = false;

            add_widget_button.Behaviors.Add(new ShadingBehavior()
            {
                UseWidgetOutlineColor = true,
                BorderWidth = 10f,
                BorderVisibility = 0f,
                DrawAsBackGround = false,
            });
            add_widget_button.Behaviors.Add(new DrawText()
            {
                Text = "+ New Widget",
                XTextPositioning = DrawText.XTextPositioningPolicy.center,
                YTextPositioning = DrawText.YTextPositioningPolicy.center
            });
            add_widget_button.OnClick += (obj, args) => {
                Widget dock_widget = DockWidget();
                dock_widget.Position = spaced_list[spaced_list.Count - 1].Position;
                spaced_list.Insert(spaced_list.Count - 1, dock_widget);
            };

            widgets_border.Center.Add(add_widget_button);
            side_grid[0, 1] = widgets_container;

            bordered_container.IsCloningSupported = false;

            return bordered_container;
        }

        private static Widget DockWidget()
        {
            Widget widget = new Widget(0, 0, 100, 100);

            Widget represented_widget = new Widget();
            widget.Behaviors.Add(new DragAndDropSource() { DragObject = represented_widget });
            widget.Behaviors.Add(new DragOffOutline());

            widget.Behaviors.Add(new DrawText()
            {
                Text = represented_widget.Name,
                XTextPositioning = DrawText.XTextPositioningPolicy.center,
                YTextPositioning = DrawText.YTextPositioningPolicy.center
            }, out var draw_text);
            represented_widget.OnRename += (sender, args) => { draw_text.Text = represented_widget.Name; };

            return widget;
        }

        private static Widget Project()
        {
            Widget project = new Widget(20, 20, 300, 200);

            project.DesignerObjects.IsEditModeEnabled = true;
            project.DesignerObjects.UserResizingPolicy = UserResizePolicyType.allow;
            project.DesignerObjects.AllowedResizingDirections = Directions2D.DR;
            project.DesignerObjects.UserRepositionPolicy = UserResizePolicyType.disallow;
            project.Behaviors.Add(new FormatAllNames());
            project.Behaviors.GroupBehaviors.ImplementPolicy(GroupBehaviors.EditModeBehaviors);

            return project;
        }
    }
}