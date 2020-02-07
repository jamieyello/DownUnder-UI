using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

// tip;
// https://social.msdn.microsoft.com/Forums/en-US/08c5860e-1a04-40bd-9706-41d2a03066d3/expandcollapse-regions-all-at-once?forum=csharpide
//      Ctrl-M, Ctrl-O will collapse all of the code to its definitions.
//      Ctrl-M, Ctrl-L will expand all of the code (actually, this one toggles it).
//      Ctrl-M, Ctrl-M will expand or collapse a single region

// tip: Always remember to update Clone.

// Combine slots with widget.
// Palettes should have ChangeColorOnHover, functionality should be removed from here.
// eventually revert old drawing code

namespace DownUnder.UI.Widgets
{
    /// <summary> A visible window object. </summary>
    [DataContract] public abstract class Widget : IWidgetParent
    {
        public bool debug_output = false;

        #region Fields/Delegates/Enums

        /// <summary> Used by some drawing code. </summary>
        private Texture2D _white_dot;

        /// <summary> The render target this Widget uses to draw to. </summary>
        private RenderTarget2D _render_target;

        /// <summary> The primary cursor press of the previous frame. (Used to trigger events on the single frame of a press) </summary>
        private bool _previous_clicking;
        
        /// <summary> Used to track the period of time where a second click would be considered a double click. (If this value is > 0) </summary>
        private float _double_click_countdown = 0f;

        /// <summary> Used to track the period of time where a third click would be considered a triple click. (If this value is > 0) </summary>
        private float _triple_click_countdown = 0f;
        
        private Point2 _previous_cursor_position = new Point2();

        /// <summary> Set to true internally to prevent usage of graphics while modifying them on another thread. </summary>
        private bool _graphics_updating = false;

        /// <summary> Set to true internally to prevent multi-threaded changes to graphics while their being used. </summary>
        private bool _graphics_in_use = false;

        /// <summary> The maximum size of a widget. (Every Widget uses a RenderTarget2D to render its contents to, this is the maximum resolution imposed by that.) </summary>
        private const int _MAXIMUM_WIDGET_SIZE = 2048;

        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        private const int _WAIT_TIME = 5;

        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        private const int _MAX_WAIT_TIME = 100;

        // The following are used by Update()/UpdatePriority().
        // They are set to true or false in UpdatePriority(), and Update() invokes events
        // by reading them.
        private bool _update_clicked;
        private bool _update_double_clicked;
        private bool _update_triple_clicked;
        private bool _update_added_to_focused;
        private bool _update_set_as_focused;
        private bool _update_hovered_over;

        // Various property backing fields.
        protected RectangleF area_backing = new RectangleF();
        private float _double_click_timing_backing = 0.5f;
        private Point2 _minimum_area_backing = new Point2(1f, 1f);
        private SpriteFont _sprite_font_backing;
        private GraphicsDevice _graphics_backing;
        private bool _is_hovered_over_backing;
        private BaseColorScheme _theme_backing;
        private Widget _parent_widget_backing;
        private DWindow _parent_window_backing;

        #endregion

        #region Public/Internal Properties

        #region Auto properties

        /// <summary> The unique name of this widget. </summary>
        [DataMember] public string Name { get; set; }

        /// <summary> If set to true, colors will shift to their hovered colors on mouse-over. </summary>
        [DataMember] public bool ChangeColorOnMouseOver { get; set; } = true;

        /// <summary> If set to false, the background color will not be drawn. </summary>
        [DataMember] public virtual bool DrawBackground { get; set; } = true;

        /// <summary> If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides) </summary>
        [DataMember] public bool DrawOutline { get; set; } = true;

        /// <summary> How thick the outline (if DrawOutline == true) should be. </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;

        /// <summary> Which sides (of the outline) are drawn (top, bottom, left, right) if (DrawOutLine == true). </summary>
        [DataMember] public Directions2D OutlineSides { get; set; } = Directions2D.UpDownLeftRight;

        /// <summary> Represents the corners this widget will snap to within the parent. </summary>
        [DataMember] public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.TopRight_BottomLeft_TopLeft_BottomRight;

        /// <summary> Unimplemented. </summary>
        [DataMember] public bool Centered { get; set; }

        /// <summary> Should any SnappingPolicy be defined, this represents the distance from the edges of the widget this is snapped to. </summary>
        [DataMember] public Size2 Spacing { get; set; }

        /// <summary> When set to true pressing enter while this widget is the primarily selected one will trigger confirmation events. </summary>
        [DataMember] public virtual bool EnterConfirms { get; set; } = true;

        /// <summary> What this widget should be regarded as when accessing the theme's defined colors. </summary>
        [DataMember] public BaseColorScheme.PaletteCategory PaletteUsage { get; set; } = BaseColorScheme.PaletteCategory.default_widget;

        /// <summary> While set to true this widget will lock its current width. </summary>
        [DataMember] public bool IsFixedWidth { get; set; } = false;

        /// <summary> While set to true this widget will lock its current height. </summary>
        [DataMember] public bool IsFixedHeight { get; set; } = false;

        /// <summary> Contains all information relevant to updating on this frame. </summary>
        public UpdateData UpdateData { get; set; } = new UpdateData();

        /// <summary> The local sprite batch used by this widget. </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        #endregion

        #region Non-auto properties

        /// <summary> Minimum time (in seconds) in-between two clicks needed for a double. </summary>
        [DataMember] public float DoubleClickTiming
        {
            get => _double_click_timing_backing;
            set
            {
                if (value > 0) { _double_click_timing_backing = value; }
                else { _double_click_timing_backing = 0f; }
            }
        }

        /// <summary> Area of this widget. (Position relative to parent widget, if any) </summary>
        [DataMember] public virtual RectangleF Area
        {
            get => area_backing;
            set
            {
                if (debug_output) Console.WriteLine($"Base area set; value = {value}");

                if (IsFixedWidth) value.Width = area_backing.Width;
                if (IsFixedHeight) value.Height = area_backing.Height;
                area_backing = value.WithMinimumSize(MinimumSize);
            }
        }

        /// <summary> Minimum size allowed when setting this widget's area. (in terms of pixels on a 1080p monitor) </summary>
        [DataMember] public virtual Point2 MinimumSize
        {
            get => _minimum_area_backing;
            set
            {
                if (value.X < 1)
                {
                    throw new Exception("Minimum area width must be at least 1.");
                }
                if (value.Y < 1)
                {
                    throw new Exception("Minimum area height must be at least 1.");
                }
                _minimum_area_backing = value;

                if (Area.WithMinimumSize(value) != Area) Area = Area.WithMinimumSize(value);
            }
        }

        /// <summary> The color palette of this widget. </summary>
        [DataMember] public BaseColorScheme Theme
        {
            get => _theme_backing;
            set
            {
                _theme_backing = value;
                if (_theme_backing != null)
                {
                    _theme_backing.Parent = this;
                }
            }
        }

        #endregion

        #region Derivatives of previous properties

        /// <summary> The width of this widget (in terms of pixels on a 1080p monitor) </summary>
        public float Width { get => Area.Width; set => Area = Area.WithWidth(value); }
        /// <summary> The height of this widget (in terms of pixels on a 1080p monitor) </summary>
        public float Height { get => Area.Height; set => Area = Area.WithHeight(value); }
        /// <summary> The position of this widget in its parent (in terms of pixels on a 1080p monitor) </summary>
        public Point2 Position { get => Area.Position; set => Area = Area.WithPosition(value); }
        /// <summary> The size of this widget (in terms of pixels on a 1080p monitor) </summary>
        public Point2 Size { get => Area.Size; set => Area = Area.WithSize(value); }
        /// <summary> The x position of this widget in its parent (in terms of pixels on a 1080p monitor) </summary>
        public float X { get => Area.X; set => Area = Area.WithX(value); }
        /// <summary> The y position of this widget in its parent (in terms of pixels on a 1080p monitor) </summary>
        public float Y { get => Area.Y; set => Area = Area.WithY(value); }
        /// <summary> Minimum height allowed when setting this widget's area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumHeight { get => MinimumSize.Y; set => MinimumSize = MinimumSize.WithY(value); }
        /// <summary> Minimum width allowed when setting this widget's area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumWidth { get => MinimumSize.X; set => MinimumSize = MinimumSize.WithX(value); }

        #endregion

        #region Other various non-serialized properties

        /// <summary> Returns true if this widget is hovered over. </summary>
        public bool IsHoveredOver
        {
            get => _is_hovered_over_backing;
            protected set
            {
                _is_hovered_over_backing = value;
                if (value)
                {
                    OnHover?.Invoke(this, EventArgs.Empty);
                }

                if (!value)
                {
                    OnHoverOff?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary> Returns true if this widget is being hovered over as well as on top of all the other widgets being hovered over. </summary>
        public bool IsPrimaryHovered => ParentWindow == null ? false : ParentWindow.HoveredWidgets.Primary == this;

        /// <summary> Returns true if this widget has been initialized graphically. Setting this widget's parent to an initialized widget will initialize graphics. </summary>
        public bool IsGraphicsInitialized { get; private set; } = false;

        /// <summary> The area of this widget relative to the parent window. </summary>
        public virtual RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary> The position of this widget relative to its window. </summary>
        public Point2 PositionInWindow => ParentWidget == null ? Position : Position.AddPoint2(ParentWidget.PositionInWindow);

        /// <summary> The area of the screen that this widget can draw to. </summary>
        public RectangleF DisplayArea => ParentWidget == null ? AreaInWindow : AreaInWindow.Intersection(ParentWidget.AreaInWindow);

        /// <summary> Returns the IWidgetParent of this widget. </summary>
        public IWidgetParent Parent
        {
            get => ParentWidget != null ? 
                (IWidgetParent)ParentWidget :
                ParentWindow;
            set
            {
                if (value is Widget)
                {
                    ParentWidget = (Widget)value;
                }
                if (value is DWindow)
                {
                    ParentWindow = (DWindow)value;
                }
            }
        }

        /// <summary> The DWindow that owns this widget. </summary>
        public DWindow ParentWindow
        {
            get => _parent_window_backing;
            set
            {
                _parent_window_backing = value;
                if (value != null)
                {
                    InitializeGraphics();

                    // todo: add disconnectevents, this would be broken if triggered multiple times
                    //ConnectEvents(value);
                }
                
                foreach (Widget child in Children)
                {
                    child.ParentWindow = value;
                }
            }
        }

        /// <summary> The widget that owns this widget. (if one exists) </summary>
        public Widget ParentWidget
        {
            get => _parent_widget_backing;
            set
            {
                _parent_widget_backing = value;
                ParentWindow = value?.ParentWindow;
            }
        }

        /// <summary> Returns true if this widget is owned by a parent. </summary>
        public bool IsOwned => ParentWidget != null || ParentWindow != null;

        /// <summary> Area relative to the screen. (not the window) </summary>
        public RectangleF AreaOnScreen => ParentWindow == null ? new RectangleF() : new RectangleF(ParentWindow.Area.Position + AreaInWindow.Position.ToPoint(), Area.Size);

        /// <summary> Represents this window's input each frame. </summary>
        public UIInputState InputState => ParentWindow?.InputState;

        /// <summary> Returns true if this is the main selected widget. </summary>
        public bool IsPrimarySelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.Primary == this;

        /// <summary>Returns true if this widget is selected. </summary>
        public bool IsSelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.IsWidgetFocused(this);

        /// <summary> This widget plus all widgets contained in this widget. </summary>
        public List<Widget> AllContainedWidgets
        {
            get
            {
                List<Widget> result = new List<Widget> { this };

                foreach (Widget child in Children)
                {
                    result.AddRange(child.AllContainedWidgets);
                }

                return result;
            }
        }

        /// <summary> All widgets this widget owns. </summary>
        public abstract List<Widget> Children { get; }

        /// <summary> The SpriteFont used by this Widget. If left null, the Parent of this Widget's SpriteFont will be used. </summary>
        public SpriteFont SpriteFont
        {
            get => Parent == null || _sprite_font_backing != null ? _sprite_font_backing : Parent.SpriteFont;
            set => _sprite_font_backing = value;
        }

        /// <summary> The GraphicsDevice used by this Widget. If left null, the Parent of this Widget's GraphicsDevice will be used. </summary>
        public GraphicsDevice GraphicsDevice
        {
            get => Parent == null || _graphics_backing != null ? _graphics_backing : Parent.GraphicsDevice;
            set => _graphics_backing = value;
        }

        /// <summary> Position of the cursor relative to this widgdet. </summary>
        public Point2 CursorPosition
        {
            get
            {
                if (UpdateData.UIInputState == null) return new Point2();
                if (this is IScrollableWidget)
                {
                    return UpdateData.UIInputState.CursorPosition - PositionInWindow - ((IScrollableWidget)this).Scroll.ToVector2();
                }
                return UpdateData.UIInputState.CursorPosition - PositionInWindow;
            }
        }

        #endregion

        #endregion

        #region Constructors/Destructors

        public Widget(IWidgetParent parent = null)
        {
            SetDefaults();
            Parent = parent;
        }

        private void SetDefaults()
        {
            Size = new Point2(10, 10);
            Theme = BaseColorScheme.Dark;
            Name = GetType().Name;
        }

        ~Widget()
        {
            Dispose();
        }

        public void Dispose()
        {
            _white_dot?.Dispose();
            _render_target?.Dispose();

            foreach (Widget child in Children)
            {
                child.Dispose();
            }
        }

        #endregion

        #region Public/Internal Methods

        // Nothing should be invoked in UpdatePriority.
        /// <summary> Updates this widget and all children recursively. This is called before Update() and updates all logic that should occur beforehand. </summary>
        internal void UpdatePriority(GameTime game_time, UIInputState ui_input)
        {
            UpdateData.GameTime = game_time;
            UpdateData.UIInputState = ui_input;

            UpdateCursorInput(game_time, ui_input);
            if (IsHoveredOver) ParentWindow?.HoveredWidgets.AddFocus(this);
            foreach (Widget widget in Children)
            {
                widget.UpdatePriority(game_time, ui_input);
            }
        }

        // Nothing should be invoked here.
        private void UpdateCursorInput(GameTime game_time, UIInputState ui_input)
        {
            _update_clicked = false;
            _update_double_clicked = false;
            _update_triple_clicked = false;
            _update_added_to_focused = false;
            _update_set_as_focused = false;
            _update_hovered_over = false;

            if (ui_input.PrimaryClick && !_previous_clicking) { _update_clicked = true; } else { _update_clicked = false; } // Set clicked to only be true on the frame the 'mouse' clicks.
            if (ui_input.CursorPosition != _previous_cursor_position)
            {
                _double_click_countdown = 0f; // Do not allow double clicks where the mouse has been moved in-between clicks.
                _triple_click_countdown = 0f;
            }

            _previous_clicking = ui_input.PrimaryClick;
            _previous_cursor_position = ui_input.CursorPosition;
            if (_double_click_countdown > 0f)
            {
                _double_click_countdown -= game_time.GetElapsedSeconds();
            }

            if (AreaInWindow.Contains(ui_input.CursorPosition))
            {
                _update_hovered_over = true;

                if (_update_clicked)
                {
                    if (ui_input.Control)  // Multi-select if the defined control is held down
                    {
                        if (!IsSelected)
                        {
                            _update_added_to_focused = true;
                        }
                    }
                    else
                    {
                        if (!IsSelected)
                        {
                            _update_set_as_focused = true;
                        }
                    }
                    if (_triple_click_countdown > 0)
                    {
                        _double_click_countdown = 0f;
                        _triple_click_countdown = 0f; // Do not allow consecutive triple clicks.
                        _update_triple_clicked = true;
                    }
                    if (_double_click_countdown > 0)
                    {
                        _double_click_countdown = 0f; // Do not allow consecutive double clicks.
                        if (!IsSelected)
                        {
                            _update_set_as_focused = true;
                        }
                        _update_double_clicked = true;
                        _triple_click_countdown = _double_click_timing_backing;
                    }
                    _double_click_countdown = _double_click_timing_backing;
                }
            }
            else
            {
                _update_hovered_over = false;
            }
        }
        
        /// <summary> Update this widget and all widgets contained. </summary>
        internal void Update(GameTime game_time, UIInputState ui_input)
        {
            if (this is IScrollableWidget s_this && false)
            {
                if (s_this.FitToContentArea)
                {
                    Point2 size = Size;
                    RectangleF c_area = s_this.ContentArea;
                    
                    if (c_area.Size.ToPoint2().IsLargerThan(size))
                    {
                        Size = c_area.Size.ToPoint2().Max(size);
                        UpdateArea(true);
                    }
                }
            }

            if (IsPrimaryHovered && ParentWindow.IsActive)
            {
                if (_update_added_to_focused) AddToFocused();
                if (_update_set_as_focused) SetAsFocused();
                if (_update_clicked) OnClick?.Invoke(this, EventArgs.Empty);
                if (_update_double_clicked) OnDoubleClick?.Invoke(this, EventArgs.Empty);
                if (_update_triple_clicked) OnTripleClick?.Invoke(this, EventArgs.Empty);
            }

            if (ui_input.Enter && IsPrimarySelected && EnterConfirms)
            {
                OnConfirm?.Invoke(this, EventArgs.Empty);
            }

            IsHoveredOver = _update_hovered_over;
            Theme.Update(game_time);
            if (this is IScrollableWidget scroll_widget)
            {
                scroll_widget.ScrollBars.Update(UpdateData.GameTime.GetElapsedSeconds(), UpdateData.UIInputState);
            }

            OnUpdate?.Invoke(this, EventArgs.Empty);

            foreach (Widget widget in Children)
            {
                widget.Update(game_time, ui_input);
            }
        }

        /// <summary> Draws this widget (and all children) to the screen. Currently clears the screen on calling. </summary>
        public void Draw()
        {
            Render();

            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_render_target, Area.ToRectangle(), Color.White);
            SpriteBatch.End();
        }

        /// <summary> Renders this widget with all children inside. </summary>
        /// <returns> This widget's render. </returns>
        private RenderTarget2D Render()
        {
            UpdateRenderTargetSizes();

            _graphics_in_use = true;
            if (_graphics_updating)
            {
                _graphics_in_use = false;
                return _render_target;
            }

            List<RenderTarget2D> renders = new List<RenderTarget2D>();
            List<RectangleF> areas = new List<RectangleF>();

            foreach (Widget child in Children)
            {
                renders.Add(child.Render());
                if (child is IScrollableWidget s_widget)
                {
                    areas.Add(child.Area.WithOffset(s_widget.Scroll.ToPoint2()));
                }
                else
                {
                    areas.Add(child.Area);
                }
            }
            
            GraphicsDevice.SetRenderTarget(_render_target);

            if (DrawBackground)
            {
                GraphicsDevice.Clear(Theme.BackgroundColor.CurrentColor);
            }

            SpriteBatch.Begin();
            OnDraw?.Invoke(this, EventArgs.Empty);

            int i = 0;
            foreach (Widget child in Children)
            {
                if (this is IScrollableWidget s_this)
                {
                    SpriteBatch.Draw
                        (
                        renders[i],
                        areas[i].Position.AddPoint2(s_this.Scroll.ToPoint2().Inverted()).Floored(),
                        Color.White
                        );
                }
                else
                {
                    SpriteBatch.Draw
                        (
                        renders[i], 
                        areas[i].Position.Floored(), 
                        Color.White
                        );
                }
                i++;
            }
            DrawOverlay();

            SpriteBatch.End();

            _graphics_in_use = false;

            return _render_target;
        }
        
        /// <summary> Initializes all graphics related content. </summary>
        private void InitializeGraphics()
        {
            if (IsGraphicsInitialized) return;

            if (GraphicsDevice == null)
            {
                throw new Exception($"GraphicsDevice cannot be null.");
            }
            
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _white_dot = DrawingTools.WhiteDot(GraphicsDevice);

            IsGraphicsInitialized = true;

            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>  Used by internal Focus object. </summary>
        internal void TriggerSelectOffEvent()
        {
            OnSelectOff?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Used by internal Focus object. </summary>
        internal void TriggerSelectEvent()
        {
            OnSelection?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Search for any methods in a DWindow for this to connect to. </summary>
        public void ConnectEvents()
        {
            System.Reflection.EventInfo[] events = GetType().GetEvents();
            for (int i = 0; i < events.GetLength(0); i++)
            {
                System.Reflection.EventInfo event_ = events[i];
                string method_name = "Slot_" + Name + "_On" + event_.Name;

                System.Reflection.MethodInfo window_method = ParentWindow.GetType().GetMethod(method_name);

                if (window_method == null)
                {
                    continue;
                }

                Delegate handler = Delegate.CreateDelegate(
                    event_.EventHandlerType,
                    ParentWindow,
                    window_method);

                event_.AddEventHandler(this, handler);
            }
        }
        
        /// <summary> Signals confirming this widget. (Such as pressing enter with this widget selected) </summary>
        public void SignalConfirm()
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region EventsHandlers

        /// <summary> Invoked when this widget is clicked on. </summary>
        public event EventHandler OnClick;
        /// <summary> Invoked when this widget is double clicked. </summary>
        public event EventHandler OnDoubleClick;
        /// <summary> Invoked when this widget is triple clicked. </summary>
        public event EventHandler OnTripleClick;
        /// <summary> Invoked when the mouse hovers over this widget. </summary>
        public event EventHandler OnHover;
        /// <summary> Invoked when the mouse hovers off this widget. </summary>
        public event EventHandler OnHoverOff;
        /// <summary> Invoked after graphics have been initialized and are ready to use. </summary>
        public event EventHandler OnGraphicsInitialized;
        /// <summary> Invoked when this widget is drawn. </summary>
        public event EventHandler OnDraw;
        /// <summary> Invoked when this widget's overlay is drawn. </summary>
        public event EventHandler OnDrawOverlay;
        /// <summary> Invoked after this widget updates.</summary>
        public event EventHandler OnUpdate;
        /// <summary> Invoked when this widget is selected. </summary>
        public event EventHandler OnSelection;
        /// <summary> Invoked when this widget is un-selected. </summary>
        public event EventHandler OnSelectOff;
        /// <summary> Invoked whwn the user confirms this widget (Such as pressing enter with this widget selected). </summary>
        public event EventHandler OnConfirm;

        #endregion
        
        #region Private/Protected Methods

        /// <summary> Embeds this widget in the given area. Takes SnappingPolicy and spacing into account. </summary>
        internal void EmbedIn(RectangleF encompassing_area)
        {
            encompassing_area = encompassing_area.SizeOnly();
            //Debug.WriteLine($"Embedding {Area} in {encompassing_area}");
            //Debug.WriteLine($"SnappingPolicy = {SnappingPolicy}");

            RectangleF new_area = Area;

            // Convert the corners into up/down left/right to determine which walls the new area should stick to
            Directions2D snapping = SnappingPolicy.ToPerpendicular();

            // left
            if (snapping.Left && !snapping.Right)
            {
                new_area.X = encompassing_area.X + Spacing.Width;
            }

            // right
            if (!snapping.Left && snapping.Right)
            {
                new_area.X = encompassing_area.X + encompassing_area.Width - new_area.Width - Spacing.Width;
            }

            // left and right
            if (snapping.Left && snapping.Right)
            {
                new_area.X = encompassing_area.X + Spacing.Width;
                new_area.Width = encompassing_area.Width - Spacing.Width * 2;
            }

            // up
            if (snapping.Up && !snapping.Down)
            {
                new_area.Y = encompassing_area.Y + Spacing.Height;
            }

            // down
            if (!snapping.Up && snapping.Down)
            {
                new_area.Y = encompassing_area.Y + encompassing_area.Height - new_area.Height - Spacing.Height;
            }

            // up and down
            if (snapping.Up && snapping.Down)
            {
                new_area.Y = encompassing_area.Y + Spacing.Height;
                new_area.Height = encompassing_area.Height - Spacing.Height * 2;
            }

            // Restrain by the widget's minimum height/width
            if (new_area.Width < MinimumWidth)
            {
                new_area.Width = MinimumWidth;
            }
            if (new_area.Height < MinimumHeight)
            {
                new_area.Height = MinimumHeight;
            }

            //Debug.WriteLine($"new_area = {new_area}");
            Area = new_area;
        }

        /// <summary> Draw anything that should be drawn on top of the content in this widget. </summary>
        private void DrawOverlay()
        {
            if (DrawOutline)
            {
                DrawingTools.DrawBorder(_white_dot, SpriteBatch, Area.SizeOnly().ToRectangle(), OutlineThickness, Theme.OutlineColor.CurrentColor, OutlineSides);
            }

            if (this is IScrollableWidget scroll_widget){
                scroll_widget.ScrollBars.Draw(SpriteBatch);
            }

            OnDrawOverlay?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Set this widget as the only focused widget. </summary>
        protected void SetAsFocused() => ParentWindow?.SelectedWidgets.SetFocus(this);

        /// <summary> Add this widget to the group of selected widgets. </summary>
        protected void AddToFocused() => ParentWindow?.SelectedWidgets.AddFocus(this);
        
        /// <summary> A function called by a widget to update both itself and its parent's area. </summary>
        protected virtual void UpdateArea(bool update_parent)
        {
            if (update_parent)
            {
                ParentWidget?.UpdateArea(update_parent);
            }
        }

        /// <summary> Resize the render target to match the current area. </summary>
        private void UpdateRenderTargetSizes()
        {
            foreach (Widget child in Children)
            {
                child.UpdateRenderTargetSizes();
            }

            if (this is IScrollableWidget s_this)
            {
                UpdateRenderTargetSize(s_this.ContentArea.Size);
            }
            else
            {
                UpdateRenderTargetSize(Size);
            }
        }
        private void UpdateRenderTargetSize(Point2 size)
        {
            _graphics_updating = true;

            if (_graphics_in_use)
            {
                int i = 0;
                while (_graphics_in_use)
                {
                    i += _WAIT_TIME;
                    if (i > _MAX_WAIT_TIME)
                    {
                        Console.WriteLine($"Hanging in UpdateRenderTarget waiting for graphics to not be in use.");
                    }

                    Thread.Sleep(_WAIT_TIME);
                }
            }

            if (size.X < 1)
            {
                size.X = 1;
            }

            if (size.Y < 1)
            {
                size.Y = 1;
            }

            if (size.ToPoint() != _render_target?.Size().ToPoint())
            {
                if (Math.Max((int)size.X, (int)size.Y) > _MAXIMUM_WIDGET_SIZE)
                {
                    size = size.Min(new Point2(_MAXIMUM_WIDGET_SIZE, _MAXIMUM_WIDGET_SIZE));
                    ConsoleOutput.WriteLine($"Maximum Widget dimensions reached (maximum size is {_MAXIMUM_WIDGET_SIZE}, given dimensions are {size}). This may cause rendering issues.");
                }

                // Dispose of previous render target
                if (_render_target != null)
                {
                    _render_target.Dispose();
                    while (!_render_target.IsDisposed) { }
                }

                _render_target = new RenderTarget2D(
                    GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    SurfaceFormat.Vector4,
                    DepthFormat.Depth24,
                    0,
                    RenderTargetUsage.DiscardContents);
            }

            _graphics_updating = false;
        }


        #endregion

        #region Cloning

        public object Clone(Widget parent = null)
        {
            object c = DerivedClone(parent);
            ((Widget)c).ParentWidget = ParentWidget;
            ((Widget)c).ParentWindow = ParentWindow;

            ((Widget)c).Name = Name;
            ((Widget)c).ChangeColorOnMouseOver = ChangeColorOnMouseOver;
            ((Widget)c).DrawBackground = DrawBackground;
            ((Widget)c).Theme = (BaseColorScheme)Theme.Clone();
            ((Widget)c).OutlineThickness = OutlineThickness;
            ((Widget)c).DrawOutline = DrawOutline;
            ((Widget)c).EnterConfirms = EnterConfirms;
            ((Widget)c).MinimumSize = MinimumSize;
            ((Widget)c).SnappingPolicy = (DiagonalDirections2D)SnappingPolicy.Clone();
            ((Widget)c).OutlineSides = (Directions2D)OutlineSides.Clone();
            ((Widget)c).Centered = Centered;
            ((Widget)c).DoubleClickTiming = DoubleClickTiming;
            ((Widget)c).Spacing = Spacing;
            ((Widget)c).Area = Area;
            ((Widget)c).IsFixedWidth = IsFixedWidth;
            ((Widget)c).IsFixedHeight = IsFixedHeight;
            ((Widget)c).PaletteUsage = PaletteUsage;

            ((Widget)c).debug_output = debug_output;

            return c;
        }

        // This is for implementing cloning in derived classes. They'll return their
        // clone for their own fields, and 'Widget' will add the base fields to it.
        // See https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        protected abstract object DerivedClone(Widget parent);

        #endregion
    }
}