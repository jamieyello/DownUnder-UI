using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetElements;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

// tip;
// https://social.msdn.microsoft.com/Forums/en-US/08c5860e-1a04-40bd-9706-41d2a03066d3/expandcollapse-regions-all-at-once?forum=csharpide
//      Ctrl-M, Ctrl-O will collapse all of the code to its definitions.
//      Ctrl-M, Ctrl-L will expand all of the code (actually, this one toggles it).
//      Ctrl-M, Ctrl-M will expand or collapse a single region

// tip: Always remember to update Clone.

// DrawingArea doesn't have consistent size between drawing modes. (?)
// SpacedList is uneven.
// Improve DrawingExtensions._DrawCircleQuarter by making accuracy exponential
// Convert RectangleF.DistanceFrom to float
// Add ICloneable and INeedsParent to BehaviorCollection
// Wonder if Directions2D should be enum
// IsHoveredOver is antiquated

namespace DownUnder.UI.Widgets
{
    /// <summary> A visible window object. </summary>
    [DataContract] public sealed class Widget : 
        IParent, IDisposable, ICloneable, IList<Widget>
    {
        public bool debug_output = false;

        #region Fields/Delegates/Enums

        /// <summary> Used by some drawing code. </summary>
        private Texture2D _white_dot;
        /// <summary> The render target this Widget uses to draw to. </summary>
        private RenderTarget2D _render_target;
        private SpriteBatch _local_sprite_batch;
        private SpriteBatch _passed_sprite_batch;
        /// <summary> Used to track the period of time where a second click would be considered a double click. (If this value is > 0) </summary>
        private float _double_click_countdown = 0f;
        /// <summary> Used to track the period of time where a third click would be considered a triple click. (If this value is > 0) </summary>
        private float _triple_click_countdown = 0f;
        /// <summary> Used to tell whether the cursor moved or not when checking for double/triple clicks. </summary>
        private Point2 _previous_cursor_position = new Point2();
        /// <summary> Set to true internally to prevent usage of graphics while modifying them on another thread. </summary>
        private bool _graphics_updating = false;
        /// <summary> Set to true internally to prevent multi-threaded changes to graphics while their being used. </summary>
        private bool _graphics_in_use = false;
        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/>. </summary>
        private bool _dragging_in = false;
        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/> that has traveled outside this <see cref="Widget"/>'s area at some point. </summary>
        private bool _dragging_off = false;
        /// <summary> The area of this <see cref="Widget"/> before the user started resizing it. (If the user is resizing) </summary>
        private RectangleF _resizing_initial_area;
        Directions2D _resizing_direction;
        /// <summary> The initial position of the cursor before the user started resizing. (If the user is resizing) </summary>
        Point2 _repositioning_origin;
        private readonly WidgetUpdateFlags _post_update_flags = new WidgetUpdateFlags();
        /// <summary> The maximum size of a widget. (Every Widget uses a RenderTarget2D to render its contents to, this is the maximum resolution imposed by that.) </summary>
        private const int _MAXIMUM_WIDGET_SIZE = 2048;
        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        private const int _WAIT_TIME = 5;
        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        private const int _MAX_WAIT_TIME = 100;
        /// <summary> How far off the cursor can be from the edge of a <see cref="Widget"/> before it's set as a resize cursor. 20f is about the Windows default. </summary>
        private const float _USER_RESIZE_BOUNDS_SIZE = 20f;
        /// <summary> Used to prevent <see cref="Widget"/>s added mid-update from updating throughout the rest of the update cycle. </summary>
        private bool _has_updated = false;

        // The following are used by Update()/UpdatePriority().
        // They are set to true or false in UpdatePriority(), and Update() invokes events
        // by reading them.
        private bool _update_clicked_on;
        private bool _update_double_clicked;
        private bool _update_triple_clicked;
        private bool _update_added_to_focused;
        private bool _update_set_as_focused;
        private bool _update_hovered_over;
        private bool _update_drag;
        private bool _update_drop;

        // Various property backing fields.
        private RectangleF area_backing = new RectangleF();
        private float _double_click_timing_backing = 0.5f;
        private Point2 _minimum_area_backing = new Point2(15f, 15f);
        private SpriteFont _sprite_font_backing;
        private GraphicsDevice _graphics_backing;
        private bool _is_hovered_over_backing;
        private bool _previous_is_hovered_over_backing;
        private BaseColorScheme _theme_backing;
        private Widget _parent_widget_backing;
        private DWindow _parent_window_backing;
        private bool _allow_highlight_backing = false;
        Directions2D _allowed_resizing_directions_backing;
        bool _allow_delete_backing;
        bool _allow_copy_backing;
        bool _allow_cut_backing;
        UserResizePolicyType _user_resize_policy_backing = UserResizePolicyType.disallow;
        UserResizePolicyType _user_reposition_policy_backing = UserResizePolicyType.disallow;
        private bool _accepts_drops_backing;
        private List<Type> _accepted_drop_types_backing = new List<Type>();

        public enum DrawingModeType {
            /// <summary> Draw nothing. </summary>
            disable,
            /// <summary> Draw directly to the current <see cref="RenderTarget2D"/> without switching or clearing it. (default) </summary>
            direct,
            /// <summary> Draw this and all children to a private <see cref="RenderTarget2D"/> before drawing to the current <see cref="RenderTarget2D"/>. (Will clear the target unless disabled)</summary>
            use_render_target
        }

        public enum UserResizePolicyType {
            disallow,
            allow,
            require_highlight
        }

        #endregion

        #region Public/Internal Properties

        #region Auto properties

        /// <summary> All <see cref="Widget"/>s this <see cref="Widget"/> owns. </summary>
        [DataMember] public WidgetList Children { get; private set; } = new WidgetList();
        /// <summary> The name of this <see cref="Widget"/>. </summary>
        [DataMember] public string Name { get; set; }
        /// <summary> If set to true, colors will shift to their hovered colors on mouse-over. </summary>
        [DataMember] public bool ChangeColorOnMouseOver { get; set; } = true;
        /// <summary> If set to false, the background color will not be drawn. </summary>
        [DataMember] public bool DrawBackground { get; set; } = true;
        /// <summary> If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides) </summary>
        [DataMember] public bool DrawOutline { get; set; } = true;
        /// <summary> How this <see cref="Widget"/> should be drawn. Unless <see cref="RenderTarget2D"/>s are needed. direct = faster, use_render_target = needed for certain effects. </summary>
        [DataMember] public DrawingModeType DrawingMode { get; set; } = DrawingModeType.direct;
        /// <summary> How thick the outline should be. 1 by default. </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;
        /// <summary> Which sides of the outline are drawn (top, bottom, left, right) if <see cref="DrawOutline"/> is true. </summary>
        [DataMember] public Directions2D OutlineSides { get; set; } = Directions2D.UDLR;
        /// <summary> Represents the corners this <see cref="Widget"/> will snap to within the <see cref="IParent"/>. </summary>
        [DataMember] public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.None;
        /// <summary> The distance from the edges of the widget this is snapped to. </summary>
        [DataMember] public Size2 Spacing { get; set; }
        /// <summary> When set to true pressing enter while this <see cref="Widget"/> is the primarily selected one will trigger confirmation events. </summary>
        [DataMember] public bool EnterConfirms { get; set; } = true;
        /// <summary> What this <see cref="Widget"/> should be regarded as when accessing the <see cref="Theme"/>'s defined colors. </summary>
        [DataMember] public BaseColorScheme.PaletteCategory PaletteUsage { get; set; } = BaseColorScheme.PaletteCategory.default_widget;
        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Width"/>. </summary>
        [DataMember] public bool IsFixedWidth { get; set; } = false;
        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Height"/>. </summary>
        [DataMember] public bool IsFixedHeight { get; set; } = false;
        /// <summary> If set to true this <see cref="Widget"/> will passthrough all mouse input to it's parent. </summary>
        [DataMember] public bool PassthroughMouse { get; set; } = false;
        /// <summary> Used by <see cref="WidgetBehavior"/>s to tag <see cref="Widget"/>s with values. </summary>
        [DataMember] internal Dictionary<Type, Dictionary<string, string>> BehaviorTags { get; set; } = new Dictionary<Type, Dictionary<string, string>>();

        /// <summary> Contains all information relevant to updating on this frame. </summary>
        public UpdateData UpdateData { get; set; } = new UpdateData();
        /// <summary> The <see cref="SpriteBatch"/> currently used by this <see cref="Widget"/>. </summary>
        public SpriteBatch SpriteBatch { get => DrawingMode == DrawingModeType.direct ? _passed_sprite_batch : _local_sprite_batch; }
        public BehaviorCollection Behaviors { get; private set; }
        public ActionCollection Actions { get; private set; }
        public DesignerModeSettings DesignerObjects { get; set; }
        public BehaviorLibraryAccessor BehaviorLibrary { get; private set; } = new BehaviorLibraryAccessor();

        #endregion

        #region Non-auto properties

        /// <summary> Minimum time (in seconds) in-between two clicks needed for a double. </summary>
        [DataMember] public float DoubleClickTiming {
            get => _double_click_timing_backing;
            set {
                if (value > 0) { _double_click_timing_backing = value; }
                else { _double_click_timing_backing = 0f; }
            }
        }

        /// <summary> Area of this <see cref="Widget"/>. (Position relative to <see cref="IParent"/>) </summary>
        [DataMember] public RectangleF Area {
            get => area_backing;
            set {
                if (IsFixedWidth) value.Width = area_backing.Width;
                if (IsFixedHeight) value.Height = area_backing.Height;
                RectangleF previous_area = area_backing;
                area_backing = value.WithMinimumSize(MinimumSize);
                if (area_backing == previous_area) return;
                OnAreaChange?.Invoke(this, EventArgs.Empty);
                if (area_backing.Position != previous_area.Position) OnResposition?.Invoke(this, EventArgs.Empty);
                if (area_backing.Size != previous_area.Size) OnResize?.Invoke(this, EventArgs.Empty);
                if (EmbedChildren && Children != null) {
                    foreach (var child in Children) child.EmbedIn(area_backing);
                }
            }
        }

        /// <summary> Minimum size allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        [DataMember] public Point2 MinimumSize {
            get => _minimum_area_backing;
            set {
                if (value.X < 1) throw new Exception("Minimum area width must be at least 1.");
                if (value.Y < 1) throw new Exception("Minimum area height must be at least 1.");
                _minimum_area_backing = value;
                if (Area.WithMinimumSize(value) != Area) Area = Area.WithMinimumSize(value);
            }
        }

