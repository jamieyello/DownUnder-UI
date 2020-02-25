using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary> A collection of widgets. </summary>
    [DataContract]
    public class Layout : Widget, IScrollableWidget
    {
        #region Private Fields

        [DataMember] private WidgetList _widgets = new WidgetList();
        
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

        #region Public Methods

        public void AddWidget(Widget widget)
        {
            widget.Parent = this;
            widget.EmbedIn(Area);
            _widgets.Add(widget);
        }

        #endregion Public Methods

        #region Properties

        public ScrollBars ScrollBars { get; private set; }
        
        public RectangleF ContentArea
        {
            get
            {
                RectangleF? result = _widgets.AreaCoverage;
                if (result == null) return Area;
                return ((RectangleF)result).Union(Area);
            }
        }

        public Scroll Scroll { get; } = new Scroll();

        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        public bool FitToContentArea { get; set; } = false;

        #endregion Properties

        #region Overrides

        public override RectangleF Area
        {
            get => area_backing.WithOffset(-Scroll.ToVector2());
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
        public override RectangleF AreaInWindow
        {
            get
            {
                var area = base.AreaInWindow;
                area.Offset(Scroll.ToVector2());
                return area;
            }
        }

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
    }
}