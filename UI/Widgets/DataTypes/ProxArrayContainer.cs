using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class ProxListPosition
    {
        ProxList parent;
        Point2 position_backing;

        internal ProxListPosition(ProxList parent, Point2 position)
        {
            this.parent = parent;
            position_backing = position;
            parent.AddToArray(this, position);
        }

        public Point2 Position 
        {
            get => position_backing;
            set => position_backing = parent.UpdatePosition(this, value);
        }

        public float X
        {
            get => position_backing.X;
            set => Position = new Point2(value, position_backing.Y);
        }

        public float Y
        {
            get => position_backing.Y;
            set => Position = new Point2(position_backing.Y, value);
        }
    }
}
