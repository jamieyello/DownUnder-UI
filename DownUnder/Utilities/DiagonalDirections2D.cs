using System;

namespace DownUnder.Utility
{
    /// <summary>
    /// Represents four diagonal directions.
    /// </summary>
    public class DiagonalDirections2D : ICloneable
    {
        public bool top_right;
        public bool top_left;
        public bool bottom_right;
        public bool bottom_left;

        /// <summary>
        /// Creates a perpendicular Direction2D by combining directions. (Not rotation.)
        /// </summary>
        /// <returns></returns>
        public Directions2D ToPerpendicular()
        {
            return new Directions2D()
            {
                Up = top_right || top_left,
                Down = bottom_left || bottom_right,
                Left = top_left || bottom_left,
                Right = top_right || bottom_right
            };
        }

        public object Clone()
        {
            DiagonalDirections2D c = new DiagonalDirections2D();
            c.top_right = top_right;
            c.top_left = top_left;
            c.bottom_right = bottom_right;
            c.bottom_left = bottom_left;

            return c;
        }

        public override string ToString()
        {
            return "{top_right:" + top_right + " top_left:" + top_left + " bottom_right:" + bottom_right + " bottom_left:" + bottom_left + "}";
        }

        public override int GetHashCode()
        {
            int result = 0;
            result = (top_right ? 1 : 0) * 1
                + (top_left ? 1 : 0) * 2
                + (bottom_right ? 1 : 0) * 4
                + (bottom_left ? 1 : 0) * 8;
            return result;
        }

        public static bool operator ==(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(d1, null))
            {
                if (Object.ReferenceEquals(d2, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return d1.Equals(d2);
        }

        public static bool operator !=(DiagonalDirections2D d1, DiagonalDirections2D d2)
        {
            return !(d1 == d2);
        }

        public override bool Equals(Object obj)
        {
            if (obj is null) return false;
            if (Object.ReferenceEquals(this, obj)) return true;

            return (obj is DiagonalDirections2D) &&
                ((DiagonalDirections2D)obj).top_right == top_right &&
                ((DiagonalDirections2D)obj).top_left == top_left &&
                ((DiagonalDirections2D)obj).bottom_right == bottom_right &&
                ((DiagonalDirections2D)obj).bottom_left == bottom_left;
        }

        public static DiagonalDirections2D TopRight_BottomLeft_TopLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                bottom_left = true,
                top_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D TopRight_BottomLeft_TopLeft
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                bottom_left = true,
                top_left = true
            };
        }

        public static DiagonalDirections2D TopRight_BottomLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                bottom_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D TopRight_TopLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                top_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D BottomLeft_TopLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                bottom_left = true,
                top_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D TopRight_BottomLeft
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                bottom_left = true
            };
        }

        public static DiagonalDirections2D TopRight_TopLeft
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                top_left = true
            };
        }

        public static DiagonalDirections2D TopRight_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                top_right = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D BottomLeft_TopLeft
        {
            get => new DiagonalDirections2D()
            {
                bottom_left = true,
                top_left = true
            };
        }

        public static DiagonalDirections2D BottomLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                bottom_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D TopLeft_BottomRight
        {
            get => new DiagonalDirections2D()
            {
                top_left = true,
                bottom_right = true
            };
        }

        public static DiagonalDirections2D TopRight
        {
            get => new DiagonalDirections2D()
            {
                top_right = true
            };
        }

        public static DiagonalDirections2D BottomLeft
        {
            get => new DiagonalDirections2D()
            {
                bottom_left = true
            };
        }

        public static DiagonalDirections2D TopLeft
        {
            get => new DiagonalDirections2D()
            {
                top_left = true
            };
        }

        public static DiagonalDirections2D BottomRight
        {
            get => new DiagonalDirections2D()
            {
                bottom_right = true
            };
        }

        public static DiagonalDirections2D None
        {
            get => new DiagonalDirections2D();
        }
    }
}