using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    /// <summary> A basic Line graph graphic that does not account for any additional behavior such as controls. </summary>
    public class SimpleLineGraph : WidgetBehavior
    {
        /// <summary> Pass the index, return the Y value ranging from 0f to 1f (or null for no entry). </summary>
        public Func<int, float?> GetY;
        public int Start = 0;
        public int? Range = null;
        public Color Color = Color.White;
        public float Thickness = 1f;

        public Point2 Panning = new Point2();
        public Point2 Zoom = new Point2(1f, 1f);

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        protected override void Initialize()
        {

        }

        protected override void ConnectEvents()
        {
            Parent.OnDraw += DrawLine;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnDraw -= DrawLine;
        }

        public override object Clone()
        {
            SimpleLineGraph c = new SimpleLineGraph();
            c.GetY = GetY;
            return c;
        }

        void DrawLine(object s, WidgetDrawArgs args)
        {
            var prev = GetPoint(Start, args.DrawingArea);
            for (
                int i = Start + 1;
                Range == null || i < Start + Range;
                i++
                )
            {
                //Debug.WriteLine(i);
                var next = GetPoint(i, args.DrawingArea);
                if (Range == null && prev == null && next == null) break;
                if (prev == null || next == null) continue;
                if (prev.Value.X > args.DrawingArea.Right && next.Value.X > args.DrawingArea.Right) break;
                //Debug.WriteLine("Drawing line " + prev.Value.ToString() + next.Value.ToString());
                args.SpriteBatch.DrawLine(prev.Value, next.Value, Color, Thickness);
                prev = next;
            }
        }

        /// <summary> Returns the drawing point for this index. </summary>
        Point2? GetPoint(int i, RectangleF drawing_area)
        {
            float? y_v = GetY(i);
            if (y_v == null) return null;
            Point2 result = new Point2((i + Panning.X) * Zoom.X, drawing_area.Height - y_v.Value * Zoom.Y).WithOffset(drawing_area.Position);
            return result;
        }
    }
}
