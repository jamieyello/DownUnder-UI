using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    class ProxArrayContainer<T>
    {
        ProximityArray<T> parent;
        Point2 position_backing;
        public readonly T Item;

        public ProxArrayContainer(ProximityArray<T> parent, T item, Point2 position)
        {
            this.parent = parent;
            Item = item;
            Position = position;
        }

        public Point2 Position 
        {
            get => position_backing;
            set => position_backing = parent.UpdatePosition(this, value);
        }
    }
}
