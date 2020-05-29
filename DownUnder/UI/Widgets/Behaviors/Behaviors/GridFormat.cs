using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A behavior that keeps children in a grid formation. </summary>
    public class GridFormat : WidgetBehavior
    {
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
            Align(this, EventArgs.Empty);
            Parent.EmbedChildren = false;
            Parent.OnResize += Align;
        }

        protected override void DisconnectFromParent()
        {
            Parent.OnResize -= Align;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        private Widget DefaultCell() =>
            new Widget(Parent) {
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
            GridWriter.Align(Parent.Children, Width, Height, Parent.Area.SizeOnly());
        }
    }
}
