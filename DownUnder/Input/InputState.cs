using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DownUnder.Input
{
    /// <summary>
    /// If you have used monogame before, (and most likely you have)
    /// this class handles updating GamePadStates and KeyboardStates.
    /// It's used just once, by InputSystem.
    /// </summary>
    public class InputState
    {
        public KeyboardState keyboard_state = Keyboard.GetState();
        public KeyboardState previous_keyboard_state;

        public MouseState mouse_state = Mouse.GetState();
        public MouseState previous_mouse_state;

        public GamePadState[] game_pad_state = new GamePadState[4];
        public GamePadState[] previous_game_pad_state = new GamePadState[4];

        public InputState()
        {
            // Update is called twice to remove old pad states.
            Update();
            Update();
        }

        public void Update(GameWindow window = null)
        {
            previous_keyboard_state = keyboard_state;
            keyboard_state = Keyboard.GetState();

            previous_mouse_state = mouse_state;
            if (window == null)
            {
                mouse_state = Mouse.GetState();
            }
            else
            {
                mouse_state = Mouse.GetState(window);
            }
            
            for (int i = 0; i < 4; i++)
            {
                previous_game_pad_state[i] = game_pad_state[i];
                game_pad_state[i] = GamePad.GetState(i);
            }
        }

        public Cursor GetCursor()
        {
            return new Cursor()
            {
                Position = mouse_state.Position,
                HeldDown = (mouse_state.LeftButton == ButtonState.Pressed),
                Triggered = (mouse_state.LeftButton == ButtonState.Pressed) && (previous_mouse_state.LeftButton != ButtonState.Pressed)
            };
        }

        /// <summary>
        /// This function compares new inputs with old inputs. It returns
        /// a list of controllers that have been pressed in-between this frame
        /// and the last.
        /// </summary>
        /// <returns></returns>
        public List<Controller> GetNewPlayerInput()
        {
            List<Controller> controllers_pressed = new List<Controller>();
            if (ButtonWasPressed(new Controller(ControllerType.keyboard, 0)))
            {
                controllers_pressed.Add(new Controller(ControllerType.keyboard, 0));
            }
            for (int i = 0; i < 4; i++)
            {
                if (ButtonWasPressed(new Controller(ControllerType.xbox, i)))
                {
                    controllers_pressed.Add(new Controller(ControllerType.xbox, i));
                }
            }

            return controllers_pressed;
        }

        // An extension of the above function.
        private bool ButtonWasPressed(Controller controller)
        {
            switch (controller.type)
            {
                case ControllerType.keyboard:
                    return (keyboard_state != previous_keyboard_state);

                case ControllerType.xbox:
                    if (Math.Abs(game_pad_state[controller.index].ThumbSticks.Left.X) > 0.9f) return true;
                    if (Math.Abs(game_pad_state[controller.index].ThumbSticks.Left.Y) > 0.9f) return true;
                    return game_pad_state[controller.index].Buttons.GetHashCode() != previous_game_pad_state[controller.index].Buttons.GetHashCode();

                case ControllerType.null_:
                    Debug.WriteLine("InputState.ButtonWasPressed: Null input type asked for.");
                    break;

                default:
                    Debug.WriteLine("InputState.ButtonWasPressed: Unsupported input type: " + controller.type.ToString());
                    break;
            }
            return false;
        }
    }
}