using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.Input {
    /// <summary> A list of "Binding"s, one for each action. </summary>
    [Serializable]
    public sealed class Bindings : IEnumerable<Binding> {
        readonly List<Binding> _bindings = new List<Binding>();

        public Binding this[int index] { get => _bindings[index]; set => _bindings[index] = value; }

        public Bindings() {
            var numActions = Enum.GetNames(typeof(ActionType)).Length;
            for (var i = 0; i < numActions; i++)
                _bindings.Add(new Binding());
        }

        Bindings(Bindings source) =>
            _bindings = source._bindings.ToList();

        public IEnumerator<Binding> GetEnumerator() => _bindings.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddCombo(
            int input_enum,
            ActionType action_type
        ) {
            var binding = new Binding();
            binding.AddCombo(input_enum);
            Add(binding, action_type);
        }// --vvvv

        public void Add(
            Buttons button,
            ActionType action_type
        ) =>
            Add((int)button, action_type);

        public void Add(
            Keys keys,
            ActionType action_type
        ) =>
            Add((int)keys, action_type);

        void Add(
            int input_enum,
            ActionType action_type
        ) {
            var binding = new Binding();
            binding.AddButton(input_enum);
            Add(binding, action_type);
        }// --v

        public void Add(
            Binding binding,
            ActionType action_type
        ) =>
            _bindings[(int)action_type] += binding;
        // ---

        public Bindings Clone() =>
            new Bindings(this);

        public static Bindings DefaultBindings(
            ControllerType controller_type
        ) {
            switch (controller_type) {
                case ControllerType.keyboard:
                    return GetDefaultKeyboardBindings();

                case ControllerType.xbox:
                    return GetDefault360Bindings();

                default:
                    Debug.WriteLine("Bindings.DefaultBindings: Unimplemented controller type.");
                    return new Bindings();
            }
        }

        static Bindings GetDefault360Bindings() =>
            new Bindings {
                { Buttons.A, ActionType.menu_select },
                { Buttons.Start, ActionType.menu_select },
                { Buttons.B, ActionType.menu_back },

                { Buttons.DPadUp, ActionType.camera_zoom_in },
                { Buttons.DPadDown, ActionType.camera_zoom_out },

                { Buttons.LeftThumbstickLeft, ActionType.menu_select_left },
                { Buttons.LeftThumbstickRight, ActionType.menu_select_right },
                { Buttons.LeftThumbstickUp, ActionType.menu_select_up },
                { Buttons.LeftThumbstickDown, ActionType.menu_select_down },

                { Buttons.RightThumbstickLeft, ActionType.menu_select_left },
                { Buttons.RightThumbstickRight, ActionType.menu_select_right },
                { Buttons.RightThumbstickUp, ActionType.menu_select_up },
                { Buttons.RightThumbstickDown, ActionType.menu_select_down },

                { Buttons.LeftThumbstickLeft, ActionType.left_movement },
                { Buttons.LeftThumbstickRight, ActionType.right_movement },
                { Buttons.LeftThumbstickUp, ActionType.up_movement },
                { Buttons.LeftThumbstickDown, ActionType.down_movement },

                { Buttons.RightTrigger, ActionType.menu_select_speed_modifier },
                { Buttons.LeftTrigger, ActionType.menu_select_speed_modifier }
            };

        static Bindings GetDefaultKeyboardBindings() =>
            new Bindings {
                { Keys.Enter, ActionType.menu_select },
                { Keys.Left, ActionType.left_movement },
                { Keys.Right, ActionType.right_movement },
                { Keys.Up, ActionType.up_movement },
                { Keys.Down, ActionType.down_movement },
                { Keys.Enter, ActionType.menu_select },
                { Keys.Space, ActionType.menu_select },
                { Keys.Left, ActionType.menu_select_left },
                { Keys.Right, ActionType.menu_select_right },
                { Keys.Up, ActionType.menu_select_up },
                { Keys.Down, ActionType.menu_select_down },
                { Keys.A, ActionType.menu_select_left },
                { Keys.D, ActionType.menu_select_right },
                { Keys.W, ActionType.menu_select_up },
                { Keys.S, ActionType.menu_select_down },
                { Keys.LeftShift, ActionType.menu_select_speed_modifier }
            };
    }
}