        /// <summary> The color palette of this <see cref="Widget"/>. </summary>
        [DataMember] public BaseColorScheme Theme {
            get => _theme_backing;
            set {
                _theme_backing = value;
                if (_theme_backing != null) _theme_backing.Parent = this;
            }
        }
        
        /// <summary> What sides are allowed to be resized when <see cref="AllowUserResize"/> is enabled. </summary>
        [DataMember] public Directions2D AllowedResizingDirections {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowedResizingDirections : _allowed_resizing_directions_backing;
            set => _allowed_resizing_directions_backing = value;
        }

        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can delete this <see cref="Widget"/> with the defined <see cref="UIInputState.Delete"/> or <see cref="UIInputState.BackSpace"/> (when highlighted). </summary>
        [DataMember] public bool AllowDelete {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowDelete : _allow_delete_backing;
            set => _allow_delete_backing = value;
        }        

        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can copy this <see cref="Widget"/> with the defined <see cref="UIInputState.Copy"/> (when highlighted). </summary>
        [DataMember] public bool AllowCopy {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowCopy : _allow_copy_backing;
            set => _allow_copy_backing = value;
        }      
        
        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can cut this <see cref="Widget"/> with the defined <see cref="UIInputState.Cut"/> (when highlighted). </summary>
        [DataMember] public bool AllowCut {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowCut : _allow_cut_backing;
            set => _allow_cut_backing = value;
        }

        /// <summary> Whether or not this <see cref="Widget"/> will accept drag and drops. </summary>
        // If developer mode is enabled, this implementation will forwarded to DeveloperObjects.
        [DataMember] public bool AcceptsDrops {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AcceptsDrops : _accepts_drops_backing;
            set => _accepts_drops_backing = value;
        }

        /// <summary> The <see cref="Type"/>s of <see cref="object"/>s this <see cref="Widget"/> will accept in a drag and drop. </summary>
        [DataMember] public List<Type> AcceptedDropTypes {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AcceptedDropTypes : _accepted_drop_types_backing;
            private set => _accepted_drop_types_backing = value;
        }

