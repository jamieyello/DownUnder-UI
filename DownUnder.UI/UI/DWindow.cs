using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DownUnder.UI.UI.DataTypes;
using DownUnder.UI.UI.Widgets;
using DownUnder.UI.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities;
using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using static DownUnder.UI.Utilities.CommonNamespace.Directions2D;
using static Microsoft.Xna.Framework.Input.MouseCursor;

namespace DownUnder.UI.UI {
    /// <summary> The class used to represent this Window. Inherits <see cref="Game"/>. </summary>
    public class DWindow : IParent, IDisposable {
        /// <summary> This <see cref="Delegate"/> is meant to grant the main thread's method to spawn new <see cref="DWindow"/>s. </summary>
        public delegate void WindowCreate(Type window_type, DWindow parent = null);

        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        const int _WAIT_TIME = 5;
        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        const int _WAIT_TIME_WARNING_THRESOLD = 100;

        public readonly Game ParentGame;
        public static IOSInterface OS { get; private set; }

        /// <summary> The <see cref="GraphicsDeviceManager"/> used by this <see cref="DWindow"/>. Is initiated on creation. </summary>
        public readonly GraphicsDeviceManager GraphicsManager;
        /// <summary> A single white pixel <see cref="Texture2D"/> to be used by drawing code. Is null before a <see cref="DWindow"/> is created. </summary>
        public static Texture2D WhiteDotTexture { get; private set; }
        readonly RenderTarget2D[] _back_buffer = new RenderTarget2D[2];
        int _current_back_buffer;

        /// <summary> Used to keep track of this <see cref="DWindow"/>'s thread. </summary>
        readonly int _thread_id;
        readonly Dictionary<int, SetGetEvent<RectangleF>> _area_set_events = new Dictionary<int, SetGetEvent<RectangleF>>();
        /// <summary> While true, event queues should not be modified. </summary>
        bool _event_queue_is_processing;
        /// <summary> Set to true once the <see cref="DWindow"/> has updated once. </summary>
        bool _first_update;
        /// <summary> Used to communicate with a spawned window in CreateWindow(). Set to 0 when child window is spawned, 1 after it activates, and -1 once the operation is completed. </summary>
        int _spawned_window_is_active = -1;

        Widget _widget_backing;
        Point2 _minimum_size_backing;
        bool _is_user_resizing_backing;

        #region Properties

        #region Auto Properties

        /// <summary> A reference to each of this window's children. </summary>
        public List<DWindow> Children { get; } = new List<DWindow>();

        /// <summary> The window that owns this window. </summary>
        public Game Parent { get; set; }

        /// <summary> Represents this window's input each frame. </summary>
        public UIInputState InputState { get; } = new UIInputState();

        /// <summary> Used for text input. Typed text is added here. </summary>
        public StringBuilder InputText { get; } = new StringBuilder();

        /// <summary> Typed CTRL + key combination chars are added here. </summary>
        public StringBuilder CommandText { get; } = new StringBuilder();

        /// <summary> A delegate to the main program's method to spawn new windows. </summary>
        public WindowCreate CreateWindowDelegate { get; set; }

        /// <summary> A reference to all widgets that are selected in this DWindow. </summary>
        public Focus SelectedWidgets { get; } = new Focus(FocusType.selection);

        /// <summary> A reference to all widgets that hovered over by a cursor in this DWindow. </summary>
        public Focus HoveredWidgets { get; } = new Focus(FocusType.hover);
        public readonly GenericDirections2D<Focus> ScrollableWidgetFocus = new GenericDirections2D<Focus>(new Focus(FocusType.hover));

        /// <summary> The <see cref="Widget"/> that has the user resize cursor focus. </summary>
        internal Widget ResizeCursorGrabber { get; set; }
        internal Directions2D ResizingDirections { get; set; }

        /// <summary> <see cref="Widget"/> (if any) that is currently being resized. </summary>
        internal Widget ResizingWidget { get; private set; }

        /// <summary> Whether or not this window will wait until the next update to continue when calling certain methods. (Currently only Area.Set) Set to true by default, set to false for faster but delayed multithreading, or if Update() is not being called. </summary>
        public bool WaitForCrossThreadCompletion { get; set; } = true;

        /// <summary> True if the thread accessing this window is the the one this window is running on. </summary>
        public bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _thread_id;

        /// <summary> Used by the UI to set the mouse cursor. Resets every frame. Disable with <see cref="UICursorsEnabled"/>. </summary>
        internal MouseCursor UICursor { get; set; } = Arrow;

        /// <summary> Set to false to disable UI mouse cursor changes. </summary>
        public bool UICursorsEnabled { get; set; } = true;

