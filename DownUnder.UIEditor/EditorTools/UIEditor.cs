//using DownUnder.UI;
//using DownUnder.UI.Widgets;
//using DownUnder.UI.Widgets.Actions;
//using DownUnder.UI.Widgets.BaseWidgets;
//using DownUnder.UI.Widgets.Behaviors;
//using DownUnder.UI.Widgets.Interfaces;
//using DownUnder.UI.Widgets.SpecializedWidgets;
//using DownUnder.Utilities;
//using DownUnder.Utility;
//using DownUnder.Widgets;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DownUnder.UIEditor.Editor_Tools
//{
//    /// <summary> A <see cref="Layout"/> meant to create <see cref="Layout"/>s during runtime.</summary>
//    class UIEditorLayout : Layout
//    {
//        /// <summary> What made the first UI that made the first UI? This. </summary>
//        /// <param name="editor_objects"> Shortcut to several widgets relevant to the UI. </param>
//        public UIEditorLayout(DWindow parent, out EditorObjects editor_objects)
//            : base(parent) {
//            EmbedIn(parent);

//            Grid main_grid = new Grid(this, 2, 1);
//            Add(main_grid);

//            Layout project = DefaultProject(this);
//            ((Layout)main_grid.GetCell(0, 0)).Add(project);
//            ((Layout)main_grid.GetCell(0, 0)).ChangeColorOnMouseOver = false;
//            //var s = new ShadingBehavior();
//            //s.ShadeColor = Color.Black;
//            //((Layout)main_grid.GetCell(0, 0)).Behaviors.Add(s);
//            //project.debug_output = true;

//            Grid sidebar = new Grid(main_grid, 1, 2, null, true);
//            main_grid.SetCell(1, 0, sidebar);

//            Button add_button = new Button(null) {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/layout"),
//                Text = "Button"
//            };
//            add_button.Behaviors.Add(add_button.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            var add_button_drag = new DragAndDropSource()
//            {
//                DragObject = new Button()
//                {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 90, 30)
//                }
//            };
//            add_button.Behaviors.Add(add_button_drag);

//            Button add_layout = new Button() {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/layout"),
//                Text = "Layout"
//            };
//            add_layout.Behaviors.Add(add_layout.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            add_layout.Behaviors.Add(new DragAndDropSource() {
//                DragObject = new Layout() {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 100, 100)
//                }
//            });

//            Button add_label = new Button() {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/label"),
//                Text = "Label"
//            };
//            add_label.Behaviors.Add(add_layout.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            add_label.Behaviors.Add(new DragAndDropSource() {
//                DragObject = new Label() {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 90, 30),
//                    Text = "New Label"
//                }
//            });
            
//            Button add_container = new Button() {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/label"),
//                Text = "Container"
//            };
//            add_container.Behaviors.Add(add_layout.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            add_container.Behaviors.Add(new DragAndDropSource() {
//                DragObject = new BorderedContainer() {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 50, 50),
//                }
//            });            

//            Button add_grid = new Button() {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/grid"),
//                Text = "Grid"
//            };
//            add_grid.Behaviors.Add(add_layout.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            add_grid.Behaviors.Add(new DragAndDropSource() {
//                DragObject = new Grid(null, 2, 3) {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 50, 50),
//                }
//            });

//            Button add_spaced_list = new Button() {
//                Size = new Point2(100, 100),
//                Image = parent.Content.Load<Texture2D>("Images/Widget Icons/spaced_list"),
//                Text = "Spaced List"
//            };
//            add_spaced_list.Behaviors.Add(add_layout.BehaviorLibrary.Visual.DragableOutlineAnimation);
//            add_spaced_list.Behaviors.Add(new DragAndDropSource() {
//                DragObject = new SpacedList() {
//                    SnappingPolicy = DiagonalDirections2D.None,
//                    Area = new RectangleF(0, 0, 50, 50),
//                }
//            });

//            SpacedList common_controls = new SpacedList(sidebar) {
//                add_button,
//                add_layout,
//                add_label,
//                add_container,
//                add_grid,
//                add_spaced_list
//            };
//            common_controls.ChangeColorOnMouseOver = false;
//            common_controls.ListSpacing = 40f;
//            sidebar.SetCell(0, 0, common_controls);
            
//            Layout property_grid_layout = new Layout(sidebar);
//            sidebar.SetCell(0, 1, property_grid_layout);

//            PropertyGrid property_grid = new PropertyGrid(property_grid_layout, project);
//            property_grid.debug_output = true;
//            property_grid_layout.Add(property_grid);
//            //property_grid_layout.debug_output = true;