        #endregion

        #region Non-serialized properties

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
        
        /// <summary> Returns true if this <see cref="Widget"/> is hovered over. </summary>
        public bool IsHoveredOver {
            get => _is_hovered_over_backing;
            private set {
                _previous_is_hovered_over_backing = _is_hovered_over_backing;
                _is_hovered_over_backing = value;
                if (value && !_previous_is_hovered_over_backing) OnHover?.Invoke(this, EventArgs.Empty);
                if (!value && _previous_is_hovered_over_backing) OnHoverOff?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary> Returns true if this <see cref="Widget"/> is being hovered over as well as on top of all the other <see cref="Widget"/>s being hovered over. </summary>
        public bool IsPrimaryHovered => ParentWindow != null && ParentWindow.HoveredWidgets.Primary == this;
        /// <summary> Returns true if this <see cref="Widget"/> has been initialized graphically. Setting this <see cref="Widget"/>'s <see cref="Parent"/> to an initialized <see cref="Widget"/> or <see cref="DWindow"/> will initialize graphics. </summary>
        public bool IsGraphicsInitialized { get; private set; } = false;
        /// <summary> The area of this <see cref="Widget"/> relative to its window. </summary>
        public RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary> The position of this <see cref="Widget"/> relative to its window. </summary>
        public Point2 PositionInWindow {
            get {
                if (ParentWidget == null) { return Position; }
                return Position.WithOffset(ParentWidget.PositionInWindow).WithOffset(ParentWidget.Scroll);
            }
        }

        Point2 IParent.PositionInRender => PositionInRender;

        /// <summary> The position of this <see cref="Widget"/> relative to the <see cref="RenderTarget2D"/> being used. </summary>
        internal Point2 PositionInRender {
            get {
                if (DrawingMode == DrawingModeType.use_render_target) return Scroll;
                if (ParentWidget == null || ParentWidget.DrawingMode == DrawingModeType.use_render_target) return Position.WithOffset(Scroll);
                return Position.WithOffset(ParentWidget.PositionInRender).WithOffset(Scroll);
            }
        }

        /// <summary> The area this <see cref="Widget"/> should be drawing to. Using this while drawing ensures the proper position between drawing modes. </summary>
        public RectangleF DrawingArea => DrawingMode == DrawingModeType.use_render_target ? Area.SizeOnly() : Area.WithPosition(PositionInRender);
        /// <summary> <see cref="DrawingArea"/> without scroll offset. </summary>
        public RectangleF DrawingAreaUnscrolled => DrawingArea.WithOffset(Scroll.Inverted());

        /// <summary> The area of the screen where this <see cref="Widget"/> can be seen. </summary>
        //public RectangleF VisibleArea => ParentWidget == null ? Area : AreaInWindow.Intersection(ParentWidget.VisibleArea);
        public RectangleF VisibleArea {
            get {
                List<Widget> tree = Parents;
                tree.Insert(0, this);
                RectangleF result = Area;

                // start at the parent, go up the tree
                for (int i = 1; i < tree.Count; i++) {
                    result.Position = result.Position.WithOffset(tree[i].Position).WithOffset(tree[i].Scroll);
                    result = result.Intersection(tree[i].Area);
                }

                return result;
            }
        }

        public RectangleF VisibleDrawingArea {
            get {
                List<Widget> tree = RenderParents;
                tree.Insert(0, this);
                RectangleF result = tree[0].Area;

                // start at the parent, go up the tree
                for (int i = 1; i < tree.Count; i++) {
                    if (tree[i].DrawingMode != DrawingModeType.use_render_target)
                    {
                        result.Position = result.Position.WithOffset(tree[i].Position).WithOffset(tree[i].Scroll);
                        result = result.Intersection(tree[i].Area);
                    }
                    else
                    {

                    }
                    
                }

                // this fixes the inner stuff
                //if (DrawingMode == DrawingModeType.use_render_target) return result.SizeOnly();
                return result;
            }
        }

        /// <summary> Return a recursive list of all parents of this <see cref="Widget"/>, index 0 is the parent <see cref="Widget"/>. </summary>
        public List<Widget> Parents {
            get {
                List<Widget> parents = new List<Widget>();
                Widget parent = ParentWidget;
                while (parent != null) {
                    parents.Add(parent);
                    parent = parent.ParentWidget;
                }
                return parents;
            }
        }

        /// <summary> Returns a parent tree up to the first render target with this <see cref="Parent"/> being first. </summary>
        private List<Widget> RenderParents {
            get {
                List<Widget> parents = new List<Widget>();
                Widget parent = ParentWidget;
                while (parent != null) {
                    parents.Add(parent);
                    if (parent.DrawingMode == DrawingModeType.use_render_target) break;
                    parent = parent.ParentWidget;
                }
                return parents;
            }
        }

        /// <summary> Returns the parent of this <see cref="Widget"/>. </summary>
        public IParent Parent {
            get => ParentWidget != null ? (IParent)ParentWidget : ParentWindow;
            set {
                if (value is Widget widget) ParentWidget = widget;
                else if (value is DWindow window) ParentWindow = window;
            }
        }

        /// <summary> The <see cref="DWindow"/> that owns this <see cref="Widget"/>. </summary>
        public DWindow ParentWindow {
            get => _parent_window_backing;
            private set {
                _parent_window_backing = value;
                if (value != null) {
                    InitializeGraphics();
                    //ConnectEvents(value);
                }
                
                foreach (Widget child in Children) child.ParentWindow = value;
            }
        }

        /// <summary> The <see cref="Widget"/> that owns this <see cref="Widget"/>. (if one exists) </summary>
        public Widget ParentWidget {
            get => _parent_widget_backing;
            private set {
                if (value != _parent_widget_backing) _parent_widget_backing?.Remove(this);
                _parent_widget_backing = value;
                ParentWindow = value?.ParentWindow;
            }
        }

        /// <summary> Returns true if this <see cref="Widget"/> is owned by a parent. </summary>
        public bool IsOwned => Parent != null;
        /// <summary> Area relative to the screen. (not the window) </summary>
        public RectangleF AreaOnScreen => ParentWindow == null ? new RectangleF() : new RectangleF(ParentWindow.Area.Position + AreaInWindow.Position.ToPoint(), Area.Size);
        /// <summary> Represents the window's input each frame. </summary>
        public UIInputState InputState => ParentWindow?.InputState;
        /// <summary> Returns true if this is the main selected <see cref="Widget"/>. </summary>
        public bool IsPrimarySelected => ParentWindow != null && ParentWindow.SelectedWidgets.Primary == this;
        /// <summary>Returns true if this <see cref="Widget"/> is selected. </summary>
        public bool IsSelected => ParentWindow != null && ParentWindow.SelectedWidgets.IsWidgetFocused(this);

        /// <summary> This <see cref="Widget"/> plus all <see cref="Widget"/>s contained in this <see cref="Widget"/>. </summary>
        public WidgetList AllContainedWidgets {
            get {
                WidgetList result = new WidgetList { this };
                foreach (Widget child in Children) result.AddRange(child.AllContainedWidgets);
                return result;
            }
        }

        /// <summary> Gets the index of this <see cref="Widget"/> in its <see cref="ParentWidget"/>. </summary>
        public int Index => ParentWidget == null ? -1 : ParentWidget.Children.IndexOf(this);
        
        /// <summary> How many children deep this <see cref="Widget"/> is. </summary>
        public int Depth {
            get {
                int result = 0;
                Widget widget = this;
                while (widget.ParentWidget != null) {
                    widget = widget.ParentWidget;
                    result++;
                }

                return result;
            }
        }

        /// <summary> The <see cref="Microsoft.Xna.Framework.Graphics.SpriteFont"/> used by this <see cref="Widget"/>. If left null, the Parent of this <see cref="Widget"/>'s <see cref="Microsoft.Xna.Framework.Graphics.SpriteFont"/> will be used. </summary>
        public SpriteFont SpriteFont {
            get => Parent == null || _sprite_font_backing != null ? _sprite_font_backing : Parent.SpriteFont;
            set => _sprite_font_backing = value;
        }

        /// <summary> The <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> used by this <see cref="Widget"/>. If left null, the <see cref="Parent"/>'s <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> will be used. </summary>
        public GraphicsDevice GraphicsDevice {
            get => Parent == null || _graphics_backing != null ? _graphics_backing : Parent.GraphicsDevice;
            set => _graphics_backing = value;
        }

        /// <summary> Position of the cursor relative to this <see cref="Widget"/>. </summary>
        public Point2 CursorPosition {
            get {
                if (UpdateData.UIInputState == null) return new Point2();
                return UpdateData.UIInputState.CursorPosition - PositionInWindow - Scroll.ToVector2();
            }
        }

        /// <summary> When set to true this <see cref="Widget"/> will become highlighted when focused (in parent <see cref="DWindow.SelectedWidgets"/>). </summary>
        public bool AllowHighlight {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowHighlight : _allow_highlight_backing;
            set => _allow_highlight_backing = value;
        }

        public UserResizePolicyType UserResizePolicy {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.UserResizingPolicy : _user_resize_policy_backing;
            set => _user_resize_policy_backing = value;
        }

        public UserResizePolicyType UserRepositionPolicy {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.UserRepositionPolicy : _user_reposition_policy_backing;
            set => _user_reposition_policy_backing = value;
        }

        /// <summary> Returns true if this <see cref="Widget"/> is not only focused, but highlighted. </summary>
        public bool IsHighlighted => AllowHighlight && IsSelected;

        private bool _IsBeingResized => ParentWindow.UserResizeModeEnable && ParentWindow.ResizingWidget == this;

        #endregion

        #endregion

        #region Constructors/Destructors

        public Widget() => SetDefaults();

        private void SetDefaults() {
            Size = new Point2(10, 10);
            Theme = BaseColorScheme.Dark;
            Name = GetType().Name;
            Behaviors = new BehaviorCollection(this);
            Actions = new ActionCollection(this);
            DesignerObjects = new DesignerModeSettings();
            DesignerObjects.Parent = this;
            if (IsGraphicsInitialized) InitializeScrollbars(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeScrollbars;
            Children.OnAdd += (sender, args) => { OnAddChild?.Invoke(this, EventArgs.Empty); };
            Children.OnRemove += (sender, args) => { OnRemoveChild?.Invoke(this, EventArgs.Empty); };
            Children.OnListChange += (sender, args) => { OnListChange?.Invoke(this, EventArgs.Empty); };
            OnAddChild += (sender, args) => { LastAddedWidget.Parent = this; }; // ??
            OnRemoveChild += (sender, args) => { };
        }

        ~Widget() => Dispose(true);

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing) {
            OnDispose?.Invoke(this, EventArgs.Empty);
            _white_dot?.Dispose();
            _render_target?.Dispose();
            _local_sprite_batch?.Dispose();
            foreach (Widget child in Children) child.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public/Internal Methods

        public void Update(GameTime game_time, UIInputState input_state) {
            UpdateGroupUpdateData(game_time, input_state);
            UpdateGroupInput();
            UpdateGroupResizeGrab();
            UpdateGroupHoverFocus();
            UpdateGroupEvents(game_time);
            UpdateGroupPost(out _);
        }
        
        private void UpdateGroupHoverFocus() {
            if (_update_hovered_over) ParentWindow?.HoveredWidgets.AddFocus(this);
            foreach (Widget widget in Children) widget.UpdateGroupHoverFocus();
        }

        private void UpdateGroupUpdateData(GameTime game_time, UIInputState ui_input) {
            UpdateData.GameTime = game_time;
            UpdateData.ElapsedSeconds = game_time.GetElapsedSeconds();
            UpdateData.UIInputState = ui_input;
            _has_updated = true;
            foreach (Widget widget in Children) widget.UpdateGroupUpdateData(game_time, ui_input);
        }

        // Nothing should be invoked here. This chunk of code is meant to set values to be processed later.
        private void UpdateGroupInput() {
            _update_clicked_on = false;
            _update_double_clicked = false;
            _update_triple_clicked = false;
            _update_added_to_focused = false;
            _update_set_as_focused = false;
            _update_hovered_over = false;
            _update_drag = false;
            _update_drop = false;

            if (!UpdateData.UIInputState.PrimaryClick) {
                _dragging_in = false;
                if (_dragging_off) {
                    _dragging_off = false;
                    _update_drop = true;
                }
            }

            if (UpdateData.UIInputState.CursorPosition != _previous_cursor_position) {
                _double_click_countdown = 0f; // Do not allow double clicks where the cursor has been moved in-between clicks.
                _triple_click_countdown = 0f;
            }
            
            if (_double_click_countdown > 0f) _double_click_countdown -= UpdateData.ElapsedSeconds;

            if (VisibleArea.Contains(UpdateData.UIInputState.CursorPosition) && !PassthroughMouse) {
                _update_hovered_over = true;
                if (UpdateData.UIInputState.PrimaryClickTriggered) _update_clicked_on = true; // Set clicked to only be true on the frame the cursor clicks.
                _previous_cursor_position = UpdateData.UIInputState.CursorPosition;

                if (_update_clicked_on) {
                    _dragging_in = true;
                    if (UpdateData.UIInputState.Control) _update_added_to_focused = true;
                    else _update_set_as_focused = true;
                    if (_triple_click_countdown > 0) {
                        _double_click_countdown = 0f;
                        _triple_click_countdown = 0f; // Do not allow consecutive triple clicks.
                        _update_triple_clicked = true;
                    }
                    if (_double_click_countdown > 0) {
                        _double_click_countdown = 0f; // Do not allow consecutive double clicks.
                        _update_double_clicked = true;
                        _triple_click_countdown = _double_click_timing_backing;
                    }
                    _double_click_countdown = _double_click_timing_backing;
                }
            }
            else if (_dragging_in && !_dragging_off) {
                _dragging_off = true;
                _update_drag = true;
            }
            
            if (UserResizePolicy == UserResizePolicyType.allow || (UserResizePolicy == UserResizePolicyType.require_highlight && IsHighlighted)) {
                // Determining window resize
                if (!PassthroughMouse
                    && ParentWidget != null && ParentWidget.AreaInWindow.Contains(ParentWindow.InputState.CursorPosition)) {
                    Directions2D resize_grab = AreaInWindow.GetCursorHoverOnBorders(
                        ParentWindow.InputState.CursorPosition,
                        _USER_RESIZE_BOUNDS_SIZE
                        ) & AllowedResizingDirections;
                    
                    if (resize_grab != Directions2D.None) {
                        ParentWindow.ResizeCursorGrabber = this;
                        ParentWindow.ResizingDirections = resize_grab;
                    }
                }
            }
            if ((UserRepositionPolicy == UserResizePolicyType.allow
                || (UserRepositionPolicy == UserResizePolicyType.require_highlight && IsHighlighted))
                && VisibleArea.Contains(InputState.CursorPosition)) {
                ParentWindow.ResizeCursorGrabber = this;
                ParentWindow.ResizingDirections = Directions2D.UDLR;
            }

            if (IsHighlighted) {
                if (AllowDelete && (InputState.BackSpace || InputState.Delete)) _post_update_flags.Delete = true;
                if (AllowCopy && InputState.Copy) _post_update_flags.Copy = true;
                if (AllowCut && InputState.Cut) _post_update_flags.Cut = true;
            }

            foreach (Widget widget in Children) widget.UpdateGroupInput();
        }

        private void UpdateGroupResizeGrab() {
            if (
                !ParentWindow.UserResizeModeEnable
                && ParentWindow.ResizeCursorGrabber != this
                && !_IsBeingResized) {
                foreach (Widget widget in Children) widget.UpdateGroupResizeGrab();
                return;
            }

            // User has resize cursor over this widget or is in the middle of resizing
            if (UpdateData.UIInputState.PrimaryClickTriggered && !ParentWindow.UserResizeModeEnable) {
                if (ParentWindow.ResizingDirections != Directions2D.None) {
                    _resizing_direction = ParentWindow.ResizingDirections;
                    ParentWindow.UserResizeModeEnable = true;
                    _resizing_initial_area = Area;
                    _repositioning_origin = InputState.CursorPosition;
                }
            }
            
            if (!UpdateData.UIInputState.PrimaryClick) ParentWindow.UserResizeModeEnable = false;

            if (_IsBeingResized) {
                RectangleF new_area = _resizing_initial_area;
                Point2 amount = UpdateData.UIInputState.CursorPosition.WithOffset(_repositioning_origin.Inverted());
                if (_resizing_direction == Directions2D.UDLR) new_area = new_area.WithOffset(amount);
                else {
                    if (_resizing_direction & Directions2D.R) new_area = new_area.ResizedBy(amount.X, Directions2D.R);
                    if (_resizing_direction & Directions2D.D) new_area = new_area.ResizedBy(amount.Y, Directions2D.D);
                    if (_resizing_direction & Directions2D.U) new_area = new_area.ResizedBy(-amount.Y, Directions2D.U);
                    if (_resizing_direction & Directions2D.L) new_area = new_area.ResizedBy(-amount.X, Directions2D.L);
                }
                RectangleF new_area2 = new_area;
                //if (ParentWidget != null) new_area2 = 
                Area = new_area;
            }

            foreach (Widget widget in Children) widget.UpdateGroupResizeGrab();
        }

        /// <summary> Update this <see cref="Widget"/> and all <see cref="Widget"/>s contained. </summary>
        private void UpdateGroupEvents(GameTime game_time) {
            if (!_has_updated) return;
            // Skip some normal behavior if the user has the resize cursor over this widget
            if (!ParentWindow.UserResizeModeEnable
                && ParentWindow.ResizeCursorGrabber != this 
                && !_IsBeingResized) {
                if (IsPrimaryHovered) {
                    if (_update_added_to_focused) AddToFocused();
                    if (_update_set_as_focused) SetAsFocused();
                    if (_update_clicked_on) OnClick?.Invoke(this, EventArgs.Empty);
                    if (_update_double_clicked) OnDoubleClick?.Invoke(this, EventArgs.Empty);
                    if (_update_triple_clicked) OnTripleClick?.Invoke(this, EventArgs.Empty);
                }

                if (_update_drag) OnDrag?.Invoke(this, EventArgs.Empty);

                IsHoveredOver = _update_hovered_over;
            }

            if (_update_clicked_on) OnPassthroughClick?.Invoke(this, EventArgs.Empty);
            if (_update_double_clicked) OnPassthroughDoubleClick?.Invoke(this, EventArgs.Empty);
            if (_update_triple_clicked) OnPassthroughTripleClick?.Invoke(this, EventArgs.Empty);

            if (_IsBeingResized) SetAsFocused();
            if (_update_drop) OnDrop?.Invoke(this, EventArgs.Empty);
            if (InputState.Enter && IsPrimarySelected && EnterConfirms) OnConfirm?.Invoke(this, EventArgs.Empty);
            
            Theme.Update(game_time);
            ScrollBars.Update(UpdateData.ElapsedSeconds, UpdateData.UIInputState);
            Actions.UpdateQuedActions();
            OnUpdate?.Invoke(this, EventArgs.Empty);
            foreach (Widget widget in Children) widget.UpdateGroupEvents(game_time);
        }

        private void UpdateGroupPost(out bool deleted) {
            if (_post_update_flags.Delete) {
                ParentWidget.DeleteChild(this);
                deleted = true;
                return;
            }

            WidgetList children = Children;
            for (int i = 0; i < children.Count; i++) {
                children[i].UpdateGroupPost(out bool deleted_);
                if (deleted_) {
                    children = Children;
                    i--;
                }
            }
            deleted = false;
        }

        /// <summary> Draws this <see cref="Widget"/> (and all children) to the screen. </summary>
        public void Draw() {
            UpdateRenderTargetSizes();
            DrawTree();
            foreach (Widget child in AllContainedWidgets) child.DrawNoClip();
            return;
        }

        // https://github.com/jamieyello/DownUnder-UI/blob/master/Images/better_diagram.gif
        private void DrawTree() {
            WidgetList tree = WidgetExtensions.GetChildrenByDepth(this);
            bool swapped_render_target = false;
            RenderTargetBinding[] previous_render_targets = GraphicsDevice.GetRenderTargets();

            for (int i = tree.Count - 1; i >= 1; i--) {
                if (tree[i].DrawingMode == DrawingModeType.use_render_target) {
                    tree[i].Render();
                    swapped_render_target = true;
                }
            }

            if (swapped_render_target) GraphicsDevice.SetRenderTargets(previous_render_targets);
            _local_sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentWindow.RasterizerState);
            DrawDirect(_local_sprite_batch);
            _local_sprite_batch.End();
        }

        private void Render() {
            GraphicsDevice.SetRenderTarget(_render_target);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentWindow.RasterizerState);
            DrawContent();
            foreach (Widget child in Children) child.DrawDirect(SpriteBatch);
            DrawOverlay();
            SpriteBatch.End();
            DrawOverlayEffects(OnDrawOverlayEffects);
        }

        private void DrawDirect(SpriteBatch sprite_batch) {
            _passed_sprite_batch = sprite_batch;

            if (DrawingMode == DrawingModeType.use_render_target) {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentWindow.RasterizerState);
                if (Parent == null) SpriteBatch.Draw(_render_target, Position, Color.White);
                else SpriteBatch.Draw(_render_target, Position.WithOffset(Parent.PositionInRender), Color.White);
                SpriteBatch.End();
                return;
            }

            DrawContent();
            foreach (Widget child in Children) child.DrawDirect(SpriteBatch);
            DrawOverlay();
        }

