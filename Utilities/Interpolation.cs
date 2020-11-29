using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;

namespace DownUnder.Utility
{
    public enum InterpolationType
    {
        /// <summary> y = x </summary>
        linear,
        /// <summary> y = x * x </summary>
        squared,
        /// <summary> y = x * x * x </summary>
        cubed,
        /// <summary> y = x^4 </summary>
        exponential4,
        /// <summary> y = x^5</summary>
        exponential5,
        /// <summary> y = x^6 </summary>
        exponential6,
        /// <summary> y = sin(x * π / 2) (A stretched sin wave where the bottom x/y is at 0 and the top x/y is at 1, recommended) </summary>
        fake_sin,
        /// <summary> y = x / y  </summary>
        inverse

    }

    public static class Interpolation
    {
        /// <summary> A function that determines the middle ground for any given two objects. </summary>
        /// <typeparam name="T">The type of object to be used and returned.</typeparam>
        /// <param name="initial_object">The first object to represent the starting point of the interpolation.</param>
        /// <param name="target_object">The second object to represent the destination of the interpolation.</param>
        /// <param name="progress">A value typically between 0f and 1f representing how far along the interpolation you want.</param>
        /// <param name="interpolation_type">The type of interpolation you want to use.</param>
        public static T GetMiddle<T>(T initial_object, T target_object, float progress, InterpolationType interpolation_type)
        {
            switch (typeof(T))
            {
                case Type _ when typeof(T).IsAssignableFrom(typeof(Color)): // Special cases need to be made for types that can't be converted to 'System.Single'
                    return (T)Convert.ChangeType
                        (
                            Color.Lerp
                            (
                                (Color)Convert.ChangeType(initial_object, typeof(Color)),
                                (Color)Convert.ChangeType(target_object, typeof(Color)),
                                Plot(progress, interpolation_type)
                            ), typeof(T)
                        );

                case Type _ when typeof(T).IsAssignableFrom(typeof(RectangleF)):
                    return (T)Convert.ChangeType
                        (
                            ((RectangleF)Convert.ChangeType(initial_object, typeof(RectangleF))).Lerp
                            (
                                (RectangleF)Convert.ChangeType(target_object, typeof(RectangleF)),
                                Plot(progress, interpolation_type)
                            ), typeof(T)
                        );

                case Type _ when typeof(T).IsAssignableFrom(typeof(Point2)):
                    return (T)Convert.ChangeType
                        (
                            ((Point2)Convert.ChangeType(initial_object, typeof(Point2))).Lerp
                            (
                                (Point2)Convert.ChangeType(target_object, typeof(Point2)),
                                Plot(progress, interpolation_type)
                            ), typeof(T)
                        );

                case Type _ when typeof(T).IsAssignableFrom(typeof(Vector3)):
                    return (T)Convert.ChangeType
                        (
                            ((Vector3)Convert.ChangeType(initial_object, typeof(Vector3))) 
                            * Plot(progress, interpolation_type), typeof(T)
                        );

                case Type _ when typeof(T).IsAssignableFrom(typeof(Vector2)):
                    return (T)Convert.ChangeType
                        (
                            Vector2.Lerp
                            (
                                (Vector2)Convert.ChangeType(initial_object, typeof(Vector2)),
                                (Vector2)Convert.ChangeType(target_object, typeof(Vector2)),
                                Plot(progress, interpolation_type)
                            ), typeof(T)
                        );

                case Type _ when typeof(T).IsAssignableFrom(typeof(VertexPositionColor)):
                    return (T)Convert.ChangeType
                        (
                            ((VertexPositionColor)Convert.ChangeType(initial_object, typeof(VertexPositionColor))).Lerp
                            (
                                (VertexPositionColor)Convert.ChangeType(target_object, typeof(VertexPositionColor)),
                                Plot(progress, interpolation_type)
                            ), typeof(T)
                        );

                // Add new cases here.

                default: // A float will be returned.
                         // If error is thrown *below*, cases need to be made for types that can't be converted to 'System.Single'

                    return // start + (target - start) * interpolated_progress
                        (T)Convert.ChangeType(
                        ((float)Convert.ChangeType(initial_object, typeof(float))
                        + ((float)Convert.ChangeType(target_object, typeof(float))
                        - (float)Convert.ChangeType(initial_object, typeof(float)))
                        * Plot(progress, interpolation_type)), typeof(T));
            }
        }

        /// <summary> Returns the 'y' value of 'x' with interpolation applied. (y = x * interpolation) </summary>
        /// <param name="x">Typically between 0 and 1.</param>
        /// <param name="type">The type of interpolation to be used.</param>
        /// <param name="cap">If true, the result will not be above or below 0.</param>
        public static float Plot(float x, InterpolationType type, bool cap = false)
        {
            float y;
            switch (type)
            {
                case InterpolationType.linear:
                    y = x;
                    break;

                case InterpolationType.squared:
                    y = x * x;
                    break;

                case InterpolationType.cubed:
                    y = x * x * x;
                    break;

                case InterpolationType.exponential4:
                    y = x * x * x * x;
                    break;

                case InterpolationType.exponential5:
                    y = x * x * x * x * x;
                    break;

                case InterpolationType.exponential6:
                    y = x * x * x * x * x * x;
                    break;

                case InterpolationType.fake_sin:
                    y = (float)Math.Sin(x * Math.PI / 2);
                    break;

                case InterpolationType.inverse:
                    y = 1 - x;
                    break;

                default:
                    Debug.WriteLine("Interpolation.GetValue: Unsupported interpolation. Using liniar.");
                    y = x;
                    break;
            }

            if (cap)
            {
                if (y > 1f) y = 1f; 
                if (y < 0f) y = 0f;
            }
            return y;
        }
    }
}