        /// <summary> The default <see cref="WindowFont"/> of this <see cref="DWindow"/>. Used by contained <see cref="Widget"/>s without a self-defined <see cref="WindowFont"/>. </summary>
        public SpriteFont WindowFont { get; set; }
        public readonly  DownUnderEffects EffectCollection = new DownUnderEffects();
        Point2 IParent.PositionInRender => new Point2();
        public UINavigator Navigation { get; }

        #endregion

        // TODO: O(n) indexer; should be a dictionary
        public Widget this[string widget_name] => MainWidget.AllContainedWidgets.FirstOrDefault(widget => widget.Name == widget_name);

        /// <summary> True if The user is currently resizing a <see cref="Widget"/> with the cursor. </summary>
        internal bool UserResizeModeEnable {
            get => _is_user_resizing_backing;
            set {
                if (value && !_is_user_resizing_backing) {
                    _is_user_resizing_backing = true;
                    ResizingWidget = ResizeCursorGrabber;
                    return;
                }

                if (value)
                    return;

                _is_user_resizing_backing = false;
                ResizingWidget = null;
            }
        }

        /// <summary> The primary <see cref="Widget"/> of this window. </summary>
        public Widget MainWidget {
            get => _widget_backing;
            set {
                if (value is { }) {
                    value.Parent = this;
                    value.Area = ParentGame.GraphicsDevice.Viewport.Bounds;
                }

                _widget_backing = value;
            }
        }

        /// <summary> Return a list of all HighLighted <see cref="Widget"/>s in this <see cref="DWindow"/>. </summary>
        public WidgetList HighLightedWidgets { get {
            var result = new WidgetList();
            foreach (var widget in SelectedWidgets.ToWidgetList())
                if (widget.IsHighlighted)
                    result.Add(widget);
            return result;
        } }

        /// <summary> The location of this window on the screen. </summary>
        public Point2 Position {
            get => Parent.GraphicsDevice.Viewport.Bounds.Location;
            set => Area = new RectangleF(value, Area.Size);
        }

        /// <summary> The size of this window. </summary>
        public Point2 Size {
            get => Area.Size;
            set => Area = new RectangleF(Area.Position, value.ToPoint());
        }

        /// <summary> This minimum size allowed when resizing this window. </summary>
        public Point2 MinimumSize {
            get => _minimum_size_backing;
            set => _minimum_size_backing = value;
            //OSInterface.SetMinimumWindowSize(Window, value.ToPoint());
        }

        /// <summary> The location and size of this window. </summary>
        public RectangleF Area {
            get => ParentGame.IsActive ? ParentGame.Window.ClientBounds : new RectangleF();
            set {
                if (IsMainThread)
                    AreaSet(value);
                else {
                    // Add the event to the queue
                    _area_set_events.Add(Thread.CurrentThread.ManagedThreadId, new SetGetEvent<RectangleF>(value));

                    // Wait for events to process
                    if (WaitForCrossThreadCompletion) {
                        var i = 0;

                        while (!_area_set_events[Thread.CurrentThread.ManagedThreadId].Completed || _event_queue_is_processing) {
                            if (i < 10000000)
                                i += _WAIT_TIME;

                            if (i > _WAIT_TIME_WARNING_THRESOLD)
                                Console.WriteLine($"Hanging in area set, setting WaitForCrossThreadCompletion to false may prevent this.");

                            if (IsMainThread)
                                ProcessQueuedEvents();

                            Thread.Sleep(_WAIT_TIME);
                        }
                    }

                    // Remove event from queue
                    if (!_area_set_events.Remove(Thread.CurrentThread.ManagedThreadId))
                        throw new Exception("DWindow: Failed to remove thread id");
                }
            }
        }

        /// <summary> The width of this <see cref="DWindow"/>. (relative to pizels on a 1080p monitor) </summary>
        public float Width {
            get => Area.Width;
            set => Area = Area.WithWidth(value);
        }

        /// <summary> The height of this <see cref="DWindow"/>. (relative to pizels on a 1080p monitor) </summary>
        public float Height {
            get => Area.Height;
            set => Area = Area.WithHeight(value);
        }

        ///// <summary> A collection of icons used by this <see cref="DWindow"/> and its <see cref="Widget"/>s. </summary>
        //public UIImages UIImages { get; protected set; }

        /// <summary> <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/> used when drawing the UI. (Necessary for clipping) </summary>
        public readonly RasterizerState RasterizerState = new RasterizerState { ScissorTestEnable = true, MultiSampleAntiAlias = true };

        public object DraggingObject { get; set; }

        internal RenderTarget2D DrawTargetBuffer => _back_buffer[_current_back_buffer];
        internal RenderTarget2D OtherBuffer => _back_buffer[_current_back_buffer == 0 ? 1 : 0];