        /// <summary> Draws content without altering render target or spritebatch. </summary>
        private void DrawContent() {
            Rectangle previous_scissor_area = new Rectangle();

            if (DrawingMode == DrawingModeType.direct) {
                previous_scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
                SpriteBatch.GraphicsDevice.ScissorRectangle = VisibleDrawingArea.ToRectangle();
            }
            if (DrawBackground) {
                if (DrawingMode == DrawingModeType.direct) SpriteBatch.FillRectangle(VisibleDrawingArea, IsHighlighted ? Color.Yellow : Theme.BackgroundColor.CurrentColor);
                else if (DrawingMode == DrawingModeType.use_render_target) GraphicsDevice.Clear(Theme.BackgroundColor.CurrentColor);
            }
            OnDraw?.Invoke(this, EventArgs.Empty);
            if (DrawingMode == DrawingModeType.direct) SpriteBatch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
        }

        /// <summary> Draw anything that should be drawn on top of the content in this <see cref="Widget"/> without altering render target or spritebatch. </summary>
        private void DrawOverlay() {
            Rectangle previous_scissor_area = new Rectangle();
            if (DrawingMode == DrawingModeType.direct) {
                previous_scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
                SpriteBatch.GraphicsDevice.ScissorRectangle = VisibleDrawingArea.ToRectangle();
            }
            if (DrawOutline) {
                DrawingTools.DrawBorder(
                    _white_dot,
                    SpriteBatch,
                    DrawingAreaUnscrolled.ToRectangle(),
                    OutlineThickness,
                    Theme.OutlineColor.CurrentColor,
                    OutlineSides
                    );
            }

            ScrollBars.Draw(SpriteBatch);
            OnDrawOverlay?.Invoke(this, EventArgs.Empty);
            if (DrawingMode == DrawingModeType.direct) SpriteBatch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
        }

