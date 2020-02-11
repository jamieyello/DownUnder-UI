using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class WidgetArray : IList<WidgetList>
    {
        private readonly List<WidgetList> _widgets = new List<WidgetList>();

        public Point Dimensions
        {
            get => _widgets.Count == 0 || _widgets[0].Count == 0 ?
                new Point() :
                new Point(_widgets.Count, _widgets[0].Count);
        }

        public RectangleF? AreaCoverage
        {
            get
            {
                if (Dimensions.X == 0 || Dimensions.Y == 0) return null;
                
                RectangleF result = (RectangleF)_widgets[0].AreaCoverage;

                for (int i = 1; i < Dimensions.X; i++)
                {
                    result = result.Union((RectangleF)_widgets[i].AreaCoverage);
                }

                return result;
            }
        }

        public WidgetArray() : base() { }
        public WidgetArray(int x_length, int y_length, Widget filler)
        {
            for (int x = 0; x < x_length; x++)
            {
                _widgets.Add(new WidgetList());
                for (int y = 0; y < y_length; y++)
                {
                    object clone = (Widget)filler.Clone();
                    _widgets[x].Add((Widget)clone);
                }
            }
        }
        public WidgetArray(List<List<Widget>> widget_array)
        {
            if (widget_array.Count == 0 || widget_array[0].Count == 0) return;

            if (IsEven(widget_array)) throw new Exception("Given array is jagged and cannot be used.");

            for (int x = 0; x < widget_array.Count; x++)
            {
                for (int y = 0; y < widget_array[x].Count; x++)
                {
                    _widgets[x][y] = widget_array[x][y];
                }
            }
        }

        public void ExpandAll(float modifier)
        {
            foreach (WidgetList list in _widgets)
            {
                list.ExpandAll(modifier);
            }
        }

        public void ExpandAll(Point2 modifier)
        {
            foreach (WidgetList list in _widgets)
            {
                list.ExpandAll(modifier);
            }
        }

        public Point IndexOf(Widget widget)
        {
            int index;
            for (int x = 0; x < Dimensions.X; x++)
            {
                index = _widgets[x].IndexOf(widget);
                if (index != -1) return new Point(x, index);
            }
            return new Point(-1, -1);
        }

        public WidgetList ToWidgetList()
        {
            WidgetList result = new WidgetList();

            foreach (WidgetList widget_list in _widgets)
            {
                ((List<Widget>)result).AddRange(widget_list);
            }

            return result;
        }

        /// <summary> Returns true if the given jagged array has no length mismatches. </summary>
        private static bool IsEven<T>(List<List<T>> jagged_array)
        {
            if (jagged_array.Count == 0 || jagged_array[0].Count == 0) return false;
            
            int expected_y_count = jagged_array[0].Count;
            for (int x = 0; x < jagged_array.Count; x++)
            {
                if (jagged_array[x].Count != expected_y_count) return true;
            }
            
            return false;
        }
        


        #region IList Implementation

        public int Count => _widgets.Count;

        public bool IsReadOnly => ((IList<WidgetList>)_widgets).IsReadOnly;

        public WidgetList this[int index]
        {
            get => _widgets[index];
            set => _widgets[index] = value;
        }
        
        public int IndexOf(WidgetList item)
        {
            return _widgets.IndexOf(item);
        }

        public void Insert(int index, WidgetList item)
        {
            _widgets.Insert(index, item);
        }

        void IList<WidgetList>.RemoveAt(int index)
        {
            _widgets.RemoveAt(index);
        }

        void ICollection<WidgetList>.Add(WidgetList item)
        {
            _widgets.Add(item);
        }

        public void Clear()
        {
            _widgets.Clear();
        }

        public bool Contains(WidgetList item)
        {
            return _widgets.Contains(item);
        }
        
        public WidgetList GetColumn(int x)
        {
            return new WidgetList(_widgets[x]);
        }

        public WidgetList GetRow(int y)
        {
            WidgetList result = new WidgetList();

            for (int x = 0; x < Dimensions.X; x++)
            {
                result.Add(_widgets[x][y]);
            }

            return result;
        }

        public void CopyTo(WidgetList[] array, int arrayIndex)
        {
            _widgets.CopyTo(array, arrayIndex);
        }

        public bool Remove(WidgetList item)
        {
            return _widgets.Remove(item);
        }

        public IEnumerator<WidgetList> GetEnumerator()
        {
            return ((IList<WidgetList>)_widgets).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<WidgetList>)_widgets).GetEnumerator();
        }

        #endregion
    }
}
