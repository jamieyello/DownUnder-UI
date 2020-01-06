using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.SpecializedWidgets;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UIEditor
{
    internal static class EditorTools
    {
        public static Layout TestLayout(DWindow parent, SpriteFont sprite_font, out EditorObjects editor_objects)
        {
            Layout layout = new Layout(parent);
            layout.Name = "test_layout";

            //Grid test_grid = new Grid(parent, 5, 5);
            //test_grid.MinimumHeight = 600;
            //layout.AddWidget(test_grid);

            //var test = parent;
            //var widget = new PropertyGrid(layout, sprite_font, test);
            Label widget = new Label(layout, sprite_font, "Test text. First line.\nSecond line.\nThird line.") { EditingEnabled = true };
            

            layout.AddWidget(widget);

            editor_objects = new EditorObjects();
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