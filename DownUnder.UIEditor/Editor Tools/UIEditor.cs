using DownUnder.UI;
using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.SpecializedWidgets;
using DownUnder.Utility;
using MonoGame.Extended;

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

            Label test = new Label(sidebar, "Future area for CommonControls.");
            sidebar.SetCell(0, 0, test);

            Layout property_grid_layout = new Layout(sidebar);
            property_grid_layout.debug_output = true;
            sidebar.SetCell(0, 1, property_grid_layout);

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
    }
}
