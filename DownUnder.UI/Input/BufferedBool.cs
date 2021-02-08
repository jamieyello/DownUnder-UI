using System.Runtime.Serialization;

namespace DownUnder.UI.Input {
    /// <summary> Applies an alternating true/false pattern to an input. Used to convert a key press into a usable text entry input. </summary>
    sealed class BufferedBool {
        float _initial_timer;
        float _consecutive_timer;

        [DataMember] public float InitialWait { get; set; } = 0.5f;
        [DataMember] public float ConsecutiveWait { get; set; } = 0.0165f;

        public BufferedBool() {
        }

        public BufferedBool(
            float initial_wait,
            int consecutive_wait
        ) {
            InitialWait = initial_wait;
            ConsecutiveWait = consecutive_wait;
        }

        public void ResetTimer() =>
            _initial_timer = _consecutive_timer = 0f;

        /// <summary> Updates logic and applies buffer. Returns true if triggered. </summary>
        /// <param name="input">Unbuffered input</param>
        /// <param name="step">Time to update in seconds.</param>
        /// <returns>Whether triggered.</returns>
        public bool GetTriggered(bool input, float step) {
            // Clear timers if input is false
            if (!input) {
                ResetTimer();
                return false;
            }

            // Return initial immediate input
            if (_initial_timer == 0f) {
                _initial_timer += step;
                return true;
            }

            // Wait for the first time
            if (_initial_timer < InitialWait) {
                _initial_timer += step;
                return false;
            }

            // Return -initial- consecutive input
            if (_consecutive_timer == 0f) {
                _consecutive_timer += step;
                return true;
            }

            // Wait consecutive waits
            if (_consecutive_timer < ConsecutiveWait)
                _consecutive_timer += step;
            else // Reset timer
                _consecutive_timer = 0f;

            return false;
        }
    }
}
