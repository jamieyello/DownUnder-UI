using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.DataTypes
{
    class SetGetEvent<T>
    {
        public SetGetEvent() { }
        public SetGetEvent(T value)
        {
            Value = value;
        }

        public bool Completed { get; set; } = false;
        public T Value { get; set; }
    }
}
