namespace DownUnder.Input
{
    /// <summary>
    /// A settings class for InputSystem.
    /// </summary>
    public class InputMode
    {
        public bool append_new_players = false;
        public bool combine_all_input = false;
        public bool remove_disabled_input_from_list = false;
        public bool replace_disabled_input_with_new = false;
        public int maximum_player_limit = 0;

        public static InputMode StandardMultiPlayer()
        {
            return new InputMode()
            {
                append_new_players = true,
                maximum_player_limit = 10000000 //10,000,000 is a party
            };
        }

        public static InputMode StandardSinglePlayer()
        {
            return new InputMode()
            {
                append_new_players = true,
                maximum_player_limit = 10000000,
                combine_all_input = true
            };
        }

        /// <summary>
        /// An input mode unseen in other games.
        ///
        /// ...Unseen here too, apparently. :P
        ///
        /// To be implemented when I remember what I was doing here.
        /// </summary>
        /// <returns></returns>
        public static InputMode AutoControllerSwap(int maximum_player_limit_)
        {
            return new InputMode()
            {
            };
        }
    }
}