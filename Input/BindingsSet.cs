using System;
using System.Collections.Generic;

namespace DownUnder.Input {
    /// <summary> A list of "Bindings"s, one for each controller. </summary>
    [Serializable]
    public sealed class BindingsSet {
        readonly List<Bindings> _bindings_set = new List<Bindings>();

        public BindingsSet() {
            var numActions = Enum.GetNames(typeof(ControllerType)).Length;

            for (var i = 0; i < numActions; i++)
                _bindings_set.Add(new Bindings());

            SetDefaults();
        }

        /// <summary> This will have to be (and stay) here. </summary>
        public void SetDefaults() {
            var numControllers = Enum.GetNames(typeof(ControllerType)).Length;

            for (var i = 0; i < numControllers; i++) {
                var controllerType = (ControllerType)i;
                SetBindings(controllerType, Bindings.DefaultBindings(controllerType));
            }
        }

        public Binding GetBinding(
            Controller controller,
            ActionType action_type
        ) =>
            _bindings_set[(int)controller.type][(int)action_type];

        public void SetBinding(
            ControllerType controller_type,
            ActionType action_type,
            Binding binding
        ) =>
            _bindings_set[(int)controller_type][(int)action_type] = binding.Clone();

        public void SetBindings(
            ControllerType controller_type,
            Bindings bindings
        ) =>
            _bindings_set[(int)controller_type] = bindings.Clone();

        public PseudoButton GetAction(
            InputState input_state,
            Controller controller,
            ActionType action_type
        ) {
            var binding = GetBinding(controller, action_type);

            var buttons_result = new PseudoButton();
            for (var i = 0; i < binding.NumButtons; i++)
                buttons_result |= DirectInput.GetPressedButton(controller, binding.GetButton(i), input_state);

            if (binding.NumCombos <= 0)
                return buttons_result | new PseudoButton(); // TODO: does ORing here do nothing?

            var combo_result = new PseudoButton(true, 1f, true);
            for (var i = 0; i < binding.NumButtons; i++)
                combo_result &= DirectInput.GetPressedButton(controller, binding.GetButton(i), input_state);

            return buttons_result | combo_result;
        }
    }
}