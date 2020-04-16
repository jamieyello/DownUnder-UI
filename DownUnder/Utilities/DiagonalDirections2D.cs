using System;

namespace DownUnder.Utility
{
    /// <summary>
    /// Represents four diagonal directions.
    /// </summary>
    public struct DiagonalDirections2D
    {
        public bool TopLeft;
        public bool TopRight;
        public bool BottomLeft;
        public bool BottomRight;
        
        /// <summary> Creates a perpendicular Direction2D by combining directions. (Not rotation.) </summary>
        public Directions2D ToPerpendicular()
        {
            return new Directions2D()
            {
                Up = TopRight || TopLeft,
                Down = BottomLeft || BottomRight,
                Left = TopLeft || BottomLeft,
                Right = TopRight || BottomRight
            };
        }

        public override string ToString()
        {
            return "{top_left:" + TopLeft + " top_right:" + TopRight + " bottom_left:" + BottomLeft + " bottom_right:" + BottomRight + "}";
        }

        public override int GetHashCode()
        {
            return (TopLeft ? 1 : 0)
                + (TopRight ? 1 : 0) * 2
                + (BottomLeft ? 1 : 0) * 4
                + (BottomRight ? 1 : 0) * 8;
        }

        public static bool operator ==(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            return !(d1 == d2);
        }

        public static bool operator true(DiagonalDirections2D d)
        {
            return d.TopLeft || d.TopRight || d.BottomLeft || d.BottomRight;
        }

        public static bool operator false(DiagonalDirections2D d)
        {
            return !(d.TopLeft || d.TopRight || d.BottomLeft || d.BottomRight);
        }

        public static DiagonalDirections2D operator !(DiagonalDirections2D d)
        {
            return new DiagonalDirections2D()
            {
                TopLeft = !d.TopLeft,
                TopRight = !d.TopRight,
                BottomLeft = !d.BottomLeft,
                BottomRight = !d.BottomRight
            };
        }

        public override bool Equals(object obj)
        {
            return (obj is DiagonalDirections2D d_obj) &&
                d_obj.TopLeft == TopLeft &&
                d_obj.TopRight == TopRight &&
                d_obj.BottomLeft == BottomLeft &&
                d_obj.BottomRight == BottomRight;
        }

        public static DiagonalDirections2D operator |(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            return new DiagonalDirections2D()
            {
                TopLeft = d1.TopLeft || d2.TopLeft,
                TopRight = d1.TopRight || d2.TopRight,
                BottomLeft = d1.BottomLeft || d2.BottomLeft,
                BottomRight = d1.BottomRight || d2.BottomRight
            };
        }

        public static DiagonalDirections2D operator &(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            return new DiagonalDirections2D()
            {
                TopLeft = d1.TopLeft && d2.TopLeft,
                TopRight = d1.TopRight && d2.TopRight,
                BottomLeft = d1.BottomLeft && d2.BottomLeft,
                BottomRight = d1.BottomRight && d2.BottomRight
            };
        }

        /// <summary> TopLeft, TopRight, BottomLeft, BottomRight = true </summary>
        public static DiagonalDirections2D TL_TR_BL_BR
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                TopRight = true,
                BottomLeft = true,
                BottomRight = true,
            };
        }

        /// <summary> TopLeft, TopRight, BottomLeft = true </summary>
        public static DiagonalDirections2D TL_TR_BL
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                TopRight = true,
                BottomLeft = true,
            };
        }

        /// <summary> TopRight, BottomLeft, BottomRight = true </summary>
        public static DiagonalDirections2D TR_BL_BR
        {
            get => new DiagonalDirections2D()
            {
                TopRight = true,
                BottomLeft = true,
                BottomRight = true,
            };
        }

        /// <summary> TopLeft, TopRight, BottomRight = true </summary>
        public static DiagonalDirections2D TL_TR_BR
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                TopRight = true,
                BottomRight = true,
            };
        }

        /// <summary> TopLeft, BottomLeft, BottomRight = true </summary>
        public static DiagonalDirections2D TL_BL_BR
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                BottomLeft = true,
                BottomRight = true,
            };
        }

        /// <summary> TopRight, BottomLeft = true </summary>
        public static DiagonalDirections2D TR_BL
        {
            get => new DiagonalDirections2D()
            {
                TopRight = true,
                BottomLeft = true,
            };
        }

        /// <summary> TopLeft, TopRight = true </summary>
        public static DiagonalDirections2D TL_TR
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                TopRight = true,
            };
        }

        /// <summary> TopRight, BottomRight = true </summary>
        public static DiagonalDirections2D TR_BR
        {
            get => new DiagonalDirections2D()
            {
                TopRight = true,
                BottomRight = true,
            };
        }

        /// <summary> TopLeft, BottomLeft = true </summary>
        public static DiagonalDirections2D TL_BL
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                BottomLeft = true,
            };
        }

        /// <summary> BottomLeft, BottomRight = true </summary>
        public static DiagonalDirections2D BL_BR
        {
            get => new DiagonalDirections2D()
            {
                BottomLeft = true,
                BottomRight = true,
            };
        }

        /// <summary> TopLeft, BottomRight = true </summary>
        public static DiagonalDirections2D TL_BR
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
                BottomRight = true,
            };
        }

        /// <summary> BottomRight = true </summary>
        public static DiagonalDirections2D TR
        {
            get => new DiagonalDirections2D()
            {
                TopRight = true,
            };
        }

        /// <summary> BottomLeft = true </summary>
        public static DiagonalDirections2D BL
        {
            get => new DiagonalDirections2D()
            {
                BottomLeft = true,
            };
        }

        /// <summary> TopLeft = true </summary>
        public static DiagonalDirections2D TL
        {
            get => new DiagonalDirections2D()
            {
                TopLeft = true,
            };
        }

        /// <summary> BottomRight = true </summary>
        public static DiagonalDirections2D BR
        {
            get => new DiagonalDirections2D()
            {
                BottomRight = true,
            };
        }

        public static DiagonalDirections2D None
        {
            get => new DiagonalDirections2D();
        }
    }
}