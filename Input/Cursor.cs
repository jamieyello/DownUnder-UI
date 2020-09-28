using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.Input
{
    public class Cursor
    {
        public bool Triggered { get; set; } = false;
        public bool HeldDown { get; set; } = false;
        public Point Position { get; set; } = new Point();
        public override string ToString()
        {
            return $"Triggered: {Triggered}, HeldDown: {HeldDown}, Position: {Position}";
        }
    }
}
