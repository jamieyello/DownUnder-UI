using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class ProxList// : IEnumerable<ProxListPosition>
    {
        List<ProxListPosition>[,] groups;
        public readonly int Width;
        public readonly int _height;
        public readonly float InteractDiameter;

        public object Current => throw new NotImplementedException();

        public ProxList(float interact_diameter, float width, float height)
        {
            InteractDiameter = interact_diameter;
            Width = (int)(width / interact_diameter) + 1;
            _height = (int)(height / interact_diameter) + 1;
            groups = new List<ProxListPosition>[Width, _height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < _height; y++) groups[x, y] = new List<ProxListPosition>();
            }
        }

        public List<ProxListPosition> GetNeighbors(ProxListPosition item)
        {
            int x = (int)(item.Position.X / InteractDiameter - 0.5f);
            int y = (int)(item.Position.Y / InteractDiameter - 0.5f);
            List<ProxListPosition> result = new List<ProxListPosition>();

            if (x > 0 && y > 0 && x < Width && y < _height) result.AddRange(groups[x, y]);
            x++;
            if (x > 0 && y > 0 && x < Width && y < _height) result.AddRange(groups[x, y]);
            x--;
            y++;
            if (x > 0 && y > 0 && x < Width && y < _height) result.AddRange(groups[x, y]);
            x++;
            if (x > 0 && y > 0 && x < Width && y < _height) result.AddRange(groups[x, y]);
            
            return result;
        }

        public ProxListPosition Add(Point2 position)
        {
            int x = (int)position.X / (int)InteractDiameter;
            int y = (int)position.Y / (int)InteractDiameter;
            var result = new ProxListPosition(this, position);
            groups[x, y].Add(result);
            return groups[x, y][groups[x, y].Count - 1];
        }

        public void Remove(ProxListPosition item)
        {
            if (!groups[(int)(item.Position.X / InteractDiameter), (int)(item.Position.Y / InteractDiameter)].Remove(item)) throw new Exception("Failed to remove given item.");
        }

        internal Point2 UpdatePosition(ProxListPosition item, Point2 position)
        {
            if (position == item.Position) return position;

            int start_x = (int)item.Position.X / (int)InteractDiameter;
            int start_y = (int)item.Position.Y / (int)InteractDiameter;
            int new_x = (int)position.X / (int)InteractDiameter;
            int new_y = (int)position.Y / (int)InteractDiameter;

            if (start_x == new_x && start_y == new_y) return position;
            if (!groups[start_x, start_y].Remove(item)) throw new Exception("Error updating position.");
            groups[new_x, new_y].Add(item);

            return position;
        }

        internal void AddToArray(ProxListPosition item, Point2 position)
        {
            int new_x = (int)position.X / (int)InteractDiameter;
            int new_y = (int)position.Y / (int)InteractDiameter;
            groups[new_x, new_y].Add(item);
        }

        //public IEnumerator<ProxListPosition> GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    for (int x = 0; x < Width; x++)
        //    {
        //        for (int y = 0; y < _height; y++)
        //        {
        //            for (int i = 0; i < groups[x, y].Count; i++)
        //            {
        //                yield return groups[x, y][i];
        //            }
        //        }
        //    }
        //}
    }
}
