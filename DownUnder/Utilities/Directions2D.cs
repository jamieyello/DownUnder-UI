using System;
using System.Runtime.Serialization;

namespace DownUnder.Utility
{
    /// <summary> Represents four (up, down, left, right) directions. </summary>
    public struct Directions2D
    {
        private bool up;
        private bool down;
        private bool left;
        private bool right;

        [DataMember] public bool DisallowOpposites;
        
        [DataMember] public bool Up
        {
            get => up;
            set
            {
                if (DisallowOpposites && value) down = false;
                up = value;
            }
        }

        [DataMember] public bool Left
        {
            get => left;
            set
            {
                if (DisallowOpposites && value) right = false;
                left = value;
            }
        }

        [DataMember] public bool Right
        {
            get => right;
            set
            {
                if (DisallowOpposites && value) left = false;
                right = value;
            }
        }

        [DataMember] public bool Down
        {
            get => down;
            set
            {
                if (DisallowOpposites && value) up = false;
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

        public static bool operator true(Directions2D d)
        {
            return d.up || d.down || d.left || d.right;
        }

        public static bool operator false(Directions2D d)
        {
            return !(d.up || d.down || d.left || d.right);
        }

        public static Directions2D operator !(Directions2D d)
        {
            return new Directions2D()
            {
                up = !d.up,
                down = !d.down,
                left = !d.left,
                right = !d.right
            };
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

        /// <summary> Up, Down, Left, Right = true </summary>
        public static Directions2D UDLR
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Left = true,
                Right = true
            };
        }

        /// <summary> Up, Down, Left = true </summary>
        public static Directions2D UDL
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Left = true
            };
        }

        /// <summary> Up, Down, Right = true </summary>
        public static Directions2D UDR
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true,
                Right = true
            };
        }

        /// <summary> Up, Left, Right = true </summary>
        public static Directions2D ULR
        {
            get => new Directions2D()
            {
                Up = true,
                Left = true,
                Right = true
            };
        }

        /// <summary> Down, Left, Right = true </summary>
        public static Directions2D DLR
        {
            get => new Directions2D()
            {
                Down = true,
                Left = true,
                Right = true
            };
        }

        /// <summary> Up, Down = true </summary>
        public static Directions2D UD
        {
            get => new Directions2D()
            {
                Up = true,
                Down = true
            };
        }

        /// <summary> Up, Left = true </summary>
        public static Directions2D UL
        {
            get => new Directions2D()
            {
                Up = true,
                Left = true
            };
        }

        /// <summary> Up, Right = true </summary>
        public static Directions2D UR
        {
            get => new Directions2D()
            {
                Up = true,
                Right = true
            };
        }

        /// <summary> Down, Left = true </summary>
        public static Directions2D DL
        {
            get => new Directions2D()
            {
                Down = true,
                Left = true
            };
        }

        /// <summary> Down, Right = true </summary>
        public static Directions2D DR
        {
            get => new Directions2D()
            {
                Down = true,
                Right = true
            };
        }

        /// <summary> Left, Right = true </summary>
        public static Directions2D LR
        {
            get => new Directions2D()
            {
                Left = true,
                Right = true
            };
        }

        /// <summary> Up = true </summary>
        public static Directions2D U
        {
            get => new Directions2D()
            {
                Up = true
            };
        }

        /// <summary> Down = true </summary>
        public static Directions2D D
        {
            get => new Directions2D()
            {
                Down = true
            };
        }

        /// <summary> Left = true </summary>
        public static Directions2D L
        {
            get => new Directions2D()
            {
                Left = true
            };
        }

        /// <summary> Right = true </summary>
        public static Directions2D R
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