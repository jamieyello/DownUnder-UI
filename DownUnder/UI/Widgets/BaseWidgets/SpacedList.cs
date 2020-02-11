using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    public class SpacedList : Widget, IList<Widget>
    {
        #region Private Fields

        private WidgetList _widgets = new WidgetList();
        private int _max_width_backing = 0;

        #endregion

        #region Public Properties

        /// <summary> How long a row is allowed to be before it displays the following widgets on the next row. If 0, there will be no limit. </summary>
        public int MaxRowWidth
        {
            get => _max_width_backing;
            set
            {
                _max_width_backing = value;
                UpdateArea(true);
            }
        }

        public float ListSpacing { get; set; } = 10f;

        public bool AlignEvenly { get; set; } = true;

        #endregion

        public SpacedList(Widget parent = null)
            : base(parent)
        {

        }

        public override RectangleF Area
        {
            get => base.Area;
            set
            {
                base.Area = value;
                AlignList(area_backing.Width);
            }
        }

        private void AlignList(float width)
        {
            Point2 size = new Point2();
            if (AlignEvenly)
            {
                foreach (Widget widget in _widgets)
                {
                    size = size.Max(widget.Size);
                }
            }
            
            Point2 current_position = new Point2(0, ListSpacing);
            int x = 0;
            foreach (Widget widget in _widgets)
            {
                current_position.X += ListSpacing;
                if (AlignEvenly)
                {
                    current_position.X += size.X;
                }
            }
        }

        protected override void UpdateArea(bool update_parent)
        {
            AlignList(Width);
            base.UpdateArea(update_parent);
        }

        public override WidgetList Children => _widgets;
        
        protected override object DerivedClone(Widget parent = null)
        {
            SpacedList c = new SpacedList(parent) { new SpacedList() };
            c.MaxRowWidth = MaxRowWidth;

            foreach (Widget widget in _widgets)
            {
                c._widgets.Add((Widget)widget.Clone(c));
            }

            return c;
        }

        #region IList Implementaion

        public int Count => ((IList<Widget>)_widgets).Count;

        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;

        public Widget this[int index]
        {
            get => ((IList<Widget>)_widgets)[index];
            set
            {
                ((IList<Widget>)_widgets)[index] = value;
                value.Parent = this;
                UpdateArea(true);
            }
        }

        public int IndexOf(Widget widget)
        {
            return ((IList<Widget>)_widgets).IndexOf(widget);
        }

        public void Insert(int index, Widget widget)
        {
            ((IList<Widget>)_widgets).Insert(index, widget);
            widget.Parent = this;
            UpdateArea(true);
        }

        public void RemoveAt(int index)
        {
            _widgets[index].Dispose();
            ((IList<Widget>)_widgets).RemoveAt(index);
            UpdateArea(true);
        }

        public void Add(Widget widget)
        {
            ((IList<Widget>)_widgets).Add(widget);
            widget.Parent = this;
            UpdateArea(true);
        }

        public void Clear()
        {
            foreach (Widget widget in _widgets)
            {
                widget.Dispose();
            }

            ((IList<Widget>)_widgets).Clear();
        }

        public bool Contains(Widget widget)
        {
            return ((IList<Widget>)_widgets).Contains(widget);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            ((IList<Widget>)_widgets).CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget widget)
        {
            widget.Dispose();
            return ((IList<Widget>)_widgets).Remove(widget);
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return ((IList<Widget>)_widgets).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Widget>)_widgets).GetEnumerator();
        }

        #endregion
    }
}
