using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.Utilities
{
    public enum InterpolationType
    {
        /// <summary> x = x </summary>
        linear,
        /// <summary> x = x * x </summary>
        squared,
        /// <summary> x = x * x * x </summary>
        cubed,
        /// <summary> x = x^4 </summary>
        exponential4,
        /// <summary> x = x^5 </summary>
        exponential5,
        /// <summary> x = x^6 </summary>
        exponential6,
        /// <summary> x = sin(x * π / 2) (A stretched sin wave where the bottom x/y is at 0 and the top x/y is at 1, recommended) </summary>
        fake_sin
    }

    public static class InterpolationFuncs
    {
        private static readonly Func<object, object, float, object> color_lerp = (io, to, p) => Color.Lerp((Color)io, (Color)to, p);
        private static readonly Func<object, object, float, object> rectanglf_lerp = (io, to, p) => ((RectangleF)io).Lerp((RectangleF)to, p);
        private static readonly Func<object, object, float, object> point2_lerp = (io, to, p) => ((Point2)io).Lerp((Point2)to, p);
        private static readonly Func<object, object, float, object> vector2_lerp = (io, to, p) => Vector2.Lerp((Vector2)io, (Vector2)to, p);
        private static readonly Func<object, object, float, object> vector3_lerp = (io, to, p) => Vector3.Lerp((Vector3)io, (Vector3)to, p);
        private static readonly Func<object, object, float, object> vector_position_color_lerp = (io, to, p) => ((VertexPositionColor)io).Lerp((VertexPositionColor)to, p);
        private static readonly Func<object, object, float, object> float_lerp = (io, to, p) => FloatLerp((float)io, (float)to, p);

        private static readonly Func<float, float> linear_plot = x => x;
        private static readonly Func<float, float> squared_plot = x => x * x;
        private static readonly Func<float, float> cubed_plot = x => x * x * x;
        private static readonly Func<float, float> exponential4_plot = x => x * x * x * x;
        private static readonly Func<float, float> exponential5_plot = x => x * x * x * x * x;
        private static readonly Func<float, float> exponential6_plot = x => x * x * x * x * x * x;
        private static readonly Func<float, float> fake_sin_plot = x => (float)Math.Sin(x * Math.PI / 2);

        static float FloatLerp(float initial_object, float target_object, float progress) => initial_object + target_object - initial_object * progress;

        public static Func<object, object, float, object> GetLerpFunc<T>()
        {
            switch (typeof(T))
            {
                case Type _ when typeof(T).IsAssignableFrom(typeof(Color)): return color_lerp;
                case Type _ when typeof(T).IsPrimitive: return float_lerp;
                case Type _ when typeof(T).IsAssignableFrom(typeof(RectangleF)): return rectanglf_lerp;
                case Type _ when typeof(T).IsAssignableFrom(typeof(Point2)): return point2_lerp;
                case Type _ when typeof(T).IsAssignableFrom(typeof(Vector2)): return vector2_lerp;
                case Type _ when typeof(T).IsAssignableFrom(typeof(Vector3)): return vector3_lerp;
                case Type _ when typeof(T).IsAssignableFrom(typeof(VertexPositionColor)): return vector_position_color_lerp;
                default: throw new Exception($"{nameof(T)} interpolation is not supported.");
            };
        }

        public static Func<float, float> GetPlotFunc(InterpolationType type) =>
            type switch
            {
                InterpolationType.linear => linear_plot,
                InterpolationType.squared => squared_plot,
                InterpolationType.cubed => cubed_plot,
                InterpolationType.exponential4 => exponential4_plot,
                InterpolationType.exponential5 => exponential5_plot,
                InterpolationType.exponential6 => exponential6_plot,
                InterpolationType.fake_sin => fake_sin_plot,
                _ => throw new Exception($"{nameof(InterpolationType)}.{type} not supported."),
            };
    }
}