using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class WidgetList : IList<Widget>
    {
        protected List<Widget> _widgets = new List<Widget>();
        
        public WidgetList() { }
        public WidgetList(List<Widget> widget_list)
        {
            foreach (Widget widget in widget_list)
            {
                ((IList<Widget>)_widgets).Add(widget);
            }
        }

        public Point2 MaxSize
        {
            get
            {
                Point2 max_size = new Point2();
                foreach (Widget widget in _widgets)
                {
                    max_size = max_size.Max(widget.Size);
                }
                return max_size;
            }
        }

        public RectangleF? AreaCoverage
        {
            get
            {
                if (Count == 0) return null;
                RectangleF result = _widgets[0].Area;
                for (int i = 1; i < _widgets.Count; i++)
                {
                    result = result.Union(_widgets[i].Area);
                }
                return result;
            }
        }

        public void ExpandAll(float modifier)
        {
            foreach (Widget widget in _widgets)
            {
                widget.Area = widget.Area.Resized(modifier);
            }
        }

        public void ExpandAll(Point2 modifier)
        {
            foreach (Widget widget in _widgets)
            {
                widget.Area = widget.Area.Resized(modifier);
            }
        }

        public List<Widget> ToList()
        {
            return _widgets;
        }

        #region IList Implementation

        public Widget this[int index]
        {
            get => ((IList<Widget>)_widgets)[index];
            set => ((IList<Widget>)_widgets)[index] = value;
        }

        public int Count => ((IList<Widget>)_widgets).Count;

        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;

        public static implicit operator List<Widget>(WidgetList v)
        {
            return v._widgets;
        }

        public void Add(Widget widget)
        {
            ((IList<Widget>)_widgets).Add(widget);
        }

        public void Clear()
        {
            ((IList<Widget>)_widgets).Clear();
        }

        public bool Contains(Widget widget)
        {
            return ((IList<Widget>)_widgets).Contains(widget);
        }

        public void CopyTo(Widget[] array, int array_index)
        {
            ((IList<Widget>)_widgets).CopyTo(array, array_index);
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return ((IList<Widget>)_widgets).GetEnumerator();
        }

        public int IndexOf(Widget widget)
        {
            return ((IList<Widget>)_widgets).IndexOf(widget);
        }

        public void Insert(int index, Widget widget)
        {
            ((IList<Widget>)_widgets).Insert(index, widget);
        }

        public bool Remove(Widget widget)
        {
            return ((IList<Widget>)_widgets).Remove(widget);
        }

        public void RemoveAt(int index)
        {
            ((IList<Widget>)_widgets).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Widget>)_widgets).GetEnumerator();
        }

        #endregion
    }
}
