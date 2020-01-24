using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
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

// Todo: Change some methods to properties.
// Combine slots with widget.
// Palettes should have ChangeColorOnHover, functionality should be removed from here.
// Remove white_dot and use Extended drawing methods instead.
// Optimize those property derivitaves

namespace DownUnder.UI.Widgets
{
    // todo: look into separating items into custom objects.
    // Clone needs to be updated (and organized)
    /// <summary> A visible window object. </summary>
    [DataContract] public abstract class Widget : IWidgetParent
    {
        public bool debug_output = false;

        #region Fields/Delegates

        /// <summary> Used by some drawing code. </summary>
        private Texture2D white_dot;

        /// <summary> The local sprite batch used by this widget. </summary>
        public SpriteBatch sprite_batch;

        /// <summary> A reference to the Widget that owns this one, if one exists. </summary>
        private Widget _parent_widget_reference;

        /// <summary> A reference to the window that owns this widget. </summary>
        private DWindow _parent_window_reference;

        /// <summary> The primary cursor press of the previous frame. (Used to trigger events on the single frame of a press) </summary>
        private bool _previous_clicking;

        // Various property backing fields.
        protected RectangleF area_backing = new RectangleF();
        private float _double_click_timing_backing = 0.5f;
        private Point2 _minimum_area_backing = new Point2(1f, 1f);
        private SpriteFont _sprite_font_backing;
        private GraphicsDevice _graphics_backing;
        private bool _is_hovered_over_backing;
        
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

        /// <summary> The render target this Widget uses to draw to. </summary>
        public RenderTarget2D render_target;

        #endregion Fields/Delegates

        #region Public/Internal Properties

        #region Auto properties

        /// <summary> The unique name of this widget. </summary>
        [DataMember] public string Name { get; set; }

        /// <summary> If set to false, the background color will not be drawn. </summary>
        [DataMember] public virtual bool DrawBackground { get; set; }

        /// <summary> If set to true, colors will shift to their hovered colors on mouse-over. </summary>
        [DataMember] public bool ChangeColorOnMouseOver { get; set; } = true;

        /// <summary> How thick the outline (if DrawOutline == true) should be. </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;

        /// <summary> If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides) </summary>
        [DataMember] public bool DrawOutline { get; set; }

        /// <summary> Which sides (of the outline) are drawn (top, bottom, left, right) if (DrawOutLine == true). </summary>
        [DataMember] public Directions2D OutlineSides { get; set; } = Directions2D.UpDownLeftRight;

        /// <summary> The UIPalette used for the background color. </summary>
        [DataMember] public virtual UIPalette BackgroundColor { get; internal set; } = new UIPalette(Color.CornflowerBlue, Color.CornflowerBlue.ShiftBrightness(1.1f));

        /// <summary> The UIPalette used for any text in this widget. </summary>
        [DataMember] public virtual UIPalette TextColor { get; internal set; } = new UIPalette(Color.Black, Color.Black);

        /// <summary> The UIPalette used for the outline of this widget. (if enabled) </summary>
        [DataMember] public virtual UIPalette OutlineColor { get; internal set; } = new UIPalette();

        /// <summary> Represents the corners this widget will snap to within the parent. </summary>
        [DataMember] public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.TopRight_BottomLeft_TopLeft_BottomRight;

        /// <summary> Unimplemented. </summary>
        [DataMember] public bool Centered { get; set; }

        /// <summary> Should any SnappingPolicy be defined, this represents the distance from the edges of the widget this is snapped to. </summary>
        [DataMember] public Size2 Spacing { get; set; }

        /// <summary> When set to true pressing enter while this widget is the primarily selected one will trigger confirmation events. </summary>
        [DataMember] public virtual bool EnterConfirms { get; set; } = true;

        /// <summary> Contains all information relevant to updating on this frame. </summary>
        public UpdateData UpdateData { get; set; } = new UpdateData();

        #endregion Auto properties

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
            get => area_backing.WithMinimumSize(MinimumSize);
            set => area_backing = value;
        }

        /// <summary> Minimum size allowed when setting this widget's area. </summary>
        [DataMember] public virtual Point2 MinimumSize
        {
            get => _minimum_area_backing;
            set
            {
                if (_minimum_area_backing.X < 1)
                {
                    throw new Exception("Minimum area width must be at least 1.");
                }
                if (_minimum_area_backing.Y < 1)
                {
                    throw new Exception("Minimum area height must be at least 1.");
                }

                _minimum_area_backing = value;
            }
        }
        
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

