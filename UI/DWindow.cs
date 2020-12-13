using DownUnder.UI.DataTypes;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MonoGame.Extended;
using DownUnder.UI.Widgets;
using DownUnder.Utility;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;

namespace DownUnder.UI
{
    /// <summary> The class used to represent this Window. Inherits <see cref="Game"/>. </summary>
    public class DWindow : IParent {
        public Game ParentGame;
        //public GraphicsDevice GraphicsDevice;

        /// <summary> This <see cref="Delegate"/> is meant to grant the main thread's method to spawn new <see cref="DWindow"/>s. </summary>
        public delegate void WindowCreate(Type window_type, DWindow parent = null);
        /// <summary> The <see cref="GraphicsDeviceManager"/> used by this <see cref="DWindow"/>. Is initiated on creation. </summary>
        public readonly GraphicsDeviceManager GraphicsManager;
        /// <summary> Used to communicate with a spawned window in CreateWindow(). Set to 0 when child window is spawned, 1 after it activates, and -1 once the operation is completed. </summary>
        private int _spawned_window_is_active = -1;
        /// <summary> Used to keep track of this <see cref="DWindow"/>'s thread. </summary>
        private static int _thread_id;
        private readonly Dictionary<int, SetGetEvent<RectangleF>> _area_set_events = new Dictionary<int, SetGetEvent<RectangleF>>();
        /// <summary> While true, event queues should not be modified. </summary>
        private bool _event_queue_is_processing = false;
        /// <summary> Set to true once the <see cref="DWindow"/> has updated once. </summary>
        private bool _first_update = false;
        /// <summary> Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed. </summary>
        private const int _WAIT_TIME = 5;
        /// <summary> How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings. </summary>
        private const int _WAIT_TIME_WARNING_THRESOLD = 100;
        // A cache used by Area.
        private RectangleF _area_cache = new RectangleF();

        private Widget _widget_backing;
        private Point2 _minimum_size_backing;
        bool _is_user_resizing_backing;

        RenderTarget2D[] _back_buffer = new RenderTarget2D[2];
        int _current_back_buffer = 0;

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
        /// <summary> The <see cref="Widget"/> that has the user resize cursor focus. </summary>
        internal Widget ResizeCursorGrabber { get; set; }
        internal Directions2D ResizingDirections { get; set; }
        /// <summary> <see cref="Widget"/> (if any) that is currently being resized. </summary>
        internal Widget ResizingWidget { get; private set; }
        /// <summary> Whether or not this window will wait until the next update to continue when calling certain methods. (Currently only Area.Set) Set to true by default, set to false for faster but delayed multithreading, or if Update() is not being called. </summary>
        public bool WaitForCrossThreadCompletion { get; set; } = true;
        /// <summary> True if the thread accessing this window is the the one this window is running on. </summary>
        private static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _thread_id;
        /// <summary> Used by the UI to set the mouse cursor. Resets every frame. Disable with <see cref="UICursorsEnabled"/>. </summary>
        internal MouseCursor UICursor { get; set; } = MouseCursor.Arrow;
        /// <summary> Set to false to disable UI mouse cursor changes. </summary>
        public bool UICursorsEnabled { get; set; } = true;
        /// <summary> The default <see cref="WindowFont"/> of this <see cref="DWindow"/>. Used by contained <see cref="Widget"/>s without a self-defined <see cref="WindowFont"/>. </summary>
        public SpriteFont WindowFont { get; set; }
        public DownUnderEffects EffectCollection = new DownUnderEffects();
        Point2 IParent.PositionInRender => new Point2();

        #endregion

        /// <summary> True if The user is currently resizing a <see cref="Widget"/> with the cursor. </summary>
        internal bool UserResizeModeEnable {
            get => _is_user_resizing_backing;
            set {
                if (value && !_is_user_resizing_backing) {
                    _is_user_resizing_backing = value;
                    ResizingWidget = ResizeCursorGrabber;
                    return;
                }
                if (!value) {
                    _is_user_resizing_backing = value;
                    ResizingWidget = null;
                    return;
                }
            }
        }

        /// <summary> The primary <see cref="Widget"/> of this window. </summary>
        public Widget MainWidget {
            get => _widget_backing;
            set {
                if (value != null) {
                    value.Parent = this;
                    value.Area = ParentGame.GraphicsDevice.Viewport.Bounds;
                }

                _widget_backing = value;
            }
        }

        /// <summary> Return a list of all HighLighted <see cref="Widget"/>s in this <see cref="DWindow"/>. </summary>
        public WidgetList HighLightedWidgets {
            get {
                WidgetList result = new WidgetList();
                foreach (Widget widget in SelectedWidgets.ToWidgetList()) {
                    if (widget.IsHighlighted) result.Add(widget);
                }
                return result;
            }
        }

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
            set {
                _minimum_size_backing = value;
                //OSInterface.SetMinimumWindowSize(Window, value.ToPoint());
            }
        }

