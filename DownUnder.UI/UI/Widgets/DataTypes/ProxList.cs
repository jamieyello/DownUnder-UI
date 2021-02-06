using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class ProxList
    {
        List<ProxListPosition>[,] groups;
        List<ProxListPosition> list_representation = new List<ProxListPosition>();
        public readonly int Width;
        public readonly int Height;
        public readonly float InteractDiameter;

        public object Current => throw new NotImplementedException();

        public ProxList(float interact_diameter, float width, float height)
        {
            InteractDiameter = interact_diameter;
            Width = (int)(width / interact_diameter) + 5;
            Height = (int)(height / interact_diameter) + 5;
            groups = new List<ProxListPosition>[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++) groups[x, y] = new List<ProxListPosition>();
            }
        }

        public List<ProxListPosition> GetNeighbors(ProxListPosition item)
        {
            int x = item.box.X;
            int y = item.box.Y;
            List<ProxListPosition> result = new List<ProxListPosition>();

            int x_mod = 1;
            int y_mod = 1;
            if (item.Position.X < 0.5f) x_mod = -1;
            if (item.Position.Y < 0.5f) y_mod = -1;
            TryAddRange(result, x, y);
            TryAddRange(result, x + x_mod, y);
            TryAddRange(result, x, y + y_mod);
            TryAddRange(result, x + x_mod, y + y_mod);

            result.Remove(item);

            return result;
        }

        void TryAddRange(List<ProxListPosition> result, int x, int y)
        {
            if (x > 0 && y > 0 && x < Width && y < Height) result.AddRange(groups[x, y]);
        }

        //public Point2[] GetNeighborPositions()

        public ProxListPosition Add(Point2 position)
        {
            return new ProxListPosition(this, position);
        }

        public void Remove(ProxListPosition item)
        {
            if (!groups[item.box.X, item.box.Y].Remove(item)) throw new Exception("Failed to remove given item.");
        }

        internal Point UpdateBox(ProxListPosition item, Point2 position)
        {
            Point new_box = GetBox(position);
            if (new_box == item.box) return item.box;

            if (!groups[item.box.X, item.box.Y].Remove(item)) throw new Exception("Error updating position.");
            groups[new_box.X, new_box.Y].Add(item);

            return new_box;
        }

        internal Point AddToArray(ProxListPosition item, Point2 position)
        {
            Point new_box = GetBox(position);
            groups[new_box.X, new_box.Y].Add(item);
            return new_box;
        }

        private Point GetBox(Point2 position)
        {
            return new Point(
                (int)(position.X / InteractDiameter),
                (int)(position.Y / InteractDiameter));
        }
    }
}
