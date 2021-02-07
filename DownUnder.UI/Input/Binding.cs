using System;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Input {
    /// <summary> A single binding, contains all buttons an action is bound to. </summary>
    [Serializable]
    public sealed class Binding {
        readonly List<int> _buttons_combo;
        readonly List<int> _buttons;

        public int NumButtons => _buttons.Count;
        public int NumCombos => _buttons_combo.Count;

        public Binding() {
            _buttons_combo = new List<int>();
            _buttons = new List<int>();
        }

        Binding(Binding source) {
            _buttons_combo = source._buttons_combo.ToList();
            _buttons = source._buttons.ToList();
        }

        public int GetButton(int index) =>
            _buttons[index];

        public int GetCombo(int index) =>
            _buttons_combo[index];

        public static Binding operator +(Binding b1, Binding b2) {
            var result = new Binding();
            result._buttons_combo.AddRange(b1._buttons_combo);
            result._buttons_combo.AddRange(b2._buttons_combo);
            result._buttons.AddRange(b1._buttons);
            result._buttons.AddRange(b2._buttons);
            return result;
        }

        public Binding Clone() =>
            new Binding(this);

        public void AddCombo(int value) =>
            _buttons_combo.Add(value);

        public void AddButton(int value) =>
            _buttons.Add(value);
    }
}