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

        protected WidgetList _widgets = new WidgetList();
        
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

        public Scroll ScrollBars { get; private set; }
        
        public RectangleF ContentArea
        {
            get
            {
                RectangleF? result = _widgets.AreaCoverage?.Union(Area.SizeOnly());
                if (result == null) return Area.WithOffset(Scroll);
                return result.Value.WithOffset(Scroll);
            }
        }

        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        public bool FitToContentArea { get; set; } = false;

        public bool EmbedChildren { get; set; } = true;

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
                    if (EmbedChildren) child.EmbedIn(area_backing);
                }
            }
        }

        public override WidgetList Children => _widgets;

        #region Events/EventHandlers

        public event EventHandler OnListChange;

        private void InitializeScrollbars(object sender, EventArgs args)
        {
            ScrollBars = new Scroll(this, GraphicsDevice);
        }

        #endregion

        protected override object DerivedClone()
        {
            Layout c = new Layout();
            
            for (int i = 0; i < _widgets.Count; i++)
            {
                c._widgets.Add((Widget)_widgets[i].Clone());
            }

            ((IScrollableWidget)c).FitToContentArea = FitToContentArea;

            return c;
        }

        protected override void HandleChildRemoval(Widget widget)
        {
            Widget child = _widgets[_widgets.IndexOf(widget)];
            child.Dispose();
            _widgets.Remove(child);
        }

        #endregion Overrides

        #region IList Implementation

        public int Count => ((IList<Widget>)_widgets).Count;

        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;

        public Point2 Scroll => ScrollBars.ToPoint2().Inverted();

        public Widget this[int index] { get => ((IList<Widget>)_widgets)[index]; set => ((IList<Widget>)_widgets)[index] = value; }

        public int IndexOf(Widget item)
        {
            return ((IList<Widget>)_widgets).IndexOf(item);
        }

        public void Insert(int index, Widget widget)
        {
            widget.Parent = this;
            if (EmbedChildren) widget.EmbedIn(Area);
            ((IList<Widget>)_widgets).Insert(index, widget);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveAt(int index)
        {
            ((IList<Widget>)_widgets).RemoveAt(index);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void Add(Widget widget)
        {
            widget.Parent = this;
            if (EmbedChildren) widget.EmbedIn(Area);
            ((IList<Widget>)_widgets).Add(widget);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            bool invoke = (Count != 0);
            ((IList<Widget>)_widgets).Clear();
            if (invoke) OnListChange?.Invoke(this, EventArgs.Empty);
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
            if (((IList<Widget>)_widgets).Remove(item))
            {
                OnListChange?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
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