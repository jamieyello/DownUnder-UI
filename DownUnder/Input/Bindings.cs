using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DownUnder.Input
{
    /// <summary>
    /// A list of "Binding"s, one for each action.
    /// </summary>
    [Serializable()]
    public class Bindings
    {
        public List<Binding> bindings = new List<Binding>();

        public Bindings()
        {
            int actions = Enum.GetNames(typeof(ActionType)).Length;
            for (int i = 0; i < actions; i++)
            {
                bindings.Add(new Binding());
            }
        }

        public void Set(List<Binding> binding_)
        {
            bindings = binding_;
        }

        public List<Binding> ToList()
        {
            return bindings;
        }

        public void AddCombo(int input_enum, ActionType action_type)
        {
            Binding binding = new Binding();
            binding.buttons_combo.Add(input_enum);
            Add(binding, action_type);
        }// --vvvv

        public void Add(int input_enum, ActionType action_type)
        {
            Binding binding = new Binding();
            binding.buttons.Add(input_enum);
            Add(binding, action_type);
        }// --v

        public void Add(Binding binding, ActionType action_type)
        {
            bindings[(int)action_type] += (Binding)binding.Clone();
        }// ---

        public object Clone()
        {
            Bindings clone = new Bindings
            {
                bindings = bindings.GetRange(0, bindings.Count)
            };
            return clone;
        }

        public static Bindings DefaultBindings(ControllerType controller_type)
        {
            switch (controller_type)
            {
                case ControllerType.keyboard:
                    return DefaultKeyboardBindings();

                case ControllerType.xbox:
                    return Default360Bindings();

                default:
                    Debug.WriteLine("Bindings.DefaultBindings: Unimplemented controller type.");
                    return new Bindings();
            }
        }

        private static Bindings Default360Bindings()
        {
            Bindings bindings = new Bindings();
            bindings.Add((int)Buttons.A, ActionType.menu_select);
            bindings.Add((int)Buttons.Start, ActionType.menu_select);
            bindings.Add((int)Buttons.B, ActionType.menu_back);

            bindings.Add((int)Buttons.DPadUp, ActionType.camera_zoom_in);
            bindings.Add((int)Buttons.DPadDown, ActionType.camera_zoom_out);

            bindings.Add((int)Buttons.LeftThumbstickLeft, ActionType.menu_select_left);
            bindings.Add((int)Buttons.LeftThumbstickRight, ActionType.menu_select_right);
            bindings.Add((int)Buttons.LeftThumbstickUp, ActionType.menu_select_up);
            bindings.Add((int)Buttons.LeftThumbstickDown, ActionType.menu_select_down);

            bindings.Add((int)Buttons.RightThumbstickLeft, ActionType.menu_select_left);
            bindings.Add((int)Buttons.RightThumbstickRight, ActionType.menu_select_right);
            bindings.Add((int)Buttons.RightThumbstickUp, ActionType.menu_select_up);
            bindings.Add((int)Buttons.RightThumbstickDown, ActionType.menu_select_down);

            bindings.Add((int)Buttons.LeftThumbstickLeft, ActionType.left_movement);
            bindings.Add((int)Buttons.LeftThumbstickRight, ActionType.right_movement);
            bindings.Add((int)Buttons.LeftThumbstickUp, ActionType.up_movement);
            bindings.Add((int)Buttons.LeftThumbstickDown, ActionType.down_movement);

            bindings.Add((int)Buttons.RightTrigger, ActionType.menu_select_speed_modifier);
            bindings.Add((int)Buttons.LeftTrigger, ActionType.menu_select_speed_modifier);
            return bindings;
        }

        private static Bindings DefaultKeyboardBindings()
        {
            Bindings bindings = new Bindings();
            bindings.Add((int)Keys.Enter, ActionType.menu_select);

            bindings.Add((int)Keys.Left, ActionType.left_movement);
            bindings.Add((int)Keys.Right, ActionType.right_movement);
            bindings.Add((int)Keys.Up, ActionType.up_movement);
            bindings.Add((int)Keys.Down, ActionType.down_movement);

            bindings.Add((int)Keys.Enter, ActionType.menu_select);
            bindings.Add((int)Keys.Space, ActionType.menu_select);
            bindings.Add((int)Keys.Left, ActionType.menu_select_left);
            bindings.Add((int)Keys.Right, ActionType.menu_select_right);
            bindings.Add((int)Keys.Up, ActionType.menu_select_up);
            bindings.Add((int)Keys.Down, ActionType.menu_select_down);

            bindings.Add((int)Keys.A, ActionType.menu_select_left);
            bindings.Add((int)Keys.D, ActionType.menu_select_right);
            bindings.Add((int)Keys.W, ActionType.menu_select_up);
            bindings.Add((int)Keys.S, ActionType.menu_select_down);
            bindings.Add((int)Keys.LeftShift, ActionType.menu_select_speed_modifier);
            return bindings;
        }
    }
}