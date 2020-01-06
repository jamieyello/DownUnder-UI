using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.SpecializedWidgets;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DownUnder.UIEditor.Editor_Tools
{
    /// <summary>
    /// A layout meant to create layouts during runtime. Used in DownUnder.UIEditor. Should be moved eventually.
    /// </summary>
    class UIEditorLayout : Layout
    {
        enum CommonWidgets
        {
            Label,
            Grid,
            Layout
        }

        // What made the first UI that made the first UI? This.
        /// <param name="sprite_font"></param>
        /// <param name="editor_objects">Shortcut to several widgets relevant to editting UI.</param>
        /// <returns></returns>
        public UIEditorLayout(DWindow parent, out EditorObjects editor_objects)
            : base(parent)
        { 
            Grid main_grid = new Grid(parent, 2, 1);

            Layout project = DefaultProject(parent);

            ((Layout)main_grid.widgets[0][0]).AddWidget(project);
            AddWidget(main_grid);
            
            Grid sidebar = new Grid(main_grid, 1, 2);
            Label test = new Label(sidebar, "Future area for CommonControls.");
            PropertyGrid property_grid = new PropertyGrid(sidebar, project);
            sidebar.AddToCell(0, 0, test);
            sidebar.AddToCell(0, 1, property_grid);
            main_grid.AddToCell(1, 0, sidebar);

            property_grid.EditingEnabled = true;
            var grid_layout = main_grid.GetCell(1, 0);
            grid_layout.Name = "Grid Container";
            property_grid.Name = "Property grid";

            editor_objects = new EditorObjects();
            editor_objects.project = project;
            //editor_objects.fields = fields;
        }

        static Layout DefaultProject(DWindow parent)
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
