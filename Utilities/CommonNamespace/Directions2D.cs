using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder
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

        public bool HasMultiple => Convert.ToByte(Up) + Convert.ToByte(Down) + Convert.ToByte(Left) + Convert.ToByte(Right) > 1;

        public Point2 ValueInDirection(float value)
        {
            Point2 result = new Point2();
            if (Up) result.Y -= value;
            if (Down) result.Y += value;
            if (Left) result.X -= value;
            if (Right) result.X += value;
            return result;
        }

        public Point2 ValueInDirection(Size2 value)
        {
            Point2 result = new Point2();
            if (Up) result.Y -= value.Height;
            if (Down) result.Y += value.Height;
            if (Left) result.X -= value.Width;
            if (Right) result.X += value.Width;
            return result;
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

        public Directions2D FlippedXY()
        {
            Directions2D result = new Directions2D();
            result.Up = Down;
            result.Down = Up;
            result.Left = Right;
            result.Right = Left;
            result.DisallowOpposites = DisallowOpposites;
            return result;
        }

        public Directions2D FlippedX()
        {
            Directions2D result = new Directions2D();
            result.Up = Up;
            result.Down = Down;
            result.Left = Right;
            result.Right = Left;
            result.DisallowOpposites = DisallowOpposites;
            return result;
        }

        public Directions2D FlippedY()
        {
            Directions2D result = new Directions2D();
            result.Up = Down;
            result.Down = Up;
            result.Left = Left;
            result.Right = Right;
            result.DisallowOpposites = DisallowOpposites;
            return result;
        }

        public bool GetDirection(Direction2D direction) =>
                direction == Direction2D.up ? up :
                direction == Direction2D.down ? down :
                direction == Direction2D.left ? left :
                right;

        public static bool operator ==(Directions2D d1, Directions2D d2)
        {
            // Check for null on left side.
            if (ReferenceEquals(d1, null))
            {
                if (ReferenceEquals(d2, null))
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
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(this, obj)) return true;

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

        public static Directions2D FromDirection2D(Direction2D direction) =>
            new Directions2D()
            {
                Up = direction == Direction2D.up,
                Down = direction == Direction2D.down,
                Left = direction == Direction2D.left,
                Right = direction == Direction2D.right
            };


        public static readonly Directions2D All = new Directions2D() { Up = true, Down = true, Left = true, Right = true };
        public static readonly Directions2D UDLR = new Directions2D() { Up = true, Down = true, Left = true, Right = true };
        public static readonly Directions2D UDL = new Directions2D() { Up = true, Down = true, Left = true };
        public static readonly Directions2D UDR = new Directions2D() { Up = true, Down = true, Right = true };
        public static readonly Directions2D ULR = new Directions2D() { Up = true, Left = true, Right = true };
        public static readonly Directions2D DLR = new Directions2D() { Down = true, Left = true, Right = true };
        public static readonly Directions2D UD = new Directions2D() { Up = true, Down = true };
        public static readonly Directions2D UL = new Directions2D() { Up = true, Left = true };
        public static readonly Directions2D UR = new Directions2D() { Up = true, Right = true };
        public static readonly Directions2D DL = new Directions2D() { Down = true, Left = true };
        public static readonly Directions2D DR = new Directions2D() { Down = true, Right = true };
        public static readonly Directions2D LR = new Directions2D() { Left = true, Right = true };
        public static readonly Directions2D U = new Directions2D() { Up = true };
        public static readonly Directions2D D = new Directions2D() { Down = true };
        public static readonly Directions2D L = new Directions2D() { Left = true };
        public static readonly Directions2D R = new Directions2D() { Right = true };
        public static readonly Directions2D None = new Directions2D();

        public static Direction2D GetOppositeSide(Direction2D direction) =>
            direction == Direction2D.up ? Direction2D.down :
            direction == Direction2D.down ? Direction2D.up :
            direction == Direction2D.left ? Direction2D.right :
            Direction2D.left;

    }
}