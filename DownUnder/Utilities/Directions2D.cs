using System;
using System.Runtime.Serialization;

namespace DownUnder.Utility
{
    /// <summary> Represents four (up, down, left, right) directions. </summary>
    public class Directions2D : ICloneable
    {
        private bool up = false;
        private bool down = false;
        private bool left = false;
        private bool right = false;

        [DataMember] public bool AllowOpposites = true;
        
        [DataMember] public bool Up
        {
            get => up;
            set
            {
                if (!AllowOpposites && value) down = false;
                up = value;
            }
        }

        [DataMember] public bool Left
        {
            get => left;
            set
            {
                if (!AllowOpposites && value) right = false;
                left = value;
            }
        }

        [DataMember] public bool Right
        {
            get => right;
            set
            {
                if (!AllowOpposites && value) left = false;
                right = value;
            }
        }

        [DataMember] public bool Down
        {
            get => down;
            set
            {
                if (!AllowOpposites && value) up = false;
                down = value;
            }
        }

        public void Clear()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }

        public object Clone()
        {
            Directions2D c = new Directions2D();
            c.Up = Up;
            c.Left = Left;
            c.Right = Right;
            c.Down = Down;

            return c;
        }

        public override string ToString()
        {
            return "{up:" + Up + " left:" + Left + " right:" + Right + " down:" + Down + "}";
        }

        public override int GetHashCode()
        {
            int result = 0;
            result = (Up ? 1 : 0) * 1
                + (Left ? 1 : 0) * 2
                + (Right ? 1 : 0) * 4
                + (Left ? 1 : 0) * 8;
            return result;
        }

        public static bool operator ==(Directions2D d1, Directions2D d2)
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

        public static bool operator !=(Directions2D d1, Directions2D d2)
        {
            return !(d1 == d2);
        }

        public override bool Equals(Object obj)
        {
            if (Object.ReferenceEquals(obj, null)) return false;
            if (Object.ReferenceEquals(this, obj)) return true;

            return (obj is Directions2D) &&
                ((Directions2D)obj).Up == Up &&
                ((Directions2D)obj).Left == Left &&
                ((Directions2D)obj).Right == Right &&
                ((Directions2D)obj).Down == Down;
        }

        public static Directions2D operator |(Directions2D d1, Directions2D d2)
        {
            return new Directions2D()
            {
                left = d1.left || d2.left,
                right = d1.right || d2.right,
                up = d1.up || d2.up,
                down = d1.down || d2.down
            };
        }

        public static Directions2D operator &(Directions2D d1, Directions2D d2)
        {
            return new Directions2D()
            {
                left = d1.left && d2.left,
                right = d1.right && d2.right,
                up = d1.up && d2.up,
                down = d1.down && d2.down
            };
        }

        public static Directions2D UpDownLeftRight
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Left = true,
                Right = true
            };
        }

        public static Directions2D UpDownLeft
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Left = true
            };
        }

        public static Directions2D UpDownRight
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Right = true
            };
        }

        public static Directions2D UpLeftRight
        {
            get => new Directions2D()
            {
                Up = true,
                Left = true,
                Right = true
            };
        }

        public static Directions2D DownLeftRight
        {
            get => new Directions2D()
            {
                Down = true,
                Left = true,
                Right = true
            };
        }

        public static Directions2D UpDown
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true
            };
        }

        public static Directions2D UpLeft
        {
            get => new Directions2D()
            {
                Up = true,
                Left = true
            };
        }

        public static Directions2D UpRight
        {
            get => new Directions2D()
            {
                Up = true,
                Right = true
            };
        }

        public static Directions2D DownLeft
        {
            get => new Directions2D()
            {
                Down = true,
                Left = true
            };
        }

        public static Directions2D DownRight
        {
            get => new Directions2D()
            {
                Down = true,
                Right = true
            };
        }

        public static Directions2D LeftRight
        {
            get => new Directions2D()
            {
                Left = true,
                Right = true
            };
        }

        public static Directions2D UpOnly
        {
            get => new Directions2D()
            {
                Up = true
            };
        }

        public static Directions2D DownOnly
        {
            get => new Directions2D()
            {
                Down = true
            };
        }

        public static Directions2D LeftOnly
        {
            get => new Directions2D()
            {
                Left = true
            };
        }

        public static Directions2D RightOnly
        {
            get => new Directions2D()
            {
                Right = true
            };
        }

        public static Directions2D None
        {
            get => new Directions2D();
        }
    }
}