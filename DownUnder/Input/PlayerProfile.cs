using System;

namespace DownUnder.Input
{
    [Serializable()]
    internal class PlayerProfile
    {
        public String name = "";
        public Controller current_controller = new Controller();
        public BindingsSet bindings_set = new BindingsSet();

        public PlayerProfile(Controller current_controller_, int index)
        {
            current_controller = (Controller)current_controller_.Clone();
            name = "Player " + index.ToString();
        }

        public PlayerProfile(Controller current_controller_, string name_)
        {
            current_controller = (Controller)current_controller_.Clone();
            name = name_;
        }

        public PlayerProfile(Controller current_controller_)
        {
            current_controller = (Controller)current_controller_.Clone();
            name = "";
        }

        public PseudoButton GetActionButton(InputState input_state, ActionType action_type)
        {
            return bindings_set.GetAction(input_state, current_controller, action_type);
        }
    }
}