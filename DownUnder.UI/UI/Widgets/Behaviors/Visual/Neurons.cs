using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.Behaviors.Visual.NeuronsObjects;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.Extensions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class Neurons : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };

        Texture2D circle_t;
        ProxList prox_list;
        readonly List<NeuronsCircle> circles = new List<NeuronsCircle>();
        readonly ConcurrentDictionary<RectangleF, float> lines = new ConcurrentDictionary<RectangleF, float>();
        const int circle_count = 400;

        const float Proximity = 250f;
        public bool DrawCircles { get; } = false;
        public bool UseSingleThread { get; } = false;
        const float LineThickness = 2f;
        readonly Color LineColor = Color.LimeGreen.ShiftBrightness(0.3f);
        readonly Point2 Offset = new Point2(-200f,-200f);
        const float circle_speed = 0.01f;

        protected override void Initialize() {
            prox_list = new ProxList(Proximity, Widget.MAXIMUM_WIDGET_SIZE, Widget.MAXIMUM_WIDGET_SIZE);

            var area = new RectangleF(0, 0, 1920 + 400, 1080 + 400);
            //Parent.Area.SizeOnly().ResizedBy(100f, Directions2D.DR);

            for (var i = 0; i < circle_count; i++)
                circles.Add(NeuronsCircle.RandomCircle(prox_list, area));

            if (Parent.ParentDWindow is null)
                return;

            LoadContent(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnParentWindowSet += LoadContent;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents() {
            Parent.OnParentWindowSet -= LoadContent;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
        }

        void LoadContent(object sender, EventArgs args) =>
            circle_t ??= Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Tiny Circle");

        void Update(object sender, EventArgs args) {
            //Debug.WriteLine("Point count: " + circles.Count);

            // Update circle positions
            for (var i = 0; i < circles.Count; i++)
                circles[i].Update(circle_speed);

            // Update connection lines
            lines.Clear();

            if (UseSingleThread) {
                foreach (var c in circles)
                    UpdateCircleLines(c);
            } else
                Parallel.ForEach(circles, UpdateCircleLines);
        }

        void UpdateCircleLines(NeuronsCircle circle) {
            foreach (var neighbor in prox_list.GetNeighbors(circle.circle_position)) {
                // use x * y to determine the first point in the rectangle (smaller first) to avoid duplicate lines
                var transparency = 1f - (float)circle.circle_position.Position.DistanceFrom(neighbor.Position) / (Proximity / 2);

                if (!(transparency > 0f))
                    continue;

                if (circle.circle_position.Position.Product() < neighbor.Position.Product())
                    lines.TryAdd(
                        new RectangleF(
                            circle.circle_position.X,
                            circle.circle_position.Y,
                            neighbor.X,
                            neighbor.Y
                        ), transparency
                    );
                else
                    lines.TryAdd(
                        new RectangleF(
                            neighbor.X,
                            neighbor.Y,
                            circle.circle_position.X,
                            circle.circle_position.Y
                        ), transparency
                    );
            }
        }

        void Draw(object sender, WidgetDrawArgs args) {
            //args.RestartDefault();

            // foreach (var neighbor in neighbors)
            //     args.SpriteBatch.DrawLine(cursor.Position, neighbor.Position, Color.White);

            foreach (var line in lines)
                args.SpriteBatch.DrawLine(line.Key.Position.WithOffset(Offset), line.Key.Size.ToPoint2().WithOffset(Offset), Color.Lerp(Color.Transparent, LineColor, line.Value), LineThickness);

            if (!DrawCircles)
                return;

            foreach (var circle in circles)
                circle.Draw(circle_t, args);
        }

        public void AddRandomCircle(RectangleF area) =>
            circles.Add(NeuronsCircle.RandomCircle(prox_list, area));

        public override object Clone() =>
            new Neurons();
    }
}