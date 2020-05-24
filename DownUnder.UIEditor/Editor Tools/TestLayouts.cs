using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UIEditor
{
    internal static class TestLayouts
    {
        public static Widget NewLayout(IParent parent)
        {
            Widget layout = new Widget(parent);

            layout.Add(new Widget()
            {
                SnappingPolicy = DiagonalDirections2D.None,
                Area = new RectangleF(100, 100, 200, 150)
            });

            return layout;
        }

        //public static Layout TestLayout(DWindow parent)
        //{
        //    Layout layout = new Layout(parent);
        //    Grid test_grid = new Grid(parent, 2, 5);
        //    test_grid.SnappingPolicy = DiagonalDirections2D.None;
        //    test_grid.InsertDivider(new Label(null, "Divider") { IsFixedHeight = true }, 2);
        //    test_grid.Area = new RectangleF(50f, 50f, 300f, 300f);
        //    Console.WriteLine("result area " + test_grid.Area);

        //    test_grid.SetCell
        //        (
        //        0, 2, new Label(test_grid, "Whoop") { ConstrainAreaToText = true }
        //        );

        //    test_grid.InsertDivider(new Label(null, "Second Divider") { IsFixedHeight = true }, 0);
        //    //test_grid.InsertDivider(new Label(null, "Third Divider") { IsFixedHeight = true }, 0);
        //    //test_grid.InsertDivider(new Label(null, "Fourth Divider") { IsFixedHeight = true }, 0);
        //    //test_grid.InsertDivider(new Label(null, "Middle Divider") { IsFixedHeight = true }, 3);
        //    //test_grid.InsertDivider(new Label(null, "Last Divider") { IsFixedHeight = false }, 5);
        //    //test_grid.IsFixedWidth = true;
        //    //test_grid.debug_output = true;
        //    //test_grid.debug_output = false;
        //    layout.Add(test_grid);

        //    //var test = parent;
        //    //var widget = new PropertyGrid(layout, sprite_font, test);
        //    //Label widget = new Label(layout, sprite_font, "Test text. First line.\nSecond line.\nThird line.") { EditingEnabled = true };
            
        //    //layout.AddWidget(widget);

        //    return layout;
        //}

        //public static Layout TestLayout2(DWindow parent)
        //{
        //    Layout layout = new Layout(parent);
        //    Grid test_grid = new Grid(parent, 2, 2);
        //    test_grid.SnappingPolicy = DiagonalDirections2D.None;
        //    test_grid.Area = new RectangleF(50f, 50f, 300f, 300f);
        //    Grid inner_grid = new Grid(test_grid, 2, 2);
        //    test_grid.SetCell(0, 0, inner_grid);
        //    inner_grid.SetCell(1, 1, new Label(inner_grid, "test"));

        //    layout.Add(test_grid);
            
        //    return layout;
        //}

        //public static Layout TestLayout3(DWindow parent)
        //{
        //    Layout layout = new Layout(parent);

        //    Label label = new Label(layout, "test text")
        //    {
        //        DrawOutline = true,
        //        SnappingPolicy = DiagonalDirections2D.None
        //    };

        //    label.Area = new RectangleF(10, 10, 50, 50);

        //    layout.Add(label);

        //    return layout;
        //}

        //public static Layout TestLayout4(DWindow parent) {
        //    Layout layout = new Layout(parent);
        //    BorderedContainer container = new BorderedContainer();
        //    container.Size = new Point2(400, 300);
        //    container.Parent = layout;
        //    container.ContainedWidget = new Button();
        //    container.SnappingPolicy = DiagonalDirections2D.None;
        //    container.Borders.Up.Widget = new Label(container, "Top Border");
        //    container.Borders.Down.Widget = new Label(container, "Bottom Border");
        //    container.Borders.Left.Widget = new Label(container, "left Sorder");
        //    container.Borders.Right.Widget = new Label(container, "right Sorder");
        //    layout.Add(container);
        //    return layout;
        //}

        //public static Layout TestLayout5(DWindow parent)
        //{
        //    Layout layout = new Layout(parent);
        //    Button button = new Button();
        //    button.SnappingPolicy = DiagonalDirections2D.None;
        //    button.Area = new RectangleF(50, 50, 100, 30);
        //    button.OnClick += (obj, sender) =>
        //    {
        //        button.Actions.Add(new PropertyTransitionAction<Point2>(nameof(button.Position), new Point2(button.X + 40, 0)) { DuplicatePolicy = UI.Widgets.Actions.WidgetAction.DuplicatePolicyType.override_ });
        //    };

        //    Layout moving_window = new Layout();
        //    moving_window.SnappingPolicy = DiagonalDirections2D.None;
        //    moving_window.Area = new RectangleF(10, 10, 400, 300);
        //    moving_window.Add(button);
        //    layout.Add(moving_window);

        //    return layout;
        //}

        //public static Layout TestLayout6(DWindow parent)
        //{
        //    Layout layout = new Layout(parent);
        //    BorderedContainer container = WidgetStuff.MenuBar();
        //    container.SnappingPolicy = DiagonalDirections2D.None;
        //    layout.Add(container);
        //    return layout;
        //}

        //private static void PrintDisplayArea(object sender, EventArgs args) {
        //    Widget w = (Widget)sender;
        //    Console.WriteLine($"DisplayArea {w.DrawingArea}");
        //}

        //public static Layout ScrollTestLayout(IParent parent, SpriteFont sprite_font, out EditorObjects editor_objects) {
        //    Layout inner_layout = new Layout(parent);
        //    inner_layout.Name = "test_layout";
        //    //inner_layout.Size.Set(500, 400);
        //    editor_objects = new EditorObjects();

        //    Grid test_grid = new Grid(parent, 2, 2);
        //    test_grid.Area = new RectangleF(0f, 0f, 200f, 700f);
        //    test_grid.SnappingPolicy = DiagonalDirections2D.None;

        //    Label label = new Label(parent, sprite_font, "test");
        //    label.Text = "Test text";

        //    test_grid.SetCell(0, 0, label);

        //    inner_layout.Add(test_grid);

        //    //throw new Exception();
        //    return inner_layout;
        //}
    }
}