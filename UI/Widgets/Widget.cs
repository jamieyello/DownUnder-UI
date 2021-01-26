using DownUnder.Content.Utilities.Serialization;
using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
// IsHoveredOver is antiquated

namespace DownUnder.UI.Widgets
{
    /// <summary> A visible window object. </summary>
    [DataContract(IsReference = true)]
    public sealed partial class Widget : IParent, IDisposable, ICloneable, IEnumerable<Widget>
    {
        [DataMember]
        public bool debug_output = false;

        #region Fields/Delegates/Enums

        /// <summary> Used by some drawing code. </summary>
        public Texture2D _white_dot;
        /// <summary> The render target this Widget uses to draw to. </summary>
        RenderTarget2D _render_target;
        SpriteBatch _local_sprite_batch;
        SpriteBatch _passed_sprite_batch;
        /// <summary> Used to track the period of time where a second click would be considered a double click. (If this value is > 0) </summary>
        float _double_click_countdown;
        /// <summary> Used to track the period of time where a third click would be considered a triple click. (If this value is > 0) </summary>
        float _triple_click_countdown;
        /// <summary> Used to tell whether the cursor moved or not when checking for double/triple clicks. </summary>
        Point2 _previous_cursor_position;
        /// <summary> Set to true internally to prevent usage of graphics while modifying them on another thread. </summary>
        bool _graphics_updating;
        /// <summary> Set to true internally to prevent multi-threaded changes to graphics while their being used. </summary>
        bool _graphics_in_use;
        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/>. </summary>
        bool _dragging_in;
        /// <summary> Is true when the user is holding the mouse click that originated inside this <see cref="Widget"/> that has traveled outside this <see cref="Widget"/>'s area at some point. </summary>
        bool _dragging_off;
        /// <summary> The area of this <see cref="Widget"/> before the user started resizing it. (If the user is resizing) </summary>
        RectangleF _resizing_initial_area;
        Directions2D _resizing_direction;
        /// <summary> The initial position of the cursor before the user started resizing. (If the user is resizing) </summary>
        Point2 _repositioning_origin;
        internal WidgetPostUpdateFlags _post_update_flags;
        /// <summary> Used to prevent <see cref="Widget"/>s added mid-update from updating throughout the rest of the update cycle. </summary>
        bool _has_updated;
        /// <summary> Will help <see cref="OnFirstUpdate"/> trigger. </summary>
        bool _trigger_first_update = true;
        /// <summary> The following are used by Update()/UpdatePriority(). They are set to true or false in UpdatePriority(), and Update() invokes events by reading them. </summary>
        WidgetUpdateFlags _update_flags = new WidgetUpdateFlags();

        /// <summary> The maximum size of a widget. (Every Widget uses a RenderTarget2D to render its contents to, this is the maximum resolution imposed by that.) </summary>
        public static int MAXIMUM_WIDGET_SIZE = 2048;
        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        const int _WAIT_TIME = 5;
        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        const int _MAX_WAIT_TIME = 100;
        /// <summary> How far off the cursor can be from the edge of a <see cref="Widget"/> before it's set as a resize cursor. 20f is about the Windows default. </summary>
        const float _USER_RESIZE_BOUNDS_SIZE = 20f;

        // Various property backing fields.
        [DataMember] string _name_backing;
        [DataMember] RectangleF _area_backing;
        [DataMember] float _double_click_timing_backing;
        [DataMember] Point2 _minimum_size_backing;
        [DataMember] WidgetList _children_backing;
        SpriteFont _sprite_font_backing;
        GraphicsDevice _graphics_backing;
        bool _is_hovered_over_backing;
        bool _previous_is_hovered_over_backing;
        [DataMember] Widget _parent_widget_backing;
        DWindow _parent_window_backing;
        bool _allow_highlight_backing = false;
        [DataMember] Directions2D _allowed_resizing_directions_backing;
        bool _allow_delete_backing;
        bool _allow_copy_backing;
        bool _allow_cut_backing;
        [DataMember] UserResizePolicyType _user_resize_policy_backing = UserResizePolicyType.disallow;
        [DataMember] UserResizePolicyType _user_reposition_policy_backing = UserResizePolicyType.disallow;
        bool _accepts_drops_backing;
        List<SerializableType> _accepted_drop_types_backing = new List<SerializableType>();
        [DataMember] BehaviorManager _behaviors_backing;
        ActionManager _actions_backing;
        bool _fit_to_content_area_backing = false;

        public enum DrawingModeType
        {
            /// <summary> Draw nothing. </summary>
            disable,
            /// <summary> Draw directly to the current <see cref="RenderTarget2D"/> without switching or clearing it. (default) </summary>
            direct,
            /// <summary> Draw this and all children to a private <see cref="RenderTarget2D"/> before drawing to the current <see cref="RenderTarget2D"/>. (Will clear the target unless disabled)</summary>
            use_render_target
        }

        public enum UserResizePolicyType
        {
            disallow,
            allow,
            require_highlight
        }

        public enum RenderTargetResizeModeType
        {
            /// <summary> Keeps the render target at the maximum size at all times. Not recommended. </summary>
            maximum_size,
            /// <summary> Resizes the render target along with the <see cref="Widget"/>. A new render target will be created each frame the Widget is resized. </summary>
            auto_resize
        }

        #endregion

        #region Public/Internal Properties

        #region Auto properties

        /// <summary> How this <see cref="Widget"/> should be drawn. Unless <see cref="RenderTarget2D"/>s are needed. direct = faster, use_render_target = needed for certain effects. </summary>
        [DataMember]
        public DrawingModeType DrawingMode { get; set; } = DrawingModeType.direct;
        /// <summary> How the render target will be handled. </summary>
        [DataMember]
        public RenderTargetResizeModeType RenderTargetResizeMode { get; set; } = RenderTargetResizeModeType.auto_resize;
        [DataMember]
        public DiagonalDirections2D SnappingPolicy { get; set; } = DiagonalDirections2D.None;
        /// <summary> The distance from the edges of the widget this is snapped to. </summary>
        [DataMember]
        public Size2 Spacing { get; set; }
        /// <summary> When set to true pressing enter while this <see cref="Widget"/> is the primarily selected one will trigger confirmation events. </summary>
        [DataMember]
        public bool EnterConfirms { get; set; } = true;
        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Width"/>. </summary>
        [DataMember]
        public bool IsFixedWidth { get; set; } = false;
        /// <summary> While set to true this <see cref="Widget"/> will lock its current <see cref="Height"/>. </summary>
        [DataMember]
        public bool IsFixedHeight { get; set; } = false;
        /// <summary> If set to true this <see cref="Widget"/> will passthrough all mouse input to it's parent. </summary>
        [DataMember]
        public bool PassthroughMouse { get; set; } = false;
        /// <summary> Used by <see cref="WidgetBehavior"/>s to tag <see cref="Widget"/>s with values. </summary>
        [DataMember]
        internal AutoDictionary<SerializableType, AutoDictionary<string, string>> BehaviorTags;
        /// <summary> When set to false this <see cref="Widget"/> will throw an <see cref="Exception"/> if <see cref="Clone"/> is called. Should be set to false if <see cref="Clone"/> cannot recreate this <see cref="Widget"/> effectively. </summary>
        [DataMember]
        public bool IsCloningSupported { get; set; } = true;
        [DataMember]
        public GeneralVisualSettings VisualSettings = new GeneralVisualSettings();
        [DataMember] 
        public bool EmbedChildren { get; set; } = true;

        public BehaviorManager Behaviors
        {
            get => _behaviors_backing;
            private set
            {
                _behaviors_backing = value;
                if (value != null) value.Parent = this;
            }
        }

        /// <summary> Contains all information relevant to updating on this frame. </summary>
        public UpdateData UpdateData { get; set; }
        /// <summary> The <see cref="SpriteBatch"/> currently used by this <see cref="Widget"/>. </summary>
        public SpriteBatch SpriteBatch { get => DrawingMode == DrawingModeType.direct ? _passed_sprite_batch : _local_sprite_batch; }
        public DesignerModeSettings DesignerObjects { get; set; }
        public Point2 Scroll = new Point2();
        
