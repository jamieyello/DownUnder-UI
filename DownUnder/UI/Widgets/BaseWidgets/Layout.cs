using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetElements;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary> A collection of <see cref="Widget"/>s. </summary>
    [DataContract] public class Layout : Widget, IScrollableWidget, IList<Widget> {
        protected WidgetList _widgets = new WidgetList();
        public event EventHandler OnListChange;
        public event EventHandler OnAddWidget;
        public event EventHandler OnRemoveWidget;

        public Layout(IParent parent = null) : base(parent) => SetDefaults();
        private void SetDefaults() {
            PaletteUsage = BaseColorScheme.PaletteCategory.default_widget;
            if (IsGraphicsInitialized) InitializeScrollbars(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeScrollbars;
        }

        public Scroll ScrollBars { get; private set; }
        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        [DataMember] public bool FitToContentArea { get; set; } = false;
        [DataMember] public bool EmbedChildren { get; set; } = true;
        public Widget LastAddedWidget { get; private set; }
        public Widget LastRemovedWidget { get; private set; }

        public RectangleF ContentArea {
            get {
                RectangleF? result = _widgets.AreaCoverage?.Union(Area.SizeOnly());
                if (result == null) return Area.WithOffset(Scroll);
                return result.Value.WithOffset(Scroll);
            }
        }

        [DataMember] public override RectangleF Area {
            get => area_backing;
            set {
                base.Area = value;
                foreach (var child in Children) {
                    if (EmbedChildren) child.EmbedIn(area_backing);
                }
            }
        }

        public override WidgetList Children => _widgets;
        
        private void InitializeScrollbars(object sender, EventArgs args) => ScrollBars = new Scroll(this, GraphicsDevice);
        
        protected override object DerivedClone() {
            Layout c = new Layout();
            for (int i = 0; i < _widgets.Count; i++) c._widgets.Add((Widget)_widgets[i].Clone());
            ((IScrollableWidget)c).FitToContentArea = FitToContentArea;
            return c;
        }

        protected override void HandleChildDelete(Widget widget) {
            widget.Dispose();
            _widgets.Remove(widget);
        }

        public int Count => ((IList<Widget>)_widgets).Count; 
        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;
        public Point2 Scroll => ScrollBars.ToPoint2().Inverted();
        public Widget this[int index] { get => ((IList<Widget>)_widgets)[index]; set => ((IList<Widget>)_widgets)[index] = value; }
        public int IndexOf(Widget item) => ((IList<Widget>)_widgets).IndexOf(item);
        public void CopyTo(Widget[] array, int arrayIndex) => ((IList<Widget>)_widgets).CopyTo(array, arrayIndex);
        public bool Contains(Widget item) => ((IList<Widget>)_widgets).Contains(item);
        public IEnumerator<Widget> GetEnumerator() => ((IList<Widget>)_widgets).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IList<Widget>)_widgets).GetEnumerator();

        public void Insert(int index, Widget widget) {
            widget.Parent = this;
            if (EmbedChildren) widget.EmbedIn(Area);
            ((IList<Widget>)_widgets).Insert(index, widget);
            LastAddedWidget = widget;
            OnAddWidget?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveAt(int index) {
            ((IList<Widget>)_widgets).RemoveAt(index);
            OnRemoveWidget?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void Add(Widget widget) {
            widget.Parent = this;
            if (EmbedChildren) widget.EmbedIn(Area);
            ((IList<Widget>)_widgets).Add(widget);
            LastAddedWidget = widget;
            OnAddWidget?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public void Clear() {
            for (int i = _widgets.Count; i >= 0; i--) Remove(_widgets[0]);
        }

        public bool Remove(Widget widget) {
            if (((IList<Widget>)_widgets).Remove(widget)) {
                OnRemoveWidget?.Invoke(this, EventArgs.Empty);
                OnListChange?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }
    }
}