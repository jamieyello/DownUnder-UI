using System;
using System.Collections.Generic;

namespace DownUnder.Input
{
    /// <summary>
    /// A list of "Bindings"s, one for each controller.
    /// </summary>
    [Serializable()]
    public class BindingsSet
    {
        public List<Bindings> bindings_set = new List<Bindings>();

        public BindingsSet()
        {
            int actions = Enum.GetNames(typeof(ControllerType)).Length;
            for (int i = 0; i < actions; i++)
            {
                bindings_set.Add(new Bindings());
            }
            SetDefaults();
        }

        /// <summary>
        /// This will have to be (and stay) here.
        /// </summary>
        public void SetDefaults()
        {
            int controllers = Enum.GetNames(typeof(ControllerType)).Length;
            for (int i = 0; i < controllers; i++)
            {
                SetBindings((ControllerType)i, Bindings.DefaultBindings((ControllerType)i));
            }
        }

        public Binding GetBinding(Controller controller, ActionType action_type)
        {
            return bindings_set[(int)controller.type].bindings[(int)action_type];
        }

        public void SetBinding(ControllerType controller_type, ActionType action_type, Binding binding)
        {
            bindings_set[(int)controller_type].bindings[(int)action_type] = (Binding)binding.Clone();
        }

        public void SetBindings(ControllerType controller_type, Bindings bindings)
        {
            bindings_set[(int)controller_type] = (Bindings)bindings.Clone();
        }

        public PseudoButton GetAction(InputState input_state, Controller controller, ActionType action_type)
        {
            Binding binding = GetBinding(controller, action_type);

            PseudoButton buttons_result = new PseudoButton();
            for (int i = 0; i < binding.buttons.Count; i++)
            {
                buttons_result |= DirectInput.GetPressedButton(controller, binding.buttons[i], input_state);
            }

            PseudoButton combo_result = new PseudoButton();
            if (binding.buttons_combo.Count > 0)
            {
                combo_result = new PseudoButton(true, 1f, true);
                for (int i = 0; i < binding.buttons.Count; i++)
                {
                    combo_result &= DirectInput.GetPressedButton(controller, binding.buttons[i], input_state);
                }
            }

            return buttons_result | combo_result;
        }
    }
}