        public ActionManager Actions
        {
            get => _actions_backing;
            private set
            {
                _actions_backing = value;
                if (value != null) value.Parent = this;
            }
        }

        /// <summary> Set to true while this <see cref="Widget"/> is deleting itself. </summary>
        public bool IsDeleting { get; private set; } = false;
        /// <summary> Set to true after this <see cref="Widget"/> has been deleted (as well as disposed) and is no longer in use. </summary>
        public bool IsDeleted { get; private set; } = false;

        /// <summary> All <see cref="Widget"/>s this <see cref="Widget"/> owns. </summary>
        //[DataMember] Serialized backing field
        public WidgetList Children
        {
            get => _children_backing;
            set
            {
                if (_children_backing != null) throw new Exception();
                _children_backing = value;
                if (value != null) value.Parent = this;
            }
        }

        #endregion

        #region Non-auto properties

        /// <summary> The name of this <see cref="Widget"/>. </summary>
        public string Name
        {
            get => _name_backing;
            set
            {
                if (_name_backing == value) return;
                string old_name = _name_backing;
                _name_backing = value;
                RenameArgs args = new RenameArgs(this, old_name, value);
                OnRename?.Invoke(this, args);
                InvokeOnRenameAny(args);
                if (args.QuietRename != null) _name_backing = args.QuietRename;
            }
        }

        /// <summary> Minimum time (in seconds) in-between two clicks needed for a double. </summary>
        public float DoubleClickTiming
        {
            get => _double_click_timing_backing;
            set
            {
                if (value > 0) { _double_click_timing_backing = value; }
                else { _double_click_timing_backing = 0f; }
            }
        }

        /// <summary> Area of this <see cref="Widget"/>. (Position relative to <see cref="IParent"/>) </summary>
        public RectangleF Area
        {
            get => _area_backing;
            set
            {
                if (IsFixedWidth) value.Width = _area_backing.Width;
                if (IsFixedHeight) value.Height = _area_backing.Height;
                RectangleF previous_area = _area_backing;
                _area_backing = value.WithMinimumSize(MinimumSize);
                if (_area_backing == previous_area) return;

                var response = new RectangleFSetOverrideArgs(previous_area);
                OnAreaChangePriority?.Invoke(this, response);
                if (response.Override != null)
                {
                    _area_backing = response.Override.Value.WithMinimumSize(MinimumSize);
                    if (_area_backing == previous_area) return;
                }

                OnAreaChange?.Invoke(this, new RectangleFSetArgs(previous_area));
                ParentWidget?.InvokeOnChildAreaChange(new RectangleFSetArgs(previous_area));
                if (_area_backing.Position != previous_area.Position)
                {
                    OnResposition?.Invoke(this, new RectangleFSetArgs(previous_area));
                    ParentWidget?.InvokeOnChildReposition(new RectangleFSetArgs(previous_area));
                }
                if (_area_backing.Size != previous_area.Size)
                {
                    OnResize?.Invoke(this, new RectangleFSetArgs(previous_area));
                    foreach (Widget child in Children.OrEmptyIfNull()) child.InvokeOnParentResize(new RectangleFSetArgs(previous_area));
                    ParentWidget?.InvokeOnChildResized(new RectangleFSetArgs(previous_area));
                }
                if (EmbedChildren)
                {
                    foreach (var child in Children.OrEmptyIfNull()) child.EmbedIn(_area_backing);
                }
            }
        }

        /// <summary> Minimum size allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public Point2 MinimumSize
        {
            get => _minimum_size_backing;
            set
            {
                if (value.X < 1) throw new Exception("Minimum area width must be at least 1.");
                if (value.Y < 1) throw new Exception("Minimum area height must be at least 1.");
                if (value == _minimum_size_backing) return;
                Point2 _previous_minimum_size = _minimum_size_backing;
                _minimum_size_backing = value;

                Point2SetOverrideArgs args = new Point2SetOverrideArgs(_previous_minimum_size);
                OnMinimumSizeSetPriority?.Invoke(this, args);
                if (args.Override != null) _minimum_size_backing = args.Override.Value;

                if (_minimum_size_backing == _previous_minimum_size) return;
                if (Area.WithMinimumSize(value) != Area) Area = Area.WithMinimumSize(value);
                if (_minimum_size_backing != _previous_minimum_size) OnMinimumSizeSet?.Invoke(this, new Point2SetArgs(_previous_minimum_size));
            }
        }

