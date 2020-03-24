using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.SpecializedWidgets;
using DownUnder.Utility;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UIEditor.Editor_Tools
{
    /// <summary> A <see cref="Layout"/> meant to create <see cref="Layout"/>s during runtime.</summary>
    class UIEditorLayout : Layout
    {
        /// <summary> I will use this later. Don't throw it away! </summary>
        enum CommonWidgets
        {
            Label,
            Grid,
            Layout
        }

        /// <summary> What made the first UI that made the first UI? This. </summary>
        /// <param name="editor_objects"> Shortcut to several widgets relevant to the UI. </param>
        public UIEditorLayout(DWindow parent, out EditorObjects editor_objects)
            : base(parent)
        {
            EmbedIn(parent);

            Grid main_grid = new Grid(this, 2, 1);
            Add(main_grid);

            Layout project = DefaultProject(parent);
            ((Layout)main_grid.GetCell(0, 0)).Add(project);
            
            Grid sidebar = new Grid(main_grid, 1, 2, null, true);
            main_grid.SetCell(1, 0, sidebar);

            Button butt = new Button(null)
            {
                Size = new Point2(100, 100),
                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/layout")
            };

            butt.OnDrag += TestDrag;
            butt.OnDrop += TestDrop;
            butt.DrawBackground = true;
            butt.Behaviors.Add(new StartDragAnimation());

            SpacedList common_controls = new SpacedList(sidebar)
            {
                butt
            };

            sidebar.SetCell(0, 0, common_controls);
            //common_controls.OnListChange += DiagnoseAreaToggled;
            common_controls.debug_output = true;
            common_controls.Add(new Label(null, "eight"));
            common_controls.debug_output = false;
            
            Layout property_grid_layout = new Layout(sidebar);
            sidebar.SetCell(0, 1, property_grid_layout);

            //debug_output = true;

            PropertyGrid property_grid = new PropertyGrid(property_grid_layout, project);
            property_grid_layout.Add(property_grid);

            editor_objects = new EditorObjects
            {
                project = project,
                property_grid = property_grid
            };
        }

        static Layout DefaultProject(DWindow parent)
        {
            // Create taskbar
            Layout project = new Layout(parent)
            {
                Width = 200,
                Height = 140,

                Spacing = new Size2(30, 30),
                SnappingPolicy = DiagonalDirections2D.TopLeft,
                DrawOutline = true,
                DrawBackground = true
            };
            //project.Theme.GetBackground(project).DefaultColor = Color.White;

            Grid grid = new Grid(parent, 2, 3)
            {
                SnappingPolicy = DiagonalDirections2D.TopRight,
                Spacing = new Size2(10, 10)
            };
            //grid.Spacing = new Size2(10, 10);

            project.Add(grid);

            return project;
        }

        private static void TestDrag(object sender, EventArgs args)
        {
            Widget w_sender = (Widget)sender;
            Console.WriteLine($"{w_sender.GetType()} drag event triggered.");
        }

        private static void TestDrop(object sender, EventArgs args)
        {
            Widget w_sender = (Widget)sender;
            Console.WriteLine($"{w_sender.GetType()} drop event triggered.");
        }

        private static void DiagnoseAreaToggled(object sender, EventArgs args)
        {
            if (((Widget)sender).debug_output) DiagnoseArea(sender, args);
        }

        private static void DiagnoseArea(object sender, EventArgs args)
        {
            Widget widget = (Widget)sender;
            Console.WriteLine();
            Console.WriteLine($"Area {widget.Area}");
            Console.WriteLine($"VisibleArea {widget.VisibleArea}");
            Console.WriteLine($"AreaInWindow {widget.AreaInWindow}");
            Console.WriteLine($"DrawingArea {widget.DrawingArea}");
            if (widget is IScrollableWidget scroll_widget)
            {
                Console.WriteLine($"Scroll {scroll_widget.Scroll.Y.GetCurrent()}");
                Console.WriteLine($"ContentArea {scroll_widget.ContentArea}");
            }
            Console.WriteLine($"parent AreaInWindow {widget.ParentWidget?.AreaInWindow}");
            Console.WriteLine();
        }
    }
}
