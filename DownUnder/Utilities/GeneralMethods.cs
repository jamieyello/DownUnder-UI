using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DownUnder.Utility
{
    public static class GeneralMethods
    {
        public enum NumericRelationship
        {
            GreaterThan = 1,
            EqualTo = 0,
            LessThan = -1
        };

        public static NumericRelationship Compare(ValueType value1, ValueType value2)
        {
            if (!IsNumeric(value1))
                throw new ArgumentException("value1 is not a number.");
            else if (!IsNumeric(value2))
                throw new ArgumentException("value2 is not a number.");

            // Use BigInteger as common integral type
            if (IsInteger(value1) && IsInteger(value2))
            {
                BigInteger bigint1 = (BigInteger)value1;
                BigInteger bigint2 = (BigInteger)value2;
                return (NumericRelationship)BigInteger.Compare(bigint1, bigint2);
            }
            // At least one value is floating point; use Double.
            else
            {
                Double dbl1 = 0;
                Double dbl2 = 0;
                try
                {
                    dbl1 = Convert.ToDouble(value1);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("value1 is outside the range of a Double.");
                }
                try
                {
                    dbl2 = Convert.ToDouble(value2);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("value2 is outside the range of a Double.");
                }
                return (NumericRelationship)dbl1.CompareTo(dbl2);
            }
        }

        public static bool IsInteger(ValueType value)
        {
            return (value is SByte || value is Int16 || value is Int32
                    || value is Int64 || value is Byte || value is UInt16
                    || value is UInt32 || value is UInt64
                    || value is BigInteger);
        }

        public static bool IsNumeric(ValueType value)
        {
            return (value is Byte ||
                    value is Int16 ||
                    value is Int32 ||
                    value is Int64 ||
                    value is SByte ||
                    value is UInt16 ||
                    value is UInt32 ||
                    value is UInt64 ||
                    value is BigInteger ||
                    value is Decimal ||
                    value is Double ||
                    value is Single);
        }

        public static bool IsFloat(ValueType value)
        {
            return (value is float | value is double | value is Decimal);
        }

        private static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

        internal static bool IsNumericType(this Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        private static HashSet<Type> NonIntegralTypes = new HashSet<Type>
        {
            typeof(float),  typeof(double),  typeof(decimal)
        };

        internal static bool IsIntegral(this Type type)
        {
            return !(NonIntegralTypes.Contains(type) || NonIntegralTypes.Contains(Nullable.GetUnderlyingType(type)));
        }

        /// <summary>
        /// Calculates the distance between a point and a rectangle.
        /// </summary>
        public static double DistanceFrom(this RectangleF rectangle, Point point)
        {
            var dx = Math.Max(Math.Max(rectangle.X - point.X, point.X - rectangle.Right), 0);
            var dy = Math.Max(Math.Max(rectangle.Top - point.Y, point.Y - rectangle.Bottom), 0);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Returns rectangle as if it's been applied to the containing rectangle.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static RectangleF RectAnd(this RectangleF rectangle, RectangleF container)
        {
            RectangleF result = rectangle;
            result.Position = new Point2(
                result.Position.X + container.Position.X,
                result.Position.Y + container.Position.Y
                );
            return result;
        }

        public static Rectangle SnapRectangleToRectangle(Rectangle inner, Rectangle outer, DiagonalDirections2D snapping_policy)
        {
            inner.Location += outer.Location;

            Directions2D snapping = snapping_policy.ToPerpendicular();

            // left
            if (snapping.Left && !snapping.Right)
            {
                inner.X = outer.X;
            }

            // right
            if (!snapping.Left && snapping.Right)
            {
                inner.X = outer.X + outer.Width - inner.Width;
            }

            // left and right
            if (snapping.Left && snapping.Right)
            {
                inner.X = outer.X;
                inner.Width = outer.Width;
            }

            // up
            if (snapping.Up && !snapping.Down)
            {
                inner.Y = outer.Y;
            }

            // down
            if (!snapping.Up && snapping.Down)
            {
                inner.Y = outer.Y + outer.Height - inner.Height;
            }

            // up and down
            if (snapping.Up && snapping.Down)
            {
                inner.Y = outer.Y;
                inner.Height = outer.Height;
            }

            return inner;
        }

        public static Point2 AddPoint2(this Point2 p1, Point2 p2)
        {
            return new Point2(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
}