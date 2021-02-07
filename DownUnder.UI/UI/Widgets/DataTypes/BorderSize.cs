using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    public struct BorderSize {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;

        public BorderSize(float top, float bottom, float left, float right)
        { Top = top; Bottom = bottom; Left = left; Right = right; }
        public BorderSize(float size)
        { Top = size; Bottom = size; Left = size; Right = size; }
        public BorderSize(float amount, Directions2D directions) {
            if (directions.Up) Top = amount; else Top = 0f;
            if (directions.Down) Bottom = amount; else Bottom = 0f;
            if (directions.Left) Left = amount; else Left = 0f;
            if (directions.Right) Right = amount; else Right = 0f;
        }

        public static BorderSize operator -(BorderSize b) => new BorderSize(-b.Top, -b.Bottom, -b.Left, -b.Right);
        public static BorderSize operator -(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top - b2.Top, b1.Bottom - b2.Bottom, b1.Left - b2.Left, b1.Right - b2.Right);
        public static BorderSize operator +(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top + b2.Top, b1.Bottom + b2.Bottom, b1.Left + b2.Left, b1.Right + b2.Right);
        public static BorderSize operator *(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top * b2.Top, b1.Bottom * b2.Bottom, b1.Left * b2.Left, b1.Right * b2.Right);
        public static BorderSize operator /(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top / b2.Top, b1.Bottom / b2.Bottom, b1.Left / b2.Left, b1.Right / b2.Right);

        public override string ToString()
        {
            return $"{nameof(Top)}: {Top} {nameof(Bottom)}: {Bottom} {nameof(Left)}: {Left} {nameof(Right)}: {Right}";
        }

        public BorderSize FlippedUpDown() =>
            new BorderSize(Bottom, Top, Left, Right);

        public BorderSize FlippedLeftRight() =>
            new BorderSize(Top, Bottom, Right, Left);

        public BorderSize FlippedUpDownLeftRight() =>
            new BorderSize(Bottom, Top, Right, Left);
    }
}