        private void DrawOverlayEffects(EventHandler handler) {
            Delegate[] delegates = handler?.GetInvocationList();
            if (delegates == null) return;
            foreach (Delegate delegate_ in delegates) {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentWindow.RasterizerState);
                delegate_.DynamicInvoke(new object[] { this, EventArgs.Empty });
                SpriteBatch.End();
            }
        }

        private void DrawNoClip() {
            if (OnDrawNoClip == null) return;
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentWindow.RasterizerState);
            OnDrawNoClip?.Invoke(this, EventArgs.Empty);
            SpriteBatch.End();
            foreach (Widget child in Children) child.DrawNoClip();
        }

        /// <summary> Initializes all graphics related content. </summary>
        private void InitializeGraphics() {
            if (IsGraphicsInitialized) return;
            if (GraphicsDevice == null) throw new Exception($"GraphicsDevice cannot be null.");
            
            _local_sprite_batch = new SpriteBatch(GraphicsDevice);
            _white_dot = DrawingTools.WhiteDot(GraphicsDevice);
            IsGraphicsInitialized = true;
            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>  Used by internal <see cref="Focus"/> object. </summary>
        internal void TriggerSelectOffEvent() => OnSelectOff?.Invoke(this, EventArgs.Empty);

        /// <summary> Used by internal <see cref="Focus"/> object. </summary>
        internal void TriggerSelectEvent() => OnSelection?.Invoke(this, EventArgs.Empty);

        /// <summary> Disposes this <see cref="Widget"/> and removes it from its parent. </summary>
        /// <param name="now"> Set to true to delete this <see cref="Widget"/> on calling this, false to delete on next update. </param>
        public void Delete(bool now = false) {
            if (now) ParentWidget.DeleteChild(this);
            else _post_update_flags.Delete = true;
        }

        private void DeleteChild(Widget widget) {
            if (!Children.Contains(widget)) throw new Exception($"Given {nameof(Widget)} is not owned by this {nameof(Widget)}.");
            HandleChildDelete(widget);
        }

        /// <summary> Search for any methods in a <see cref="DWindow"/> for this to connect to. </summary>
        //public void ConnectEvents()
        //{
        //    System.Reflection.EventInfo[] events = GetType().GetEvents();
        //    for (int i = 0; i < events.GetLength(0); i++)
        //    {
        //        System.Reflection.EventInfo event_ = events[i];
        //        string method_name = "Slot_" + Name + "_On" + event_.Name;
        //        System.Reflection.MethodInfo window_method = ParentWindow.GetType().GetMethod(method_name);
        //        if (window_method == null)
        //        {
        //            continue;
        //        }
        //        Delegate handler = Delegate.CreateDelegate(
        //            event_.EventHandlerType,
        //            ParentWindow,
        //            window_method);
        //        event_.AddEventHandler(this, handler);
        //    }
        //}

        /// <summary> Signals confirming this <see cref="Widget"/>. (Such as pressing enter with this <see cref="Widget"/> selected) </summary>
        public void SignalConfirm() => OnConfirm?.Invoke(this, EventArgs.Empty);

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
        /// <summary> Invoked when this <see cref="Widget"/>'s overlay is drawn. Calls a new <see cref="SpriteBatch.Draw()"/> for each <see cref="Effect"/> applied here and draws a transparent rectangle. </summary>
        public event EventHandler OnDrawOverlayEffects;
        /// <summary> Invoked when this <see cref="Widget"/> is drawn to the buffer/parent <see cref="Widget"/>'s <see cref="RenderTarget2D"/>. <see cref="Widget.DrawingMode"/> must be set to <see cref="DrawingModeType.use_render_target"/> to use. </summary>
        public event EventHandler OnDrawRenderEffects;
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
        /// <summary> Invoked when this <see cref="Widget"/>'s size changes. </summary>
        public event EventHandler OnResize;
        /// <summary> Invoked when this <see cref="Widget"/>'s position changes. </summary>
        public event EventHandler OnResposition;
        /// <summary> Invoked when this <see cref="Widget"/>'s area changes. </summary>
        public event EventHandler OnAreaChange;
        /// <summary> Invoked when a <see cref="Widget"/> is added to this one. (See added <see cref="Widget"/> in <see cref="LastAddedWidget"/>.) </summary>
        public event EventHandler OnAddChild;
        /// <summary> Invoked when a child <see cref="Widget"/> is removed from this. (See removed <see cref="Widget"/> in <see cref="LastRemovedWidget"/>.) </summary>
        public event EventHandler OnRemoveChild;
        /// <summary> Invoked when a child is added or removed from this <see cref="Widget"/>. </summary>
        public event EventHandler OnListChange;
        /// <summary> Invoked when this <see cref="Widget"/> is disposed. </summary>
        public event EventHandler OnDispose;

        #endregion

        #region Private/Protected Methods

        /// <summary> Insert this <see cref="Widget"/> in a new <see cref="Widget"/> and return the container. </summary>
        /// <returns> The containing <see cref="Widget"/>. </returns>
        public Widget SendToContainer() => new Widget() { this };

        public Widget WithAddedBehavior(WidgetBehavior behavior) {
            Behaviors.Add(behavior);
            return this;
        }

        public Widget WithAddedBehavior(IEnumerable<WidgetBehavior> behaviors) {
            Behaviors.AddRange(behaviors);
            return this;
        }

        public Widget WithAddedAction(WidgetAction action) {
            Actions.Add(action);
            return this;
        }

        public Widget WithAddedAction(IEnumerable<WidgetAction> actions) {
            Actions.AddRange(actions);
            return this;
        }

        public void EmbedIn(IParent parent) {
            if (parent == null) return;
            EmbedIn(parent.Area);
        }

        /// <summary> Embeds this <see cref="Widget"/> in the given area. Takes <see cref="SnappingPolicy"/> and <see cref="Spacing"/> into account. </summary>
        internal void EmbedIn(RectangleF encompassing_area) {
            encompassing_area = encompassing_area.SizeOnly();
            RectangleF new_area = Area;

            // Convert the corners into up/down left/right to determine which walls the new area should stick to
            Directions2D snapping = SnappingPolicy.ToPerpendicular();
            
            if (snapping.Left && !snapping.Right) new_area.X = encompassing_area.X + Spacing.Width; // left
            if (!snapping.Left && snapping.Right) new_area.X = encompassing_area.X + encompassing_area.Width - new_area.Width - Spacing.Width; // right 
            if (snapping.Left && snapping.Right) { // left and right 
                new_area.X = encompassing_area.X + Spacing.Width;
                new_area.Width = encompassing_area.Width - Spacing.Width * 2;
            }
            if (snapping.Up && !snapping.Down) new_area.Y = encompassing_area.Y + Spacing.Height; // up
            if (!snapping.Up && snapping.Down) new_area.Y = encompassing_area.Y + encompassing_area.Height - new_area.Height - Spacing.Height;// down
            if (snapping.Up && snapping.Down) { // up and down
                new_area.Y = encompassing_area.Y + Spacing.Height;
                new_area.Height = encompassing_area.Height - Spacing.Height * 2;
            }

            Area = new_area.WithMinimumSize(MinimumSize);
        }

        /// <summary> Set this <see cref="Widget"/> as the only focused <see cref="Widget"/>. </summary>
        private void SetAsFocused() => ParentWindow?.SelectedWidgets.SetFocus(this);

        /// <summary> Add this <see cref="Widget"/> to the group of selected <see cref="Widget"/>s. </summary>
        private void AddToFocused() => ParentWindow?.SelectedWidgets.AddFocus(this);

        /// <summary> Called by a child <see cref="Widget"/> to signal that it's area has changed. </summary>
        //internal void SignalChildAreaChanged() => ParentWidget?.SignalChildAreaChanged();

        /// <summary> Resize the <see cref="RenderTarget2D"/> to match the current area. </summary>
        private void UpdateRenderTargetSizes() {
            if (DrawingMode == DrawingModeType.use_render_target) {
                UpdateRenderTargetSize(DrawingArea.Size);
            }

            foreach (Widget child in Children) child.UpdateRenderTargetSizes();
        }
        private void UpdateRenderTargetSize(Point2 size) {
            if (_render_target != null && (int)size.X == _render_target.Width && (int)size.Y == _render_target.Height) return;
            _graphics_updating = true;

            if (_graphics_in_use) {
                int i = 0;
                while (_graphics_in_use) {
                    i += _WAIT_TIME;
                    if (i > _MAX_WAIT_TIME) {
                        Console.WriteLine($"Hanging in UpdateRenderTarget waiting for graphics to not be in use.");
                    }
                    Thread.Sleep(_WAIT_TIME);
                }
            }

            if (size.X < 1) size.X = 1;
            if (size.Y < 1) size.Y = 1;

            if (Math.Max((int)size.X, (int)size.Y) > _MAXIMUM_WIDGET_SIZE) {
                size = size.Min(new Point2(_MAXIMUM_WIDGET_SIZE, _MAXIMUM_WIDGET_SIZE));
                Console.WriteLine($"DownUnder WARNING: Maximum Widget dimensions reached (maximum size is {_MAXIMUM_WIDGET_SIZE}, given dimensions are {size}). This may cause rendering issues.");
            }
            
            if (_render_target != null) {
                _render_target.Dispose();
                while (!_render_target.IsDisposed) { Thread.Sleep(10); }
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
            
            _graphics_updating = false;
        }

        #endregion

        public void HandleDrop(object drop) 
        {
            if (DesignerObjects.IsEditModeEnabled) 
            { 
                DesignerObjects.HandleDrop(drop);
                return;
            }
        }

        public bool IsDropAcceptable(object drop)
        {
            if (!AcceptsDrops) return false;
            foreach (Type type in AcceptedDropTypes)
            {
                if (type.IsAssignableFrom(drop?.GetType()))
                {
                    return true;
                }
            }
            return false;
        }

        #region ICloneable Implementation

        public object Clone() {
            Widget c = new Widget();
            for (int i = 0; i < Children.Count; i++) c.Children.Add((Widget)Children[i].Clone());
            c.FitToContentArea = FitToContentArea;

            c.Name = Name;
            c.ChangeColorOnMouseOver = ChangeColorOnMouseOver;
            c.DrawBackground = DrawBackground;
            c.Theme = (BaseColorScheme)Theme.Clone();
            c.OutlineThickness = OutlineThickness;
            c.DrawOutline = DrawOutline;
            c.EnterConfirms = EnterConfirms;
            c.MinimumSize = MinimumSize;
            c.SnappingPolicy = SnappingPolicy;
            c.OutlineSides = OutlineSides;
            c.DoubleClickTiming = DoubleClickTiming;
            c.Spacing = Spacing;
            c.Area = Area;
            c.IsFixedWidth = IsFixedWidth;
            c.IsFixedHeight = IsFixedHeight;
            c.PaletteUsage = PaletteUsage;
            c.DrawingMode = DrawingMode;
            c.debug_output = debug_output;
            c.PassthroughMouse = PassthroughMouse;

            c._accepts_drops_backing = _accepts_drops_backing;

            c._user_resize_policy_backing = _user_resize_policy_backing;
            c._user_reposition_policy_backing = _user_reposition_policy_backing;
            c._allowed_resizing_directions_backing = _allowed_resizing_directions_backing;
            c._allow_highlight_backing = _allow_highlight_backing;
            c._allow_delete_backing = _allow_delete_backing;
            c._allow_copy_backing = _allow_copy_backing;
            c._allow_cut_backing = _allow_cut_backing;

            foreach (Type type in _accepted_drop_types_backing) c._accepted_drop_types_backing.Add(type);
            foreach (WidgetBehavior behavior in Behaviors) c.Behaviors.Add((WidgetBehavior)behavior.Clone());

            return c;
        }

        #endregion

        #region Ilist

        public Scroll ScrollBars { get; private set; }
        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        [DataMember] public bool FitToContentArea { get; set; } = false;
        [DataMember] public bool EmbedChildren { get; set; } = true;

        public RectangleF ContentArea {
            get {
                RectangleF? result = Children.AreaCoverage?.Union(Area.SizeOnly());
                if (result == null) return Area.WithOffset(Scroll);
                return result.Value.WithOffset(Scroll);
            }
        }

        private void InitializeScrollbars(object sender, EventArgs args) => ScrollBars = new Scroll(this, GraphicsDevice);

        private void HandleChildDelete(Widget widget)
        {
            widget.Dispose();
            Children.Remove(widget);
        }

        public int IndexOf(Widget item) => Children.IndexOf(item);
        public void Insert(int index, Widget item) => Children.Insert(index, item);
        public void RemoveAt(int index) => Children.RemoveAt(index);
        public void Add(Widget item) => Children.Add(item);
        public void Clear() => Children.Clear();
        public bool Contains(Widget item) => Children.Contains(item);
        public void CopyTo(Widget[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        public bool Remove(Widget item) => Children.Remove(item);
        public IEnumerator<Widget> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
        public Point2 Scroll => ScrollBars.ToPoint2().Inverted();
        public int Count => Children.Count;
        public bool IsReadOnly => Children.IsReadOnly;
        public Widget this[int index] { get => Children[index]; set => Children[index] = value; }
        public Widget this[int x, int y] 
        {
            get
            {
                GridFormat grid = Behaviors.GetFirst<GridFormat>();
                if (grid == null) throw new Exception($"This {nameof(Widget)} does not have a {nameof(GridFormat)} behavior.");
                return grid[x, y];
            }
            set
            {
                GridFormat grid = Behaviors.GetFirst<GridFormat>();
                if (grid == null) throw new Exception($"This {nameof(Widget)} does not have a {nameof(GridFormat)} behavior.");
                grid[x, y] = value;
            }
        }
        public Widget LastAddedWidget => Children.LastAddedWidget;
        public Widget LastRemovedWidget => Children.LastRemovedWidget;
        
        #endregion
    }
}