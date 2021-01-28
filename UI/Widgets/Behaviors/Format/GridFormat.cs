using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Format
{
    [DataContract]
    public class GridFormat : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        private bool _enable_internal_align = true;

        [DataMember] public int Width { get; private set; }
        [DataMember] public int Height { get; private set; }
        public Point Dimensions => new Point(Width, Height);

        [DataMember] public Widget Filler { get; set; }
        [DataMember] public bool DisposeOldOnSet { get; set; } = true;
        // Should be removed
        [DataMember] public bool SizeToContent { get; set; } = true;

        [DataMember] public Point2 Spacing = new Point2();

        public GridFormat(Point dimensions, Widget filler = null, Point2? spacing = null) : this(dimensions.X, dimensions.Y, filler, spacing) { }

        public GridFormat() : this(0) { }
        public GridFormat(int width = 0, int height = 0, Widget filler = null, Point2? spacing = null)
        {
            if (width < 0 || height < 0) throw new Exception($"Invalid {nameof(GridFormat)} dimensions. ({width}, {height})");
            if (width == 0 || height == 0)
            {
                width = 0;
                height = 0;
            }

            Width = width;
            Height = height;
            Filler = (Widget)filler?.Clone();
            if (spacing.HasValue) Spacing = spacing.Value;
        }

        protected override void Initialize()
        {
            Parent.EmbedChildren = false;
            Parent.VisualSettings.DrawBackground = false;
            Parent.VisualSettings.VisualRole = GeneralVisualSettings.VisualRoleType.simple_background;
            if (Filler == null) Filler = DefaultCell();
            GridWriter.InsertFiller(Parent, Width, Height, Filler);
            Align(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            foreach (Widget child in Parent.Children) child.OnAreaChangePriority += InternalAlign;
            Parent.OnResize += Align;
            Parent.OnAddChild += AddInternalAlign;
            Parent.OnRemoveChild += RemoveInternalAlign;
        }

        protected override void DisconnectEvents()
        {
            foreach (Widget child in Parent.Children) child.OnAreaChangePriority -= InternalAlign;
            Parent.OnResize -= Align;
            Parent.OnAddChild -= AddInternalAlign;
            Parent.OnRemoveChild -= RemoveInternalAlign;
        }

        public override object Clone()
        {
            GridFormat c = new GridFormat(Dimensions);
            c.Filler = (Widget)Filler.Clone();
            c.DisposeOldOnSet = DisposeOldOnSet;
            c.SizeToContent = SizeToContent;
            c.Spacing = Spacing;

            return c;
        }

        private Widget DefaultCell()
        {
            Widget result = new Widget()
            {
                FitToContentArea = true,
                SnappingPolicy = DiagonalDirections2D.None
            };
            if (Spacing == new Point2()) result.VisualSettings.OutlineSides = Directions2D.DR;
            return result;
        }

        private void SetSizeToContent()
        {
            if (SizeToContent && Parent.Count > 0) Parent.Size = Parent.Children.AreaCoverage.Value.Size + Spacing + Spacing;
        }

        public WidgetList GetRow(int y_row) => GridReader.GetRow(Parent.Children, Width, y_row);
        public WidgetList GetColumn(int x_column) => GridReader.GetColumn(Parent.Children, Width, Height, x_column);
        public Widget this[int x, int y]
        {
            get => Parent.Children[y * Width + x];
            set
            {
                value.Parent = Parent;
                if (DisposeOldOnSet) Parent.Children[y * Width + x].Dispose();
                Parent.Children[y * Width + x] = value;
            }
        }
        public Point IndexOf(Widget widget) => GridReader.IndexOf(Width, Parent.Children.IndexOf(widget));


        public void AddRow(IEnumerable<Widget> widgets = null)
        {
            InsertRow(Height, widgets);
        }
        public void AddRow(Widget widget)
        {
            if (Dimensions.X > 1) throw new Exception($"Cannot add a single {nameof(Widget)} row to a grid with an X dimension greater than 1.");
            AddRow(new Widget[] { widget }); 
        }

        public void AddColumn(IEnumerable<Widget> widgets = null)
        {
            InsertRow(Width, widgets);
        }
        public void AddColumn(Widget widget)
        {
            if (Dimensions.Y > 1) throw new Exception($"Cannot add a single {nameof(Widget)} column to a grid with a Y dimension greater than 1.");
            AddColumn(new Widget[] { widget });
        }

        public void InsertRow(int row, IEnumerable<Widget> widgets = null)
        {
            if (widgets == null && Width == 0) throw new Exception($"Cannot create new row in a grid with no width if no widgets are given.");
            if (widgets != null && widgets.Count() == 0) throw new Exception("Cannot add empty widget collection.");
            if (Width == 0) Width = widgets.Count();
            if (widgets == null)
            {
                WidgetList new_widget_list = new WidgetList();
                for (int i = 0; i < Width; i++) new_widget_list.Add((Widget)Filler.Clone());
                widgets = new_widget_list;
            }
            if (widgets.Count() != Width) throw new Exception("Row count width mismatch.");

            bool p = _enable_internal_align;
            _enable_internal_align = false;
            GridWriter.AddRow(Parent.Children, Width, Height++, row, widgets);
            _enable_internal_align = p;
            Align(this, EventArgs.Empty);
        }
        public void InsertRow(int row, Widget widget)
        {
            if (Dimensions.X > 1) throw new Exception($"Cannot insert a single {nameof(Widget)} row in a grid with a X dimension greater than 1.");
            InsertRow(row, new Widget[] { widget });
        }

        public void InsertColumn(int column, IEnumerable<Widget> widgets = null)
        {
            if (widgets == null && Height == 0) throw new Exception($"Cannot create new column in a grid with no height if no widgets are given.");
            if (widgets != null && widgets.Count() == 0) throw new Exception("Cannot add empty widget collection.");
            if (Height == 0) Height = widgets.Count();
            if (widgets == null)
            {
                WidgetList new_widget_list = new WidgetList();
                for (int i = 0; i < Height; i++) new_widget_list.Add((Widget)Filler.Clone());
                widgets = new_widget_list;
            }
            else if (widgets.Count() != Height) throw new Exception("Row count height mismatch.");

            bool p = _enable_internal_align;
            _enable_internal_align = false;
            GridWriter.AddColumn(Parent.Children, Width++, Height, column, widgets);
            _enable_internal_align = p;
            Align(this, EventArgs.Empty);
        }
        public void InsertColumn(int column, Widget widget)
        {
            if (Dimensions.Y > 1) throw new Exception($"Cannot insert a single {nameof(Widget)} column in a grid with a Y dimension greater than 1.");
            InsertColumn(column, new Widget[] { widget });
        }

        public void RemoveRow(int row)
        {
            bool p = _enable_internal_align;
            _enable_internal_align = false;
            GridWriter.RemoveRow(Parent.Children, Width, Height--, row);
            _enable_internal_align = p;
            Align(this, EventArgs.Empty);

            if (Height == 0) Width = 0;
        }

        public void RemoveColumn(int column)
        {
            bool p = _enable_internal_align;
            _enable_internal_align = false;
            GridWriter.RemoveColumn(Parent.Children, Width--, Height, column);
            _enable_internal_align = p;
            Align(this, EventArgs.Empty);

            if (Width == 0) Height = 0;
        }

        private void Align(object sender, EventArgs args)
        {
            bool p = _enable_internal_align;
            _enable_internal_align = false;

            // !-------------------------------------------------
            // !------------------------------------------------- // why is this here?
            // !-------------------------------------------------
            // !-------------------------------------------------

            GridWriter.Align(Parent.Children, Width, Height, Parent.Area.SizeOnly(), Spacing);
            SetSizeToContent();
            _enable_internal_align = p;
        }

        // Adds and removes InternalAlign to/from child widgets
        private void AddInternalAlign(object sender, EventArgs args)
        {
            ((Widget)sender).LastAddedWidget.OnAreaChangePriority -= InternalAlign;
            ((Widget)sender).LastAddedWidget.OnAreaChangePriority += InternalAlign;
        }

        private void RemoveInternalAlign(object sender, EventArgs args)
        {
            ((Widget)sender).LastRemovedWidget.OnAreaChangePriority -= InternalAlign;
        }

        private void InternalAlign(object sender, RectangleFSetOverrideArgs args)
        {
            if (!_enable_internal_align) return;
            bool p = _enable_internal_align;
            _enable_internal_align = false;
            Widget widget = (Widget)sender;
            Point index = IndexOf(widget);
            if (index == new Point(-1, -1)) throw new Exception($"Given {nameof(Widget)} does not belong to this {nameof(GridFormat)}");

            var difference = widget.Area.Difference(args.PreviousArea);
            args.Override = widget.Area;

            WidgetList current_row = GetRow(index.Y);
            Point2 current_row_minimum_size = current_row.MinimumWidgetSize;
            WidgetList current_column = GetColumn(index.X);
            Point2 current_column_minimum_size = current_column.MinimumWidgetSize;

            if (difference.Top != 0)
            { // Resizing a widget's top
                if (index.Y == 0)
                {
                    //args.Override = args.Override.Value.ResizedBy(difference.Top, Directions2D.U);
                }
                else
                {
                    WidgetList above_row = GetRow(index.Y - 1); // Resize above row first
                    above_row.ResizeBy(difference.Top, Directions2D.D, true);
                    float above_row_bottom = this[index.X, index.Y - 1].Area.Bottom;

                    foreach (Widget widget_ in current_row)
                    { // Resize current row to match one above (A more convoluted current_row.ResizeBy as to not resize the working widget directly)
                        if (widget_ != widget) widget_.Area = widget_.Area.ResizedBy(widget_.Area.Top - above_row_bottom, Directions2D.U, current_row_minimum_size);
                    }
                    args.Override = args.Override.Value.ResizedBy(args.Override.Value.Top - above_row_bottom, Directions2D.U, current_row_minimum_size);
                }
            }

            if (difference.Left != 0) // mostly same as difference.Top
            {
                if (index.X == 0)
                {
                    //args.Override = args.Override.Value.ResizedBy(difference.Left, Directions2D.L);
                }
                else
                {
                    WidgetList previous_column = GetColumn(index.X - 1);
                    previous_column.ResizeBy(difference.Left, Directions2D.R, true);
                    float previous_column_right = this[index.X - 1, index.Y].Area.Right;

                    foreach (Widget widget_ in current_column)
                    {
                        if (widget_ != widget) widget_.Area = widget_.Area.ResizedBy(widget_.Area.Left - previous_column_right, Directions2D.L, current_column_minimum_size);
                    }
                    args.Override = args.Override.Value.ResizedBy(args.Override.Value.Left - previous_column_right, Directions2D.L, current_column_minimum_size);
                }
            }

            if (difference.Right != 0)
            {
                if (index.X == Width - 1)
                {
                    //args.Override = args.Override.Value.ResizedBy(difference.Right, Directions2D.R);
                }
                else
                {
                    WidgetList next_column = GetColumn(index.X + 1);
                    next_column.ResizeBy(-difference.Right, Directions2D.L, true);
                    float next_column_left = this[index.X + 1, index.Y].Area.Left;

                    foreach (Widget widget_ in current_column)
                    {
                        if (widget_ != widget) widget_.Area = widget_.Area.ResizedBy(-(widget_.Area.Right - next_column_left), Directions2D.R, current_column_minimum_size);

                    }

                    args.Override = args.Override.Value.ResizedBy(-(args.Override.Value.Right - next_column_left), Directions2D.R, current_column_minimum_size);
                }
            }


            if (difference.Bottom != 0)
            {
                if (index.Y == Height - 1)
                {
                    //args.Override = args.Override.Value.ResizedBy(difference.Bottom, Directions2D.D);
                }
                else
                {
                    WidgetList next_row = GetRow(index.Y + 1);
                    next_row.ResizeBy(-difference.Bottom, Directions2D.U, true);
                    float next_row_top = this[index.X, index.Y + 1].Area.Top;

                    foreach (Widget widget_ in current_row)
                    {
                        if (widget_ != widget) widget_.Area = widget_.Area.ResizedBy(-(widget_.Area.Bottom - next_row_top), Directions2D.D, current_row_minimum_size);
                    }

                    args.Override = args.Override.Value.ResizedBy(-(args.Override.Value.Bottom - next_row_top), Directions2D.D, current_row_minimum_size);
                }
            }

            SetSizeToContent();
            _enable_internal_align = p;
        }
    }
}