        /// <summary> Swap buffers. The contents of the old buffer will be drawn to the new one. The purpose of this is to create a RenderTarget2D that can be used to draw its own contents to itself. </summary>
        internal void SwapBackBuffer(
            GraphicsDevice graphics,
            SpriteBatch sprite_batch
        ) {
            var old_buffer_index = _current_back_buffer;
            var new_buffer_index = _current_back_buffer == 0 ? 1 : 0;
            graphics.SetRenderTarget(_back_buffer[new_buffer_index]);
            sprite_batch.Draw(_back_buffer[old_buffer_index], new Vector2(), Color.White);
            _current_back_buffer = new_buffer_index;
        }

        IParent IParent.Parent => (IParent)Parent;

        GraphicsDevice IParent.GraphicsDevice => ParentGame?.GraphicsDevice;

        public bool IsFullscreen => GraphicsManager.IsFullScreen;

        #endregion Properties

        #region Constructors

        public DWindow(
            GraphicsDeviceManager graphics,
            Game parent,
            IOSInterface os_interface
        ) {
            GraphicsManager = graphics;
            if (WhiteDotTexture == null) {
                WhiteDotTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
                WhiteDotTexture.SetData(new[] { Color.White });
            }

            _thread_id = Thread.CurrentThread.ManagedThreadId;
            ParentGame = parent;
            Navigation = new UINavigator(this);
            OS = os_interface;

            MainWidget = new Widget();

            if (parent != null) {
                Parent = parent;
                //parent.Children.Add(this);
            }

            _back_buffer[0] = new RenderTarget2D(graphics.GraphicsDevice, (int)Area.Width, (int)Area.Height);
            _back_buffer[1] = new RenderTarget2D(graphics.GraphicsDevice, (int)Area.Width, (int)Area.Height);

            ParentGame.Window.ClientSizeChanged += ResetBuffers;
            //ParentGame.Window.TextInput += ProcessKeys;
            ParentGame.Exiting += ExitAll;

            ParentGame.Window.AllowUserResizing = true;
            ParentGame.IsMouseVisible = true;

            MinimumSize = new Point2(100, 100);
            const double time = 1000d / 144 * 10_000d;
            ParentGame.TargetElapsedTime = new TimeSpan((long)time);
            //Window.IsBorderless = true;

            if (parent is { })
                parent.Exiting += (s, e) => {
                    foreach (var widget in MainWidget.AllContainedWidgets)
                        widget.InvokeOnWindowClose();
                };

            LoadDWindow();
        }

        void ResetBuffers(object s, EventArgs a) {
            if (MainWidget is { })
                MainWidget.Size = Area.Size;

            _back_buffer[0].Dispose();
            _back_buffer[1].Dispose();
            _back_buffer[0] = new RenderTarget2D(GraphicsManager.GraphicsDevice, (int)Area.Width, (int)Area.Height);
            _back_buffer[1] = new RenderTarget2D(GraphicsManager.GraphicsDevice, (int)Area.Width, (int)Area.Height);
        }

        public void Dispose() =>
            GraphicsManager.Dispose();

        public virtual Widget DisplayWidget {
            get => MainWidget;
            set => MainWidget = value;
        }

        #endregion Constructors

        #region Event Handlers

        public event EventHandler<EventArgs> OnFirstUpdate;
        public event EventHandler<EventArgs> OnUpdate;
        public event EventHandler<EventArgs> OnToggleFullscreen;

        #endregion

        #region Events

        //void ProcessKeys(object s, TextInputEventArgs a) {
        //    Debug.WriteLine("DWINDOW ProcessKeys1");
        //    if (a.Character == 8)
        //        InputState.BackSpace = true;
        //    else if (a.Character == 10 || a.Character == 13) {
        //        InputText.Append('\n');
        //        InputState.Enter = true;
        //    } else {
        //        var state = Keyboard.GetState();
        //        Debug.WriteLine("DWINDOW ProcessKeys2" + state.IsKeyDown(Keys.LeftControl));

        //        if (state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl)) {
        //            CommandText.Append(a.Character);
        //            Debug.WriteLine("DWINDOW ProcessKeys");
        //        } else
        //            InputText.Append(a.Character);
        //    }
        //}

        /// <summary> Closes all child <see cref="DWindow"/>s and removes own reference from parent on exiting. </summary>
        void ExitAll(object s, EventArgs a) {
            foreach (var child in Children)
                child.ParentGame.Exit(); // Kill each of the children
            //Parent?.Children.Remove(this);
        }

        public void SignalSpawnWindowAsActive(object s, EventArgs a) {
            Console.WriteLine("DWindow.SignalSpawnWindowAsActive: Setting spawned window as active");
            _spawned_window_is_active = 1;
            Console.WriteLine($"_spawned_window_is_active = {_spawned_window_is_active}");
        }

