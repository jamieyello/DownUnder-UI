using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
// Make more properties compact.

namespace DownUnder.UI.Widgets
{
    // todo: look into separating items into custom objects.
    // Clone needs to be updated (and organized)
    /// <summary>
    /// A visible window object.
    /// </summary>
    [DataContract]
    public abstract class Widget : IWidgetParent//, IDebugFeatures
    {
        #region Fields/Delegates

        //temporary
        public Texture2D white_dot;
        public SpriteBatch sprite_batch;

        /// <summary>
        /// A reference to the Widget that owns this one, if one exists.
        /// </summary>
        [NonSerialized] private Widget _parent_widget_reference;

        /// <summary>
        /// A reference to the window that owns this widget.
        /// </summary>
        [NonSerialized] protected DWindow _parent_window_reference;

        /// <summary>
        /// The primary cursor press of the previous frame. (Used to trigger events on the single frame of a press)
        /// </summary>
        private bool _previous_clicking;

        // Various property backing fields.
        protected RectangleF area_backing = new RectangleF();
        private float _double_click_timing_backing = 0.5f;
        private Point2 _minimum_area_backing = new Point2(1f, 1f);
        private SpriteFont _sprite_font_backing;
        private GraphicsDevice _graphics_backing;

        /// <summary>
        /// Used to track the period of time where a second click would be considered a double click. (If this value is > 0)
        /// </summary>
        private float _double_click_countdown = 0f;

        /// <summary>
        /// Used to track the period of time where a third click would be considered a triple click. (If this value is > 0)
        /// </summary>
        private float _triple_click_countdown = 0f;

        private bool _is_hovered_over_backing;

        private Point2 _previous_cursor_position = new Point2();

        /// <summary>
        /// The maximum size of a widget. (Every Widget uses a RenderTarget2D to render its contents to, this is the maximum resolution imposed by that.)
        /// </summary>
        protected const int _MAXIMUM_WIDGET_SIZE = 2048;
        private bool _graphics_updating = false;
        private bool _graphics_in_use = false;

        /// <summary>
        /// Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed.
        /// </summary>
        private readonly int _WAIT_TIME = 5;

        /// <summary>
        /// How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings.
        /// </summary>
        private readonly int _MAX_WAIT_TIME = 100;

        // The following are used by Update()/UpdatePriority().
        // They are set to true or false in UpdatePriority(), and Update() invokes events
        // by reading them.
        bool update_clicked;
        bool update_double_clicked;
        bool update_triple_clicked;
        bool update_added_to_focused;
        bool update_set_as_focused;
        bool update_hovered_over;

        /// <summary>
        /// The render target this Widget uses to draw to.
        /// </summary>
        public RenderTarget2D local_render_target;

        #endregion Fields/Delegates

        #region Public/Internal Properties

        #region Auto properties

        /// <summary>
        /// The unique name of this widget.
        /// </summary>
        [DataMember] public string Name { get; set; }

        /// <summary>
        /// If set to false, the background color will not be drawn.
        /// </summary>
        [DataMember] public virtual bool DrawBackground { get; set; }

        /// <summary>
        /// If set to true, colors will shift to their hovered colors on mouse-over.
        /// </summary>
        [DataMember] public bool ChangeColorOnMouseOver { get; set; } = true;

        /// <summary>
        /// Unimplemented.
        /// </summary>
        [DataMember] private bool OutlineOnMouseOver { get; set; }

        /// <summary>
        /// How thick the outline (if DrawOutline == true) should be.
        /// </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;

        /// <summary>
        /// If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides)
        /// </summary>
        [DataMember] public bool DrawOutline { get; set; }

        /// <summary>
        /// Which sides (of the outline) are drawn (top, bottom, left, right) if (DrawOutLine == true).
        /// </summary>
        [DataMember] public Directions2D OutlineSides { get; set; } = Directions2D.UpDownLeftRight;

        [DataMember] public virtual UIPalette BackgroundColor { get; internal set; } = new UIPalette(Color.CornflowerBlue, Color.White);
        [DataMember] public virtual UIPalette TextColor { get; internal set; } = new UIPalette(Color.Black, Color.Gray);
        [DataMember] public virtual UIPalette OutlineColor { get; internal set; } = new UIPalette();

