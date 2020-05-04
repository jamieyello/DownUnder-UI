namespace DownUnder.UI.Widgets.DataTypes {
    public struct BorderSize {
        public float Top;
        public float Down;
        public float Left;
        public float Right;

        public BorderSize(float top, float down, float left, float right) 
        { Top = top; Down = down; Left = left; Right = right; }

        public static BorderSize operator -(BorderSize b) => new BorderSize(-b.Top, -b.Down, -b.Left, -b.Right);
        public static BorderSize operator -(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top - b2.Top, b1.Down - b2.Down, b1.Left - b2.Left, b1.Right - b2.Right);
        public static BorderSize operator +(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top + b2.Top, b1.Down + b2.Down, b1.Left + b2.Left, b1.Right + b2.Right);
        public static BorderSize operator *(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top * b2.Top, b1.Down * b2.Down, b1.Left * b2.Left, b1.Right * b2.Right);
        public static BorderSize operator /(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top / b2.Top, b1.Down / b2.Down, b1.Left / b2.Left, b1.Right / b2.Right);
    }
}
