using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownUnder.Input
{
    /// <summary>
    /// Applies an alternating true/false pattern to an input. Used to convert a key press into a usable text entry input.
    /// </summary>
    class BufferedBool
    {
        float initial_timer = 0f;
        float consecutive_timer = 0f;

        [DataMember]
        public float InitialWait { get; set; } = 0.5f;

        [DataMember]
        public float ConsecutiveWait { get; set; } = 0.05f;

        public BufferedBool() { }
        public BufferedBool(float initial_wait, int consecutive_wait)
        {
            this.InitialWait = initial_wait;
            this.ConsecutiveWait = consecutive_wait;
        }

        public void ResetTimer()
        {
            initial_timer = 0f;
            consecutive_timer = 0f;
        }

        /// <summary>
        /// Updates logic and applies buffer. Returns true if triggered.
        /// </summary>
        /// <param name="input">Unbuffered input</param>
        /// <param name="step">Time to update in seconds.</param>
        /// <returns></returns>
        public bool GetTriggered(bool input, float step)
        {
            // Clear timers if input is false
            if (!input)
            {
                initial_timer = 0f;
                consecutive_timer = 0f;
                return false;
            }

            // Return initial immediate input
            if (initial_timer == 0f)
            {
                initial_timer += step;
                return true;
            }

            // Wait for the first time
            if (initial_timer < InitialWait)
            {
                initial_timer += step;
                return false;
            }

            // Return -initial- consecutive input
            if (consecutive_timer == 0f)
            {
                consecutive_timer += step;
                return true;
            }

            // Wait consecutive waits
            if (consecutive_timer < ConsecutiveWait)
            {
                consecutive_timer += step;
            }
            else
            {
                // Reset timer
                consecutive_timer = 0f;
            }
            return false;
        }
    }
}