        /// <summary>
        /// Represents the corners this widget will snap to.
        /// </summary>
        [DataMember] public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.TopRight_BottomLeft_TopLeft_BottomRight;

        /// <summary>
        /// Unimplemented.
        /// </summary>
        [DataMember] private bool Centered { get; set; }

        /// <summary>
        /// Should any SnappingPolicy be defined, this represents the distance from the edges of the widget this is snapped to.
        /// </summary>
        [DataMember] public Size2 Spacing { get; set; }

        /// <summary>
        /// Contains all information relevant to updating on this frame.
        /// </summary>
        public UpdateData UpdateData { get; set; } = new UpdateData();

        /// <summary>
        /// The SpriteFont used by this Widget. If left null, the Parent of this Widget's SpriteFont will be used.
        /// </summary>
        public SpriteFont SpriteFont
        {
            get => Parent == null || _sprite_font_backing != null ? _sprite_font_backing : Parent.SpriteFont;
            set => _sprite_font_backing = value;
        }

        /// <summary>
        /// The GraphicsDevice used by this Widget. If left null, the Parent of this Widget's GraphicsDevice will be used.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get => Parent == null || _graphics_backing != null ? _graphics_backing : Parent.GraphicsDevice;
            set => _graphics_backing = value;
        }
        
        /// <summary>
        /// Position of the cursor relative to this widgdet.
        /// </summary>
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

        #endregion Auto properties

        #region Non-auto properties

        /// <summary>
        /// Minimum time (in seconds) in-between two clicks needed for a double/triple-click to register with this widget. Default (and Windows standard) is 0.5f.
        /// </summary>
        [DataMember] public float DoubleClickTiming
        {
            get => _double_click_timing_backing;
            set
            {
                if (value > 0) { _double_click_timing_backing = value; }
                else { _double_click_timing_backing = 0f; }
            }
        }

        /// <summary>
        /// Area of this widget. (Position relative to parent widget, if any)
        /// </summary>
        [DataMember] public virtual RectangleF Area
        {
            get => area_backing;
            set
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
                            Console.WriteLine($"Hanging in Area.Set waiting for graphics to not be in use.");
                        }

