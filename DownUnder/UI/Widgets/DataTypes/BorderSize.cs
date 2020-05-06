namespace DownUnder.UI.Widgets.DataTypes {
    public struct BorderSize {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;

        public BorderSize(float top, float bottom, float left, float right) 
        { Top = top; Bottom = bottom; Left = left; Right = right; }
        public BorderSize(float size) 
        { Top = size; Bottom = size; Left = size; Right = size; }

        public static BorderSize operator -(BorderSize b) => new BorderSize(-b.Top, -b.Bottom, -b.Left, -b.Right);
        public static BorderSize operator -(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top - b2.Top, b1.Bottom - b2.Bottom, b1.Left - b2.Left, b1.Right - b2.Right);
        public static BorderSize operator +(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top + b2.Top, b1.Bottom + b2.Bottom, b1.Left + b2.Left, b1.Right + b2.Right);
        public static BorderSize operator *(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top * b2.Top, b1.Bottom * b2.Bottom, b1.Left * b2.Left, b1.Right * b2.Right);
        public static BorderSize operator /(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top / b2.Top, b1.Bottom / b2.Bottom, b1.Left / b2.Left, b1.Right / b2.Right);
    }
}
