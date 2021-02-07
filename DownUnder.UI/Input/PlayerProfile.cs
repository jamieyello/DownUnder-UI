using System;

namespace DownUnder.UI.Input {
    [Serializable]
    sealed class PlayerProfile {
        readonly string _name;
        public Controller _current_controller;
        public BindingsSet _bindings_set = new BindingsSet();

        public PlayerProfile(
            Controller current_controller,
            string name = ""
        ) {
            _current_controller = current_controller.Clone();
            _name = name;
        }

        public PlayerProfile(
            Controller current_controller,
            int index
        ) : this(
            current_controller,
            $"Player {index}"
        ) {
        }

        public PseudoButton GetActionButton(
            InputState input_state,
            ActionType action_type
        ) =>
            _bindings_set.GetAction(input_state, _current_controller, action_type);
    }
}