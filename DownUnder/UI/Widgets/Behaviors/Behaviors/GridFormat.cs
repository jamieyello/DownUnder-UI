using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A behavior that keeps children in a grid formation. </summary>
    public class GridFormat : WidgetBehavior
    {
        private bool _enable_internal_align = true;
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Point Dimensions => new Point(Width, Height);

        public Widget Filler { get; set; }
        public bool DisposeOldOnSet { get; set; } = true;

        public GridFormat(Point dimensions, Widget filler = null)
        {
            Width = dimensions.X;
            Height = dimensions.Y;
            Filler = (Widget)filler?.Clone();
        }
        public GridFormat(int width, int height, Widget filler = null)
        {
            Width = width;
            Height = height;
            Filler = (Widget)filler?.Clone();
        }

        protected override void ConnectToParent()
        {
            if (Filler == null) Filler = DefaultCell();
            GridWriter.InsertFiller(Parent, Width, Height, Filler);
            foreach (Widget child in Parent.Children) child.OnResize += InternalAlign;
            Align(this, EventArgs.Empty);
            Parent.EmbedChildren = false;
            Parent.OnResize += Align;
            Parent.OnAddChild += AddInternalAlign;
            Parent.OnRemoveChild += RemoveInternalAlign;
        }

        protected override void DisconnectFromParent()
        {
            foreach (Widget child in Parent.Children) child.OnResize -= InternalAlign;
            Parent.OnResize -= Align;
            Parent.OnAddChild -= AddInternalAlign;
            Parent.OnRemoveChild -= RemoveInternalAlign;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        private Widget DefaultCell() =>
            new Widget() {
                OutlineSides = Directions2D.DR,
                FitToContentArea = true,
                SnappingPolicy = DiagonalDirections2D.None
            };

        public WidgetList GetRow(int y_row) => GridReader.GetRow(Parent.Children, Width, y_row);
        public WidgetList GetColumn(int x_column) => GridReader.GetColumn(Parent.Children, Width, Height, x_column);
        public Widget this[int x, int y] {
            get => Parent.Children[y * Width + x];
            set
            {
                value.Parent = Parent;
                if (DisposeOldOnSet) Parent.Children[y * Width + x].Dispose();
                Parent.Children[y * Width + x] = value;
            }
        }
        public Point IndexOf(Widget widget) => GridReader.IndexOf(Width, Parent.Children.IndexOf(widget));

        private void Align(object sender, EventArgs args)
        {
            _enable_internal_align = false;
            GridWriter.Align(Parent.Children, Width, Height, Parent.Area.SizeOnly());
            _enable_internal_align = true;
        }

        // Adds and removes InternalAlign to/from child widgets
        private void AddInternalAlign(object sender, EventArgs args) {
            ((Widget)sender).LastAddedWidget.OnResize += InternalAlign;
        }
        private void RemoveInternalAlign(object sender, EventArgs args) {
            ((Widget)sender).LastRemovedWidget.OnResize -= InternalAlign;
        }

        private void InternalAlign(object sender, EventArgs args)
        {
            if (!_enable_internal_align) return;
            _enable_internal_align = false;

            Widget widget = (Widget)sender;
            bool previous_is_fixed_height = widget.IsFixedHeight;
            bool previous_is_fixed_width = widget.IsFixedWidth;
            widget.IsFixedWidth = true;
            widget.IsFixedHeight = true;
            GridWriter.Align(Parent.Children, Width, Height, Parent.Area.SizeOnly());
            widget.IsFixedWidth = previous_is_fixed_width;
            widget.IsFixedHeight = previous_is_fixed_height;

            _enable_internal_align = true;
        }
    }
}
