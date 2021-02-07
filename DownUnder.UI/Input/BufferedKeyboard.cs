using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.Input {
    sealed class BufferedKeyboard {
        readonly List<BufferedBool> _keys = new List<BufferedBool>();
        KeyboardState _keyboard_state;

        // Because the logic is done when a key is read, the following
        // fields store the last result so a key is not updated more
        // than once a frame.
        readonly List<Keys> _accessed_keys = new List<Keys>();
        readonly Dictionary<int, bool> _last_results = new Dictionary<int, bool>();
        float _step;

        public BufferedKeyboard() {
            foreach (var _ in Enum.GetValues(typeof(Keys)))
                _keys.Add(new BufferedBool());
        }

        public void Update(float step) {
            _step = step;
            _keyboard_state = Keyboard.GetState();
            _accessed_keys.Clear();
            _last_results.Clear();
        }

        public bool IsKeyTriggered(Keys key) {
            if (_accessed_keys.Contains(key))
                return _last_results[(int)key];

            _accessed_keys.Add(key);
            var result = _keys[(int)key].GetTriggered(_keyboard_state.IsKeyDown(key), _step);
            _last_results.Add((int)key, result);
            return result;
        }
    }
}