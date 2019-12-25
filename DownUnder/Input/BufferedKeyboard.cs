using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.Input
{
    class BufferedKeyboard
    {
        List<BufferedBool> keys = new List<BufferedBool>();
        KeyboardState _keyboard_state;

        // Because the logic is done when a key is read, the following
        // fields store the last result so a key is not updated more
        // than once a frame.
        List<Keys> _accessed_keys = new List<Keys>();
        Dictionary<int, bool> _last_results = new Dictionary<int, bool>();
        float _step = 0f;

        public BufferedKeyboard()
        {
            foreach (Keys val in Enum.GetValues(typeof(Keys)))
            {
                keys.Add(new BufferedBool());
            }
        }

        public void Update(float step)
        {
            _step = step;
            _keyboard_state = Keyboard.GetState();
            _accessed_keys.Clear();
            _last_results.Clear();
        }

        public bool IsKeyTriggered(Keys key)
        {
            if (_accessed_keys.Contains(key))
            {
                return _last_results[(int)key];
            }

            _accessed_keys.Add(key);
            bool result = keys[(int)key].GetTriggered(_keyboard_state.IsKeyDown(key), _step);
            _last_results.Add((int)key, result);
            return result;
        }
    }
}
