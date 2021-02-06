using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DownUnder.Input {
    /// <summary>
    /// A class meant to encompass the PlayerProfile class, integrating
    /// inputs from multiple different controllers.
    /// </summary>
    public sealed class InputSystem {
        readonly List<PlayerProfile> _players = new List<PlayerProfile>();
        readonly InputState _input_state = new InputState();
        readonly InputMode _input_mode = InputMode.StandardMultiPlayer();

        int _mouse_owner;
        int MouseOwner {
            get {
                if (_mouse_owner == -1) _mouse_owner = 0;
                if (_mouse_owner >= _players.Count) _mouse_owner = _players.Count - 1;
                return _mouse_owner;
            }
            set => _mouse_owner = value;
        }

        public InputSystem() {
        }

        public InputSystem(InputMode input_mode) =>
            _input_mode = input_mode;

        public Cursor GetCursor(int player_index) =>
            _input_state.GetCursor();

        public PseudoButton GetAction(
            int player_index,
            ActionType action_type
        ) {
            if (player_index >= _players.Count) {
                if (_players.Count == 0)
                    return new PseudoButton();

                Debug.WriteLine("InputSystem.GetAction: Player " + player_index + " not connected. " + _players.Count + " player(s) are connected.");
                return new PseudoButton();
            }

            var result = _players[player_index].GetActionButton(_input_state, action_type);

            if (_input_mode.combine_all_input && _players.Count > 1)
                for (var i = 1; i < _players.Count; i++)
                    result |= _players[i].GetActionButton(_input_state, action_type);

            return result;
        }

        /// <summary> Reorder update and manage new inputs. </summary>
        public void Update(GameWindow maybe_window = null) {
            _input_state.Update(maybe_window);
            ProcessNewInput(_input_state.GetNewPlayerInput());
        }

        public void DisconnectPlayers() =>
            _players.Clear();

        /// <summary> This function handles automatically adding new input. </summary>
        /// <param name="controllers_pressed"></param>
        void ProcessNewInput(IEnumerable<Controller> controllers_pressed) {
            // evaluate every new button press
            foreach (var c in controllers_pressed) {
                if (_players.Count >= _input_mode.maximum_player_limit)
                    break; // cancel if there are already too many controllers

                if (ControllerIsInUse(c))
                    continue; // skip if controller is already being used

                if (!_input_mode.append_new_players)
                    continue;

                _players.Add(new PlayerProfile(c, _players.Count + 1)); // add new player using new controller (if the option is enabled in input_mode)
                Debug.WriteLine($"Added new player; index = {_players.Count - 1}");
            }
        }

        bool ControllerIsInUse(Controller controller) {
            for (var p = 0; p < _players.Count; p++)
                if (_players[p]._current_controller.Equals(controller))
                    return true;

            return false;
        }
    }
}