//            //add_button_drag.OnSetWindowClone += (obj, sender) =>
//            //{
//            //    ((Widget)((DragAndDropSource)obj).Parent.ParentWindow.DraggingObject).Behaviors.Add(new WritePropertyToConsole(nameof(VisibleDrawingArea), "New button VisibleDrawingArea = "));
//            //    ((Widget)((DragAndDropSource)obj).Parent.ParentWindow.DraggingObject).Behaviors.Add(new WritePropertyToConsole(nameof(Area), "New button Area = "));
//            //};
//            //project.Behaviors.Add(new WritePropertyToConsole(nameof(VisibleDrawingArea), "Project VisisbleDrawingArea = "));
            
//            project.OnPassthroughClick += SetNewPropertyGridAsync;
//            //project.OnPassthroughClick += WaitAsync;

//            editor_objects = new EditorObjects {
//                project = project,
//                property_grid = property_grid
//            };
//        }

//        static Layout DefaultProject(IParent parent) {
//            Layout project = new Layout(parent)
//            {
//                Size = new Point2(300f, 200f),
//                Spacing = new Size2(30, 30),
//                SnappingPolicy = DiagonalDirections2D.TL,
//                DrawOutline = true,
//                DrawBackground = true
//            };
//            project.DesignerObjects.IsEditModeEnabled = true;
//            project.DesignerObjects.UserResizingPolicy = UserResizePolicyType.allow;
//            project.DesignerObjects.AllowedResizingDirections = Directions2D.DR;
//            project.DesignerObjects.AllowHighlight = false;
//            project.DesignerObjects.UserRepositionPolicy = UserResizePolicyType.disallow;
//            project.Behaviors.Add(new ShadingBehavior());
//            project.DrawingMode = DrawingModeType.use_render_target;

//            return project;
//        }

//        private async void WaitAsync(object sender, EventArgs args)
//        {
//            Console.WriteLine("Waiting...");
//            await Task.Run(() => Thread.Sleep(3000));
//            Console.WriteLine("Done waiting.");
//        }

//        private void SetNewPropertyGridAsync(object sender, EventArgs args)
//        {
//            MainWindow main_window = (MainWindow)((Widget)sender).ParentWindow;
//            Layout grid_layout = (Layout)main_window.editor_objects.property_grid.ParentWidget;
//            Widget target = main_window.SelectedWidgets.Primary;
//            if (target == null) target = main_window.HoveredWidgets.Primary;

//            grid_layout.Actions.Add(new AutoLayoutSlide(new PropertyGrid(grid_layout, target), Directions2D.R));
//            //good working single threaded code above
//            //broken dogshit below
//            //grid_layout.Actions.Add(new AsyncLayoutSlide(Task.Run(() => (Widget)new PropertyGrid(grid_layout, target)), Directions2D.R, InterpolationSettings.Default)
//            //{
//            //    DuplicatePolicy = WidgetAction.DuplicatePolicyType.wait,
//            //    DuplicateDefinition = WidgetAction.DuplicateDefinitionType.interferes_with
//            //});
//        }

//        private static void DiagnoseAreaToggled(object sender, EventArgs args) {
//            if (((Widget)sender).debug_output) DiagnoseArea(sender, args);
//        }

//        private static void DiagnoseArea(object sender, EventArgs args) {
//            Widget widget = (Widget)sender;
//            Console.WriteLine();
//            Console.WriteLine($"Area {widget.Area}");
//            Console.WriteLine($"VisibleArea {widget.VisibleArea}");
//            Console.WriteLine($"AreaInWindow {widget.AreaInWindow}");
//            Console.WriteLine($"DrawingArea {widget.DrawingArea}");
//            if (widget is IScrollableWidget scroll_widget)
//            {
//                Console.WriteLine($"Scroll {scroll_widget.Scroll.Y}");
//                Console.WriteLine($"ContentArea {scroll_widget.ContentArea}");
//            }
//            Console.WriteLine($"parent AreaInWindow {widget.ParentWidget?.AreaInWindow}");
//            Console.WriteLine();
//        }

//        private static void DiagnoseChildren(object sender, EventArgs args) {
//            Widget w_sender = (Widget)sender;
//            Console.WriteLine($"w_s.Children Count {w_sender.Children.Count}");
//            for (int i = 0; i < w_sender.Children.Count; i++) {
//                Console.WriteLine($"{i} {w_sender.Children[i].Area}");
//            }
//        }
//    }
//}
