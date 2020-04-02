using DownUnder.Input;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

//Todo: Trigger inputs shopuld not have set parameters
namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> Used to communicate to widgets input. Can be updated or set manually. </summary>
    public class UIInputState
    {
        private Vector2 _previous_mouse_wheel_scroll = new Vector2(Mouse.GetState().HorizontalScrollWheelValue, Mouse.GetState().ScrollWheelValue);
        private MouseState _mouse_state = Mouse.GetState();
        private MouseState _previous_mouse_state = Mouse.GetState();
        private KeyboardState _keyboard_state = Keyboard.GetState();
        private KeyboardState _previous_keyboard_state = Keyboard.GetState();
        private BufferedKeyboard buffered_keyboard = new BufferedKeyboard();
        private bool _insert_key_down = false;
        private bool _previous_insert_key_down = false;

        /// <summary> True when primary input is held down./ </summary>
        public bool PrimaryClick { get; set; } = false;
        
        /// <summary> True the frame primary input is held down./ </summary>
        public bool PrimaryClickTriggered { get; set; } = false;

        /// <summary> Used when secondary input is held down. </summary>
        public bool SecondaryClick { get; set; } = false;

        /// <summary> True the frame secondary input is held down. </summary>
        public bool SecondaryClickTriggered { get; set; } = false;

        /// <summary> Used for selecting multiple widgets. </summary>
        public bool Control { get; set; } = false;

        /// <summary> Used to move the cursor. </summary>
        public Directions2D TextCursorMovement = new Directions2D() { AllowOpposites = false };

        /// <summary> Used for backspacing text. </summary>
        public bool BackSpace { get; set; } = false;

        /// <summary> Used to represent the Enter key. </summary>
        public bool Enter { get; set; } = false;

        /// <summary> The mouse position relative to the parent window. </summary>
        public Point2 CursorPosition { get; set; } = new Point2();

        /// <summary> Used for text input. </summary>
        public string Text { get; set; } = "";

        public Vector2 Scroll { get; set; } = new Vector2();
        
        /// <summary> Represents the shift key./ </summary>
        public bool Shift { get; set; } = false;

        /// <summary> Represents the DEL delete key. </summary>
        public bool Delete { get; set; } = false;

        /// <summary> Represents the INS insert key. </summary>
        public bool Insert { get; set; } = false;

        /// <summary> Represents the home key. </summary>
        public bool Home { get; set; } = false;

        /// <summary> Represents the end key. </summary>
        public bool End { get; set; } = false;

        /// <summary> Represents the state of Caps Lock. </summary>
        public bool CapsLock { get; set; } = false;

        /// <summary> Represents the state of Num Lock. </summary>
        public bool NumLock { get; set; } = false;

        /// <summary> Used to select all items/text. </summary>
        public bool SelectAll { get; set; } = false;

        /// <summary> Used to copy items/text. </summary>
        public bool Copy { get; set; } = false;

        /// <summary> Used to cut items/text. </summary>
        public bool Cut { get; set; } = false;

        /// <summary> Used to paste items/text. </summary>
        public bool Paste { get; set; } = false;

        /// <summary> Used to save the current state/project to file. (ctrl + s) </summary>
        public bool Save { get; set; } = false;

        /// <summary> Used to create a new project. </summary>
        public bool New { get; set; } = false;

        /// <summary> Used to undo changes to items/text. </summary>
        public bool Undo { get; set; } = false;

        /// <summary> Used to redo changes to items/text. </summary>
        public bool Redo { get; set; } = false;

        /// <summary> Set this UIUnputState's values to the typical input from a mouse + keyboard. </summary>
        public void UpdateAll(DWindow dwindow, GameTime game_time)
        {
            _previous_mouse_state = _mouse_state;
            _mouse_state = Mouse.GetState(dwindow.Window);
            _previous_keyboard_state = _keyboard_state;
            _keyboard_state = Keyboard.GetState();

            PrimaryClick = _mouse_state.LeftButton == ButtonState.Pressed;
            SecondaryClick = _mouse_state.RightButton == ButtonState.Pressed;

            if (PrimaryClick && _previous_mouse_state.LeftButton == ButtonState.Released)
            {
                PrimaryClickTriggered = true;
            }
            else
            {
                PrimaryClickTriggered = false;
            }
            if (SecondaryClick && _previous_mouse_state.RightButton == ButtonState.Released)
            {
                SecondaryClickTriggered = true;
            }
            else
            {
                SecondaryClickTriggered = false;
            }

            CursorPosition = _mouse_state.Position;

            Scroll = new Vector2(_mouse_state.HorizontalScrollWheelValue, _mouse_state.ScrollWheelValue) - _previous_mouse_wheel_scroll;
            _previous_mouse_wheel_scroll = new Vector2(_mouse_state.HorizontalScrollWheelValue, _mouse_state.ScrollWheelValue);

            UpdateKeyboardInput(dwindow, game_time);
        }

        /// <summary> Set this UIUnputState's values to the typical input from a keyboard. Doesn't update cursor values. </summary>
        public void UpdateKeyboardInput(DWindow dwindow, GameTime game_time)
        {
            buffered_keyboard.Update(game_time.GetElapsedSeconds());

            TextCursorMovement.Left = buffered_keyboard.IsKeyTriggered(Keys.Left);
            TextCursorMovement.Right = buffered_keyboard.IsKeyTriggered(Keys.Right);
            TextCursorMovement.Up = buffered_keyboard.IsKeyTriggered(Keys.Up);
            TextCursorMovement.Down = buffered_keyboard.IsKeyTriggered(Keys.Down);

            Shift = _keyboard_state.IsKeyDown(Keys.LeftShift) || _keyboard_state.IsKeyDown(Keys.RightShift);
            Control = _keyboard_state.IsKeyDown(Keys.LeftControl) || _keyboard_state.IsKeyDown(Keys.RightControl);
            Delete = buffered_keyboard.IsKeyTriggered(Keys.Delete);
            Home = buffered_keyboard.IsKeyTriggered(Keys.Home);
            End = buffered_keyboard.IsKeyTriggered(Keys.End);
            CapsLock = _keyboard_state.CapsLock;
            NumLock = _keyboard_state.NumLock;
            
            Text = dwindow.InputText.ToString();
            SelectAll = false;
            Copy = false;
            Cut = false;
            Paste = false;
            Save = false;
            New = false;
            Undo = false;
            Redo = false;
            foreach (char c in dwindow.CommandText.ToString())
            {
                //Console.WriteLine((int)c);
                if (c == 1) SelectAll = true;
                if (c == 3) Copy = true;
                if (c == 24) Cut = true;
                if (c == 22) Paste = true;
                if (c == 19) Save = true;
                if (c == 14) New = true;
                if (c == 26) Undo = true;
                if (c == 25) Redo = true;
            }

            _previous_insert_key_down = _insert_key_down;
            _insert_key_down = buffered_keyboard.IsKeyTriggered(Keys.Insert);
            if (_insert_key_down && !_previous_insert_key_down)
            {
                Insert = !Insert;
            }
        }
    }
}