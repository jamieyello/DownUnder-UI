using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Widgets.Behaviors.Format
{
    public class GridFormat : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        private bool _enable_internal_align = true;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Point Dimensions => new Point(Width, Height);

        public Widget Filler { get; set; }
        public bool DisposeOldOnSet { get; set; } = true;
        public bool SizeToContent { get; set; } = true;

        public Point2 Spacing = new Point2();

        public GridFormat(Point dimensions, Widget filler = null, Point2? spacing = null)
        {
            if (dimensions.X < 0 || dimensions.Y < 0) throw new Exception($"Invalid {nameof(GridFormat)} dimensions. ({dimensions})");
            if (dimensions.X == 0 || dimensions.Y == 0) dimensions = new Point();

            Width = dimensions.X;
            Height = dimensions.Y;
            Filler = (Widget)filler?.Clone();
            if (spacing.HasValue) Spacing = spacing.Value;
        }

        public GridFormat(int width, int height, Widget filler = null, Point2? spacing = null)
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
            if (SizeToContent) Parent.Size = Parent.Children.AreaCoverage.Value.Size + Spacing + Spacing;
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

        public void AddColumn(IEnumerable<Widget> widgets = null)
        {
            InsertRow(Width, widgets);
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
            // !-------------------------------------------------
            // !-------------------------------------------------
            // !-------------------------------------------------

            GridWriter.Align(Parent.Children, Width, Height, Parent.Area.SizeOnly(), Spacing);
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
        }
    }
}
