using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace DownUnder.Input
{
    /// <summary>
    /// Not to be confused with "the other" DInput, this Direct Input accesses input directly.
    /// </summary>
    public static class DirectInput
    {
        public static PseudoButton GetPressedButton(Controller controller, int button_index, InputState input_state)
        {
            switch (controller.type)
            {
                case ControllerType.keyboard:
                    return new PseudoButton(
                        input_state.keyboard_state.IsKeyDown((Keys)button_index),                                                                       // bool value from a key press
                        (float)System.Convert.ToDouble((input_state.keyboard_state).IsKeyDown((Keys)button_index)),                                     // float value from a key press
                        input_state.keyboard_state.IsKeyDown((Keys)button_index) && !input_state.previous_keyboard_state.IsKeyDown((Keys)button_index)  // triggered value for the first frame
                        );

                case ControllerType.xbox:
                    return new PseudoButton(
                        input_state.game_pad_state[controller.index].IsButtonDown((Buttons)button_index),                      // bool value from a key press
                        GetXboxAnalog(button_index, input_state.game_pad_state[controller.index]),                                // float value from a key press
                        input_state.game_pad_state[controller.index].IsButtonDown((Buttons)button_index) && !input_state.previous_game_pad_state[controller.index].IsButtonDown((Buttons)button_index)
                        );

                case ControllerType.mouse:
                    double mouse_button = GetMouseButton(button_index, input_state, out bool triggered);
                    return new PseudoButton(
                        mouse_button >= 1,
                        (float)mouse_button,
                        triggered
                        );
            }

            Debug.WriteLine("Button Action.GetPressedButton: ControllerType." + controller.type.ToString() + " is not supported.");
            return new PseudoButton();
        }

        private static double GetMouseButton(int button_index, InputState input_state, out bool triggered)
        {
            switch ((MouseButtons)button_index)
            {
                case MouseButtons.left_button:
                    if (input_state.mouse_state.LeftButton == ButtonState.Pressed)
                    {
                        triggered = false;
                        if (input_state.previous_mouse_state.LeftButton != ButtonState.Pressed)
                        {
                            triggered = true;
                        }
                        return 1;
                    }
                    triggered = false;
                    return 0;

                case MouseButtons.right_button:
                    if (input_state.mouse_state.RightButton == ButtonState.Pressed)
                    {
                        triggered = false;
                        if (input_state.previous_mouse_state.RightButton != ButtonState.Pressed)
                        {
                            triggered = true;
                        }
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

        private static float GetXboxAnalog(int button_index, GamePadState game_pad_state)
        {
            switch ((Buttons)button_index)
            {
                case Buttons.LeftThumbstickLeft:
                    if (game_pad_state.ThumbSticks.Left.X > 0) { return 0f; }
                    return -game_pad_state.ThumbSticks.Left.X;

                case Buttons.LeftThumbstickRight:
                    if (game_pad_state.ThumbSticks.Left.X < 0) { return 0f; }
                    return game_pad_state.ThumbSticks.Left.X;

                case Buttons.LeftThumbstickUp:
                    if (game_pad_state.ThumbSticks.Left.Y < 0) { return 0f; }
                    return game_pad_state.ThumbSticks.Left.Y;

                case Buttons.LeftThumbstickDown:
                    if (game_pad_state.ThumbSticks.Left.Y > 0) { return 0f; }
                    return -game_pad_state.ThumbSticks.Left.Y;

                case Buttons.RightThumbstickLeft:
                    if (game_pad_state.ThumbSticks.Right.X > 0) { return 0f; }
                    return -game_pad_state.ThumbSticks.Right.X;

                case Buttons.RightThumbstickRight:
                    if (game_pad_state.ThumbSticks.Right.X < 0) { return 0f; }
                    return game_pad_state.ThumbSticks.Right.X;

                case Buttons.RightThumbstickUp:
                    if (game_pad_state.ThumbSticks.Right.Y < 0) { return 0f; }
                    return game_pad_state.ThumbSticks.Right.Y;

                case Buttons.RightThumbstickDown:
                    if (game_pad_state.ThumbSticks.Right.Y > 0) { return 0f; }
                    return -game_pad_state.ThumbSticks.Right.Y;

                case Buttons.RightTrigger:
                    return game_pad_state.Triggers.Right;

                case Buttons.LeftTrigger:
                    return game_pad_state.Triggers.Left;

                default:
                    if (game_pad_state.IsButtonDown((Buttons)button_index))
                    {
                        return 1f;
                    }
                    return 0f;
            }
        }
    }
}