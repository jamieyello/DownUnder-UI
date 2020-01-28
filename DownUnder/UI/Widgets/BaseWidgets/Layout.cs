using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary>
    /// A collection of widgets.
    /// </summary>
    [DataContract]
    public class Layout : Widget, IScrollableWidget
    {
        #region Private Fields

        [DataMember] private List<Widget> widgets = new List<Widget>();
        
        #endregion Private Fields

        #region Constructors

        public Layout(IWidgetParent parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            PaletteUsage = BaseColorScheme.PaletteCatagory.default_;
            if (IsGraphicsInitialized) InitializeScrollbars(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeScrollbars;
        }
        
        #endregion Constructors

        #region Public Methods

        public void AddWidget(Widget widget)
        {
            widget.SetOwnership(this);
            widget.EmbedIn(Area);
            widgets.Add(widget);
        }

        #endregion Public Methods

        #region Properties

        public ScrollBars ScrollBars { get; private set; }
        
        public RectangleF ContentArea
        {
            get
            {
                RectangleF result = Area;
                result.Position = new Point2(0, 0);
                foreach (var child in Children)
                {
                    result = result.Union(child.Area);
                }
                return result;
            }
        }

        public Scroll Scroll { get; } = new Scroll();

        /// <summary> When set to true this widget will try to resize itself to contain all content. </summary>
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
                    child.EmbedIn(value);
                }
            }
        }

        /// <summary> Area of the widget in the window the scroll offset. </summary>
        public override RectangleF AreaInWindow
        {
            get
            {
                var area = base.AreaInWindow;
                area.Offset(Scroll.ToVector2());
                return area;
            }
        }

        public override List<Widget> Children => widgets;
        
        protected override void UpdateArea(bool update_parent)
        {
            RectangleF result = new RectangleF();
            foreach (var child in Children)
            {
                result = result.Union(child.Area);
            }
            base.UpdateArea(update_parent);
        }

        #region Events
        
        private void InitializeScrollbars(object sender, EventArgs args)
        {
            ScrollBars = new ScrollBars(this, GraphicsDevice);
        }

        #endregion

        protected override object DerivedClone()
        {
            Layout c = new Layout();
            
            for (int i = 0; i < widgets.Count; i++)
            {
                c.widgets.Add((Widget)widgets[i].Clone());
            }

            ((IScrollableWidget)c).FitToContentArea = FitToContentArea;

            return c;
        }
        
        #endregion Overrides
    }
}