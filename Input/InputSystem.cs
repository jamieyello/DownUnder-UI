using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace DownUnder.Input
{
    /// <summary>
    /// A class meant to encompass the PlayerProfile class, integrating
    /// inputs from multiple different controllers.
    /// </summary>
    public class InputSystem
    {
        private List<PlayerProfile> players = new List<PlayerProfile>();
        public InputState input_state = new InputState();
        public InputMode input_mode = InputMode.StandardMultiPlayer();

        int _mouse_owner = 0;
        int _MouseOwner
        {
            get
            {
                if (_mouse_owner == -1)
                {
                    _mouse_owner = 0;
                }

                if (_mouse_owner >= players.Count)
                {
                    _mouse_owner = players.Count - 1;
                }
                return _mouse_owner;
            }
            set => _mouse_owner = value;
        }

        public InputSystem()
        {
        }

        public InputSystem(InputMode input_mode)
        {
            this.input_mode = input_mode;
        }

        public Cursor GetCursor(int player_index)
        {
            return input_state.GetCursor();
        }

        public PseudoButton GetAction(int player_index, ActionType action_type)
        {
            if (player_index >= players.Count)
            {
                if (players.Count == 0) { return new PseudoButton(); }
                Debug.WriteLine("InputSystem.GetAction: Player " + player_index.ToString() + " not connected. " + players.Count + " player(s) are connected.");
                return new PseudoButton();
            }

            PseudoButton result = players[player_index].GetActionButton(input_state, action_type);

            if (input_mode.combine_all_input && (players.Count > 1))
            {
                for (int i = 1; i < players.Count; i++)
                {
                    result |= players[i].GetActionButton(input_state, action_type);
                }
            }

            return result;
        }

        /// <summary>
        /// Reorder update and manage new inputs.
        /// </summary>
        public void Update(GameWindow window = null)
        {
            input_state.Update(window);
            ProcessNewInput(input_state.GetNewPlayerInput());
        }

        public void DisconnectPlayers()
        {
            players.Clear();
        }

        /// <summary>
        /// This function handles automatically adding new input.
        /// </summary>
        /// <param name="controllers_pressed"></param>
        private void ProcessNewInput(List<Controller> controllers_pressed)
        {
            for (int c = 0; c < controllers_pressed.Count; c++) // evaluate every new button press
            {
                if (players.Count >= input_mode.maximum_player_limit) break; // cancel if there are already too many controllers
                if (ControllerIsInUse(controllers_pressed[c])) continue; // skip if controller is already being used
                if (input_mode.append_new_players)
                {
                    players.Add(new PlayerProfile(controllers_pressed[c], players.Count + 1)); // add new player using new controller (if the option is enabled in input_mode)
                    Debug.WriteLine("Added new player; index = " + (players.Count - 1).ToString());
                }
            }
        }

        private bool ControllerIsInUse(Controller controller)
        {
            for (int p = 0; p < players.Count; p++)
            {
                if (players[p].current_controller.Equals(controller)) return true;
            }
            return false;
        }
    }
}