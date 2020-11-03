using DownUnder.Utilities;
using System;

namespace DownUnder.Utility
{
    public struct DiagonalDirections2D
    {
        public bool TopLeft;
        public bool TopRight;
        public bool BottomLeft;
        public bool BottomRight;

        public DiagonalDirections2D(bool top_left, bool top_right, bool bottom_left, bool bottom_right) {
            TopLeft = top_left;
            TopRight = top_right;
            BottomLeft = bottom_left;
            BottomRight = bottom_right;
        }

        public DiagonalDirections2D(DiagonalDirection2D direction) {
            TopLeft = direction == DiagonalDirection2D.top_left;
            TopRight = direction == DiagonalDirection2D.top_right;
            BottomLeft = direction == DiagonalDirection2D.bottom_left;
            BottomRight = direction == DiagonalDirection2D.bottom_right;
        }

        public Directions2D ToPerpendicular() =>
            new Directions2D {
                Up = TopRight || TopLeft,
                Down = BottomLeft || BottomRight,
                Left = TopLeft || BottomLeft,
                Right = TopRight || BottomRight
            };

        public override string ToString() =>
            "{TopLeft:" + TopLeft +
            " TopRight:" + TopRight +
            " BottomLeft:" + BottomLeft +
            " BottomRight:" + BottomRight +
            "}";

        public override int GetHashCode() =>
            (TopLeft ? 1 : 0)
             + (TopRight ? 1 : 0) * 2
             + (BottomLeft ? 1 : 0) * 4
             + (BottomRight ? 1 : 0) * 8;

        public bool HasMultiple => Convert.ToByte(TopLeft) + Convert.ToByte(TopRight) + Convert.ToByte(BottomLeft) + Convert.ToByte(BottomRight) <= 1;

        public static bool operator ==(DiagonalDirections2D d1, DiagonalDirections2D d2) => d1.Equals(d2);
        public static bool operator !=(DiagonalDirections2D d1, DiagonalDirections2D d2) => !(d1 == d2);
        public static bool operator true(DiagonalDirections2D d) => d.TopLeft || d.TopRight || d.BottomLeft || d.BottomRight;
        public static bool operator false(DiagonalDirections2D d) => !(d.TopLeft || d.TopRight || d.BottomLeft || d.BottomRight);

        public static DiagonalDirections2D operator !(DiagonalDirections2D d) =>
            new DiagonalDirections2D {
                TopLeft = !d.TopLeft,
                TopRight = !d.TopRight,
                BottomLeft = !d.BottomLeft,
                BottomRight = !d.BottomRight
            };

        public override bool Equals(object obj) =>
            (obj is DiagonalDirections2D d_obj) &&
            d_obj.TopLeft == TopLeft &&
            d_obj.TopRight == TopRight &&
            d_obj.BottomLeft == BottomLeft &&
            d_obj.BottomRight == BottomRight;

        public static DiagonalDirections2D operator |(DiagonalDirections2D d1, DiagonalDirections2D d2) =>
            new DiagonalDirections2D {
                TopLeft = d1.TopLeft || d2.TopLeft,
                TopRight = d1.TopRight || d2.TopRight,
                BottomLeft = d1.BottomLeft || d2.BottomLeft,
                BottomRight = d1.BottomRight || d2.BottomRight
            };

        public static DiagonalDirections2D operator &(DiagonalDirections2D d1, DiagonalDirections2D d2) => new DiagonalDirections2D {
                TopLeft = d1.TopLeft && d2.TopLeft,
                TopRight = d1.TopRight && d2.TopRight,
                BottomLeft = d1.BottomLeft && d2.BottomLeft,
                BottomRight = d1.BottomRight && d2.BottomRight
            };

        public static DiagonalDirections2D All => new DiagonalDirections2D { TopLeft = true, TopRight = true, BottomLeft = true, BottomRight = true };
        public static DiagonalDirections2D TL_TR_BL_BR => new DiagonalDirections2D { TopLeft = true, TopRight = true, BottomLeft = true, BottomRight = true };
        public static DiagonalDirections2D TL_TR_BL => new DiagonalDirections2D { TopLeft = true, TopRight = true, BottomLeft = true };
        public static DiagonalDirections2D TR_BL_BR => new DiagonalDirections2D { TopRight = true, BottomLeft = true, BottomRight = true };
        public static DiagonalDirections2D TL_TR_BR => new DiagonalDirections2D { TopLeft = true, TopRight = true, BottomRight = true };
        public static DiagonalDirections2D TL_BL_BR => new DiagonalDirections2D { TopLeft = true, BottomLeft = true, BottomRight = true };
        public static DiagonalDirections2D TR_BL => new DiagonalDirections2D { TopRight = true, BottomLeft = true };
        public static DiagonalDirections2D TL_TR => new DiagonalDirections2D { TopLeft = true, TopRight = true };
        public static DiagonalDirections2D TR_BR => new DiagonalDirections2D { TopRight = true, BottomRight = true };
        public static DiagonalDirections2D TL_BL => new DiagonalDirections2D { TopLeft = true, BottomLeft = true };
        public static DiagonalDirections2D BL_BR => new DiagonalDirections2D { BottomLeft = true,BottomRight = true };
        public static DiagonalDirections2D TL_BR => new DiagonalDirections2D { TopLeft = true, BottomRight = true };
        public static DiagonalDirections2D TR => new DiagonalDirections2D { TopRight = true };
        public static DiagonalDirections2D BL => new DiagonalDirections2D { BottomLeft = true };
        public static DiagonalDirections2D TL => new DiagonalDirections2D { TopLeft = true };
        public static DiagonalDirections2D BR => new DiagonalDirections2D { BottomRight = true };
        public static DiagonalDirections2D None => new DiagonalDirections2D();
    }
}