        void AreaSet(RectangleF value) {
            GraphicsManager.PreferredBackBufferWidth = (int)value.Width;
            GraphicsManager.PreferredBackBufferHeight = (int)value.Height;

            try {
                GraphicsManager.ApplyChanges();
                //OSInterface.SetWindowPosition(this, value.Position.ToPoint());
            } catch (Exception e) { Console.WriteLine($"DWindow.AreaSet: Error, Failed to resize window. Message: {e.Message}"); }

            MainWidget.Area = value.SizeOnly();
        }

        #endregion Event Handlers

        #region Public Methods

        public DWindow CreateWindow(Type window_type) {
            _spawned_window_is_active = 0;
            CreateWindowDelegate.Invoke(window_type, this);

            do { // Wait until the window is active to return
                Console.WriteLine($"{GetType().Name}.CreateWindow: Waiting for spawned window to be active _spawned_window_is_active = {_spawned_window_is_active}");
                Thread.Sleep(5);
            } while (_spawned_window_is_active != 1);
            Console.WriteLine($"{GetType().Name}: Waiting done");

            return Children[^1];
        }

        public void LoadDWindow() {
            EffectCollection.ShadingEffect = ParentGame.Content.Load<Effect>("DownUnder Native Content/Effects/Gradient");
            WindowFont = ParentGame.Content.Load<SpriteFont>("DownUnder Native Content/SpriteFonts/Default Font");
        }

        public void Update(GameTime game_time) {
            OnUpdate?.Invoke(this, EventArgs.Empty);

            if (!_first_update) {
                OnFirstUpdate?.Invoke(this, EventArgs.Empty);
                _first_update = true;
            }

            ProcessQueuedEvents();
            HoveredWidgets.Reset();

            foreach (var f in ScrollableWidgetFocus)
                f.Reset();

            ResizeCursorGrabber = null;
            if (!InputState.PrimaryClick)
                DraggingObject = null;

            InputState.UpdateAll(this, game_time);

            //Debug.WriteLine("FPS: " + (1 / game_time.ElapsedGameTime.TotalSeconds));

            InputText.Clear();
            CommandText.Clear();

            ResizingDirections = None;
            MainWidget.Update(game_time, InputState);

            if (ResizingDirections == UDLR)
                UICursor = SizeAll;
            else {
                if (ResizingDirections == U || ResizingDirections == D) UICursor = SizeNS;
                if (ResizingDirections == L || ResizingDirections == R) UICursor = SizeWE;
                if (ResizingDirections == UR || ResizingDirections == DL) UICursor = SizeNESW;
                if (ResizingDirections == UL || ResizingDirections == DR) UICursor = SizeNWSE;
            }
            if (UICursorsEnabled) Mouse.SetCursor(UICursor);
            UICursor = Arrow;
            InputState.BackSpace = false;
            InputState.Enter = false;
        }

        public void Draw(SpriteBatch sprite_batch) =>
            MainWidget.Draw(sprite_batch);

        public void ToggleFullscreen() {
            GraphicsManager.HardwareModeSwitch = false;
            GraphicsManager.ToggleFullScreen();
            OnToggleFullscreen?.Invoke(this, EventArgs.Empty);
        }

        public void ShowPopUpMessage(string message) {
            var window = new Widget {
                Size = new Point2(400, 300),
                VisualSettings = {
                    VisualRole = GeneralVisualSettings.VisualRoleType.pop_up
                }
            };

            //window.Behaviors.Add(new CenterContent());
            window.Behaviors.Add(new PinWidget { Pin = InnerWidgetLocation.Centered });
            window.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.975f), RectanglePart.Uniform(0.5f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Faster }, out var pop_in_out);
            window.Add(CommonWidgets.Label(message), out var label);

            label.PassthroughMouse = true;
            label.Behaviors.Common.DrawText.YTextPositioning = Widgets.Behaviors.Visual.DrawText.YTextPositioningPolicy.center;
            label.Behaviors.Common.DrawText.XTextPositioning = Widgets.Behaviors.Visual.DrawText.XTextPositioningPolicy.center;
            label.SnappingPolicy = DiagonalDirections2D.All;

            window.OnClick += (s, a) => pop_in_out.Close();

            DisplayWidget.ParentWidget.Add(window);
        }

        #endregion Protected Methods

        void ProcessQueuedEvents() {
            _event_queue_is_processing = true;

            foreach (var key in _area_set_events.Keys) {
                AreaSet(_area_set_events[key].Value);
                _area_set_events[key].Completed = true;
            }

            _event_queue_is_processing = false;
        }
    }
}