using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    class ProximityArray<T> : IEnumerable<ProxArrayContainer<T>>
    {
        List<ProxArrayContainer<T>>[,] groups;
        int _width;
        int _height;
        float _interact_diameter;

        public object Current => throw new NotImplementedException();

        public ProximityArray(float interact_diameter, float width, float height)
        {
            _interact_diameter = interact_diameter;
            _width = (int)(width / interact_diameter) + 1;
            _height = (int)(height / interact_diameter) + 1;
            groups = new List<ProxArrayContainer<T>>[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++) groups[x, y] = new List<ProxArrayContainer<T>>();
            }
        }

        public List<ProxArrayContainer<T>> GetNeighbors(ProxArrayContainer<T> item)
        {
            int x = (int)(item.Position.X / _interact_diameter);
            int y = (int)(item.Position.Y / _interact_diameter);
            List<ProxArrayContainer<T>> result = new List<ProxArrayContainer<T>>();

            if (x > 0 && y > 0 && x < _width && y < _height) result.AddRange(groups[x, y]);
            x++;
            if (x > 0 && y > 0 && x < _width && y < _height) result.AddRange(groups[x, y]);
            x--;
            y++;
            if (x > 0 && y > 0 && x < _width && y < _height) result.AddRange(groups[x, y]);
            x++;
            if (x > 0 && y > 0 && x < _width && y < _height) result.AddRange(groups[x, y]);
            
            return result;
        }

        public ProxArrayContainer<T> Add(T item, Point2 position)
        {
            int x = (int)(position.X / _interact_diameter);
            int y = (int)(position.Y / _interact_diameter);
            groups[x, y].Add(new ProxArrayContainer<T>(this, item, position));
            return groups[x, y][groups[x, y].Count - 1];
        }

        public void Remove(ProxArrayContainer<T> item)
        {
            if (!groups[(int)(item.Position.X / _interact_diameter), (int)(item.Position.Y / _interact_diameter)].Remove(item)) throw new Exception("Failed to remove given item.");
        }

        internal Point2 UpdatePosition(ProxArrayContainer<T> item, Point2 position)
        {
            if (position == item.Position) return position;

            int start_x = (int)(item.Position.X / _interact_diameter);
            int start_y = (int)(item.Position.Y / _interact_diameter);
            int new_x = (int)(position.X / _interact_diameter);
            int new_y = (int)(position.Y / _interact_diameter);

            if (start_x == new_x && start_y == new_y) return position;
            if (!groups[start_x, start_y].Remove(item)) throw new Exception("Error updating position.");
            groups[new_x, new_y].Add(item);

            return position;
        }

        public IEnumerator<ProxArrayContainer<T>> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int i = 0; i < groups[x, y].Count; i++)
                    {
                        yield return groups[x, y][i];
                    }
                }
            }
        }
    }
}
