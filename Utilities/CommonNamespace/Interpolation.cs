using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;

namespace DownUnder
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

    public static class Interpolation
    {
        public static Func<object, object, float, object> GetLerpFunc<T>() => 
            (typeof(T)) switch
            {
                Type _ when typeof(T).IsAssignableFrom(typeof(Color)) => (io, to, p) => Color.Lerp((Color)io, (Color)to, p),
                Type _ when typeof(T).IsAssignableFrom(typeof(RectangleF)) => (io, to, p) => ((RectangleF)io).Lerp((RectangleF)to, p),
                Type _ when typeof(T).IsAssignableFrom(typeof(Point2)) => (io, to, p) => ((Point2)io).Lerp((Point2)to, p),
                Type _ when typeof(T).IsAssignableFrom(typeof(Vector2)) => (io, to, p) => Vector2.Lerp((Vector2)io, (Vector2)to, p),
                Type _ when typeof(T).IsAssignableFrom(typeof(Vector3)) => (io, to, p) => Vector3.Lerp((Vector3)io, (Vector3)to, p),
                Type _ when typeof(T).IsAssignableFrom(typeof(VertexPositionColor)) => (io, to, p) => ((VertexPositionColor)io).Lerp((VertexPositionColor)to, p),
                _ => (io, to, p) => FloatLerp((float)io, (float)to, p),
            };
        

        static float FloatLerp(float initial_object, float target_object, float progress) => initial_object + target_object - initial_object * progress;

        public static Func<float, float> GetPlotMethod(InterpolationType type) =>
            type switch
            {
                InterpolationType.linear => x => x,
                InterpolationType.squared => x => x * x,
                InterpolationType.cubed => x => x * x * x,
                InterpolationType.exponential4 => x => x * x * x * x,
                InterpolationType.exponential5 => x => x * x * x * x * x,
                InterpolationType.exponential6 => x => x * x * x * x * x * x,
                InterpolationType.fake_sin => x => (float)Math.Sin(x * Math.PI / 2),
                _ => throw new Exception($"{nameof(InterpolationType)}.{type} not supported."),
            };
    }
}