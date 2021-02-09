using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    public sealed class ProxList {
        readonly List<ProxListPosition>[,] _groups;
        readonly List<ProxListPosition> _list_representation = new List<ProxListPosition>();

        public int Width { get; }
        public int Height { get; }
        public float InteractDiameter { get; }

        public object Current => throw new NotImplementedException();

        public ProxList(
            float interact_diameter,
            float width,
            float height
        ) {
            InteractDiameter = interact_diameter;
            Width = (int)(width / interact_diameter) + 5;
            Height = (int)(height / interact_diameter) + 5;
            _groups = new List<ProxListPosition>[Width, Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                _groups[x, y] = new List<ProxListPosition>();
        }

        public List<ProxListPosition> GetNeighbors(
            ProxListPosition item
        ) {
            var x = item.box.X;
            var y = item.box.Y;
            var result = new List<ProxListPosition>();

            var x_mod = 1;
            var y_mod = 1;
            if (item.Position.X < 0.5f) x_mod = -1;
            if (item.Position.Y < 0.5f) y_mod = -1;
            TryAddRange(result, x, y);
            TryAddRange(result, x + x_mod, y);
            TryAddRange(result, x, y + y_mod);
            TryAddRange(result, x + x_mod, y + y_mod);

            result.Remove(item);
            return result;
        }

        void TryAddRange(
            List<ProxListPosition> result,
            int x,
            int y
        ) {
            if (x > 0 && y > 0 && x < Width && y < Height)
                result.AddRange(_groups[x, y]);
        }

        //public Point2[] GetNeighborPositions()

        public ProxListPosition Add(Point2 position) =>
            new ProxListPosition(this, position);

        public void Remove(ProxListPosition item) {
            if (!_groups[item.box.X, item.box.Y].Remove(item))
                throw new Exception("Failed to remove given item.");
        }

        internal Point UpdateBox(ProxListPosition item, Point2 position) {
            var new_box = GetBox(position);
            if (new_box == item.box)
                return item.box;

            if (!_groups[item.box.X, item.box.Y].Remove(item))
                throw new Exception("Error updating position.");

            _groups[new_box.X, new_box.Y].Add(item);

            return new_box;
        }

        internal Point AddToArray(ProxListPosition item, Point2 position) {
            var new_box = GetBox(position);
            _groups[new_box.X, new_box.Y].Add(item);
            return new_box;
        }

        Point GetBox(Point2 position) =>
            new Point(
                (int)(position.X / InteractDiameter),
                (int)(position.Y / InteractDiameter)
            );
    }
}