using DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects;
using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class Neurons : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };
        Texture2D circle_t;
        ProxList prox_list;
        List<NeuronsCircle> circles = new List<NeuronsCircle>();
        ProxListPosition cursor;
        List<ProxListPosition> neighbors = new List<ProxListPosition>();
        ConcurrentDictionary<RectangleF, float> lines = new ConcurrentDictionary<RectangleF, float>();
        int circle_count = 400;

        public float Proximity = 250f;
        public bool DrawCircles = false;
        public bool UseSingleThread = false;
        public float LineThickness = 2f;
        public Color LineColor = Color.LimeGreen.ShiftBrightness(0.3f);
        public Point2 Offset = new Point2(-200f,-200f);
        public float circle_speed = 0.01f;

        protected override void Initialize()
        {
            prox_list = new ProxList(Proximity, Widget.MAXIMUM_WIDGET_SIZE, Widget.MAXIMUM_WIDGET_SIZE);
            cursor = prox_list.Add(new Point2());
            RectangleF area = new RectangleF(0, 0, 1920 + 400, 1080 + 400); //Parent.Area.SizeOnly().ResizedBy(100f, Directions2D.DR);
            for (int i = 0; i < circle_count; i++) circles.Add(NeuronsCircle.RandomCircle(prox_list, area));
            if (Parent.ParentDWindow != null) LoadContent(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += LoadContent;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= LoadContent;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
        }

        void LoadContent(object sender, EventArgs args)
        {
            if (circle_t == null) circle_t = Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Tiny Circle");
        }

        void Update(object sender, EventArgs args)
        {
            //Debug.WriteLine("Point count: " + circles.Count);

            // Update circle positions
            for (int i = 0; i < circles.Count; i++) circles[i].Update(circle_speed);

            // Update connection lines
            lines.Clear();
            if (UseSingleThread)
            {
                for (int i = 0; i < circles.Count; i++) UpdateCircleLines(circles[i]);
            }
            else Parallel.ForEach(circles, (circle) => UpdateCircleLines(circle));
        }

        private void UpdateCircleLines(NeuronsCircle circle)
        {
            foreach (var neighbor in prox_list.GetNeighbors(circle.circle_position))
            {
                // use x * y to determine the first point in the rectangle (smaller first) to avoid duplicate lines
                float transparency = 1f - (float)circle.circle_position.Position.DistanceFrom(neighbor.Position) / (Proximity / 2);
                if (transparency > 0f)
                {
                    if (circle.circle_position.Position.Product() < neighbor.Position.Product())
                    {
                        lines.TryAdd(new RectangleF
                            (
                            circle.circle_position.X,
                            circle.circle_position.Y,
                            neighbor.X,
                            neighbor.Y
                            ), transparency);
                    }
                    else
                    {
                        lines.TryAdd(new RectangleF
                            (
                            neighbor.X,
                            neighbor.Y,
                            circle.circle_position.X,
                            circle.circle_position.Y
                            ), transparency);
                    }
                }
            }
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            //args.RestartDefault();
            foreach (var neighbor in neighbors)
            {
                args.SpriteBatch.DrawLine(cursor.Position, neighbor.Position, Color.White);
            }
            foreach (var line in lines) args.SpriteBatch.DrawLine(line.Key.Position.WithOffset(Offset), line.Key.Size.ToPoint2().WithOffset(Offset), Color.Lerp(Color.Transparent, LineColor, line.Value), LineThickness);
            if (DrawCircles) foreach (var circle in circles) circle.Draw(circle_t, args);
        }

        public void AddRandomCircle(RectangleF area)
        {
            circles.Add(NeuronsCircle.RandomCircle(prox_list, area));
        }

        public override object Clone()
        {
            return new Neurons();
        }
    }
}
