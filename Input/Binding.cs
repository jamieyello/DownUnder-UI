using System;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.Input
{
    /// <summary>
    /// A single binding, contains all buttons an action is bound to.
    /// </summary>
    [Serializable()]
    public class Binding
    {
        public Binding()
        {
        }

        public List<int> buttons_combo = new List<int>();
        public List<int> buttons = new List<int>();

        public static Binding operator +(Binding b1, Binding b2)
        {
            Binding result = new Binding();
            result.buttons_combo.AddRange(b1.buttons_combo);
            result.buttons_combo.AddRange(b2.buttons_combo);
            result.buttons.AddRange(b1.buttons);
            result.buttons.AddRange(b2.buttons);
            return result;
        }

        public object Clone()
        {
            Binding clone = new Binding();
            clone.buttons_combo = buttons_combo.ToList();
            clone.buttons = buttons.ToList();
            return clone;
        }
    }
}