        /// <summary> What sides are allowed to be resized when <see cref="AllowUserResize"/> is enabled. </summary>
        public Directions2D AllowedResizingDirections
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowedResizingDirections : _allowed_resizing_directions_backing;
            set => _allowed_resizing_directions_backing = value;
        }

        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can delete this <see cref="Widget"/> with the defined <see cref="UIInputState.Delete"/> or <see cref="UIInputState.BackSpace"/> (when highlighted). </summary>
        [DataMember]
        public bool AllowDelete
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowDelete : _allow_delete_backing;
            set => _allow_delete_backing = value;
        }

        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can copy this <see cref="Widget"/> with the defined <see cref="UIInputState.Copy"/> (when highlighted). </summary>
        [DataMember]
        public bool AllowCopy
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowCopy : _allow_copy_backing;
            set => _allow_copy_backing = value;
        }

        /// <summary> When enabled (along with <see cref="AllowHighlight"/>), the user can cut this <see cref="Widget"/> with the defined <see cref="UIInputState.Cut"/> (when highlighted). </summary>
        [DataMember]
        public bool AllowCut
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowCut : _allow_cut_backing;
            set => _allow_cut_backing = value;
        }

        /// <summary> Whether or not this <see cref="Widget"/> will accept drag and drops. </summary>
        // If developer mode is enabled, this implementation will forwarded to DeveloperObjects.
        [DataMember]
        public bool AcceptsDrops
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AcceptsDrops : _accepts_drops_backing;
            set => _accepts_drops_backing = value;
        }

        /// <summary> The <see cref="Type"/>s of <see cref="object"/>s this <see cref="Widget"/> will accept in a drag and drop. </summary>
        [DataMember]
        public List<SerializableType> AcceptedDropTypes
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AcceptedDropTypes : _accepted_drop_types_backing;
            private set => _accepted_drop_types_backing = value;
        }

        /// <summary> When set to true this <see cref="Widget"/> will try to resize itself to contain all content. </summary>
        [DataMember]
        public bool FitToContentArea
        {
            get => _fit_to_content_area_backing;
            set
            {
                if (value && !_fit_to_content_area_backing && Children != null && Children.Count != 0) Size = ContentArea.Size;
                _fit_to_content_area_backing = value;
            }
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

        public float Top => Area.Top;
        public float Bottom => Area.Bottom;
        public float Left => Area.Left;
        public float Right => Area.Right;

        public Point2 TopLeft => Area.TopLeft;
        public Point2 TopRight => Area.TopRight;
        public Point2 BottomLeft => Area.BottomLeft;
        public Point2 BottomRight => Area.BottomRight;
        
        /// <summary> Minimum height allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumHeight { get => MinimumSize.Y; set => MinimumSize = MinimumSize.WithY(value); }
        /// <summary> Minimum width allowed when setting this <see cref="Widget"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public float MinimumWidth { get => MinimumSize.X; set => MinimumSize = MinimumSize.WithX(value); }
        /// <summary> <see cref="WidgetAction"/>s that must be completed before this <see cref="Widget"/> can be closed. </summary>
        public ActionSet ClosingActions { get; private set; } = new ActionSet();

        /// <summary> Returns true if this <see cref="Widget"/> is hovered over. </summary>
        public bool IsHoveredOver
        {
            get => _is_hovered_over_backing;
            private set
            {
                _previous_is_hovered_over_backing = _is_hovered_over_backing;
                _is_hovered_over_backing = value;
                if (value && !_previous_is_hovered_over_backing) OnHover?.Invoke(this, EventArgs.Empty);
                if (!value && _previous_is_hovered_over_backing) OnHoverOff?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary> Returns true if this <see cref="Widget"/> is being hovered over as well as on top of all the other <see cref="Widget"/>s being hovered over. </summary>
        public bool IsPrimaryHovered => ParentDWindow != null && ParentDWindow.HoveredWidgets.Primary == this;
        /// <summary> Returns true if this <see cref="Widget"/> has been initialized graphically. Setting this <see cref="Widget"/>'s <see cref="Parent"/> to an initialized <see cref="Widget"/> or <see cref="DWindow"/> will initialize graphics. </summary>
        public bool IsGraphicsInitialized { get; private set; } = false;
        /// <summary> The area of this <see cref="Widget"/> relative to its window. </summary>
        public RectangleF AreaInWindow => Area.WithPosition(PositionInWindow);

        /// <summary> The position of this <see cref="Widget"/> relative to its window. </summary>
        public Point2 PositionInWindow
        {
            get
            {
                if (ParentWidget == null) { return Position; }
                return Position.WithOffset(ParentWidget.PositionInWindow).WithOffset(ParentWidget.Scroll);
            }
        }

        Point2 IParent.PositionInRender => PositionInRender;

        /// <summary> The position of this <see cref="Widget"/> relative to the <see cref="RenderTarget2D"/> being used. </summary>
        internal Point2 PositionInRender
        {
            get
            {
                if (DrawingMode == DrawingModeType.use_render_target) return Scroll;
                if (ParentWidget == null || DrawingMode == DrawingModeType.use_render_target) return Position.WithOffset(Scroll);
                return Position.WithOffset(ParentWidget.PositionInRender).WithOffset(Scroll);
            }
        }

        public RectangleF ContentArea
        {
            get
            {
                RectangleF? result = Children.AreaCoverage?.Union(Area.SizeOnly());
                if (result == null) return Area.WithOffset(Scroll);
                return result.Value.WithOffset(Scroll);
            }
        }

        /// <summary> The area this <see cref="Widget"/> should be drawing to. Using this while drawing ensures the proper position between drawing modes. </summary>
        RectangleF DrawingArea => DrawingMode == DrawingModeType.use_render_target? Area: AreaInWindow;

        /// <summary> The area of the screen where this <see cref="Widget"/> can be seen. </summary>
        RectangleF VisibleArea
        {
            get
            {
                WidgetList tree = Parents;
                tree.Insert(0, this);
                RectangleF result = Area;

                // start at the parent, go up the tree
                for (int i = 1; i < tree.Count; i++)
                {
                    result.Position = result.Position.WithOffset(tree[i].Position).WithOffset(tree[i].Scroll);
                    result = result.Intersection(tree[i].Area);
                }

                return result;
            }
        }

        /// <summary> Return a recursive list of all parents of this <see cref="Widget"/>, index 0 is the parent <see cref="Widget"/>. </summary>
        public WidgetList Parents
        {
            get
            {
                WidgetList parents = new WidgetList();
                Widget parent = ParentWidget;
                while (parent != null)
                {
                    parents.Add(parent);
                    parent = parent.ParentWidget;
                }
                return parents;
            }
        }

        /// <summary> Returns the parent of this <see cref="Widget"/>. The parent <see cref="Widget"/> if one exists. </summary>
        public IParent Parent
        {
            get => ParentWidget != null ? (IParent)ParentWidget : ParentDWindow;
            set
            {
                if (value is Widget widget) ParentWidget = widget;
                else if (value is DWindow window) ParentDWindow = window;
                //else if (value == null) ParentWidget = null;
            }
        }

        /// <summary> The <see cref="DWindow"/> that owns this <see cref="Widget"/>. </summary>
        public DWindow ParentDWindow
        {
            get => _parent_window_backing;
            private set
            {
                if (_parent_window_backing == value) return;

                _parent_window_backing = value;
                bool initialized = false;
                if (value != null)
                {
                    initialized = InitializeGraphics();
                    OnParentWindowSet?.Invoke(this, EventArgs.Empty);
                    //ConnectEvents(value);
                }

                for (int i = 0; i < Children.Count; i++) Children[i].ParentDWindow = value;
                
                if (initialized) OnPostGraphicsInitialized?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary> The <see cref="Widget"/> that owns this <see cref="Widget"/>. (if one exists) </summary>
        public Widget ParentWidget
        {
            get => _parent_widget_backing;
            private set
            {
                if (value != _parent_widget_backing) _parent_widget_backing?.Remove(this);
                _parent_widget_backing = value;
                ParentDWindow = value?.ParentDWindow;
                if (value == null) return;
                foreach (GroupBehaviorPolicy policy in _parent_widget_backing.Behaviors.GroupBehaviors.InheritedPolicies)
                {
                    if (Behaviors.GroupBehaviors.AcceptancePolicy.IsBehaviorAllowed(this, policy)) Behaviors.TryAdd((WidgetBehavior)policy.Behavior.Clone());
                }
                OnParentWidgetSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public GameWindow Window => ParentGame?.Window;

        /// <summary> Returns true if this <see cref="Widget"/> is owned by a parent. </summary>
        public bool IsOwned => Parent != null;
        /// <summary> True if this <see cref="Widget"/> is the main widget in its current parent <see cref="DWindow"/>. </summary>
        public bool IsMainWindow => ParentDWindow?.MainWidget == this;
        /// <summary> Area relative to the screen. (not the window) </summary>
        public RectangleF AreaOnScreen => ParentDWindow == null ? new RectangleF() : new RectangleF(ParentDWindow.Area.Position + AreaInWindow.Position.ToPoint(), Area.Size);
        /// <summary> Represents the window's input each frame. </summary>
        public UIInputState InputState => ParentDWindow?.InputState;
        /// <summary> Returns true if this is the main selected <see cref="Widget"/>. </summary>
        public bool IsPrimarySelected => ParentDWindow != null && ParentDWindow.SelectedWidgets.Primary == this;
        /// <summary>Returns true if this <see cref="Widget"/> is selected. </summary>
        public bool IsSelected => ParentDWindow != null && ParentDWindow.SelectedWidgets.IsWidgetFocused(this);
        /// <summary> The <see cref="Game"/> this <see cref="Widget"/> belongs to. </summary>
        public Game ParentGame => ParentDWindow?.ParentGame;


        /// <summary> This <see cref="Widget"/> plus all <see cref="Widget"/>s contained in this <see cref="Widget"/>. </summary>
        public WidgetList AllContainedWidgets
        {
            get
            {
                WidgetList result = new WidgetList { this };
                foreach (Widget child in Children) result.AddRange(child.AllContainedWidgets);
                return result;
            }
        }

        /// <summary> Gets the index of this <see cref="Widget"/> in its <see cref="ParentWidget"/>. </summary>
        public int Index => ParentWidget == null ? -1 : ParentWidget.Children.IndexOf(this);

        /// <summary> How many children deep this <see cref="Widget"/> is. </summary>
        public int Depth
        {
            get
            {
                int result = 0;
                Widget widget = this;
                while (widget.ParentWidget != null)
                {
                    widget = widget.ParentWidget;
                    result++;
                }

                return result;
            }
        }

        /// <summary> The <see cref="SpriteFont"/> used by this <see cref="Widget"/>. If left null, the Parent of this <see cref="Widget"/>'s <see cref="Microsoft.Xna.Framework.Graphics.SpriteFont"/> will be used. </summary>
        public SpriteFont WindowFont
        {
            get => Parent == null || _sprite_font_backing != null ? _sprite_font_backing : Parent.WindowFont;
            set => _sprite_font_backing = value;
        }

        public ContentManager Content => ParentDWindow?.ParentGame.Content;

        /// <summary> The <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> used by this <see cref="Widget"/>. If left null, the <see cref="Parent"/>'s <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> will be used. </summary>
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
                return UpdateData.UIInputState.CursorPosition - PositionInWindow - Scroll.ToVector2();
            }
        }

        /// <summary> When set to true this <see cref="Widget"/> will become highlighted when focused (in parent <see cref="DWindow.SelectedWidgets"/>). </summary>
        public bool AllowHighlight
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.AllowHighlight : _allow_highlight_backing;
            set => _allow_highlight_backing = value;
        }

        public UserResizePolicyType UserResizePolicy
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.UserResizingPolicy : _user_resize_policy_backing;
            set => _user_resize_policy_backing = value;
        }

        public UserResizePolicyType UserRepositionPolicy
        {
            get => DesignerObjects.IsEditModeEnabled ? DesignerObjects.UserRepositionPolicy : _user_reposition_policy_backing;
            set => _user_reposition_policy_backing = value;
        }

        /// <summary> Returns true if this <see cref="Widget"/> is not only focused, but highlighted. </summary>
        public bool IsHighlighted => AllowHighlight && IsSelected;

        private bool _IsBeingResized => ParentDWindow.UserResizeModeEnable && ParentDWindow.ResizingWidget == this;

        private WidgetList AllRenderedWidgets
        {
            get
            {
                WidgetList result = AllContainedWidgets;
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    if (result[i].DrawingMode != DrawingModeType.use_render_target) result.RemoveAt(i);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region Constructors/Destructors

        public Widget() => SetDefaults();

        public Widget(RectangleF? area) : this()
        {
            if (area.HasValue) Area = area.Value;
        }

        public Widget(float x, float y, float width, float height) : this(new RectangleF(x, y, width, height)) { }
        public Widget(Point2 location, Point2 size) : this(new RectangleF(location, size)) { }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            SetNonSerialized();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            ///foreach (var b in Behaviors) b.Parent = this;
        }

        public void SetDefaults()
        {
            SetNonSerialized();

            _name_backing = "";
            _area_backing = new RectangleF();
            _double_click_timing_backing = 0.5f;
            _minimum_size_backing = new Point2(15f, 15f);
            _allowed_resizing_directions_backing = Directions2D.All;

            Size = new Point2(10, 10);
            //Theme = BaseColorScheme.Dark;
            Name = GetType().Name;
            BehaviorTags = new AutoDictionary<SerializableType, AutoDictionary<string, string>>();
            Behaviors = new BehaviorManager(this);
            Actions = new ActionManager(this);
            Children.Parent = this;
        }

        void SetNonSerialized()
        {
            _double_click_countdown = 0f;
            _triple_click_countdown = 0f;
            _previous_cursor_position = new Point2();
            _graphics_updating = false;
            _graphics_in_use = false;
            _dragging_in = false;
            _dragging_off = false;
            _has_updated = false;

            _post_update_flags = new WidgetPostUpdateFlags();
            _update_flags = new WidgetUpdateFlags();

            Children = new WidgetList(this);
            Actions = new ActionManager(this);
            UpdateData = new UpdateData();
            DesignerObjects = new DesignerModeSettings();
            DesignerObjects.Parent = this;
            OnAddChild += (s, a) =>
            {
                LastAddedWidget.EmbedIn(this);
                Behaviors.GroupBehaviors.ImplementPolicies();
            };
        }

        ~Widget() => Dispose(true);

        public void Dispose() => Dispose(true);

        void Dispose(bool disposing)
        {
            OnDispose?.Invoke(this, EventArgs.Empty);
            _white_dot?.Dispose();
            _render_target?.Dispose();
            _local_sprite_batch?.Dispose();
            foreach (Widget child in Children) child.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public/Internal Methods

        public void Update(GameTime game_time, UIInputState input_state)
        {
            if (!IsGraphicsInitialized) throw new Exception($"Cannot update a {nameof(Widget)} that's not been graphically initialized.");
            UpdateGroupUpdateData(game_time, input_state);
            UpdateGroupInput();
            UpdateGroupResizeGrab();
            UpdateGroupHoverFocus();
            UpdateGroupPreUpdate();
            UpdateGroupEvents(game_time);
            UpdateGroupPost(out _);
        }

        void UpdateGroupHoverFocus()
        {
            if (_update_flags._update_hovered_over) ParentDWindow?.HoveredWidgets.AddFocus(this);
            for (int i = 0; i < Children.Count; i++) Children[i].UpdateGroupHoverFocus();
        }

        void UpdateGroupPreUpdate()
        {
            OnPreUpdate?.Invoke(this, new PreUpdateArgs(_update_flags));
            for (int i = 0; i < Children.Count; i++) Children[i].UpdateGroupPreUpdate();
        }

        void UpdateGroupUpdateData(GameTime game_time, UIInputState ui_input)
        {
            UpdateData.GameTime = game_time;
            UpdateData.ElapsedSeconds = game_time.GetElapsedSeconds();
            UpdateData.SpeedModifier = UpdateData.ElapsedSeconds * 60f;
            UpdateData.UIInputState = ui_input;
            _has_updated = true;
            for (int i = 0; i < Children.Count; i++) Children[i].UpdateGroupUpdateData(game_time, ui_input);
        }

        // Nothing should be invoked here. This chunk of code is meant to set values to be processed later.
        void UpdateGroupInput()
        {
            _update_flags.Reset();

            if (!UpdateData.UIInputState.PrimaryClick)
            {
                _dragging_in = false;
                if (_dragging_off)
                {
                    _dragging_off = false;
                    _update_flags._update_drop = true;
                }
            }

            if (UpdateData.UIInputState.CursorPosition != _previous_cursor_position)
            {
                _double_click_countdown = 0f; // Do not allow double clicks where the cursor has been moved in-between clicks.
                _triple_click_countdown = 0f;
            }

            if (_double_click_countdown > 0f) _double_click_countdown -= UpdateData.ElapsedSeconds;

            if (VisibleArea.Contains(UpdateData.UIInputState.CursorPosition) && !PassthroughMouse)
            {
                _update_flags._update_hovered_over = true;
                if (UpdateData.UIInputState.PrimaryClickTriggered) _update_flags._update_clicked_on = true; // Set clicked to only be true on the frame the cursor clicks.
                if (UpdateData.UIInputState.SecondaryClickTriggered) _update_flags._update_right_clicked_on = true;
                _previous_cursor_position = UpdateData.UIInputState.CursorPosition;

                if (_update_flags._update_right_clicked_on) _update_flags._update_set_as_focused = true;
                if (_update_flags._update_clicked_on)
                {
                    _dragging_in = true;
                    if (UpdateData.UIInputState.Control) _update_flags._update_added_to_focused = true;
                    else _update_flags._update_set_as_focused = true;
                    if (_triple_click_countdown > 0)
                    {
                        _double_click_countdown = 0f;
                        _triple_click_countdown = 0f; // Do not allow consecutive triple clicks.
                        _update_flags._update_triple_clicked = true;
                    }
                    if (_double_click_countdown > 0)
                    {
                        _double_click_countdown = 0f; // Do not allow consecutive double clicks.
                        _update_flags._update_double_clicked = true;
                        _triple_click_countdown = _double_click_timing_backing;
                    }
                    _double_click_countdown = _double_click_timing_backing;
                }
            }
            else
            {
                if (UpdateData.UIInputState.PrimaryClickTriggered || UpdateData.UIInputState.SecondaryClickTriggered) _update_flags._update_clicked_off = true;
                if (_dragging_in && !_dragging_off)
                {
                    _dragging_off = true;
                    _update_flags._update_drag = true;
                }
            }

            if (UserResizePolicy == UserResizePolicyType.allow || (UserResizePolicy == UserResizePolicyType.require_highlight && IsHighlighted))
            {
                // Determining window resize
                if (!PassthroughMouse
                    && ParentWidget != null && ParentWidget.AreaInWindow.Contains(ParentDWindow.InputState.CursorPosition))
                {
                    Directions2D resize_grab = AreaInWindow.GetCursorHoverOnBorders(
                        ParentDWindow.InputState.CursorPosition,
                        _USER_RESIZE_BOUNDS_SIZE
                        ) & AllowedResizingDirections;

                    if (resize_grab != Directions2D.None)
                    {
                        ParentDWindow.ResizeCursorGrabber = this;
                        ParentDWindow.ResizingDirections = resize_grab;
                    }
                }
            }
            if ((UserRepositionPolicy == UserResizePolicyType.allow
                || (UserRepositionPolicy == UserResizePolicyType.require_highlight && IsHighlighted))
                && VisibleArea.Contains(InputState.CursorPosition))
            {
                ParentDWindow.ResizeCursorGrabber = this;
                ParentDWindow.ResizingDirections = Directions2D.UDLR;
            }

            if (IsHighlighted)
            {
                if (AllowDelete && (InputState.BackSpace || InputState.Delete)) _post_update_flags.Delete = true;
                if (AllowCopy && InputState.Copy) _post_update_flags.Copy = true;
                if (AllowCut && InputState.Cut) _post_update_flags.Cut = true;
            }

            for (int i = 0; i < Children.Count; i++) Children[i].UpdateGroupInput();
        }

        void UpdateGroupResizeGrab()
        {
            if (
                !ParentDWindow.UserResizeModeEnable
                && ParentDWindow.ResizeCursorGrabber != this
                && !_IsBeingResized)
            {
                for (int i = 0; i < Children.Count; i++) Children[i].UpdateGroupResizeGrab();
                
                return;
            }

            // User has resize cursor over this widget or is in the middle of resizing
            if (UpdateData.UIInputState.PrimaryClickTriggered && !ParentDWindow.UserResizeModeEnable)
            {
                if (ParentDWindow.ResizingDirections != Directions2D.None)
                {
                    _resizing_direction = ParentDWindow.ResizingDirections;
                    ParentDWindow.UserResizeModeEnable = true;
                    _resizing_initial_area = Area;
                    _repositioning_origin = InputState.CursorPosition;
                }
            }

            if (!UpdateData.UIInputState.PrimaryClick) ParentDWindow.UserResizeModeEnable = false;

            if (_IsBeingResized)
            {
                RectangleF new_area = _resizing_initial_area;
                Point2 amount = UpdateData.UIInputState.CursorPosition.WithOffset(_repositioning_origin.Inverted());
                if (_resizing_direction == Directions2D.UDLR) new_area = new_area.WithOffset(amount);
                else
                {
                    if (_resizing_direction & Directions2D.R) new_area = new_area.ResizedBy(amount.X, Directions2D.R, MinimumSize);
                    if (_resizing_direction & Directions2D.D) new_area = new_area.ResizedBy(amount.Y, Directions2D.D, MinimumSize);
                    if (_resizing_direction & Directions2D.U) new_area = new_area.ResizedBy(-amount.Y, Directions2D.U, MinimumSize);
                    if (_resizing_direction & Directions2D.L) new_area = new_area.ResizedBy(-amount.X, Directions2D.L, MinimumSize);
                }

                Area = new_area;
            }

            for (int i1 = 0; i1 < Children.Count; i1++) Children[i1].UpdateGroupResizeGrab();
        }

        /// <summary> Update this <see cref="Widget"/> and all <see cref="Widget"/>s contained. </summary>
        void UpdateGroupEvents(GameTime game_time)
        {
            if (!_has_updated) return;
            // Skip some normal behavior if the user has the resize cursor over this widget
            if (!ParentDWindow.UserResizeModeEnable
                && ParentDWindow.ResizeCursorGrabber != this
                && !_IsBeingResized)
            {
                if (IsPrimaryHovered)
                {
                    if (_update_flags._update_added_to_focused) AddToFocused();
                    if (_update_flags._update_set_as_focused) SetAsFocused();
                    if (_update_flags._update_clicked_on) OnClick?.Invoke(this, EventArgs.Empty);
                    if (_update_flags._update_right_clicked_on) OnRightClick?.Invoke(this, EventArgs.Empty);
                    if (_update_flags._update_double_clicked) OnDoubleClick?.Invoke(this, EventArgs.Empty);
                    if (_update_flags._update_triple_clicked) OnTripleClick?.Invoke(this, EventArgs.Empty);
                }

                if (_update_flags._update_drag) OnDrag?.Invoke(this, EventArgs.Empty);

                IsHoveredOver = _update_flags._update_hovered_over;
            }

            if (_update_flags._update_clicked_on) OnPassthroughClick?.Invoke(this, EventArgs.Empty);
            if (_update_flags._update_clicked_off) OnClickOff?.Invoke(this, EventArgs.Empty);
            if (_update_flags._update_double_clicked) OnPassthroughDoubleClick?.Invoke(this, EventArgs.Empty);
            if (_update_flags._update_triple_clicked) OnPassthroughTripleClick?.Invoke(this, EventArgs.Empty);

            if (_IsBeingResized) SetAsFocused();
            if (_update_flags._update_drop) OnDrop?.Invoke(this, EventArgs.Empty);
            if (InputState.Enter && IsPrimarySelected && EnterConfirms) OnConfirm?.Invoke(this, EventArgs.Empty);

            VisualSettings.Update(game_time.GetElapsedSeconds(), IsPrimaryHovered);
            Actions.UpdateQuedActions();
            OnUpdate?.Invoke(this, EventArgs.Empty);
            if (_trigger_first_update)
            {
                OnFirstUpdate?.Invoke(this, EventArgs.Empty);
                _trigger_first_update = false;
            }
            WidgetList list = new WidgetList(null, Children);
            for (int i = 0; i < list.Count; i++) list[i].UpdateGroupEvents(game_time);
        }

        void UpdateGroupPost(out bool deleted)
        {
            if (_post_update_flags.Delete)
            {
                if (!IsDeleting)
                {
                    IsDeleting = true;
                    Actions.Add(ClosingActions);
                }
                if (ClosingActions.IsCompleted)
                {
                    deleted = true;
                    IsDeleted = true;
                    OnDelete?.Invoke(this, EventArgs.Empty);
                    if (ParentWidget != null) ParentWidget.DeleteChild(this);
                    else ParentDWindow?.ParentGame.Exit();
                    return;
                }
            }

            if (_post_update_flags.SendToFront)
            {
                SendToFront(true);
                _post_update_flags.SendToFront = false;
            }
            if (_post_update_flags.SendToBack)
            {
                SendToBack(true);
                _post_update_flags.SendToBack = false;
            }
            _post_update_flags._updated = true;

            WidgetPostUpdateFlags.SetUpdatedFlagsToFalse(Children);
            do
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i]._post_update_flags._updated) continue;
                    Children[i].UpdateGroupPost(out bool deleted_);
                    if (deleted_)
                    {
                        i--;
                    }
                }
            } while (!WidgetPostUpdateFlags.UpdatedFlagsAreTrue(Children)); // This is to account for widgets re-ordering themselves, potentially skipping an update.

            deleted = false;
        }

        #region Drawing Code

        // It may be better that these are left here
        //RenderTarget2D ParentRender => ParentWindow._back_buffer; //ParentWidget == null ? null : ParentWidget.DrawingMode == DrawingModeType.use_render_target ? ParentWidget._render_target : null;
        WidgetDrawArgs DirectEventArgs(SpriteBatch sprite_batch = null) => new WidgetDrawArgs(this, GraphicsDevice, AreaInWindow, AreaInWindow, sprite_batch ?? SpriteBatch, InputState.CursorPosition);
        WidgetDrawArgs RenderTargetEventArgs => new WidgetDrawArgs(this, GraphicsDevice, Size.AsRectangleSize(), AreaInWindow, SpriteBatch, CursorPosition);

        public void Draw(SpriteBatch sprite_batch)
        {
            _passed_sprite_batch = sprite_batch;
            var previous_targets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(ParentDWindow.DrawTargetBuffer);
            UpdateRenderTargetSizes();
            if (PrepareRenders())
            {
                GraphicsDevice.SetRenderTargets(ParentDWindow.DrawTargetBuffer);
            }
            DrawFinal();
            DrawNoClip();
            GraphicsDevice.SetRenderTargets(previous_targets);
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, ParentDWindow.RasterizerState);
            sprite_batch.Draw(ParentDWindow.DrawTargetBuffer, ParentDWindow.Area.SizeOnly().ToRectangle(), Color.White);
            sprite_batch.End();
        }

        /// <summary>
        /// Prepare all render targets by drawing all widget content to them.
        /// </summary>
        /// <returns> True if the render target was changed. </returns>
        bool PrepareRenders()
        {
            bool rendered = false;
            foreach (Widget widget in AllRenderedWidgets)
            {
                rendered = true;
                GraphicsDevice.SetRenderTarget(widget._render_target);
                GraphicsDevice.Clear(Color.Transparent);
                widget.Render();
            }
            return rendered;
        }

        void Render()
        {
            _local_sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, ParentDWindow.RasterizerState);
            OnDrawBackground?.Invoke(this, RenderTargetEventArgs);
            OnDraw?.Invoke(this, RenderTargetEventArgs);
            _local_sprite_batch.End();
        }

        void DrawFinal()
        {
            // Draw BG/base content
            foreach (Widget widget in AllContainedWidgets)
            {
                Rectangle previous_scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
                SpriteBatch.GraphicsDevice.ScissorRectangle = widget.VisibleArea.ToRectangle();
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, ParentDWindow.RasterizerState);
                if (widget.DrawingMode == DrawingModeType.direct) widget.DrawDirect(SpriteBatch);
                if (widget.DrawingMode == DrawingModeType.use_render_target) SpriteBatch.Draw(widget._render_target, widget.AreaInWindow.ToRectangle(), new Rectangle(0, 0, (int)widget.Width, (int)widget.Height), Color.White);
                SpriteBatch.End();
                SpriteBatch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
            }

            // Draw overlay
            foreach (Widget widget in AllContainedWidgets)
            {
                Rectangle previous_scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
                SpriteBatch.GraphicsDevice.ScissorRectangle = widget.VisibleArea.ToRectangle();
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, ParentDWindow.RasterizerState);
                DrawOverlay(widget, widget.AreaInWindow, SpriteBatch);
                SpriteBatch.End();
                SpriteBatch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
            }
        }

        /// <summary> Draws content without altering render target or spritebatch. </summary>
        void DrawDirect(SpriteBatch sprite_batch)
        {
            _passed_sprite_batch = sprite_batch;

            if (DrawingMode == DrawingModeType.direct)
            {
                Rectangle previous_scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
                //SpriteBatch.GraphicsDevice.ScissorRectangle = VisibleArea.ToRectangle();
                OnDrawBackground?.Invoke(this, DirectEventArgs());
                OnDraw?.Invoke(this, DirectEventArgs());
                SpriteBatch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
            }
        }

        /// <summary> Draw anything that should be drawn on top of the content in this <see cref="Widget"/> without altering render target or spritebatch. </summary>
        static void DrawOverlay(Widget widget, RectangleF drawing_area, SpriteBatch sprite_batch)
        {
            Rectangle previous_scissor_area = new Rectangle();
            if (widget.DrawingMode == DrawingModeType.direct)
            {
                previous_scissor_area = sprite_batch.GraphicsDevice.ScissorRectangle;
                sprite_batch.GraphicsDevice.ScissorRectangle = drawing_area.ToRectangle();
            }

            widget.InvokeDrawOverlay(widget.DirectEventArgs(sprite_batch));
            if (widget.DrawingMode == DrawingModeType.direct) sprite_batch.GraphicsDevice.ScissorRectangle = previous_scissor_area;
        }

        void DrawNoClip()
        {
            if (OnDrawNoClip == null) return;
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, ParentDWindow.RasterizerState);
            OnDrawNoClip?.Invoke(this, EventArgs.Empty);
            SpriteBatch.End();
            for (int i = 0; i < Children.Count; i++) Children[i].DrawNoClip();
        }

        #endregion

        /// <summary> Initializes all graphics related content. </summary>
        bool InitializeGraphics()
        {
            if (IsGraphicsInitialized) return false;
            if (GraphicsDevice == null) throw new Exception($"GraphicsDevice cannot be null.");

            _local_sprite_batch = new SpriteBatch(GraphicsDevice);
            _white_dot = DrawingTools.WhiteDot(GraphicsDevice);
            IsGraphicsInitialized = true;
            OnGraphicsInitialized?.Invoke(this, EventArgs.Empty);
            UpdateRenderTargetSize(Size);
            return true;
        }

        /// <summary> Disposes this <see cref="Widget"/> and removes it from its parent. </summary>
        /// <param name="now"> Set to true to delete this <see cref="Widget"/> on calling this, false to delete on next update. </param>
        public void Delete(bool now = false)
        {
            if (now)
            {
                ParentWidget.DeleteChild(this);
                OnDelete?.Invoke(this, EventArgs.Empty);
            }
            else _post_update_flags.Delete = true;
        }

        void DeleteChild(Widget widget)
        {
            if (!Children.Contains(widget)) throw new Exception($"Given {nameof(Widget)} is not owned by this {nameof(Widget)}.");
            HandleChildDelete(widget);
        }

        void HandleChildDelete(Widget widget)
        {
            widget.Dispose();
            Children.Remove(widget);
        }

        public void SaveToXML(string path)
        {
            Widget parent_widget = _parent_widget_backing;
            _parent_widget_backing = null;
            XmlHelper.ToXmlFile(this, path, WidgetBehavior.KnownTypes);
            _parent_widget_backing = parent_widget;
        }

        public static Widget LoadFromXML(string path)
        {
            Widget result = XmlHelper.FromXmlFile<Widget>(path, WidgetBehavior.KnownTypes);
            //throw new Exception();
            
            foreach (Widget w in result.AllContainedWidgets)
            {
                foreach (var b in w.Behaviors) b.Parent = w;
            }
            return result;
        }

        #endregion

        #region EventsHandlers

        /// <summary> Invoked when this <see cref="Widget"/>'s <see cref="Name"/> changes. </summary>
        public event EventHandler<RenameArgs> OnRename;
        /// <summary> Invoked when this <see cref="Widget"/>'s or any of its children <see cref="Name"/> changes. </summary>
        public event EventHandler<RenameArgs> OnRenameAny;
        /// <summary> Invoked when this <see cref="Widget"/> is clicked on. </summary>
        public event EventHandler OnClick;
        /// <summary> Invoked when this <see cref="Widget"/> is double clicked. </summary>
        public event EventHandler OnDoubleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is triple clicked. </summary>
        public event EventHandler OnTripleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is triple clicked. </summary>
        public event EventHandler OnRightClick;
        /// <summary> Invoked when this <see cref="Widget"/> is clicked on. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughClick;
        /// <summary> Invoked when this <see cref="Widget"/> is double clicked. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughDoubleClick;
        /// <summary> Invoked when this <see cref="Widget"/> is triple clicked. Triggers even if it's not the primary <see cref="Widget"/>. </summary>
        public event EventHandler OnPassthroughTripleClick;
        /// <summary> Invoked when the user clicks on something other than this <see cref="Widget"/>. </summary>
        public event EventHandler OnClickOff;
        /// <summary> Invoked when the mouse hovers over this <see cref="Widget"/>. </summary>
        public event EventHandler OnHover;
        /// <summary> Invoked when the mouse hovers off this <see cref="Widget"/>. </summary>
        public event EventHandler OnHoverOff;
        /// <summary> Invoked after graphics have been initialized and are ready to use. </summary>
        public event EventHandler OnGraphicsInitialized;
        /// <summary> Invoked after the this <see cref="Widget"/> and its children have been initialized. </summary>
        public event EventHandler OnPostGraphicsInitialized;
        /// <summary> Invoked when this <see cref="Widget"/>'s base content is drawn. </summary>
        public event EventHandler<WidgetDrawArgs> OnDraw;
        /// <summary> Invoked when this <see cref="Widget"/>'s overlay is drawn. Contents are drawn over child <see cref="Widget"/>s. </summary>
        public event EventHandler<WidgetDrawArgs> OnDrawOverlay;
        /// <summary> Called before <see cref="OnDraw"/>. Used to draw anything under the content of this <see cref="Widget"/>. </summary>
        public event EventHandler<WidgetDrawArgs> OnDrawBackground;
        /// <summary> Invoked when this <see cref="Widget"/> draws content outside of its area. </summary>
        public event EventHandler OnDrawNoClip;
        /// <summary> Invoked (through all <see cref="Widget"/>s recursively) before <see cref="OnUpdate"/>. </summary>
        public event EventHandler<PreUpdateArgs> OnPreUpdate;
        /// <summary> Invoked after this <see cref="Widget"/> updates.</summary>
        public event EventHandler OnUpdate;
        /// <summary> Invoked when this <see cref="Widget"/> is first updated. </summary>
        public event EventHandler OnFirstUpdate;
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
        public event EventHandler<RectangleFSetArgs> OnResize;
        /// <summary> Invoked when this <see cref="Widget"/>'s position changes. </summary>
        public event EventHandler<RectangleFSetArgs> OnResposition;
        /// <summary> Invoked when this <see cref="Widget"/>'s area changes. </summary>
        public event EventHandler<RectangleFSetArgs> OnAreaChange;
        /// <summary> Invoked when this <see cref="Widget"/>'s area changes. Invoked before <see cref="OnAreaChange"/>, allows overriding the set area. </summary>
        public event EventHandler<RectangleFSetOverrideArgs> OnAreaChangePriority;
        /// <summary> Invoked when a <see cref="Widget"/> is added to this one. (See added <see cref="Widget"/> in <see cref="LastAddedWidget"/>.) </summary>
        public event EventHandler OnAddChild;
        /// <summary> Invoked when a <see cref="Widget"/> is added to this <see cref="Widget"/> or any of it's children. </summary>
        public event EventHandler<WidgetArgs> OnAddChildAny;
        /// <summary> Invoked when a child <see cref="Widget"/> is removed from this. (See removed <see cref="Widget"/> in <see cref="LastRemovedWidget"/>.) </summary>
        public event EventHandler OnRemoveChild;
        public event EventHandler<WidgetArgs> OnRemoveChildAny;
        /// <summary> Invoked when a child is added or removed from this <see cref="Widget"/>. </summary>
        public event EventHandler OnListChange;
        /// <summary> Invoked when this <see cref="Widget"/> is disposed. </summary>
        public event EventHandler OnDispose;
        /// <summary> Invoked when this <see cref="Widget"/>'s <see cref="MinimumSize"/> is set to something new. </summary>
        public event EventHandler<Point2SetArgs> OnMinimumSizeSet;
        /// <summary> Invoked when this <see cref="Widget"/>'s <see cref="MinimumSize"/> is set to something new. Use this with <see cref="Point2SetOverrideArgs"/> to override the set size. </summary>
        public event EventHandler<Point2SetOverrideArgs> OnMinimumSizeSetPriority;
        /// <summary> Invoked after this <see cref="Widget"/> is deleted by <see cref="Delete"/>. </summary>
        public event EventHandler OnDelete;
        /// <summary> Invoked when this <see cref="Widget"/>'s <see cref="Widget.ParentDWindow"/> value is set. (to a non-null value) </summary>
        public event EventHandler OnParentWindowSet;
        /// <summary> Invoked when this <see cref="Widget"/>'s <see cref="Widget.ParentWidget"/> value is set. (to a non-null value) </summary>
        public event EventHandler OnParentWidgetSet;
        /// <summary> Invoked when the Parent <see cref="Widget"/> is resized. </summary>
        public event EventHandler<RectangleFSetArgs> OnParentResize;
        /// <summary> Invoked whenever a child <see cref="Widget"/>'s area is resized. </summary>
        public event EventHandler<RectangleFSetArgs> OnChildResize;
        /// <summary> Invoked whenever a child <see cref="Widget"/>'s area changes. </summary>
        public event EventHandler<RectangleFSetArgs> OnChildAreaChange;
        /// <summary> Invoked whenever a child <see cref="Widget"/>'s position is changed. </summary>
        public event EventHandler<RectangleFSetArgs> OnChildReposition;
        /// <summary> Invoked when the parent <see cref="DWindow"/> is closed. </summary>
        public event EventHandler OnWindowClose;
        /// <summary> Invoked when a <see cref="WidgetBehavior"/> is added to this <see cref="Widget"/>. </summary>
        public event EventHandler<WidgetBehaviorArgs> OnAddBehavior;
        /// <summary> Invoked when a <see cref="WidgetBehavior"/> is removed from this <see cref="Widget"/>. </summary>
        public event EventHandler<WidgetBehaviorArgs> OnRemoveBehavior;

        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeOnWindowClose() => OnWindowClose?.Invoke(this, EventArgs.Empty);
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeSelectOffEvent() => OnSelectOff?.Invoke(this, EventArgs.Empty);
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeSelectEvent() => OnSelection?.Invoke(this, EventArgs.Empty);
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeOnAdd() => OnAddChild?.Invoke(this, EventArgs.Empty);
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeOnAddAny(WidgetArgs args)
        {
            if (OnAddChildAny != null)
            {
                foreach (Widget widget in args.Widget.AllContainedWidgets) OnAddChildAny.Invoke(this, new WidgetArgs(widget));
            }

            ParentWidget?.InvokeOnAddAny(args);
        }
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeOnRemove() => OnRemoveChild?.Invoke(this, EventArgs.Empty);
        internal void InvokeOnRemoveAny(WidgetArgs args)
        {
            if (OnAddChildAny != null)
            {
                foreach (Widget widget in args.Widget.AllContainedWidgets) OnRemoveChildAny.Invoke(this, new WidgetArgs(widget));
            }

            ParentWidget?.InvokeOnRemoveAny(args);
        }
        /// <summary> Internal invoke method. Avoid calling. </summary>
        internal void InvokeOnListChange() => OnListChange?.Invoke(this, EventArgs.Empty);
        internal void InvokeOnAddBehavior(WidgetBehaviorArgs args) => OnAddBehavior?.Invoke(this, args);
        internal void InvokeOnRemoveBehavior(WidgetBehaviorArgs args) => OnRemoveBehavior?.Invoke(this, args);
        void InvokeOnParentResize(RectangleFSetArgs args) => OnParentResize?.Invoke(this, args);
        void InvokeOnChildResized(RectangleFSetArgs args) => OnChildResize?.Invoke(this, args);
        void InvokeOnChildAreaChange(RectangleFSetArgs args) => OnChildAreaChange?.Invoke(this, args);
        void InvokeOnChildReposition(RectangleFSetArgs args) => OnChildReposition?.Invoke(this, args);
        void InvokeDrawOverlay(WidgetDrawArgs args) => OnDrawOverlay?.Invoke(this, args);
        void InvokeDrawBG(WidgetDrawArgs args) => OnDrawBackground?.Invoke(this, args);
        void InvokeDrawOverlayEffects(WidgetDrawArgs args) => OnDrawOverlay?.Invoke(this, args);
        void InvokeOnRenameAny(RenameArgs args)
        {
            OnRenameAny?.Invoke(this, args);
            ParentWidget?.InvokeOnRenameAny(args);
        }
        
        #endregion

        #region Methods

        /// <summary> Insert this <see cref="Widget"/> in a new <see cref="Widget"/> and return the container. </summary>
        /// <returns> The containing <see cref="Widget"/>. </returns>
        public Widget SendToContainer(bool preserve_area = true, bool snap_to_container = false)
        {
            Widget parent = ParentWidget;
            Widget result = new Widget();
            result.VisualSettings.DrawBackground = false;
            result.VisualSettings.DrawOutline = false;
            if (preserve_area) result.Area = Area;
            if (snap_to_container) SnappingPolicy = DiagonalDirections2D.All;
            result.Add(this);
            Position = new Point2();
            parent?.Add(result);
            return result;
        }

        public void AnimatedReplace(Widget new_widget, WidgetTransitionAnimation animation, bool dispose_old = true)
        {
            AnimatedReplace(new_widget, animation.NewWidgetStart, animation.OldWidgetEnd, animation.NewWidgetMovement, animation.OldWidgetMovement, dispose_old);
        }

        public void AnimatedReplace(Widget new_widget, InnerWidgetLocation new_widget_start = null, InnerWidgetLocation old_widget_end = null, InterpolationSettings? new_widget_movement = null, InterpolationSettings? old_widget_movement = null, bool dispose_old = true)
        {
            //new_widget.Size = Size;
            new_widget.SnappingPolicy = SnappingPolicy;
            Actions.Add(new ReplaceWidget(new_widget, new_widget_start ?? InnerWidgetLocation.OutsideRight, old_widget_end ?? InnerWidgetLocation.OutsideLeft, new_widget_movement, old_widget_movement, dispose_old));
        }

        /// <summary> Deletes this <see cref="Widget"/>'s <see cref="ParentWidget"/> and adds this to the parent above. </summary>
        /// <returns> The deleted parent <see cref="Widget"/>. </returns>
        public Widget ReplaceContainer(bool replace_position = true, bool replace_size = false)
        {
            Widget old_parent = ParentWidget;
            Widget new_parent = ParentWidget.ParentWidget;

            if (replace_position && !replace_size) Position = old_parent.Position;
            if (!replace_position && replace_size) Size = old_parent.Size;
            if (replace_position && replace_size) Area = old_parent.Area;

            new_parent.Add(this);
            old_parent.Delete();
            return old_parent;
        }

        public Widget WithAddedBehavior(WidgetBehavior behavior)
        {
            Behaviors.Add(behavior);
            return this;
        }

        public Widget WithAddedBehavior<T>(T behavior, out T added_behavior)
        {
            if (!(behavior is WidgetBehavior behavior_)) throw new Exception($"Given item is not a {nameof(WidgetBehavior)}.");
            Behaviors.Add(behavior_);
            added_behavior = behavior;
            return this;
        }

        public Widget WithAddedBehavior(IEnumerable<WidgetBehavior> behaviors)
        {
            Behaviors.AddRange(behaviors);
            return this;
        }

        public Widget WithAddedAction(WidgetAction action)
        {
            Actions.Add(action);
            return this;
        }

        public Widget WithAddedAction<T>(T action, out T added_action)
        {
            if (!(action is WidgetAction action_)) throw new Exception($"Given item is not a {nameof(WidgetAction)}.");
            Actions.Add(action_);
            added_action = action;
            return this;
        }

        public Widget WithAddedAction(IEnumerable<WidgetAction> actions)
        {
            Actions.AddRange(actions);
            return this;
        }

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

            if (snapping.Left && !snapping.Right) new_area.X = encompassing_area.X + Spacing.Width; // left
            if (!snapping.Left && snapping.Right) new_area.X = encompassing_area.X + encompassing_area.Width - new_area.Width - Spacing.Width; // right 
            if (snapping.Left && snapping.Right)
            { // left and right 
                new_area.X = encompassing_area.X + Spacing.Width;
                new_area.Width = encompassing_area.Width - Spacing.Width * 2;
            }
            if (snapping.Up && !snapping.Down) new_area.Y = encompassing_area.Y + Spacing.Height; // up
            if (!snapping.Up && snapping.Down) new_area.Y = encompassing_area.Y + encompassing_area.Height - new_area.Height - Spacing.Height;// down
            if (snapping.Up && snapping.Down)
            { // up and down
                new_area.Y = encompassing_area.Y + Spacing.Height;
                new_area.Height = encompassing_area.Height - Spacing.Height * 2;
            }

            Area = new_area.WithMinimumSize(MinimumSize);
        }

        /// <summary> Set this <see cref="Widget"/> as the only focused <see cref="Widget"/>. </summary>
        void SetAsFocused() => ParentDWindow?.SelectedWidgets.SetFocus(this);

        /// <summary> Add this <see cref="Widget"/> to the group of selected <see cref="Widget"/>s. </summary>
        void AddToFocused() => ParentDWindow?.SelectedWidgets.AddFocus(this);

        /// <summary> Resize the <see cref="RenderTarget2D"/> to match the current area. </summary>
        void UpdateRenderTargetSizes()
        {
            if (DrawingMode == DrawingModeType.use_render_target) UpdateRenderTargetSize(DrawingArea.Size);

            for (int i = 0; i < Children.Count; i++) Children[i].UpdateRenderTargetSizes();
        }

        void UpdateRenderTargetSize(Point2 size)
        {
            if (DrawingMode != DrawingModeType.use_render_target) return;
            if (RenderTargetResizeMode == RenderTargetResizeModeType.maximum_size)
            {
                size = new Point2(MAXIMUM_WIDGET_SIZE, MAXIMUM_WIDGET_SIZE);
            }

            if (_render_target != null && (int)size.X == _render_target.Width && (int)size.Y == _render_target.Height) return;
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

            if (size.X < 1) size.X = 1;
            if (size.Y < 1) size.Y = 1;

            if (Math.Max((int)size.X, (int)size.Y) > MAXIMUM_WIDGET_SIZE)
            {
                size = size.Min(new Point2(MAXIMUM_WIDGET_SIZE, MAXIMUM_WIDGET_SIZE));
                Console.WriteLine($"DownUnder WARNING: Maximum Widget dimensions reached (maximum size is {MAXIMUM_WIDGET_SIZE}, given dimensions are {size}). This may cause rendering issues.");
            }

            if (_render_target != null)
            {
                _render_target.Dispose();
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

        public void SendToBack(bool immediate = false)
        {
            if (immediate) ParentWidget?.Children.SendWidgetToBack(this);
            else _post_update_flags.SendToBack = true;
        }

        public void SendToFront(bool immediate = false)
        {
            if (immediate) ParentWidget?.Children.SendWidgetToFront(this);
            else _post_update_flags.SendToFront = true;
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

        public object Clone()
        {
            if (!IsCloningSupported) throw new Exception($"Cloning was flagged as unsupported with this {nameof(Widget)}. ({nameof(IsCloningSupported)} == false)");
            Widget c = new Widget();
            for (int i = 0; i < Children.Count; i++) c.Children.Add((Widget)Children[i].Clone());
            c.FitToContentArea = FitToContentArea;

            c.Name = Name;
            c.VisualSettings = (GeneralVisualSettings)VisualSettings.Clone();

            c.EnterConfirms = EnterConfirms;
            c.MinimumSize = MinimumSize;
            c.SnappingPolicy = SnappingPolicy;
            c.DoubleClickTiming = DoubleClickTiming;
            c.Spacing = Spacing;
            c.Area = Area;
            c.IsFixedWidth = IsFixedWidth;
            c.IsFixedHeight = IsFixedHeight;
            c.VisualSettings = (GeneralVisualSettings)VisualSettings.Clone();
            c.DrawingMode = DrawingMode;
            c.debug_output = debug_output;
            c.PassthroughMouse = PassthroughMouse;
            c.BehaviorTags = (AutoDictionary<SerializableType, AutoDictionary<string, string>>)BehaviorTags.Clone();
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
            foreach (WidgetAction action in ClosingActions) c.Actions.Add((WidgetAction)action.InitialClone());

            return c;
        }

        #endregion

        #region Ilist

        public void Add(Widget widget, out Widget added_widget)
        {
            Add(widget);
            added_widget = widget;
        }

        public void Insert(int index, Widget item, out Widget inserted_widget)
        {
            Insert(index, item);
            inserted_widget = item;
        }

        public int IndexOf(Widget item) => Children.IndexOf(item);
        public void Insert(int index, Widget item) => Children.Insert(index, item);
        public void RemoveAt(int index) => Children.RemoveAt(index);
        public void Add(Widget item) => Children.Add(item);
        public void AddRange(IEnumerable<Widget> widgets) => Children.AddRange(widgets);
        public void Clear() => Children.Clear();
        public bool Contains(Widget item) => Children.Contains(item);
        public void CopyTo(Widget[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        public bool Remove(Widget item) => Children.Remove(item);
        public IEnumerator<Widget> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }

        public int Count => Children.Count;
        public bool IsReadOnly => Children.IsReadOnly;
        public Widget this[int index] { get => Children[index]; set => Children[index] = value; }
        public Widget this[int x, int y]
        {
            get
            {
                GridFormat grid = Behaviors.Get<GridFormat>();
                if (grid == null) throw new Exception($"This {nameof(Widget)} does not have a {nameof(GridFormat)} behavior.");
                return grid[x, y];
            }
            set
            {
                GridFormat grid = Behaviors.Get<GridFormat>();
                if (grid == null) throw new Exception($"This {nameof(Widget)} does not have a {nameof(GridFormat)} behavior.");
                grid[x, y] = value;
            }
        }
        public Widget this[string child_name] => Children[child_name];
        
        public Widget LastAddedWidget => Children.LastAddedWidget;
        public Widget LastRemovedWidget => Children.LastRemovedWidget;

        #endregion
    }
}