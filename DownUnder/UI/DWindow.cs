using DownUnder.UI.DataTypes;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DownUnder.UI
{
    public abstract class DWindow : Game, IWidgetParent//, IDebugFeatures
    {
        #region Fields/Delegates

        /// <summary>
        /// This delegate is meant to grant the main thread's method to spawn
        /// new windows.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="new_window"></param>
        public delegate void WindowCreate(Type window_type, DWindow parent = null);

        private Layout _layout;

        private Point _minimum_size = new Point(100, 100);

        protected readonly GraphicsDeviceManager Graphics;

        // Used to communicate with a spawned window in CreateWindow().
        // Set to 0 when child window is spawned, 1 after it activates, and -1
        // once the operation is completed.
        private int _spawned_window_is_active = -1;

        private static int _thread_id;
        private readonly Dictionary<int, SetGetEvent<Rectangle>> _area_set_events = new Dictionary<int, SetGetEvent<Rectangle>>();

        /// <summary>
        /// While true, event queues should not be modified.
        /// </summary>
        private bool _event_queue_is_processing = false;

        /// <summary>
        /// Set to true once the window has updated once.
        /// </summary>
        private bool _first_update = false;

        /// <summary>
        /// Interval (in milliseconds) the program will wait before checking to see if a seperate process is completed.
        /// </summary>
        private const int _WAIT_TIME = 5;

        /// <summary>
        /// How long (in milliseconds) the program will wait for a seperate process before outputting hanging warnings.
        /// </summary>
        private const int _WAIT_TIME_WARNING_THRESOLD = 100;
        private Rectangle _area_cache = new Rectangle();

        #endregion Fields/Delegates

        #region Properties

        /// <summary>
        /// A reference to each of this window's children.
        /// </summary>
        public List<DWindow> Children { get; } = new List<DWindow>();

        /// <summary>
        /// The window that owns this window.
        /// </summary>
        public DWindow Parent { get; set; }

        /// <summary>
        /// Represents this window's input each frame.
        /// </summary>
        public UIInputState InputState { get; } = new UIInputState();

        /// <summary>
        /// Used for text input.
        /// </summary>
        public StringBuilder InputText { get; } = new StringBuilder();

        /// <summary>
        /// The Layout Widget of this window.
        /// </summary>
        public Layout Layout
        {
            get => _layout;
            set
            {
                if (value != null)
                {
                    value.ParentWindow = this;
                    value.Area = GraphicsDevice.Viewport.Bounds;
                }

                _layout = value;
            }
        }

        /// <summary>
        /// A delegate to the main program's method to spawn new windows.
        /// </summary>
        public WindowCreate CreateWindowDelegate { get; set; }

        // Should these be part of widget?
        public Focus SelectedWidgets { get; } = new Focus(FocusType.selection);

        public Focus HoveredWidgets { get; } = new Focus(FocusType.hover);

        public SpriteFont DefaultSpriteFont { get; protected set; }

        /// <summary>
        /// The location of this window on the screen.
        /// </summary>
        public Point Location
        {
            get => GraphicsDevice.Viewport.Bounds.Location;
            set => Area = new Rectangle(value, Area.Size);
        }

        /// <summary>
        /// The size of this window.
        /// </summary>
        public Point Size
        {
            get => GraphicsDevice.Viewport.Bounds.Size;
            set => Area = new Rectangle(Area.Location, value);
        }

        /// <summary>
        /// The location and size of this window.
        /// </summary>
        public Rectangle Area
        {
            get
            {
                if (!IsActive)
                {
                    return new Rectangle();
                }

                if (IsMainThread)
                {
                    // Only accessible by the main thread.
                    return System.Windows.Forms.Control.FromHandle(Window.Handle).DisplayRectangle.ToMonoRectangle();
                }
                else
                {
                    // Because an event based solution caused too many logic issues (too much work), a cache is used here.
                    return _area_cache;
                }
            }
            set
            {
                // To make this code safe for cross-thread access, the set method
                // isn't actually called until UpdateDWindow() triggers the event.

                if (IsMainThread)
                {
                    AreaSet(value);
                }
                else
                {
                    // Add the event to the queue
                    _area_set_events.Add(Thread.CurrentThread.ManagedThreadId, new SetGetEvent<Rectangle>(value));

                    // Wait for events to process
                    if (WaitForCrossThreadCompletion)
                    {
                        int i = 0;
                        while (!_area_set_events[Thread.CurrentThread.ManagedThreadId].Completed || _event_queue_is_processing)
                        {
                            if (i < 10000000)
                            {
                                i += _WAIT_TIME;
                            }

                            if (i > _WAIT_TIME_WARNING_THRESOLD)
                            {
                                //this.PrintDebug($"Hanging in area set, setting WaitForCrossThreadCompletion to false may prevent this.");
                            }

                            if (IsMainThread)
                            {
                                ProcessQueuedEvents();
                            }

                            Thread.Sleep(_WAIT_TIME);
                        }
                    }

                    // Remove event from queue
                    if (!_area_set_events.Remove(Thread.CurrentThread.ManagedThreadId))
                    {
                        throw new Exception("DWindow: Failed to remove thread id");
                    }
                }
            }
        }

        /// <summary>
        /// Whether or not this window will wait until the next update to continue 
        /// when calling certain methods. (Currently only Area.Set) Set to 
        /// true by default, set to false for faster but delayed multithreading, or
        /// if Update() is not being called.
        /// </summary>
        public bool WaitForCrossThreadCompletion { get; set; } = true;

        private static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _thread_id;

        public bool DebugOutputEnabled { get; set; } = true;

        /// <summary>
        /// Used by the UI to set the mouse cursor. Resets every frame. Disable with UICursorsEnabled.
        /// </summary>
        internal MouseCursor UICursor { get; set; } = MouseCursor.Arrow;

        /// <summary>
        /// Set to false to disable UI mouse cursor changes.
        /// </summary>
        public bool UICursorsEnabled { get; set; } = true;

        #endregion Properties

        #region Constructors

        public DWindow(DWindow parent = null)
        {
            _thread_id = Thread.CurrentThread.ManagedThreadId;

            if (parent != null)
            {
                Parent = parent;
                parent.Children.Add(this);
            }

            Graphics = new GraphicsDeviceManager(this);
            FirstUpdate += SetThreadID;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            Window.ClientSizeChanged += SetLayoutAreaToWindowArea;
            Window.TextInput += ProcessKeys;
            Exiting += ExitAll;
            System.Windows.Forms.Control.FromHandle(Window.Handle).MinimumSize = _minimum_size.ToSystemSize();
        }

        #endregion Constructors

        #region Event Handlers

        public event EventHandler<EventArgs> FirstUpdate;
        public event EventHandler<EventArgs> Updating;

        #endregion

        #region Events

        private void SetThreadID(object sender, EventArgs e)
        {
            _thread_id = Thread.CurrentThread.ManagedThreadId;
        }

        private void SetLayoutAreaToWindowArea(object sender, EventArgs e)
        {
            if (Layout != null)
            {
                Layout.Size = Area.Size;
            }
        }

        private void ProcessKeys(object sender, TextInputEventArgs e)
        {
            if (e.Character == 8) // If the key press is BackSpace
            {
                InputState.BackSpace = true;
            }
            else if (e.Character == 10 || e.Character == 13)
            {
                InputText.Append('\n');
                InputState.Enter = true;
            }
            else
            {
                InputText.Append(e.Character);
            }
        }

        /// <summary>
        /// Closes all child windows and removes own reference from parent on exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ExitAll(object sender, EventArgs args)
        {
            // Kill each of the children
            foreach (DWindow child in Children)
            {
                child.Exit();
            }

            Parent?.Children.Remove(this);
        }

        public void SignalSpawnWindowAsActive(object sender, EventArgs args)
        {
            Console.WriteLine("DWindow.SignalSpawnWindowAsActive: Setting spawned window as active");
            _spawned_window_is_active = 1;
            Debug.WriteLine($"_spawned_window_is_active = {_spawned_window_is_active}");
        }

        private void AreaSet(Rectangle value)
        {
            Graphics.PreferredBackBufferWidth = value.Width;
            Graphics.PreferredBackBufferHeight = value.Height;

            try
            {
                Graphics.ApplyChanges();
                ((System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle))
                    .Location = new System.Drawing.Point(value.X, value.Y);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"DWindow.AreaSet: Error, Failed to resize window. Message: {e.Message}");
            }

            Layout.Area = value.SizeOnly();
        }

        #endregion Event Handlers

        #region Protected Methods

        public DWindow CreateWindow(Type window_type)
        {
            _spawned_window_is_active = 0;
            CreateWindowDelegate.Invoke(window_type, this);

            Debug.WriteLine($"{GetType().Name}: Waiting for spawned window to be active");
            // Wait until the window is active to return
            do
            {
                Debug.WriteLine($"{GetType().Name}.CreateWindow: Waiting for spawned window to be active _spawned_window_is_active = {_spawned_window_is_active}");
                Thread.Sleep(5);
            } while (_spawned_window_is_active != 1);
            Debug.WriteLine($"{GetType().Name}: Waiting done");

            return Children[Children.Count - 1];
        }
        
        protected void UpdateDWindow(GameTime game_time)
        {
            _area_cache = Area;
            Updating?.Invoke(this, EventArgs.Empty);
            if (!_first_update)
            {
                FirstUpdate?.Invoke(this, EventArgs.Empty);
                _first_update = true;
            }
            ProcessQueuedEvents();
            HoveredWidgets.Reset();
            InputState.UpdateAll(Window, InputText.ToString(), game_time);
            //Console.WriteLine($"_keyboard_state.IsKeyDown(Keys.Left) = {Keyboard.GetState().IsKeyDown(Keys.Left)}");
            //Console.WriteLine(InputState.TextCursorMovement.Down);
            //Input.BufferedKeyboard o = new Input.BufferedKeyboard();
            //Input.BufferedKeyboard.Test();

            InputText.Clear();
            Layout.UpdatePriority(game_time, InputState);
            if (!Window.ClientBounds.Contains(InputState.CursorPosition + Window.ClientBounds.Location))
            {
                HoveredWidgets.Reset();
            }
            Layout.Update(game_time, InputState);
            if (UICursorsEnabled) Mouse.SetCursor(UICursor);
            UICursor = MouseCursor.Arrow;
            InputState.BackSpace = false;
            InputState.Enter = false;
            
        }

        #endregion Protected Methods

        private void ProcessQueuedEvents()
        {
            _event_queue_is_processing = true;

            foreach (int key in _area_set_events.Keys)
            {
                AreaSet(_area_set_events[key].Value);
                _area_set_events[key].Completed = true;
            }

            _event_queue_is_processing = false;
        }
    }
}