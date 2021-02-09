using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    public readonly struct BorderSize {
        public float Top { get; }
        public float Bottom { get; }
        public float Left { get; }
        public float Right { get; }

        public BorderSize(
            float top,
            float bottom,
            float left,
            float right
        ) {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public BorderSize(float size) =>
            Top = Bottom = Left = Right = size;

        public BorderSize(
            float amount,
            Directions2D directions
        ) {
            Top = directions.Up ? amount : 0f;
            Bottom = directions.Down ? amount : 0f;
            Left = directions.Left ? amount : 0f;
            Right = directions.Right ? amount : 0f;
        }

        public static BorderSize operator -(BorderSize b) => new BorderSize(-b.Top, -b.Bottom, -b.Left, -b.Right);
        public static BorderSize operator -(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top - b2.Top, b1.Bottom - b2.Bottom, b1.Left - b2.Left, b1.Right - b2.Right);
        public static BorderSize operator +(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top + b2.Top, b1.Bottom + b2.Bottom, b1.Left + b2.Left, b1.Right + b2.Right);
        public static BorderSize operator *(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top * b2.Top, b1.Bottom * b2.Bottom, b1.Left * b2.Left, b1.Right * b2.Right);
        public static BorderSize operator /(BorderSize b1, BorderSize b2) => new BorderSize(b1.Top / b2.Top, b1.Bottom / b2.Bottom, b1.Left / b2.Left, b1.Right / b2.Right);

        public override string ToString() =>
            $"{nameof(Top)}: {Top} {nameof(Bottom)}: {Bottom} {nameof(Left)}: {Left} {nameof(Right)}: {Right}";

        public BorderSize FlippedUpDown() =>
            new BorderSize(Bottom, Top, Left, Right);

        public BorderSize FlippedLeftRight() =>
            new BorderSize(Top, Bottom, Right, Left);

        public BorderSize FlippedUpDownLeftRight() =>
            new BorderSize(Bottom, Top, Right, Left);
    }
}