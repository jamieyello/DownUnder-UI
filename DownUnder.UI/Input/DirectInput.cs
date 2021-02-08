using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.Input {
    /// <summary> Not to be confused with "the other" DInput, this Direct Input accesses input directly. </summary>
    public static class DirectInput {
        public static PseudoButton GetPressedButton(
            Controller controller,
            int button_index,
            InputState input_state
        ) {
            switch (controller.Type) {
                case ControllerType.keyboard:
                    var keys = (Keys)button_index;
                    var was_key_down = input_state.previous_keyboard_state.IsKeyDown(keys);
                    var is_key_down = input_state.keyboard_state.IsKeyDown(keys);
                    var key_was_just_pressed = is_key_down && !was_key_down;

                    return new PseudoButton(
                        is_key_down,                          // bool value from a key press
                        System.Convert.ToSingle(is_key_down), // float value from a key press
                        key_was_just_pressed
                    );

                case ControllerType.xbox:
                    var buttons = (Buttons)button_index;
                    var was_button_down = input_state.previous_game_pad_state[controller.Index].IsButtonDown(buttons);
                    var game_pad_State = input_state.game_pad_state[controller.Index];
                    var is_button_down = game_pad_State.IsButtonDown(buttons);
                    var button_was_just_pressed = is_button_down && !was_button_down;

                    return new PseudoButton(
                        is_button_down,                              // bool value from a button press
                        GetXboxAnalog(button_index, game_pad_State), // float value from a button press
                        button_was_just_pressed
                    );

                case ControllerType.mouse:
                    var mouse_button = GetMouseButton(button_index, input_state, out var triggered);

                    return new PseudoButton(
                        mouse_button >= 1,
                        (float)mouse_button,
                        triggered
                    );
            }

            Debug.WriteLine("Button Action.GetPressedButton: ControllerType." + controller.Type + " is not supported.");
            return new PseudoButton();
        }

        static double GetMouseButton(
            int button_index,
            InputState input_state,
            out bool triggered
        ) {
            switch ((MouseButtons)button_index) {
                case MouseButtons.left_button:
                    if (input_state.mouse_state.LeftButton == ButtonState.Pressed) {
                        triggered = input_state.previous_mouse_state.LeftButton != ButtonState.Pressed;
                        return 1;
                    }
                    triggered = false;
                    return 0;

                case MouseButtons.right_button:
                    if (input_state.mouse_state.RightButton == ButtonState.Pressed)
                    {
                        triggered = input_state.previous_mouse_state.RightButton != ButtonState.Pressed;
                        return 1;
                    }
                    triggered = false;
                    return 0;

                case MouseButtons.x_movement:
                    triggered = false;
                    return input_state.mouse_state.Position.X - input_state.previous_mouse_state.Position.X;

                case MouseButtons.y_movement:
                    triggered = false;
                    return input_state.mouse_state.Position.Y - input_state.previous_mouse_state.Position.Y;

                default:
                    Debug.WriteLine($"Error in DirectInput.cs GetMouseButton(): mouse button {button_index} does not exist.");
                    triggered = false;
                    return 0;
            }
        }

        static float GetXboxAnalog(
            int button_index,
            GamePadState game_pad_state
        ) {
            var left = game_pad_state.ThumbSticks.Left;
            var right = game_pad_state.ThumbSticks.Right;

            switch ((Buttons)button_index) {
                case Buttons.LeftThumbstickLeft: return left.X > 0 ? 0 : -left.X;
                case Buttons.LeftThumbstickRight: return left.X < 0 ? 0 : left.X;

                case Buttons.LeftThumbstickUp: return left.Y < 0 ? 0 : left.Y;
                case Buttons.LeftThumbstickDown: return left.Y > 0 ? 0 : -left.Y;

                case Buttons.RightThumbstickLeft: return right.X > 0 ? 0 : -right.X;
                case Buttons.RightThumbstickRight: return right.X < 0 ? 0f : right.X;

                case Buttons.RightThumbstickUp: return right.Y < 0 ? 0f : right.Y;
                case Buttons.RightThumbstickDown: return right.Y > 0 ? 0f : -right.Y;

                case Buttons.RightTrigger: return game_pad_state.Triggers.Right;
                case Buttons.LeftTrigger: return game_pad_state.Triggers.Left;

                default: return game_pad_state.IsButtonDown((Buttons)button_index) ? 1f : 0f;
            }
        }
    }
}