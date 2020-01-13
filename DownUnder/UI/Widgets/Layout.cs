using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets
{
    /// <summary>
    /// A collection of widgets.
    /// </summary>
    [DataContract]
    public class Layout : Widget, IScrollableWidget
    {
        #region Private Fields

        [DataMember] public List<Widget> widgets = new List<Widget>();
        
        #endregion Private Fields

        #region Constructors

        public Layout(IWidgetParent parent = null)
            : base(parent)
        {
            if (parent is DWindow)
            {
                Size = ((DWindow)parent).GraphicsDevice.Viewport.Bounds.Size;
            }
            else
            {
                Size = new Point2(100, 100);
            }

            SetDefaults();
            if (IsGraphicsInitialized) InitializeGraphics();
        }

        private void SetDefaults()
        {
            Size = new Point2(100, 100);
            OnGraphicsInitialized += InitializeScrollbars;
        }
        
        #endregion Constructors

        #region Public Methods

        public void AddWidget(Widget widget)
        {
            widget.InitializeAllReferences(_parent_window_reference, this);
            widget.EmbedIn(Area);
            widget.SetOwnership(this);
            widgets.Add(widget);
        }

        #endregion Public Methods

        #region Properties

        public ScrollBars ScrollBars { get; private set; }

        /// <summary>
        /// How wide this widget's content is.
        /// </summary>
        public float ContentWidth { get => ContentArea.Width; set => ContentArea = new RectangleF(ContentArea.X, ContentArea.Y, value, ContentArea.Height); }

        /// <summary>
        /// How tall this widget's content is.
        /// </summary>
        public float ContentHeight { get => ContentArea.Height; set => ContentArea = new RectangleF(ContentArea.X, ContentArea.Y, ContentArea.Width, value); }

        /// <summary>
        /// The position of this widget's content (relative to the parent widget/window).
        /// </summary>
        public Point2 ContentPosition { get => ContentArea.Position; set => ContentArea = new RectangleF(value, ContentArea.Size); }

        /// <summary>
        /// The overall size of this widget's content.
        /// </summary>
        public Point2 ContentSize { get => ContentArea.Size; set => ContentArea = new RectangleF(ContentArea.Position, value); }

        /// <summary>
        /// The X position of this widget's content relative to the parent widget (or window).
        /// </summary>
        public float ContentX { get => ContentArea.X; set => ContentArea = new RectangleF(value, ContentArea.Y, ContentArea.Width, ContentArea.Height); }

        /// <summary>
        /// The Y position of this widget's content relative to the parent widget (or window).
        /// </summary>
        public float ContentY { get => ContentArea.Y; set => ContentArea = new RectangleF(ContentArea.X, value, ContentArea.Width, ContentArea.Height); }

        // Area of this widget
        public RectangleF ContentArea
        {
            get
            {
                //if (Name == "test_layout") { Debug.WriteLine($"Getting ContentArea, child count = {widgets.Count}"); }
                int db = 0;
                RectangleF result = Area;
                result.Position = new Point2(0, 0);
                foreach (var child in Children)
                {
                    //Debug.WriteLine($"child[{db}].Area = {child.Area}");
                    db++;
                    result = result.Union(child.Area);
                }
                return result;
            }
            set { }
        }

        public Scroll Scroll { get; } = new Scroll();

        #endregion Properties

        #region Overrides

        public override RectangleF Area
        {
            get
            {
                var result = area_backing;
                result.Offset(-Scroll.ToVector2());
                return result;
            }
            set
            {
                //value.Offset(Scroll.ToVector2());
                base.Area = value;
                foreach (var child in Children)
                {
                    child.EmbedIn(value);
                }
            }
        }

        /// <summary>
        /// Area of the widget in the window the scroll offset.
        /// </summary>
        public override RectangleF AreaInWindow
        {
            get
            {
                var area = base.AreaInWindow;
                area.Offset(Scroll.ToVector2());
                return area;
            }
        }

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

            return c;
        }

        public override List<Widget> Children
        {
            get => widgets;
        }

        #endregion Overrides
    }
}