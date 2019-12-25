using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.SpecializedWidgets;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UIEditor.Editor_Tools
{
    /// <summary>
    /// A layout meant to create layouts during runtime. Used in DownUnder.UIEditor. Should be moved eventually.
    /// </summary>
    class UIEditorLayout : Layout
    {
        // What made the first UI that made the first UI? This.
        /// <param name="sprite_font"></param>
        /// <param name="editor_objects">Shortcut to several widgets relevant to editting UI.</param>
        /// <returns></returns>
        public UIEditorLayout(DWindow parent, SpriteFont sprite_font, out EditorObjects editor_objects)
            : base(parent)
        { 
            Grid main_grid = new Grid(parent, 2, 1);

            Layout project = DefaultProject(parent, sprite_font);

            ((Layout)main_grid.widgets[0][0]).AddWidget(project);
            AddWidget(main_grid);

            PropertyGrid property_grid = new PropertyGrid(main_grid, sprite_font, project);
            property_grid.EditingEnabled = true;
            main_grid.AddToCell(1, 0, property_grid);
            var grid_layout = main_grid.GetCell(1, 0);
            grid_layout.Name = "Grid Container";
            property_grid.Name = "Property grid";

            editor_objects = new EditorObjects();
            editor_objects.project = project;
            //editor_objects.fields = fields;
        }

        static Layout DefaultProject(DWindow parent, SpriteFont sprite_font)
        {
            // Create taskbar
            Layout project = new Layout(parent);
            project.Width = 200;
            project.Height = 140;

            project.Spacing = new Size2(30, 30);
            project.SnappingPolicy = DiagonalDirections2D.TopLeft;
            project.DrawOutline = true;
            project.DrawBackground = true;
            project.ChangeColorOnMouseOver = true;
            project.BackgroundColor.DefaultColor = Color.White;

            Grid grid = new Grid(parent, 2, 3);

            grid.SnappingPolicy = DiagonalDirections2D.TopRight;
            grid.Spacing = new Size2(10, 10);
            //grid.Spacing = new Size2(10, 10);

            project.AddWidget(grid);

            return project;
        }
    }
}
