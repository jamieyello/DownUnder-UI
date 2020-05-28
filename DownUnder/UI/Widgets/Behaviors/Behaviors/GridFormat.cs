using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A behavior that keeps children in a grid formation. </summary>
    public class GridFormat : WidgetBehavior
    {
        private int width;
        private int height;

        public Widget Filler;
        public bool DisposeOldOnSet = true;

        public GridFormat(int width, int height, Widget filler = null)
        {
            this.width = width;
            this.height = height;
            Filler = (Widget)filler?.Clone();
        }

        protected override void ConnectToParent()
        {
            if (Filler == null) Filler = DefaultCell();
            GridWriter.InsertFiller(Parent, width, height, Filler);
            Align(this, EventArgs.Empty);
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

        public WidgetList GetRow(int y_row) => GridReader.GetRow(Parent.Children, width, y_row);
        public WidgetList GetColumn(int x_column) => GridReader.GetColumn(Parent.Children, width, height, x_column);
        public Widget this[int x, int y]
        {
            get => Parent.Children[y * width + x];
            set
            {
                value.Parent = Parent;
                if (DisposeOldOnSet) Parent.Children[y * width + x].Dispose();
                Parent.Children[y * width + x] = value;
            }
        }
        public Point IndexOf(Widget widget) => GridReader.IndexOf(width, Parent.Children.IndexOf(widget));

        private void Align(object sender, EventArgs args)
        {
            GridWriter.Align(Parent.Children, width, height, Parent.Area.SizeOnly());
        }
    }
}
