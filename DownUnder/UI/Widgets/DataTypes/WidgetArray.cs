using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> A general interface for an array of Widgets. </summary>
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

        public void Align(RectangleF? new_area = null)
        {
            if (_widgets.Count == 0 || _widgets[0].Count == 0) return;
            if (new_area != null) SetSize(new_area.Value.Size);
            AutoSizeAllWidgets();
            AutoSpaceAllWidgets(new_area == null ? new Point2() : new_area.Value.Position);
        }

        public void SetSize(Point2 new_size)
        {
            Point2 original_size = new Point2(GetRow(0).CombinedWidth, GetColumn(0).CombinedHeight);
            Point2 fixed_size = FixedContentSizeTotal();
            Point2 modifier = new_size.DividedBy(original_size.WithOffset(fixed_size.Inverted()).WithMinValue(0.0001f));
            ExpandAll(modifier);
        }

        private void AutoSpaceAllWidgets(Point2 start)
        {
            Point2 position = start;
            
            for (int x = 0; x < _widgets.Count; x++)
            {
                position.Y = start.Y;
                for (int y = 0; y < _widgets[0].Count; y++)
                {
                    _widgets[x][y].Position = position;
                    position.Y += _widgets[x][y].Height;
                }

                position.X += _widgets[x][0].Width;
            }

            Console.WriteLine($"Start {start} AreaCoverage {AreaCoverage}");
        }
        
        /// <summary> This will find the longest/tallest widget in each row/collumn and make every other element match. </summary>
        private void AutoSizeAllWidgets()
        {
            for (int x = 0; x < _widgets.Count; x++)
            {
                GetColumn(x).AutoSizeWidth();
            }

            for (int y = 0; y < _widgets[0].Count; y++)
            {
                GetRow(y).AutoSizeHeight();
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

        public float InsertSpaceY(int row_index, float space)
        {
            float? space_y = null;
            for (int y = row_index; y < Dimensions.Y; y++)
            {
                foreach (Widget widget in GetRow(y))
                {
                    if (space_y == null) space_y = widget.Y;
                    widget.Y += space;
                }
            }

            return (float)space_y;
        }

        public float InsertSpaceX(int collumn, float space)
        {
            float? space_x = null;
            for (int x = collumn; x < Dimensions.Y; x++)
            {
                foreach (Widget widget in GetColumn(x))
                {
                    if (space_x == null) space_x = widget.X;
                    widget.X += space;
                }
            }

            return (float)space_x;
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

        /// <summary> The total height/width of contained widgets that won't resize. </summary>
        private Point2 FixedContentSizeTotal()
        {
            Point2 fixed_size = new Point2();

            for (int i = 0; i < Dimensions.X; i++)
            {
                float fixed_width = FixedWidthOfColumn(i);
                if (fixed_width != -1f)
                {
                    fixed_size.X += fixed_width;
                }
            }

            for (int i = 0; i < Dimensions.Y; i++)
            {
                float fixed_height = FixedHeightOfRow(i);
                if (fixed_height != -1f)
                {
                    fixed_size.Y += fixed_height;
                }
            }

            return fixed_size;
        }

        private float FixedHeightOfRow(int row)
        {
            for (int i = 0; i < Dimensions.X; i++)
            {
                if (_widgets[i][row].IsFixedHeight) return _widgets[i][row].Height;
            }

            return -1f;
        }

        private float FixedWidthOfColumn(int column)
        {
            for (int i = 0; i < Dimensions.Y; i++)
            {
                if (_widgets[column][i].IsFixedWidth) return _widgets[column][i].Width;
            }

            return -1f;
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