                        Thread.Sleep(_WAIT_TIME);
                    }
                }

                // Prevent the widget's area from being set to something the render target can't draw. (A zero/negative value)
                if ((int)value.Width < 1)
                {
                    value.Width = 1f;
                }

                if ((int)value.Height < 1)
                {
                    value.Height = 1f;
                }

                if ((int)value.Width < MinimumWidth)
                {
                    value.Width = MinimumWidth;
                }

                if ((int)value.Height < MinimumHeight)
                {
                    value.Height = MinimumHeight;
                }

                area_backing = value;
                if (IsGraphicsInitialized)
                {
                    UpdateRenderTarget(value);
                }
                _graphics_updating = false;
            }
        }

        /// <summary>
        /// Minimum size allowed when setting this widget's area.
        /// </summary>
        [DataMember] public Point2 MinimumSize
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
                Area = Area;
            }
        }

        #endregion Non-auto properties

        #region Derivatives of previous properties

        public float Width { get => Area.Width; set => Area = new RectangleF(Area.X, Area.Y, value, Area.Height); }
        public float Height { get => Area.Height; set => Area = new RectangleF(Area.X, Area.Y, Area.Width, value); }
        public Point2 Position { get => Area.Position; set => Area = new RectangleF(value, Area.Size); }
        public Point2 Size { get => Area.Size; set => Area = new RectangleF(Area.Position, value); }
        public float X { get => Area.X; set => Area = new RectangleF(value, Area.Y, Area.Width, Area.Height); }
        public float Y { get => Area.Y; set => Area = new RectangleF(Area.X, value, Area.Width, Area.Height); }
        public float MinimumHeight { get => MinimumSize.Y; set => MinimumSize = new Point2(MinimumSize.X, value); }
        public float MinimumWidth { get => MinimumSize.X; set => MinimumSize = new Point2(value, MinimumSize.Y); }

        #endregion Derivatives of previous properties

        #region Other various non-serialized properties

        /// <summary>
        /// Returns true if this widget is hovered over.
        /// </summary>
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

        /// <summary>
        /// Returns true if this widget is being hovered over as well as on top of all the other widgets being hovered over.
        /// </summary>
        public bool IsPrimaryHovered => ParentWindow == null ? false : ParentWindow.HoveredWidgets.Primary == this;

        /// <summary>
        /// Returns true if this widget has been initialized graphically. (If this widget has not been graphically initialized, it cannot be drawn. Call InitializeGraphics() to initialize graphics.)
        /// </summary>
        public bool IsGraphicsInitialized => GraphicsDevice != null;

        /// <summary>
        /// The area of this widget relative to the parent window.
        /// </summary>
        public virtual RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary>
        /// The position of this widget relative to its window.
        /// </summary>
        public Point2 PositionInWindow => ParentWidget == null ? Position : Position.AddPoint2(ParentWidget.PositionInWindow);

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

        internal bool IsOwned => ParentWidget != null || ParentWindow != null;

        /// <summary>
        /// Area relative to the screen. (not the window)
        /// </summary>
        public RectangleF AreaOnScreen => ParentWindow == null ? new RectangleF() : new RectangleF(ParentWindow.Area.Location + AreaInWindow.Position.ToPoint(), Area.Size);

        public UIInputState InputState => ParentWindow?.InputState;

        public bool IsPrimarySelected
        {
            get
            {
                if (ParentWindow == null)
                {
                    return false;
                }

                return ParentWindow.SelectedWidgets.Primary == this;
            }
        }

        public bool IsSelected
        {
            get
            {
                if (ParentWindow == null)
                {
                    return false;
                }
                return ParentWindow.SelectedWidgets.IsWidgetFocused(this);
            }
        }

        protected bool EnterConfirms { get; set; } = true;

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
            local_render_target?.Dispose();
        }

        #endregion Constructors

        #region Public/Internal Methods

        // Nothing should be invoked in UpdatePriority.
        /// <summary>
        /// Updates this widget and all children recursively. This is called before Update() and updates all logic that should occur beforehand.
        /// </summary>
        /// <param name="game_time"></param>
        /// <param name="ui_input"></param>
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
            update_clicked = false;
            update_double_clicked = false;
            update_triple_clicked = false;
            update_added_to_focused = false;
            update_set_as_focused = false;
            update_hovered_over = false;

            if (ui_input.PrimaryClick && !_previous_clicking) { update_clicked = true; } else { update_clicked = false; } // Set clicked to only be true on the frame the 'mouse' clicks.
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
                update_hovered_over = true;

                if (update_clicked)
                {
                    if (ui_input.Control)  // Multi-select if the defined control is held down
                    {
                        if (!IsSelected)
                        {
                            update_added_to_focused = true;
                        }
                    }
                    else
                    {
                        if (!IsSelected)
                        {
                            update_set_as_focused = true;
                        }
                    }
                    if (_triple_click_countdown > 0)
                    {
                        _double_click_countdown = 0f;
                        _triple_click_countdown = 0f; // Do not allow consecutive triple clicks.
                        update_triple_clicked = true;
                    }
                    if (_double_click_countdown > 0)
                    {
                        _double_click_countdown = 0f; // Do not allow consecutive double clicks.
                        if (!IsSelected)
                        {
                            update_set_as_focused = true;
                        }
                        update_double_clicked = true;
                        _triple_click_countdown = _double_click_timing_backing;
                    }
                    _double_click_countdown = _double_click_timing_backing;
                }
            }
            else
            {
                update_hovered_over = false;
            }
        }
        
        /// <summary>
        /// Update this widget and all widgets contained.
        /// </summary>
        /// <param name="game_time"></param>
        /// <param name="ui_input"></param>
        internal void Update(GameTime game_time, UIInputState ui_input)
        {
            if (IsPrimaryHovered && ParentWindow.IsActive)
            {
                if (update_added_to_focused) AddToFocused();
                if (update_set_as_focused) SetAsFocused();
                if (update_clicked) OnClick?.Invoke(this, EventArgs.Empty);
                if (update_double_clicked) OnDoubleClick?.Invoke(this, EventArgs.Empty);
                if (update_triple_clicked) OnTripleClick?.Invoke(this, EventArgs.Empty);
            }

            if (ui_input.Enter && IsPrimarySelected && EnterConfirms)
            {
                OnConfirm?.Invoke(this, EventArgs.Empty);
            }

            IsHoveredOver = update_hovered_over;
            UpdatePalette(game_time);

            OnUpdate?.Invoke(this, EventArgs.Empty);

            foreach (Widget widget in Children)
            {
                widget.Update(game_time, ui_input);
            }
        }

        /// <summary>
        /// Draw the contents of this widget.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void Draw(SpriteBatch sprite_batch, RenderTarget2D render_target = null)
        {
            _graphics_in_use = true;
            if (_graphics_updating)
            {
                _graphics_in_use = false;
                return;
            }

            this.sprite_batch = sprite_batch;

            if (render_target == null) { render_target = local_render_target; }

            // Preserve the GraphicsDevice's previous render target (avoid unpredictable behavior)
            RenderTargetBinding[] previous_render_targets = GraphicsDevice.GetRenderTargets();

            // Set the graphics device to render to this widget's local render target.
            GraphicsDevice.SetRenderTarget(render_target);
            GraphicsDevice.Clear(Color.Transparent);

            if (DrawBackground)
            {
                sprite_batch.Draw(white_dot, AreaInWindow.ToRectangle(), BackgroundColor.CurrentColor);
            }

            GraphicsDevice.SetRenderTarget(local_render_target);
            
            foreach (Widget widget in Children)
            {
                widget.Draw(sprite_batch, local_render_target);
            }
            
            OnDraw?.Invoke(this, EventArgs.Empty);
            
            DrawOverlay(sprite_batch);

            // Restore the original render targets.
            GraphicsDevice.SetRenderTargets(previous_render_targets);
            _graphics_in_use = false;
        }

        /// <summary>
        /// Initializes all graphic related fields and updates all references.
        /// (To be used after a widget is created without parameters.)
        /// </summary>
        /// <param name="parent_window"></param>
        /// <param name="graphics_device"></param>
        /// <param name="sprite_font"></param>
        public void Initialize(DWindow parent_window, GraphicsDevice graphics_device, SpriteFont sprite_font = null)
        {
            InitializeAllReferences(parent_window, _parent_widget_reference); // The parent widget reference shouldn't be modified here
            InitializeGraphics();
        }

        // internal
        internal void InitializeGraphics()
        {
            if (GraphicsDevice == null)
            {
                throw new Exception($"{GetType().Name}.InitializeGraphics: GraphicsDevice cannot be null.");
            }

            white_dot = DrawingTools.WhiteDot(GraphicsDevice);
            if (SpriteFont == null)
            {
                SpriteFont = ParentWindow.DefaultSpriteFont;
            }

            foreach (Widget child in Children)
            {
                child.InitializeGraphics();
            }

            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// To be called post-serialization, this updates all references in this and all children.
        /// </summary>
        /// <param name="parent_window"></param>
        /// <param name="parent_widget"></param>
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

        /// <summary>
        /// Used by internal Focus object.
        /// </summary>
        internal void TriggerSelectOffEvent()
        {
            OnSelectOff?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Used by internal Focus object.
        /// </summary>
        internal void TriggerSelectEvent()
        {
            OnSelection?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Search for any methods in a DWindow for this to connect to.
        /// </summary>
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

        /// <summary>
        /// Signals confirming this widget. (Such as pressing enter with this widget selected)
        /// </summary>
        public void SignalConfirm()
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
        }

        #endregion Public/Internal Methods

        #region EventHandlers/Events

        #region EventsHandlers

        /// <summary>
        /// Invoked when this widget is clicked on.
        /// </summary>
        public event EventHandler OnClick;
        /// <summary>
        /// Invoked when this widget is double clicked.
        /// </summary>
        public event EventHandler OnDoubleClick;
        /// <summary>
        /// Invoked when this widget is triple clicked.
        /// </summary>
        public event EventHandler OnTripleClick;
        /// <summary>
        /// Invoked when the mouse hovers over this widget.
        /// </summary>
        public event EventHandler OnHover;
        /// <summary>
        /// Invoked when the mouse hovers off this widget.
        /// </summary>
        public event EventHandler OnHoverOff;
        /// <summary>
        /// Invoked after graphics have been initialized and are ready to use.
        /// </summary>
        public event EventHandler OnGraphicsInitialized;
        /// <summary>
        /// Invoked after this widget has been drawn.
        /// </summary>
        public event EventHandler OnDraw;
        /// <summary>
        /// Invoked after this widget updates.
        /// </summary>
        public event EventHandler OnUpdate;
        /// <summary>
        /// Invoked when this widget is selected.
        /// </summary>
        public event EventHandler OnSelection;
        /// <summary>
        /// Invoked when this widget is un-selected.
        /// </summary>
        public event EventHandler OnSelectOff;
        /// <summary>
        /// Invoked whwn the user confirms this widget (Such as pressing enter with this widget selected).
        /// </summary>
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

        /// <summary>
        /// Creates a new area taking the snapping_policy into account.
        /// </summary>
        /// <param name="encompassing_widget"></param>
        /// <returns></returns>
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

            Debug.WriteLine($"new_area = {new_area}");
            return new_area;
        }

        /// <summary>
        /// This widget plus all widgets contained in this widget.
        /// </summary>
        /// <returns></returns>
        public List<Widget> GetAllWidgets()
        {
            List<Widget> result = new List<Widget> { this };

            foreach (Widget child in Children)
            {
                foreach (Widget child_widget in child.GetAllWidgets())
                {
                    result.Add(child_widget);
                }
            }

            return result;
        }

        /// <summary>
        /// All widgets this widget owns.
        /// </summary>
        /// <returns></returns>
        public abstract List<Widget> Children { get; }

        /// <summary>
        /// Draw anything that should be drawn on top of the content in this widget.
        /// </summary>
        /// <param name="sprite_batch"></param>
        /// <param name="container"></param>
        protected void DrawOverlay(SpriteBatch sprite_batch)
        {
            if (DrawOutline)
            {
                RectangleF rectangle_f = AreaInWindow;
                Rectangle rectangle = new Rectangle((int)rectangle_f.X, (int)rectangle_f.Y, (int)rectangle_f.Width, (int)rectangle_f.Height);
                DrawingTools.DrawBorder(white_dot, sprite_batch, rectangle, OutlineThickness, OutlineColor.CurrentColor, OutlineSides);
            }
        }

        /// <summary>
        /// Set this widget as the only focused widget.
        /// </summary>
        protected void SetAsFocused() => _parent_window_reference?.SelectedWidgets.SetFocus(this);

        /// <summary>
        /// Add this widget to the group of selected widgets.
        /// </summary>
        protected void AddToFocused() => _parent_window_reference?.SelectedWidgets.AddFocus(this);
        
        /// <summary>
        /// A function called by a widget to update both itself and its parent's area.
        /// </summary>
        protected virtual void UpdateArea(bool update_parent)
        {
            if (IsGraphicsInitialized)
            {
                UpdateRenderTarget();
            }

            if (update_parent)
            {
                _parent_widget_reference?.UpdateArea(update_parent);
            }
        }

        /// <summary>
        /// Resize the render target to match the current area.
        /// </summary>
        private void UpdateRenderTarget()
        {
            UpdateRenderTarget(Area);
        }
        private void UpdateRenderTarget(RectangleF area)
        {
            if (Math.Max((int)area.Width, (int)area.Height) > _MAXIMUM_WIDGET_SIZE)
            {
                return;
            }
            //throw new Exception($"{GetType().Name}.LocalArea: Maximum Widget dimensions reached (maximum size is {_MAXIMUM_WIDGET_SIZE}, given dimensions are {area_backing}).");

            // Dispose of previous render target
            if (local_render_target != null)
            {
                local_render_target.Dispose();
                while (!local_render_target.IsDisposed) { }
            }

            local_render_target = new RenderTarget2D(
                GraphicsDevice,
                (int)area.Width,
                (int)area.Height,
                false,
                SurfaceFormat.Vector4,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.PreserveContents);
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
            ((Widget)c).OutlineOnMouseOver = OutlineOnMouseOver;
            ((Widget)c).OutlineThickness = OutlineThickness;
            ((Widget)c).DrawOutline = DrawOutline;
            ((Widget)c).SnappingPolicy = (DiagonalDirections2D)SnappingPolicy.Clone();
            ((Widget)c).OutlineSides = (Directions2D)OutlineSides.Clone();
            ((Widget)c).Centered = Centered;
            ((Widget)c).DoubleClickTiming = DoubleClickTiming;
            ((Widget)c).Spacing = Spacing;
            ((Widget)c).Area = Area;

            return c;
        }

        // This is for implementing cloning in derived classes. They'll return their
        // clone for their own fields, and 'Widget' will add the base fields to it.
        // See https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        protected abstract object DerivedClone();

        #endregion Cloning
    }
}