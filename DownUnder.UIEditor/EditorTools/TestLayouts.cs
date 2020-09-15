using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Examples;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Security.Cryptography.X509Certificates;

namespace DownUnder.UIEditor
{
    internal static class TestLayouts
    {
        public static Widget NewLayout()
        {
            Widget layout = new Widget();

            Widget new_widget = new Widget
            {
                SnappingPolicy = DiagonalDirections2D.None,
                Area = new RectangleF(100, 100, 200, 150)
            };
            layout.Add(new_widget);

            GridFormat grid_formatter = new GridFormat(2, 2);
            new_widget.Behaviors.Add(grid_formatter);
            grid_formatter[0, 0].Behaviors.Add(new DrawText() { Text = "test" });

            return layout;
        }

        public static Widget ContainerTest()
        {
            Widget layout = new Widget();

            Widget inner = new Widget
            {
                SnappingPolicy = DiagonalDirections2D.None,
                Area = new RectangleF(40, 40, 400, 300)
            };

            inner.Behaviors.Add(new BorderFormat(), out var border_format);
            border_format.TopBorder = new Widget();
            border_format.Center = new Widget();
            border_format.BottomBorder = new Widget();
            border_format.LeftBorder = new Widget();
            border_format.RightBorder = new Widget();
            layout.Add(inner);

            inner.UserResizePolicy = Widget.UserResizePolicyType.allow;
            inner.AllowedResizingDirections = Directions2D.All;

            return layout;
        }

        public static Widget InnerGirdResizeTest()
        {
            Widget layout = new Widget();

            Widget inner = new Widget();
            inner.Area = new RectangleF(50, 50, 300, 150);
            inner.Behaviors.Add(new GridFormat(2, 1), out var grid_format);
            grid_format[0, 0].UserResizePolicy = Widget.UserResizePolicyType.allow;
            inner.UserResizePolicy = Widget.UserResizePolicyType.allow;

            layout.Add(inner);

            Widget second = new Widget {
                Area = new RectangleF(400, 50, 250, 250)
            }.WithAddedBehavior(new GridFormat(3, 3), out var grid);

            grid[1, 1].UserResizePolicy = Widget.UserResizePolicyType.allow;

            layout.Add(second);

            return layout;
        }

        public static Widget RenderScrollDebug()
        {
            Widget layout = new Widget();

            Widget inner = new Widget
            {
                Area = new RectangleF(10, 10, 400, 300)
            };

            inner.Add(new Widget() { Position = new Point2(50,50) });
            Widget test = inner.LastAddedWidget;
            test.Behaviors.Add(new DrawText() { Text = "t" });
            inner.Add(new Widget { Position = new Point2(150,50) });
            inner.Add(new Widget { Position = new Point2(250,50) });
            inner.Add(new Widget { Position = new Point2(350,50) });
            inner.Add(new Widget { Position = new Point2(450,50) });
            inner.Add(new Widget { Position = new Point2(50, 150) });
            inner.Add(new Widget { Position = new Point2(50, 250) });
            inner.Add(new Widget { Position = new Point2(50, 350) });
            inner.Add(new Widget { Position = new Point2(50, 450) });

            Widget inner2 = new Widget()
            {
                Area = new RectangleF(420, 10, 400, 400)
            };

            inner.Behaviors.Add(ShadingBehavior.SubtleBlue);

            test.debug_output = true;
            test.OnDraw += (s, a) => { Console.WriteLine($"test.DrawingArea = {test.DrawingArea}"); };
            inner.OnDraw += (s, a) => { Console.WriteLine($"inner.DrawingArea = {inner.DrawingArea}"); };

            layout.Add(inner);
            layout.Add(inner2);

            return layout;
        }

        public static Widget CubeTest()
        {
            Widget layout = new Widget();

            Widget inner = new Widget
            {
                Area = new RectangleF(10, 10, 400, 300)
                , UserResizePolicy = Widget.UserResizePolicyType.allow
            };

            inner.Behaviors.Add(new Draw3DCube());

            layout.Add(inner);

            return layout;
        }

        public static Widget GridEdit()
        {
            Widget layout = new Widget();

            Widget grid = new Widget() { Position = new Point2(30, 60) }.WithAddedBehavior(new GridFormat(2, 2));
            grid.Size = new Point2(300, 300);
            grid.FitToContentArea = true;
            grid.UserResizePolicy = Widget.UserResizePolicyType.allow;
            layout.Add(grid);

            Widget add_row_button = new Widget { Position = new Point2(30, 30) }.WithAddedBehavior(new DrawText("Add Row"));
            add_row_button.OnClick += (s, a) => { grid.Behaviors.GetFirst<GridFormat>().InsertRow(1); };
            layout.Add(add_row_button);
            
            Widget add_column_button = new Widget { Position = new Point2(120, 30) }.WithAddedBehavior(new DrawText("Add Column"));
            add_column_button.OnClick += (s, a) => { grid.Behaviors.GetFirst<GridFormat>().InsertColumn(1); };
            layout.Add(add_column_button);

            Widget remove_row_button = new Widget { Position = new Point2(200, 30) }.WithAddedBehavior(new DrawText("Remove Row"));
            remove_row_button.OnClick += (s, a) => { grid.Behaviors.GetFirst<GridFormat>().RemoveRow(1); };
            layout.Add(remove_row_button);

            Widget remove_column_button = new Widget { Position = new Point2(300, 30) }.WithAddedBehavior(new DrawText("Remove Column"));
            remove_column_button.OnClick += (s, a) => { grid.Behaviors.GetFirst<GridFormat>().RemoveColumn(1); };
            layout.Add(remove_column_button);
           
            return layout;
        }

        public static Widget GridEdit2()
        {
            Widget layout = new Widget();

            Widget grid = new Widget() { Position = new Point2(30, 60) }.WithAddedBehavior(new GridFormat(0, 0), out var grid_format);
            grid.Size = new Point2(300, 300);
            grid.FitToContentArea = true;
            grid.UserResizePolicy = Widget.UserResizePolicyType.allow;
            layout.Add(grid);

            grid_format.AddRow(new Widget[] { new Widget(), new Widget() });
            grid_format.AddRow();
            grid_format.AddRow();
            grid_format.AddRow(new Widget[] { new Widget(), new Widget() });

            return layout;
        }

        public static Widget PropertyTest()
        {
            Widget layout = new Widget();

            layout.Add(new MemberViewer(new Widget()).CreateWidget(), out var property_grid);
            property_grid.UserResizePolicy = Widget.UserResizePolicyType.allow;

            return layout;
        }

        public static Widget EditTextTest()
        {
            Widget layout = new Widget();

            layout.Add(new DrawText { Text = "whoop" }.CreateWidget());
            layout[0].Behaviors.Add(new DrawEditableText { });
            layout[0].UserResizePolicy = Widget.UserResizePolicyType.allow;
            return layout;
        }
    }
}