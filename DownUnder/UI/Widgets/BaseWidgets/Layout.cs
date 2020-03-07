using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary> A collection of <see cref="Widget"/>s. </summary>
    [DataContract] public class Layout : Widget, IScrollableWidget, IList<Widget>
    {
        #region Private Fields

        private WidgetList _widgets = new WidgetList();
        
        #endregion Private Fields

        #region Constructors

        public Layout(IParent parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            PaletteUsage = BaseColorScheme.PaletteCategory.default_widget;
            if (IsGraphicsInitialized) InitializeScrollbars(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeScrollbars;
        }
        
        #endregion Constructors

        #region Properties

        public ScrollBars ScrollBars { get; private set; }
        
        public RectangleF ContentArea
        {
            get
            {
                RectangleF? result = _widgets.AreaCoverage?.Union(Area.SizeOnly());
                if (result == null) return Area.WithOffset(Scroll.ToPoint2().Inverted());
                return result.Value.WithOffset(Scroll.ToPoint2().Inverted());
            }
        }

        public Scroll Scroll { get; } = new Scroll();

        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        public bool FitToContentArea { get; set; } = false;

        #endregion Properties

        #region Overrides

        public override RectangleF Area
        {
            get => area_backing;
            set
            {
                base.Area = value;
                foreach (var child in Children)
                {
                    child.EmbedIn(area_backing);
                }
            }
        }

        /// <summary> Area of the <see cref="Widget"/> in the window (without the scroll offset). </summary>
        //public override RectangleF AreaInWindow
        //{
        //    get
        //    {
        //        var area = base.AreaInWindow;
        //        area.Offset(Scroll.ToVector2());
        //        return area;
        //    }
        //}

        public override WidgetList Children => _widgets;

        #region Events

        private void InitializeScrollbars(object sender, EventArgs args)
        {
            ScrollBars = new ScrollBars(this, GraphicsDevice);
        }

        #endregion

        protected override object DerivedClone(Widget parent = null)
        {
            Layout c = new Layout(parent);
            
            for (int i = 0; i < _widgets.Count; i++)
            {
                c._widgets.Add((Widget)_widgets[i].Clone(parent));
            }

            ((IScrollableWidget)c).FitToContentArea = FitToContentArea;

            return c;
        }

        #endregion Overrides

        #region IList Implementation

        public int Count => ((IList<Widget>)_widgets).Count;

        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;

        public Widget this[int index] { get => ((IList<Widget>)_widgets)[index]; set => ((IList<Widget>)_widgets)[index] = value; }

        public int IndexOf(Widget item)
        {
            return ((IList<Widget>)_widgets).IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            ((IList<Widget>)_widgets).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Widget>)_widgets).RemoveAt(index);
        }

        public void Add(Widget widget)
        {
            widget.Parent = this;
            widget.EmbedIn(Area);
            ((IList<Widget>)_widgets).Add(widget);
        }

        public void Clear()
        {
            ((IList<Widget>)_widgets).Clear();
        }

        public bool Contains(Widget item)
        {
            return ((IList<Widget>)_widgets).Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            ((IList<Widget>)_widgets).CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            return ((IList<Widget>)_widgets).Remove(item);
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