        #endregion Non-auto properties

        #region Derivatives of previous properties

        public float Width
        {
            get => Area.Width;
            set => Area = new RectangleF(Area.X, Area.Y, value, Area.Height);
        }
        public float Height
        {
            get => Area.Height; set => Area = new RectangleF(Area.X, Area.Y, Area.Width, value); }
        public Point2 Position { get => Area.Position; set => Area = new RectangleF(value, Area.Size); }
        public Point2 Size { get => Area.Size; set => Area = new RectangleF(Area.Position, value); }
        public float X { get => Area.X; set => Area = new RectangleF(value, Area.Y, Area.Width, Area.Height); }
        public float Y { get => Area.Y; set => Area = new RectangleF(Area.X, value, Area.Width, Area.Height); }
        public float MinimumHeight { get => MinimumSize.Y; set => MinimumSize = new Point2(MinimumSize.X, value); }
        public float MinimumWidth { get => MinimumSize.X; set => MinimumSize = new Point2(value, MinimumSize.Y); }

        #endregion Derivatives of previous properties

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

        /// <summary> Returns true if this widget has been initialized graphically. (If this widget has not been graphically initialized, it cannot be drawn. Call InitializeGraphics() to initialize graphics.) </summary>
        public bool IsGraphicsInitialized => GraphicsDevice != null;

        /// <summary> The area of this widget relative to the parent window. </summary>
        public virtual RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary> The position of this widget relative to its window. </summary>
        public Point2 PositionInWindow => ParentWidget == null ? Position : Position.AddPoint2(ParentWidget.PositionInWindow);

        /// <summary> The area of the screen that this widget can draw to. </summary>
        public RectangleF DisplayArea => ParentWidget == null ? AreaInWindow : AreaInWindow.Intersection(ParentWidget.AreaInWindow);

        /// <summary> Returns the IWidgetParent of this widget. </summary>
        public IWidgetParent Parent => _parent_widget_reference != null ? (IWidgetParent)_parent_widget_reference : _parent_window_reference;

        public DWindow ParentWindow
        {
            get => _parent_window_reference;
            set
            {
                _parent_window_reference = value;
                if (value == null)
                {
                    return;
                }
                
                ConnectEvents(value);
                
                foreach (Widget child in Children)
                {
                    child.ParentWindow = value;
                }
            }
        }

        internal Widget ParentWidget
        {
            get => _parent_widget_reference;
            set
            {
                _parent_widget_reference = value;
                if (value != null) { ParentWindow = value.ParentWindow; }
            }
        }

        /// <summary> Returns true if this widget is owned by a parent. </summary>
        internal bool IsOwned => ParentWidget != null || ParentWindow != null;

        /// <summary> Area relative to the screen. (not the window) </summary>
        public RectangleF AreaOnScreen => ParentWindow == null ? new RectangleF() : new RectangleF(ParentWindow.Area.Location + AreaInWindow.Position.ToPoint(), Area.Size);

        /// <summary> Represents this window's input each frame. </summary>
        public UIInputState InputState => ParentWindow?.InputState;

        /// <summary> Returns true if this is the main selected widget. </summary>
        public bool IsPrimarySelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.Primary == this;

        /// <summary>Returns true if this widget is selected. </summary>
        public bool IsSelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.IsWidgetFocused(this);
        
        #endregion Other various non-serialized properties

        #endregion Public/Internal Properties

        #region Constructors

        public Widget(IWidgetParent parent = null)
        {
            SetDefaults();
            SetOwnership(parent);
        }

        private void SetDefaults()
        {
            Name = GetType().Name;
        }

        ~Widget()
        {
            white_dot.Dispose();
            render_target?.Dispose();
        }

        #endregion Constructors

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
        void UpdateCursorInput(GameTime game_time, UIInputState ui_input)
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
            UpdatePalette(game_time);
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
            UpdateRenderTargetSizes();
            DrawToRenderTargets();
            GetRender();

