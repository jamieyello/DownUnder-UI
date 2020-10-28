using Microsoft.Xna.Framework;
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
        internal Point box { get; private set; }

        internal ProxListPosition(ProxList parent, Point2 position)
        {
            this.parent = parent;
            position_backing = position;
            box = parent.AddToArray(this, position);
        }

        public Point2 Position 
        {
            get => position_backing;
            set
            {
                position_backing = value;
                box = parent.UpdateBox(this, value);
            }
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
