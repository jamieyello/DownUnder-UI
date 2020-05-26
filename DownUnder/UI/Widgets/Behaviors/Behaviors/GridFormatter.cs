using DownUnder.UI.DataTypes;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors
{
    public class GridFormat : WidgetBehavior
    {
        private int width;
        private int height;

        public Widget Filler;

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
            GridWriter.Align(Parent.Children, width, height, Parent.Area.SizeOnly());
        }

        internal override void DisconnectFromParent()
        {
            throw new NotImplementedException();
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
            set => Parent.Children[y * width + x] = value;
        }
        public Point IndexOf(Widget widget) => GridReader.IndexOf(width, Parent.Children.IndexOf(widget));
    }
}