            GraphicsDevice.SetRenderTarget(null);
            sprite_batch.Begin();
            sprite_batch.Draw(render_target, Area.ToRectangle(), Color.White);
            sprite_batch.End();
        }

        private void DrawToRenderTargets()
        {
            _graphics_in_use = true;
            if (_graphics_updating)
            {
                _graphics_in_use = false;
                return;
            }
            
            GraphicsDevice.SetRenderTarget(render_target);

            sprite_batch.Begin();

            if (DrawBackground)
            {
                GraphicsDevice.Clear(BackgroundColor.CurrentColor);
            }

            OnDraw?.Invoke(this, EventArgs.Empty);
            //DrawOverlay();
            sprite_batch.End();
            
            foreach (Widget widget in Children)
            {
                widget.DrawToRenderTargets();
            }
            
            _graphics_in_use = false;
        }

        private RenderTarget2D GetRender()
        {
            List<RenderTarget2D> renders = new List<RenderTarget2D>();
            List<RectangleF> areas = new List<RectangleF>();

            foreach (Widget child in Children)
            {
                renders.Add(child.GetRender());
                if (child is IScrollableWidget s_widget)
                {
                    areas.Add(child.Area.WithOffset(s_widget.Scroll.ToPoint2()));
                }
                else
                {
                    areas.Add(child.Area);
                }
            }
            
            GraphicsDevice.SetRenderTarget(render_target);
            sprite_batch.Begin();

            int i = 0;
            foreach (Widget child in Children)
            {
                if (this is IScrollableWidget s_this)
                {
                    sprite_batch.Draw(renders[i], areas[i].Position.AddPoint2(s_this.Scroll.ToPoint2().Inverted()), Color.White);
                }
                else
                {
                    sprite_batch.Draw(renders[i], areas[i].Position, Color.White);
                }
                i++;
            }
            DrawOverlay();

            sprite_batch.End();

            return render_target;
        }

        /// <summary> Initializes all graphic related fields and updates all references. (To be used after a widget is created without parameters.) </summary>
        public void Initialize(DWindow parent_window, GraphicsDevice graphics_device, SpriteFont sprite_font = null)
        {
            InitializeAllReferences(parent_window, _parent_widget_reference); // The parent widget reference shouldn't be modified here
            InitializeGraphics();
        }
        
        internal protected void InitializeGraphics()
        {
            if (GraphicsDevice == null)
            {
                throw new Exception($"GraphicsDevice cannot be null.");
            }

            sprite_batch = new SpriteBatch(GraphicsDevice);
            white_dot = DrawingTools.WhiteDot(GraphicsDevice);

            foreach (Widget child in Children)
            {
                child.InitializeGraphics();
            }

            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> To be called post-serialization, this updates all references in this and all children. </summary>
        internal void InitializeAllReferences(DWindow parent_window, Widget parent_widget = null)
        {
            ParentWindow = parent_window;
            ParentWidget = parent_widget;

            foreach (Widget child in Children)
            {
                child.InitializeAllReferences(parent_window, this);
            }
        }

        internal void EmbedIn(RectangleF area)
        {
            Area = EmbeddedIn(area);
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
        /// <param name="window"></param>
        public void ConnectEvents(DWindow window)
        {
            System.Reflection.EventInfo[] events = GetType().GetEvents();
            for (int i = 0; i < events.GetLength(0); i++)
            {
                System.Reflection.EventInfo event_ = events[i];
                string method_name = "Slot_" + Name + "_On" + event_.Name;

                System.Reflection.MethodInfo window_method = window.GetType().GetMethod(method_name);

                if (window_method == null)
                {
                    continue;
                }

                Delegate handler = Delegate.CreateDelegate(
                    event_.EventHandlerType,
                    window,
                    window_method);

                event_.AddEventHandler(this, handler);
            }
        }

        internal void SetOwnership(IWidgetParent parent)
        {
            // If the parent is a widget
            if (parent is Widget parent_widget)
            {
                ParentWidget = parent_widget;
            }

            // If the parent is a window
            else if (parent is DWindow parent_window)
            {
                ParentWidget = null;
                ParentWindow = parent_window;
            }

            else if (parent == null)
            {
                ParentWidget = null;
                ParentWindow = null;
            }
        }

        /// <summary> Signals confirming this widget. (Such as pressing enter with this widget selected) </summary>
        public void SignalConfirm()
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
        }

        #endregion Public/Internal Methods

        #region EventHandlers/Events

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

        #endregion EventsHandlers

        #region Events

        private void UpdatePalette(GameTime game_time)
        {
            if (IsPrimaryHovered && ChangeColorOnMouseOver)
            {
                BackgroundColor.Hovered = true;
                TextColor.Hovered = true;
                OutlineColor.Hovered = true;
            }
            else
            {
                BackgroundColor.Hovered = false;
                TextColor.Hovered = false;
                OutlineColor.Hovered = false;
            }
            
            TextColor.Update(game_time.GetElapsedSeconds());
            BackgroundColor.Update(game_time.GetElapsedSeconds());
            OutlineColor.Update(game_time.GetElapsedSeconds());
        }

        #endregion Events

        #endregion EventHandlers/Events

        #region Private/Protected Methods

        /// <summary> Creates a new area taking the snapping_policy into account. </summary>
        private RectangleF EmbeddedIn(RectangleF encompassing_area)
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
            return new_area;
        }

        /// <summary> This widget plus all widgets contained in this widget. </summary>
        public List<Widget> GetAllWidgets()
        {
            List<Widget> result = new List<Widget> { this };

            foreach (Widget child in Children)
            {
                result.AddRange(child.GetAllWidgets());
            }

            return result;
        }

        /// <summary> All widgets this widget owns. </summary>
        public abstract List<Widget> Children { get; }

        /// <summary> Draw anything that should be drawn on top of the content in this widget. </summary>
        private void DrawOverlay()
        {
            if (DrawOutline)
            {
                DrawingTools.DrawBorder(white_dot, sprite_batch, Area.SizeOnly().ToRectangle(), OutlineThickness, OutlineColor.CurrentColor, OutlineSides);
            }

            if (this is IScrollableWidget scroll_widget){
                scroll_widget.ScrollBars.Draw(sprite_batch);
            }

            OnDrawOverlay?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Set this widget as the only focused widget. </summary>
        protected void SetAsFocused() => _parent_window_reference?.SelectedWidgets.SetFocus(this);

        /// <summary> Add this widget to the group of selected widgets. </summary>
        protected void AddToFocused() => _parent_window_reference?.SelectedWidgets.AddFocus(this);
        
        /// <summary> A function called by a widget to update both itself and its parent's area. </summary>
        protected virtual void UpdateArea(bool update_parent)
        {
            if (update_parent)
            {
                _parent_widget_reference?.UpdateArea(update_parent);
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
                UpdateRenderTargetSize(s_this.ContentSize);
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
            
            if (Math.Max((int)size.X, (int)size.Y) > _MAXIMUM_WIDGET_SIZE)
            {
                throw new Exception($"Maximum Widget dimensions reached (maximum size is {_MAXIMUM_WIDGET_SIZE}, given dimensions are {area_backing}).");
            }

            if (size.X < 1)
            {
                size.X = 1;
            }

            if (size.Y < 1)
            {
                size.Y = 1;
            }

            if (size.ToPoint() != render_target?.Size().ToPoint())
            {
                // Dispose of previous render target
                if (render_target != null)
                {
                    render_target.Dispose();
                    while (!render_target.IsDisposed) { }
                }

                render_target = new RenderTarget2D(
                    GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    SurfaceFormat.Vector4,
                    DepthFormat.Depth24,
                    0,
                    RenderTargetUsage.PreserveContents);
            }

            _graphics_updating = false;
        }


        #endregion Private/Protected Methods

        #region Cloning

        public object Clone()
        {
            object c = DerivedClone();
            ((Widget)c)._parent_widget_reference = _parent_widget_reference;
            ((Widget)c)._parent_window_reference = _parent_window_reference;

            ((Widget)c).Name = Name;
            ((Widget)c).DrawBackground = DrawBackground;
            ((Widget)c).BackgroundColor = (UIPalette)BackgroundColor.Clone();
            ((Widget)c).TextColor = (UIPalette)TextColor.Clone();
            ((Widget)c).OutlineColor = (UIPalette)OutlineColor.Clone();
            ((Widget)c).ChangeColorOnMouseOver = ChangeColorOnMouseOver;
            ((Widget)c).TextColor = TextColor;
            ((Widget)c).OutlineThickness = OutlineThickness;
            ((Widget)c).DrawOutline = DrawOutline;
            ((Widget)c).SnappingPolicy = (DiagonalDirections2D)SnappingPolicy.Clone();
            ((Widget)c).OutlineSides = (Directions2D)OutlineSides.Clone();
            ((Widget)c).Centered = Centered;
            ((Widget)c).DoubleClickTiming = DoubleClickTiming;
            ((Widget)c).Spacing = Spacing;
            ((Widget)c).Area = Area;

            ((Widget)c).debug_output = debug_output;
            return c;
        }

        // This is for implementing cloning in derived classes. They'll return their
        // clone for their own fields, and 'Widget' will add the base fields to it.
        // See https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        protected abstract object DerivedClone();

        #endregion Cloning
    }
}