using DownUnder.UI;
using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UIEditor
{
    internal static class EditorTools
    {
        public static Layout TestLayout(DWindow parent)
        {
            Layout layout = new Layout(parent);
            layout.DrawBackground = true;
            Grid test_grid = new Grid(parent, 2, 5);
            test_grid.SnappingPolicy = DiagonalDirections2D.TopLeft;
            test_grid.SetCell
                (
                0, 2, new Label(test_grid, "Whoop") { ConstrainAreaToText = true }
                );

            test_grid.InsertDivider(new Label(null, "Divider"), 0);
            test_grid.InsertDivider(new Label(null, "Second Divider"), 0);
            test_grid.InsertDivider(new Label(null, "Third Divider"), 0);
            test_grid.InsertDivider(new Label(null, "Fourth Divider"), 0);
            test_grid.InsertDivider(new Label(null, "Middle Divider"), 3);
            test_grid.InsertDivider(new Label(null, "Last Divider"), 5);
            test_grid.Width = 300f;
            test_grid.Height = 300f;
            layout.AddWidget(test_grid);

            //var test = parent;
            //var widget = new PropertyGrid(layout, sprite_font, test);
            //Label widget = new Label(layout, sprite_font, "Test text. First line.\nSecond line.\nThird line.") { EditingEnabled = true };
            
            //layout.AddWidget(widget);

            return layout;
        }

        public static Layout ScrollTestLayout(IWidgetParent parent, SpriteFont sprite_font, out EditorObjects editor_objects)
        {
            Layout inner_layout = new Layout(parent);
            inner_layout.Name = "test_layout";
            //inner_layout.Size.Set(500, 400);
            editor_objects = new EditorObjects();

            Grid test_grid = new Grid(parent, 2, 2);
            test_grid.Area = new RectangleF(0f, 0f, 200f, 700f);
            test_grid.SnappingPolicy = DiagonalDirections2D.None;

            Label label = new Label(parent, sprite_font, "test");
            label.Text = "Test text";

            test_grid.SetCell(0, 0, label);

            inner_layout.AddWidget(test_grid);

            //throw new Exception();
            return inner_layout;
        }
    }
}