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
    /// <summary> A class to make interfacing with a List<Widget> easier. </summary>
    public class WidgetList : IList<Widget>
    {
        protected List<Widget> Widgets { get; set; } = new List<Widget>();
        
        public WidgetList() { }
        public WidgetList(List<Widget> widget_list)
        {
            foreach (Widget widget in widget_list)
            {
                ((IList<Widget>)Widgets).Add(widget);
            }
        }

        public Point2 MaxSize
        {
            get
            {
                Point2 max_size = new Point2();
                foreach (Widget widget in Widgets)
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
                RectangleF result = Widgets[0].Area;
                for (int i = 1; i < Widgets.Count; i++)
                {
                    result = result.Union(Widgets[i].Area);
                }
                return result;
            }
        }

        public float CombinedHeight
        {
            get
            {
                float result = 0f;
                foreach (Widget widget in Widgets)
                {
                    result += widget.Height;
                }
                return result;
            }
        }

        public float CombinedWidth
        {
            get
            {
                float result = 0f;
                foreach (Widget widget in Widgets)
                {
                    result += widget.Width;
                }
                return result;
            }
        }

        public void ExpandAll(float modifier)
        {
            foreach (Widget widget in Widgets)
            {
                widget.Area = widget.Area.Resized(modifier);
            }
        }

        public void ExpandAll(Point2 modifier)
        {
            foreach (Widget widget in Widgets)
            {
                widget.Area = widget.Area.Resized(modifier);
            }
        }

        /// <summary> Set all the widget's width values to match each other. </summary>
        public void AutoSizeWidth()
        {
            float new_width = MaxSize.X;
            
            foreach (Widget widget in Widgets)
            {
                if (widget.IsFixedWidth)
                {
                    new_width = widget.Width;
                    break;
                }
            }

            SetAllWidth(new_width);
        }

        /// <summary> Set all the widget's height values to match each other. </summary>
        public void AutoSizeHeight()
        {
            float new_height = MaxSize.Y;

            foreach (Widget widget in Widgets)
            {
                if (widget.IsFixedHeight)
                {
                    new_height = widget.Height;
                    break;
                }
            }

            SetAllHeight(new_height);
        }

        public void SetAllWidth(float width)
        {
            foreach (Widget widget in Widgets)
            {
                widget.Width = width;
            }
        }

        public void SetAllHeight(float height)
        {
            foreach (Widget widget in Widgets)
            {
                widget.Height = height;
            }
        }

        public List<Widget> ToList()
        {
            return Widgets;
        }

        #region IList Implementation

        public Widget this[int index]
        {
            get => ((IList<Widget>)Widgets)[index];
            set => ((IList<Widget>)Widgets)[index] = value;
        }

        public int Count => ((IList<Widget>)Widgets).Count;

        public bool IsReadOnly => ((IList<Widget>)Widgets).IsReadOnly;
        
        public static implicit operator List<Widget>(WidgetList v)
        {
            return v.Widgets;
        }

        public void Add(Widget widget)
        {
            ((IList<Widget>)Widgets).Add(widget);
        }

        public void Clear()
        {
            ((IList<Widget>)Widgets).Clear();
        }

        public bool Contains(Widget widget)
        {
            return ((IList<Widget>)Widgets).Contains(widget);
        }

        public void CopyTo(Widget[] array, int array_index)
        {
            ((IList<Widget>)Widgets).CopyTo(array, array_index);
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return ((IList<Widget>)Widgets).GetEnumerator();
        }

        public int IndexOf(Widget widget)
        {
            return ((IList<Widget>)Widgets).IndexOf(widget);
        }

        public void Insert(int index, Widget widget)
        {
            ((IList<Widget>)Widgets).Insert(index, widget);
        }

        public bool Remove(Widget widget)
        {
            return ((IList<Widget>)Widgets).Remove(widget);
        }

        public void RemoveAt(int index)
        {
            ((IList<Widget>)Widgets).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Widget>)Widgets).GetEnumerator();
        }

        internal void AddRange(WidgetList allContainedWidgets)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
