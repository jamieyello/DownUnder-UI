namespace DownUnder.Input {
    /// <summary> A settings class for InputSystem. </summary>
    public sealed class InputMode {
        public bool append_new_players;
        public bool combine_all_input;
        public bool remove_disabled_input_from_list;
        public bool replace_disabled_input_with_new;
        public int maximum_player_limit;

        public static InputMode StandardMultiPlayer() =>
            new InputMode {
                append_new_players = true,
                maximum_player_limit = 10_000_000 //10,000,000 is a party
            };

        public static InputMode StandardSinglePlayer() =>
            new InputMode {
                append_new_players = true,
                maximum_player_limit = 10_000_000,
                combine_all_input = true
            };

        /// <summary> An input mode unseen in other games.
        ///
        /// ...Unseen here too, apparently. :P
        ///
        /// To be implemented when I remember what I was doing here. </summary>
        /// <returns></returns>
        public static InputMode AutoControllerSwap(
            int maximum_player_limit_
        ) =>
            new InputMode();
    }
}