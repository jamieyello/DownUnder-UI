using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

// Palettes should have ChangeColorOnHover, functionality should be removed from here.
// By current logic, should DrawMode be an auto-property?
// DrawingArea doesn't have consistent size between drawing modes.
// SignalChildAreaChanged should be called in base area only, _disable_update_area should be in base.
// SpacedList is uneven.
// Improve DrawingExtensions._DrawCircleQuarter by making accuracy exponential
// Grid dividers are broken
// Scrollbars are ugly
// Serialization code is scary
// Convert RectangleF.DistanceFrom to float

namespace DownUnder.UI.Widgets
{
    /// <summary> A visible window object. </summary>
    [DataContract] public abstract class Widget 
        : IParent, IDisposable, ICloneable, IAcceptsDrops
    {
        public bool debug_output = false;

        #region Fields/Delegates/Enums

        /// <summary> Used by some drawing code. </summary>
        private Texture2D _white_dot;

        /// <summary> The render target this Widget uses to draw to. </summary>
        private RenderTarget2D _render_target;

        /// <summary> Used to track the period of time where a second click would be considered a double click. (If this value is > 0) </summary>
        private float _double_click_countdown = 0f;

        /// <summary> Used to track the period of time where a third click would be considered a triple click. (If this value is > 0) </summary>
        private float _triple_click_countdown = 0f;

        /// <summary> Used to tell whether the cursor moved or not when checking for double/triple clicks. </summary>
        private Point2 _previous_cursor_position = new Point2();

        /// <summary> A cache of the previous scissor rectangle to check whether the scissor rectangle has to be updated or not. (Performance impact untested) </summary>
        private Rectangle _previous_scissor_rectangle;

        /// <summary> Set to true internally to prevent usage of graphics while modifying them on another thread. </summary>
        private bool _graphics_updating = false;

        /// <summary> Set to true internally to prevent multi-threaded changes to graphics while their being used. </summary>
        private bool _graphics_in_use = false;

        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/>. </summary>
        private bool _dragging_in = false;

        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/> that has traveled outside this <see cref="Widget"/>'s area at some point. </summary>
        private bool _dragging_off = false;

        /// <summary> Is true when the user is resizing this Widget. </summary>
        private bool _is_user_resizing = false;

        /// <summary> The sides (if any) the user is resizing. </summary>
        private Directions2D _resize_grab;

        /// <summary> The area of this <see cref="Widget"/> before the user started resizing it. (If the user is resizing) </summary>
        private RectangleF _resizing_initial_area;

        // why is there a second one?
        Directions2D _resizing_direction;

        /// <summary> The initial position of the cursor before the user started resizing. (If the user is resizing) </summary>
        Point2 _repositioning_origin;

        /// <summary> The maximum size of a widget. (Every Widget uses a RenderTarget2D to render its contents to, this is the maximum resolution imposed by that.) </summary>
        private const int _MAXIMUM_WIDGET_SIZE = 2048;

        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        private const int _WAIT_TIME = 5;

        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        private const int _MAX_WAIT_TIME = 100;

        /// <summary> How far off the cursor can be from the edge of a <see cref="Widget"/> before it's set as a resize cursor. 20f is about the Windows default. </summary>
        private const float _USER_RESIZE_BOUNDS_SIZE = 20f;

        // The following are used by Update()/UpdatePriority().
        // They are set to true or false in UpdatePriority(), and Update() invokes events
        // by reading them.
        private bool _update_clicked;
        private bool _update_double_clicked;
        private bool _update_triple_clicked;
        private bool _update_added_to_focused;
        private bool _update_set_as_focused;
        private bool _update_hovered_over;
        private bool _update_drag;
        private bool _update_drop;

        // Various property backing fields.
        protected RectangleF area_backing = new RectangleF();
        private float _double_click_timing_backing = 0.5f;
        private Point2 _minimum_area_backing = new Point2(1f, 1f);
        private SpriteFont _sprite_font_backing;
        private GraphicsDevice _graphics_backing;
        private bool _is_hovered_over_backing;
        private bool _previous_is_hovered_over_backing;
        private BaseColorScheme _theme_backing;
        private Widget _parent_widget_backing;
        private DWindow _parent_window_backing;
        private DrawingMode _draw_mode_backing = DrawingMode.direct;
        private SpriteBatch _local_sprite_batch_backing;
        private SpriteBatch _passed_sprite_batch_backing;
        private bool _allow_user_resizing_backing = false;

        public enum DrawingMode
        {
            /// <summary> Draw nothing. </summary>
            disable,
            /// <summary> Draw directly to the current <see cref="RenderTarget2D"/> without switching or clearing it. (default) </summary>
            direct,
            /// <summary> Draw this and all children to a private <see cref="RenderTarget2D"/> before drawing to the current <see cref="RenderTarget2D"/>. (Will clear the target unless disabled)</summary>
            use_render_target
        }

        #endregion

        #region Public/Internal Properties

        #region Auto properties

        /// <summary> The name of this <see cref="Widget"/>. </summary>
        [DataMember] public string Name { get; set; }

        /// <summary> If set to true, colors will shift to their hovered colors on mouse-over. </summary>
        [DataMember] public bool ChangeColorOnMouseOver { get; set; } = true;

        /// <summary> If set to false, the background color will not be drawn. </summary>
        [DataMember] public virtual bool DrawBackground { get; set; } = true;

        /// <summary> If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides) </summary>
        [DataMember] public bool DrawOutline { get; set; } = true;

        /// <summary> How thick the outline should be. 1 by default. </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;

        /// <summary> Which sides of the outline are drawn (top, bottom, left, right) if <see cref="DrawOutline"/> is true. </summary>
        [DataMember] public Directions2D OutlineSides { get; set; } = Directions2D.UpDownLeftRight;

        /// <summary> Represents the corners this <see cref="Widget"/> will snap to within the <see cref="IParent"/>. </summary>
        [DataMember] public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.TopRight_BottomLeft_TopLeft_BottomRight;

        /// <summary> Unimplemented. </summary>
        [DataMember] public bool Centered { get; set; }

        /// <summary> The distance from the edges of the widget this is snapped to. </summary>
        [DataMember] public Size2 Spacing { get; set; }

        /// <summary> When set to true pressing enter while this <see cref="Widget"/> is the primarily selected one will trigger confirmation events. </summary>
        [DataMember] public virtual bool EnterConfirms { get; set; } = true;

        /// <summary> What this <see cref="Widget"/> should be regarded as when accessing the <see cref="Theme"/>'s defined colors. </summary>
        [DataMember] public BaseColorScheme.PaletteCategory PaletteUsage { get; set; } = BaseColorScheme.PaletteCategory.default_widget;

        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Width"/>. </summary>
        [DataMember] public bool IsFixedWidth { get; set; } = false;

        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Height"/>. </summary>
        [DataMember] public bool IsFixedHeight { get; set; } = false;

        /// <summary> If set to true this <see cref="Widget"/> will passthrough all mouse input to it's parent. </summary>
        [DataMember] public bool PassthroughMouse { get; set; } = false;

        /// <summary> Whether or not this <see cref="Widget"/> will accept drag and drops. </summary>
        [DataMember] public bool AcceptsDrops { get; set; }

        /// <summary> The <see cref="Type"/>s of <see cref="object"/>s this <see cref="Widget"/> will accept in a drag and drop. </summary>
        [DataMember] public List<Type> AcceptedDropTypes { get; set; } = new List<Type>();

        /// <summary> Contains all information relevant to updating on this frame. </summary>
        public UpdateData UpdateData { get; set; } = new UpdateData();
        
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

        /// <summary> Area of this <see cref="Widget"/>. (Position relative to <see cref="IParent"/>) </summary>
        [DataMember] public virtual RectangleF Area
        {
            get => area_backing;
            set
            {
                if (IsFixedWidth) value.Width = area_backing.Width;
                if (IsFixedHeight) value.Height = area_backing.Height;
                area_backing = value.WithMinimumSize(MinimumSize);
            }
        }

        /// <summary> Minimum size allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
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

        /// <summary> The color palette of this <see cref="Widget"/>. </summary>
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

        /// <summary> How this <see cref="Widget"/> should be drawn. Unless <see cref="RenderTarget2D"/>s are needed, <see cref="DrawingMode.direct"/> should be used for performance. </summary>
        [DataMember] public DrawingMode DrawMode
        {
            get => _draw_mode_backing;
            set
            {
                _draw_mode_backing = value;
                foreach (Widget child in Children)
                {
                    child.DrawMode = value;
                }
            }
        }

        /// <summary> When enabled the user will able to resize this <see cref="Widget"/> with the cursor. </summary>
        [DataMember] public bool AllowUserResize
        {
            get => DeveloperObjects.IsEditModeEnabled ? DeveloperObjects.AllowUserResizing : _allow_user_resizing_backing;
            set => _allow_user_resizing_backing = value;
        }

        #endregion

        #region Derivatives of previous properties

        /// <summary> The width of this <see cref="Widget"/> (in terms of pixels on a 1080p monitor) </summary>
        public float Width { get => Area.Width; set => Area = Area.WithWidth(value); }
        /// <summary> The height of this <see cref="Widget"/> (in terms of pixels on a 1080p monitor) </summary>
        public float Height { get => Area.Height; set => Area = Area.WithHeight(value); }
        /// <summary> The position of this <see cref="Widget"/> in its <see cref="IParent"/> (in terms of pixels on a 1080p monitor) </summary>
        public Point2 Position { get => Area.Position; set => Area = Area.WithPosition(value); }
        /// <summary> The size of this <see cref="Widget"/> (in terms of pixels on a 1080p monitor) </summary>
        public Point2 Size { get => Area.Size; set => Area = Area.WithSize(value); }
        /// <summary> The x position of this <see cref="Widget"/> in its <see cref="IParent"/> (in terms of pixels on a 1080p monitor) </summary>
        public float X { get => Area.X; set => Area = Area.WithX(value); }
        /// <summary> The y position of this <see cref="Widget"/> in its <see cref="IParent"/> (in terms of pixels on a 1080p monitor) </summary>
        public float Y { get => Area.Y; set => Area = Area.WithY(value); }
        /// <summary> Minimum height allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumHeight { get => MinimumSize.Y; set => MinimumSize = MinimumSize.WithY(value); }
        /// <summary> Minimum width allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumWidth { get => MinimumSize.X; set => MinimumSize = MinimumSize.WithX(value); }

        #endregion

        #region Other various non-serialized properties

        /// <summary> Returns true if this <see cref="Widget"/> is hovered over. </summary>
        public bool IsHoveredOver
        {
            get => _is_hovered_over_backing;
            protected set
            {
                _previous_is_hovered_over_backing = _is_hovered_over_backing;
                _is_hovered_over_backing = value;
                if (value && !_previous_is_hovered_over_backing)
                {
                    OnHover?.Invoke(this, EventArgs.Empty);
                }

                if (!value && _previous_is_hovered_over_backing)
                {
                    OnHoverOff?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary> Returns true if this <see cref="Widget"/> is being hovered over as well as on top of all the other <see cref="Widget"/>s being hovered over. </summary>
        public bool IsPrimaryHovered => ParentWindow == null ? false : ParentWindow.HoveredWidgets.Primary == this;

        /// <summary> Returns true if this <see cref="Widget"/> has been initialized graphically. Setting this <see cref="Widget"/>'s <see cref="Parent"/> to an initialized <see cref="Widget"/> or <see cref="DWindow"/> will initialize graphics. </summary>
        public bool IsGraphicsInitialized { get; private set; } = false;

        /// <summary> The area of this <see cref="Widget"/> relative to its window. </summary>
        public virtual RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary> The position of this <see cref="Widget"/> relative to its window. </summary>
        public Point2 PositionInWindow
        {
            get
            {
                if (ParentWidget == null) { return Position; }
                if (ParentWidget is IScrollableWidget iscroll_parent)
                {
                    return Position.WithOffset(ParentWidget.PositionInWindow).WithOffset(iscroll_parent.Scroll);
                }
                return Position.WithOffset(ParentWidget.PositionInWindow);
            }
        }

        /// <summary> The area of the screen where this <see cref="Widget"/> can be seen. </summary>
        public RectangleF VisibleArea => ParentWidget == null ? Area : AreaInWindow.Intersection(ParentWidget.VisibleArea);

        /// <summary> The area this <see cref="Widget"/> should be drawing to. Using this while drawing ensures the proper position between drawing modes. </summary>
        public RectangleF DrawingArea => DrawMode == DrawingMode.use_render_target ? Area : AreaInWindow;

        /// <summary> Returns the parent of this <see cref="Widget"/>. </summary>
        public IParent Parent
        {
            get => ParentWidget != null ? 
                (IParent)ParentWidget :
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

        /// <summary> The <see cref="DWindow"/> that owns this <see cref="Widget"/>. </summary>
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

        /// <summary> The <see cref="Widget"/> that owns this <see cref="Widget"/>. (if one exists) </summary>
        public Widget ParentWidget
        {
            get => _parent_widget_backing;
            set
            {
                _parent_widget_backing = value;
                ParentWindow = value?.ParentWindow;
                if (value != null)
                {
                    value.DeveloperObjects.LastAddedChild = this;
                    DrawMode = value.DrawMode;
                }
            }
        }

        /// <summary> Returns true if this <see cref="Widget"/> is owned by a parent. </summary>
        public bool IsOwned => Parent != null;

        /// <summary> Area relative to the screen. (not the window) </summary>
        public RectangleF AreaOnScreen => ParentWindow == null ? new RectangleF() : new RectangleF(ParentWindow.Area.Position + AreaInWindow.Position.ToPoint(), Area.Size);

        /// <summary> Represents the window's input each frame. </summary>
        public UIInputState InputState => ParentWindow?.InputState;

        /// <summary> Returns true if this is the main selected <see cref="Widget"/>. </summary>
        public bool IsPrimarySelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.Primary == this;

        /// <summary>Returns true if this <see cref="Widget"/> is selected. </summary>
        public bool IsSelected => ParentWindow == null ? false : ParentWindow.SelectedWidgets.IsWidgetFocused(this);

        /// <summary> This <see cref="Widget"/> plus all <see cref="Widget"/>s contained in this <see cref="Widget"/>. </summary>
        public WidgetList AllContainedWidgets
        {
            get
            {
                WidgetList result = new WidgetList { this };

                foreach (Widget child in Children)
                {
                    result.ToList().AddRange(child.AllContainedWidgets);
                }

                return result;
            }
        }

        /// <summary> All <see cref="Widget"/>s this <see cref="Widget"/> owns. </summary>
        public abstract WidgetList Children { get; }

        /// <summary> The SpriteFont used by this <see cref="Widget"/>. If left null, the Parent of this Widget's SpriteFont will be used. </summary>
        public SpriteFont SpriteFont
        {
            get => Parent == null || _sprite_font_backing != null ? _sprite_font_backing : Parent.SpriteFont;
            set => _sprite_font_backing = value;
        }

        /// <summary> The GraphicsDevice used by this <see cref="Widget"/>. If left null, the <see cref="Parent"/>'s GraphicsDevice will be used. </summary>
        public GraphicsDevice GraphicsDevice
        {
            get => Parent == null || _graphics_backing != null ? _graphics_backing : Parent.GraphicsDevice;
            set => _graphics_backing = value;
        }

        /// <summary> Position of the cursor relative to this <see cref="Widget"/>. </summary>
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

        /// <summary> The <see cref="SpriteBatch"/> currently used by this <see cref="Widget"/>. </summary>
        public SpriteBatch SpriteBatch
        {
            get
            {
                if (DrawMode == DrawingMode.use_render_target) return _local_sprite_batch_backing;
                return _passed_sprite_batch_backing;
            }
        }

        public BehaviorCollection Behaviors { get; private set; }

        public DesignerModeTools DeveloperObjects { get; set; }

        #endregion

        #endregion

        #region Constructors/Destructors

        public Widget(IParent parent = null)
        {
            SetDefaults();
            Parent = parent;
        }

        private void SetDefaults()
        {
            Size = new Point2(10, 10);
            Theme = BaseColorScheme.Dark;
            Name = GetType().Name;
            Behaviors = new BehaviorCollection(this);
            DeveloperObjects = new DesignerModeTools();
            DeveloperObjects.Parent = this;
        }

        ~Widget()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            _white_dot?.Dispose();
            _render_target?.Dispose();
            _local_sprite_batch_backing?.Dispose();

            foreach (Widget child in Children)
            {
                child.Dispose(true);
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public/Internal Methods

        // Nothing should be invoked in UpdatePriority.
        /// <summary> Updates this <see cref="Widget"/> and all children recursively. This is called before <see cref="Update(GameTime, UIInputState)"/> and updates all logic that should occur beforehand. </summary>
        internal void UpdatePriority(GameTime game_time, UIInputState ui_input)
        {
            UpdateData.GameTime = game_time;
            UpdateData.ElapsedSeconds = game_time.GetElapsedSeconds();
            UpdateData.UIInputState = ui_input;

            UpdateCursorInput();
            
            foreach (Widget widget in Children)
            {
                widget.UpdatePriority(game_time, ui_input);
            }
        }
        
        // Nothing should be invoked here.
        private void UpdateCursorInput()
        {
            _update_clicked = false;
            _update_double_clicked = false;
            _update_triple_clicked = false;
            _update_added_to_focused = false;
            _update_set_as_focused = false;
            _update_hovered_over = false;
            _update_drag = false;
            _update_drop = false;

            if (UpdateData.UIInputState.CursorPosition != _previous_cursor_position)
            {
                _double_click_countdown = 0f; // Do not allow double clicks where the cursor has been moved in-between clicks.
                _triple_click_countdown = 0f;
            }
            
            if (_double_click_countdown > 0f)
            {
                _double_click_countdown -= UpdateData.ElapsedSeconds;
            }

            if (!UpdateData.UIInputState.PrimaryClick)
            {
                _dragging_in = false;
                if (_dragging_off)
                {
                    _dragging_off = false;
                    _update_drop = true;
                }
            }

            if (VisibleArea.Contains(UpdateData.UIInputState.CursorPosition) && !PassthroughMouse)
            {
                _update_hovered_over = true;

                if (UpdateData.UIInputState.PrimaryClickTriggered) { _update_clicked = true; } // Set clicked to only be true on the frame the cursor clicks.
                _previous_cursor_position = UpdateData.UIInputState.CursorPosition;

                if (_update_clicked) { _dragging_in = true; }

                if (_update_clicked)
                {
                    if (UpdateData.UIInputState.Control)  // Multi-select if the defined control is held down
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
                if (_dragging_in && !_dragging_off)
                {
                    _dragging_off = true;
                    _update_drag = true;
                }
            }

            // Resizing the window
            if (
                AllowUserResize
                && !PassthroughMouse
                && ParentWindow.ResizeGrabber == null
                && (ParentWidget == null || ParentWidget.AreaInWindow.Contains(ParentWindow.InputState.CursorPosition))
                )
            {
                Directions2D resize_grab = DrawingArea.GetCursorHoverOnBorders(
                    ParentWindow.InputState.CursorPosition,
                    _USER_RESIZE_BOUNDS_SIZE);
                if (resize_grab != Directions2D.None)
                {
                    ParentWindow.ResizeGrabber = this;
                    _resize_grab = resize_grab;
                }
            }
        }

        /// <summary> Update this <see cref="Widget"/> and all <see cref="Widget"/>s contained. </summary>
        internal void Update(GameTime game_time, UIInputState ui_input)
        {
            if (_update_hovered_over) ParentWindow?.HoveredWidgets.AddFocus(this);
            
            // Skip some normal behavior if the user has the resize cursor over this widget
            if (ParentWindow.ResizeGrabber != this && !ParentWindow.IsUserResizing)
            {
                if (IsPrimaryHovered && ParentWindow.IsActive)
                {
                    if (_update_added_to_focused) AddToFocused();
                    if (_update_set_as_focused) SetAsFocused();
                    if (_update_clicked) OnClick?.Invoke(this, EventArgs.Empty);
                    if (_update_double_clicked) OnDoubleClick?.Invoke(this, EventArgs.Empty);
                    if (_update_triple_clicked) OnTripleClick?.Invoke(this, EventArgs.Empty);
                }

                if (_update_clicked) OnPassthroughClick?.Invoke(this, EventArgs.Empty);
                if (_update_double_clicked) OnPassthroughDoubleClick?.Invoke(this, EventArgs.Empty);
                if (_update_triple_clicked) OnPassthroughTripleClick?.Invoke(this, EventArgs.Empty);

                if (_update_drag)
                {
                    OnDrag?.Invoke(this, EventArgs.Empty);
                }

                IsHoveredOver = _update_hovered_over;

            }
            else // User has resize cursor over this widget or is in the middle of resizing
            {
                if ((_resize_grab == Directions2D.UpOnly) || (_resize_grab == Directions2D.DownOnly)) { ParentWindow.UICursor = MouseCursor.SizeNS; }
                if ((_resize_grab == Directions2D.LeftOnly) || (_resize_grab == Directions2D.RightOnly)) { ParentWindow.UICursor = MouseCursor.SizeWE; }
                if ((_resize_grab == Directions2D.UpRight) || (_resize_grab == Directions2D.DownLeft)) { ParentWindow.UICursor = MouseCursor.SizeNESW; }
                if ((_resize_grab == Directions2D.UpLeft) || (_resize_grab == Directions2D.DownRight)) { ParentWindow.UICursor = MouseCursor.SizeNWSE; }

                if (_update_clicked && !ParentWindow.IsUserResizing)
                {
                    if (_resize_grab != Directions2D.None)
                    {
                        _resizing_direction = (Directions2D)_resize_grab.Clone();
                        ParentWindow.IsUserResizing = true;
                        _is_user_resizing = true;
                        _resizing_initial_area = Area;
                        _repositioning_origin = InputState.CursorPosition;
                    }
                }

                if (!_update_clicked)
                {
                    _is_user_resizing = false;
                    ParentWindow.IsUserResizing = false;
                }

                if (_is_user_resizing)
                {
                    RectangleF new_area = _resizing_initial_area;
                    Point2 amount = ui_input.CursorPosition.WithOffset(_repositioning_origin.Inverted());
                    if (_resizing_direction & Directions2D.RightOnly) { new_area = new_area.ResizedBy(amount.X, Directions2D.RightOnly); }
                    if (_resizing_direction & Directions2D.DownOnly) { new_area = new_area.ResizedBy(amount.Y, Directions2D.DownOnly); }
                    if (_resizing_direction & Directions2D.UpOnly) { new_area = new_area.ResizedBy(-amount.Y, Directions2D.UpOnly); }
                    if (_resizing_direction & Directions2D.LeftOnly) { new_area = new_area.ResizedBy(-amount.X, Directions2D.LeftOnly); }

                    Area = new_area;
                }
            }

            if (_update_drop)
            {
                OnDrop?.Invoke(this, EventArgs.Empty);
            }

            if (ui_input.Enter && IsPrimarySelected && EnterConfirms)
            {
                OnConfirm?.Invoke(this, EventArgs.Empty);
            }
            
            Theme.Update(game_time);
            if (this is IScrollableWidget scroll_widget)
            {
                scroll_widget.ScrollBars.Update(UpdateData.ElapsedSeconds, UpdateData.UIInputState);
            }

            OnUpdate?.Invoke(this, EventArgs.Empty);

            foreach (Widget widget in Children)
            {
                widget.Update(game_time, ui_input);
            }
        }

        /// <summary> Draws this <see cref="Widget"/> (and all children) to the screen. </summary>
        public void Draw(SpriteBatch sprite_batch = null)
        {
            switch (DrawMode)
            {
                case DrawingMode.direct:
                    DrawDirect(sprite_batch);
                    DrawNoClip();
                    break;

                case DrawingMode.use_render_target:
                    DrawUsingRenderTargets();
                    break;

                case DrawingMode.disable:
                    break;

                default:
                    throw new Exception("DrawingMode not supported in Draw.");
            }
        }
        
        private void DrawDirect(SpriteBatch sprite_batch)
        {
            RectangleF visible_area = VisibleArea;
            if (visible_area.Height == 0f || visible_area.Width == 0f) return;

            _graphics_in_use = true;
            if (_graphics_updating)
            {
                _graphics_in_use = false;
                return;
            }

            _passed_sprite_batch_backing = sprite_batch;

            if (DrawBackground)
            {
                sprite_batch.FillRectangle(visible_area, Theme.BackgroundColor.CurrentColor);
            }

            _previous_scissor_rectangle = sprite_batch.GraphicsDevice.ScissorRectangle;
            sprite_batch.GraphicsDevice.ScissorRectangle = visible_area.ToRectangle();

            OnDraw?.Invoke(this, EventArgs.Empty);

            foreach (Widget child in Children)
            {
                child.DrawDirect(sprite_batch);
            }

            sprite_batch.GraphicsDevice.ScissorRectangle = visible_area.ToRectangle();

            DrawOverlay(sprite_batch);

            sprite_batch.GraphicsDevice.ScissorRectangle = _previous_scissor_rectangle;
        }

        private void DrawNoClip()
        {
            OnDrawNoClip?.Invoke(this, EventArgs.Empty);

            foreach (Widget child in Children)
            {
                child.DrawNoClip();
            }
        }

        private void DrawUsingRenderTargets()
        {
            var previous_render_targets = GraphicsDevice.GetRenderTargets();
            Render();
            GraphicsDevice.SetRenderTargets(previous_render_targets);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_render_target, Area.ToRectangle(), Color.White);
            SpriteBatch.End();
        }

        /// <summary> Renders this <see cref="Widget"/> with all children inside. </summary>
        /// <returns> This <see cref="Widget"/>'s render. </returns>
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
                    areas.Add(child.Area.WithOffset(s_widget.Scroll.Inverted()));
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
                        areas[i].Position.WithOffset(s_this.Scroll).Floored(),
                        Color.White
                        );
                }
                else
                {
                    SpriteBatch.Draw
                        (
                        renders[i], 
                        areas[i].ToRectangle(),
                        Color.White
                        );
                }
                i++;
            }
            DrawOverlay(SpriteBatch);

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
            
            _local_sprite_batch_backing = new SpriteBatch(GraphicsDevice);
            _white_dot = DrawingTools.WhiteDot(GraphicsDevice);

            IsGraphicsInitialized = true;

            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>  Used by internal <see cref="Focus"/> object. </summary>
        internal void TriggerSelectOffEvent()
        {
            OnSelectOff?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Used by internal <see cref="Focus"/> object. </summary>
        internal void TriggerSelectEvent()
        {
            OnSelection?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Search for any methods in a <see cref="DWindow"/> for this to connect to. </summary>
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

        /// <summary> Signals confirming this <see cref="Widget"/>. (Such as pressing enter with this <see cref="Widget"/> selected) </summary>
        public void SignalConfirm()
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
        }

        public bool IsDropAcceptable(object drop)
        {
            IAcceptsDrops i_this = this;

            if (i_this.AcceptsDrops)
            {
                foreach (Type type in i_this.AcceptedDropTypes)
                {
                    if (type.IsAssignableFrom(drop?.GetType()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region EventsHandlers

        /// <summary> Invoked when this <see cref="Widget"/> is clicked on. </summary>
        public event EventHandler OnClick;
        /// <summary> Invoked when this <see cref="Widget"/> is double clicked. </summary>
        public event EventHandler OnDoubleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is triple clicked. </summary>
        public event EventHandler OnTripleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is clicked on. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughClick;
        /// <summary> Invoked when this <see cref="Widget"/> is double clicked. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughDoubleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is triple clicked. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughTripleClick;
        /// <summary> Invoked when the mouse hovers over this <see cref="Widget"/>. </summary>
        public event EventHandler OnHover;
        /// <summary> Invoked when the mouse hovers off this <see cref="Widget"/>. </summary>
        public event EventHandler OnHoverOff;
        /// <summary> Invoked after graphics have been initialized and are ready to use. </summary>
        public event EventHandler OnGraphicsInitialized;
        /// <summary> Invoked when this <see cref="Widget"/> is drawn. </summary>
        public event EventHandler OnDraw;
        /// <summary> Invoked when this <see cref="Widget"/>'s overlay is drawn. </summary>
        public event EventHandler OnDrawOverlay;
        /// <summary> Invoked when this <see cref="Widget"/> draws content outside of its area. </summary>
        public event EventHandler OnDrawNoClip;
        /// <summary> Invoked after this <see cref="Widget"/> updates.</summary>
        public event EventHandler OnUpdate;
        /// <summary> Invoked when this <see cref="Widget"/> is selected. </summary>
        public event EventHandler OnSelection;
        /// <summary> Invoked when this <see cref="Widget"/> is un-selected. </summary>
        public event EventHandler OnSelectOff;
        /// <summary> Invoked whwn the user confirms this <see cref="Widget"/> (Such as pressing enter with this <see cref="Widget"/> selected). </summary>
        public event EventHandler OnConfirm;
        /// <summary> Invoked when the user clicks and holds this <see cref="Widget"/> while leaving the area. (For drag and drop) </summary>
        public event EventHandler OnDrag;
        /// <summary> Invoked when the user releases the primary cursor button while "dragging and dropping". </summary>
        public event EventHandler OnDrop;
        internal event EventHandler OnAddWidgetSpacingChange;

        internal void SignalAddWidgetSpacingChange()
        {
            OnAddWidgetSpacingChange?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private/Protected Methods

        public void EmbedIn(IParent parent)
        {
            if (parent == null) return;
            EmbedIn(parent.Area);
        }

        /// <summary> Embeds this <see cref="Widget"/> in the given area. Takes <see cref="SnappingPolicy"/> and <see cref="Spacing"/> into account. </summary>
        internal void EmbedIn(RectangleF encompassing_area)
        {
            encompassing_area = encompassing_area.SizeOnly();
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
            
            Area = new_area;
        }

        /// <summary> Draw anything that should be drawn on top of the content in this <see cref="Widget"/>. </summary>
        private void DrawOverlay(SpriteBatch sprite_batch)
        {
            if (DrawOutline)
            {
                DrawingTools.DrawBorder(
                    _white_dot,
                    sprite_batch,
                    DrawingArea.ToRectangle(), 
                    OutlineThickness, 
                    Theme.OutlineColor.CurrentColor, 
                    OutlineSides
                    );
            }

            if (this is IScrollableWidget scroll_widget){
                scroll_widget.ScrollBars.Draw(sprite_batch);
            }

            OnDrawOverlay?.Invoke(this, EventArgs.Empty);
        }

        /// <summary> Set this <see cref="Widget"/> as the only focused <see cref="Widget"/>. </summary>
        protected void SetAsFocused() => ParentWindow?.SelectedWidgets.SetFocus(this);

        /// <summary> Add this <see cref="Widget"/> to the group of selected <see cref="Widget"/>s. </summary>
        protected void AddToFocused() => ParentWindow?.SelectedWidgets.AddFocus(this);

        /// <summary> Called by a child <see cref="Widget"/> to signal that it's area has changed. </summary>
        internal virtual void SignalChildAreaChanged()
        {
            ParentWidget?.SignalChildAreaChanged();
        }

        /// <summary> Resize the <see cref="RenderTarget2D"/> to match the current area. </summary>
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
                //Console.WriteLine($"\nUpdating {Name} Area\nPrevious: {_render_target?.Size().ToPoint()}\nNew: {size.ToPoint()}\n");
                if (Math.Max((int)size.X, (int)size.Y) > _MAXIMUM_WIDGET_SIZE)
                {
                    size = size.Min(new Point2(_MAXIMUM_WIDGET_SIZE, _MAXIMUM_WIDGET_SIZE));
                    Console.WriteLine($"DownUnder WARNING: Maximum Widget dimensions reached (maximum size is {_MAXIMUM_WIDGET_SIZE}, given dimensions are {size}). This may cause rendering issues.");
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

        #region IAcceptsDrops Implementation

        // If developer mode is enabled, this implementation will forwarded to DeveloperObjects.
        bool IAcceptsDrops.AcceptsDrops => DeveloperObjects.IsEditModeEnabled ? ((IAcceptsDrops)DeveloperObjects).AcceptsDrops : AcceptsDrops;
        List<Type> IAcceptsDrops.AcceptedDropTypes => DeveloperObjects.IsEditModeEnabled ? ((IAcceptsDrops)DeveloperObjects).AcceptedDropTypes : AcceptedDropTypes;
        bool IAcceptsDrops.IsDropAcceptable(object drop) => DeveloperObjects.IsEditModeEnabled ? ((IAcceptsDrops)DeveloperObjects).IsDropAcceptable(drop) : IsDropAcceptable(drop);
        void IAcceptsDrops.HandleDrop(object drop)
        {
            if (DeveloperObjects.IsEditModeEnabled) { ((IAcceptsDrops)DeveloperObjects).HandleDrop(drop); } else { HandleDrop(drop); }
        }

        void HandleDrop(object drop)
        {

        }

        #endregion

        #region ICloneable Implementation

        public object Clone()
        {
            object c = DerivedClone();
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
            ((Widget)c).DrawMode = DrawMode;
            ((Widget)c).debug_output = debug_output;
            ((Widget)c).PassthroughMouse = PassthroughMouse;
            ((Widget)c).AcceptsDrops = AcceptsDrops;
            ((Widget)c).AllowUserResize = AllowUserResize;
            foreach (Type type in AcceptedDropTypes) { ((Widget)c).AcceptedDropTypes.Add(type); }

            return c;
        }

        // This is for implementing cloning in derived classes. They'll return their
        // clone for their own fields, and 'Widget' will add the base fields to it.
        // See https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        protected abstract object DerivedClone();

        #endregion
    }
}