        /// <summary> The location and size of this window. </summary>
        public RectangleF Area {
            get {
                if (!ParentGame.IsActive) return new RectangleF();
                return ParentGame.Window.ClientBounds;
                //if (IsMainThread) return System.Windows.Forms.Control.FromHandle(Window.Handle).DisplayRectangle.ToMonoRectangleF();
                return _area_cache;
            }
            set {
                if (IsMainThread) AreaSet(value);
                else {
                    // Add the event to the queue
                    _area_set_events.Add(Thread.CurrentThread.ManagedThreadId, new SetGetEvent<RectangleF>(value));

                    // Wait for events to process
                    if (WaitForCrossThreadCompletion) {
                        int i = 0;
                        while (!_area_set_events[Thread.CurrentThread.ManagedThreadId].Completed || _event_queue_is_processing) {
                            if (i < 10000000) i += _WAIT_TIME;
                            if (i > _WAIT_TIME_WARNING_THRESOLD) Console.WriteLine($"Hanging in area set, setting WaitForCrossThreadCompletion to false may prevent this."); 
                            if (IsMainThread) ProcessQueuedEvents();
                            Thread.Sleep(_WAIT_TIME);
                        }
                    }

                    // Remove event from queue
                    if (!_area_set_events.Remove(Thread.CurrentThread.ManagedThreadId)) throw new Exception("DWindow: Failed to remove thread id");
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

        /// <summary> A collection of icons used by this <see cref="DWindow"/> and its <see cref="Widget"/>s. </summary>
        //public UIImages UIImages { get; protected set; }

        /// <summary> <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/> used when drawing the UI. (Necessary for clipping) </summary>
        public RasterizerState RasterizerState = new RasterizerState() { ScissorTestEnable = true, MultiSampleAntiAlias = true };

        public object DraggingObject { get; set; }

        internal RenderTarget2D DrawTargetBuffer => _back_buffer[_current_back_buffer];
        internal RenderTarget2D OtherBuffer => _back_buffer[_current_back_buffer == 0 ? 1 : 0];
        
        /// <summary> Swap buffers. The contents of the old buffer will be drawn to the new one. The purpose of this is to create a RenderTarget2D that can be used to draw its own contents to itself. </summary>
        internal void SwapBackBuffer(GraphicsDevice graphics, SpriteBatch sprite_batch)
        {
            int old_buffer = _current_back_buffer;
            int new_buffer = _current_back_buffer == 0 ? 1 : 0;
            graphics.SetRenderTarget(_back_buffer[new_buffer]);
            sprite_batch.Draw(_back_buffer[old_buffer], new Vector2(), Color.White);
            _current_back_buffer = new_buffer;
        }

        IParent IParent.Parent => (IParent)Parent;

        GraphicsDevice IParent.GraphicsDevice => ParentGame?.GraphicsDevice;

        public bool IsFullscreen => GraphicsManager.IsFullScreen;

        #endregion Properties

        #region Constructors

        public DWindow(GraphicsDeviceManager graphics, Game parent) {
            GraphicsManager = graphics;
            _thread_id = Thread.CurrentThread.ManagedThreadId;
            ParentGame = parent;
            
            MainWidget = new Widget();

            if (parent != null) {
                Parent = parent;
                //parent.Children.Add(this);
            }

            _back_buffer[0] = new RenderTarget2D(graphics.GraphicsDevice, (int)Area.Width, (int)Area.Height);
            _back_buffer[1] = new RenderTarget2D(graphics.GraphicsDevice, (int)Area.Width, (int)Area.Height);

            // unneeded possibly
            OnFirstUpdate += (sender, args) => _thread_id = Thread.CurrentThread.ManagedThreadId;

            ParentGame.Window.ClientSizeChanged += ResetBuffers;
            ParentGame.Window.TextInput += ProcessKeys;
            ParentGame.Exiting += ExitAll;

            //GraphicsManager = ParentGame.Components.;
            ParentGame.Window.AllowUserResizing = true;
            ParentGame.IsMouseVisible = true;
            
            MinimumSize = new Point2(100, 100);
            double time = (1000d / 144) * 10000d;
            ParentGame.TargetElapsedTime = new TimeSpan((long)time);
            //Window.IsBorderless = true;

            LoadDWindow(graphics.GraphicsDevice);
        }

        internal void ResetBuffers(object sender, EventArgs args)
        {
            if (MainWidget != null) MainWidget.Size = Area.Size;
            _back_buffer[0].Dispose();
            _back_buffer[1].Dispose();
            _back_buffer[0] = new RenderTarget2D(GraphicsManager.GraphicsDevice, (int)Area.Width, (int)Area.Height);
            _back_buffer[1] = new RenderTarget2D(GraphicsManager.GraphicsDevice, (int)Area.Width, (int)Area.Height);
        }

        public void Dispose() {
            GraphicsManager.Dispose();
        }

        public virtual Widget DisplayWidget
        {
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

        private void ProcessKeys(object sender, TextInputEventArgs e) {
            if (e.Character == 8) InputState.BackSpace = true;
            else if (e.Character == 10 || e.Character == 13) {
                InputText.Append('\n');
                InputState.Enter = true;
            } 
            else {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl)) CommandText.Append(e.Character);
                else InputText.Append(e.Character);
            }
        }

        /// <summary> Closes all child <see cref="DWindow"/>s and removes own reference from parent on exiting. </summary>
        private void ExitAll(object sender, EventArgs args) { 
            foreach (DWindow child in Children) child.ParentGame.Exit(); // Kill each of the children
            //Parent?.Children.Remove(this);
        }

        public void SignalSpawnWindowAsActive(object sender, EventArgs args) {
            Console.WriteLine("DWindow.SignalSpawnWindowAsActive: Setting spawned window as active");
            _spawned_window_is_active = 1;
            Console.WriteLine($"_spawned_window_is_active = {_spawned_window_is_active}");
        }

        private void AreaSet(RectangleF value) {
            GraphicsManager.PreferredBackBufferWidth = (int)value.Width;
            GraphicsManager.PreferredBackBufferHeight = (int)value.Height;

            try {
                GraphicsManager.ApplyChanges();
                //OSInterface.SetWindowPosition(this, value.Position.ToPoint());
            } catch (Exception e) { Console.WriteLine($"DWindow.AreaSet: Error, Failed to resize window. Message: {e.Message}"); }

            MainWidget.Area = value.SizeOnly();
        }

        #endregion Event Handlers

        #region Protected Methods

        public DWindow CreateWindow(Type window_type) {
            _spawned_window_is_active = 0;
            CreateWindowDelegate.Invoke(window_type, this);

            do { // Wait until the window is active to return
                Console.WriteLine($"{GetType().Name}.CreateWindow: Waiting for spawned window to be active _spawned_window_is_active = {_spawned_window_is_active}");
                Thread.Sleep(5);
            } while (_spawned_window_is_active != 1);
            Console.WriteLine($"{GetType().Name}: Waiting done");

            return Children[Children.Count - 1];
        }

        public void LoadDWindow(GraphicsDevice graphics) {
            EffectCollection.ShadingEffect = ParentGame.Content.Load<Effect>("DownUnder Native Content/Effects/Gradient");
            WindowFont = ParentGame.Content.Load<SpriteFont>("DownUnder Native Content/SpriteFonts/Default Font");
        }

        public void Update(GameTime game_time) {
            _area_cache = Area;
            OnUpdate?.Invoke(this, EventArgs.Empty);
            if (!_first_update)
            {
                OnFirstUpdate?.Invoke(this, EventArgs.Empty);
                _first_update = true;
            }
            ProcessQueuedEvents();
            HoveredWidgets.Reset();
            ResizeCursorGrabber = null;
            if (!InputState.PrimaryClick) DraggingObject = null;
            InputState.UpdateAll(this, game_time);

            //Debug.WriteLine("FPS: " + (1 / game_time.ElapsedGameTime.TotalSeconds));

            InputText.Clear();
            CommandText.Clear();

            ResizingDirections = Directions2D.None;
            MainWidget.Update(game_time, InputState);
            if (ResizingDirections == Directions2D.UDLR) UICursor = MouseCursor.SizeAll;
            else {
                if ((ResizingDirections == Directions2D.U) || (ResizingDirections == Directions2D.D)) UICursor = MouseCursor.SizeNS;
                if ((ResizingDirections == Directions2D.L) || (ResizingDirections == Directions2D.R)) UICursor = MouseCursor.SizeWE;
                if ((ResizingDirections == Directions2D.UR) || (ResizingDirections == Directions2D.DL)) UICursor = MouseCursor.SizeNESW;
                if ((ResizingDirections == Directions2D.UL) || (ResizingDirections == Directions2D.DR)) UICursor = MouseCursor.SizeNWSE;
            }
            if (UICursorsEnabled) Mouse.SetCursor(UICursor);
            UICursor = MouseCursor.Arrow;
            InputState.BackSpace = false;
            InputState.Enter = false;
        }

        public void Draw(SpriteBatch sprite_batch, GameTime game_time) {
            MainWidget.Draw(sprite_batch);
        }

        public void ToggleFullscreen()
        {
            GraphicsManager.HardwareModeSwitch = false;
            GraphicsManager.ToggleFullScreen();
            OnToggleFullscreen?.Invoke(this, EventArgs.Empty);
        }

        public void ShowPopUpMessage(string message)
        {
            Widget window = new Widget { Size = new Point2(400, 300) };

            window.Behaviors.Add(new CenterContent());
            window.Behaviors.Add(new PinWidget { Pin = InnerWidgetLocation.Centered });
            window.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.975f), RectanglePart.Uniform(0.5f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Faster }, out var pop_in_out);
            window.Add(CommonWidgets.Label(message), out var label);

            label.PassthroughMouse = true;
            window.OnClick += (s, a) => pop_in_out.Close();

            DisplayWidget.Add(window);
        }

        #endregion Protected Methods

        private void ProcessQueuedEvents() {
            _event_queue_is_processing = true;

            foreach (int key in _area_set_events.Keys) {
                AreaSet(_area_set_events[key].Value);
                _area_set_events[key].Completed = true;
            }

            _event_queue_is_processing = false;
        }
    }
}