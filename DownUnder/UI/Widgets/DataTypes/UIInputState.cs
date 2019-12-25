using DownUnder.Input;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary>
    /// Used to update widgets with mouse/keyboard information.
    /// </summary>
    public class UIInputState
    {
        private Vector2 _previous_mouse_wheel_scroll = new Vector2(Mouse.GetState().HorizontalScrollWheelValue, Mouse.GetState().ScrollWheelValue);
        private MouseState _mouse_state = Mouse.GetState();
        private MouseState _previous_mouse_state = Mouse.GetState();
        private KeyboardState _keyboard_state = Keyboard.GetState();
        private KeyboardState _previous_keyboard_state = Keyboard.GetState();
        private BufferedKeyboard buffered_keyboard = new BufferedKeyboard();

        /// <summary>
        /// Used for selecting widgets.
        /// </summary>
        public bool PrimaryClick { get; set; }

        public bool PrimaryClickTriggered { get; set; }

        /// <summary>
        /// Used to create right-click menus.
        /// </summary>
        public bool SecondaryClick { get; set; }

        public bool SecondaryClickTriggered { get; set; }

        /// <summary>
        /// Used for selecting multiple widgets.
        /// </summary>
        public bool Control { get; set; }

        /// <summary>
        /// Used to move the cursor.
        /// </summary>
        public Directions2D TextCursorMovement = new Directions2D() { AllowOpposites = false };

        /// <summary>
        /// Used for backspacing text.
        /// </summary>
        public bool Back { get; set; }

        /// <summary>
        /// Used for confirming selection/text entry.
        /// </summary>
        public bool Confirm { get; set; }

        /// <summary>
        /// The mouse position relative to the parent window.
        /// </summary>
        public Point2 CursorPosition { get; set; }

        /// <summary>
        /// Used for text input.
        /// </summary>
        public string Text { get; set; } = "";

        public Vector2 Scroll { get; set; } = new Vector2();
        
        /// <summary>
        /// Set this UIUnputState to the typical input from a mouse + keyboard.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public void UpdateAll(GameWindow window, string text_input, GameTime game_time)
        {
            _previous_mouse_state = _mouse_state;
            _mouse_state = Mouse.GetState(window);
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
            Control = _keyboard_state.IsKeyDown(Keys.Up | Keys.RightControl);
            Text = text_input;

            buffered_keyboard.Update(game_time.GetElapsedSeconds());

            TextCursorMovement.Left = buffered_keyboard.IsKeyTriggered(Keys.Left);
            TextCursorMovement.Right = buffered_keyboard.IsKeyTriggered(Keys.Right);
            TextCursorMovement.Up = buffered_keyboard.IsKeyTriggered(Keys.Up);
            TextCursorMovement.Down = buffered_keyboard.IsKeyTriggered(Keys.Down);
            
            Scroll = new Vector2(_mouse_state.HorizontalScrollWheelValue, _mouse_state.ScrollWheelValue) - _previous_mouse_wheel_scroll;
            _previous_mouse_wheel_scroll = new Vector2(_mouse_state.HorizontalScrollWheelValue, _mouse_state.ScrollWheelValue);
        }
    }
}