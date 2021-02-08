using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.Input {
    /// <summary>
    /// If you have used monogame before, (and most likely you have)
    /// this class handles updating GamePadStates and KeyboardStates.
    /// It's used just once, by InputSystem.
    /// </summary>
    public sealed class InputState {
        public KeyboardState keyboard_state = Keyboard.GetState();
        public KeyboardState previous_keyboard_state;

        public MouseState mouse_state = Mouse.GetState();
        public MouseState previous_mouse_state;

        public readonly GamePadState[] game_pad_state = new GamePadState[4];
        public readonly GamePadState[] previous_game_pad_state = new GamePadState[4];

        public InputState() {
            // Update is called twice to remove old pad states.
            Update();
            Update();
        }

        public void Update(GameWindow maybe_window = null) {
            previous_keyboard_state = keyboard_state;
            keyboard_state = Keyboard.GetState();

            previous_mouse_state = mouse_state;

            if (maybe_window is { } window)
                mouse_state = Mouse.GetState(window);
            else
                mouse_state = Mouse.GetState();

            for (var i = 0; i < 4; i++) {
                previous_game_pad_state[i] = game_pad_state[i];
                game_pad_state[i] = GamePad.GetState(i);
            }
        }

        public Cursor GetCursor() =>
            new Cursor {
                Position = mouse_state.Position,
                HeldDown = mouse_state.LeftButton == ButtonState.Pressed,
                Triggered = mouse_state.LeftButton == ButtonState.Pressed && previous_mouse_state.LeftButton != ButtonState.Pressed
            };

        /// <summary>
        /// This function compares new inputs with old inputs. It returns
        /// a list of controllers that have been pressed in-between this frame
        /// and the last.
        /// </summary>
        /// <returns></returns>
        public List<Controller> GetNewPlayerInput() {
            var controllers_pressed = new List<Controller>();

            var keyboard = new Controller(ControllerType.keyboard, 0);
            if (ButtonWasPressed(keyboard))
                controllers_pressed.Add(keyboard);

            for (var i = 0; i < 4; i++) {
                if (!ButtonWasPressed(new Controller(ControllerType.xbox, i)))
                    continue;

                controllers_pressed.Add(new Controller(ControllerType.xbox, i));
            }

            return controllers_pressed;
        }

        // An extension of the above function.
        bool ButtonWasPressed(Controller controller) {
            switch (controller.Type) {
                case ControllerType.keyboard:
                    return keyboard_state != previous_keyboard_state;

                case ControllerType.xbox:
                    var state = game_pad_state[controller.Index];
                    var left = state.ThumbSticks.Left;

                    if (Math.Abs(left.X) > 0.9f) return true;
                    if (Math.Abs(left.Y) > 0.9f) return true;

                    var prev_state = previous_game_pad_state[controller.Index];
                    return state.Buttons.GetHashCode() != prev_state.Buttons.GetHashCode();

                case ControllerType.null_:
                    Debug.WriteLine("InputState.ButtonWasPressed: Null input type asked for.");
                    break;

                default:
                    Debug.WriteLine("InputState.ButtonWasPressed: Unsupported input type: " + controller.Type);
                    break;
            }
            return false;
        }
    }
}