using DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
        
        public float Proximity = 50f;
        public bool DrawCircles = true;
        public bool UseSingleThread = false;

        protected override void Initialize()
        {
            prox_list = new ProxList(Proximity, Widget.MAXIMUM_WIDGET_SIZE, Widget.MAXIMUM_WIDGET_SIZE);
            cursor = prox_list.Add(new Point2());
            if (Parent.ParentWindow != null) LoadContent(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += LoadContent;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
            Parent.OnClick += AddCircle;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= LoadContent;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
            Parent.OnClick -= AddCircle;
        }

        void LoadContent(object sender, EventArgs args)
        {
            if (circle_t == null) circle_t = Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Tiny Circle");
        }

        void Update(object sender, EventArgs args)
        {
            // Update circle positions
            for (int i = 0; i < circles.Count; i++) circles[i].Update();

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
                float transparency = 1f - (float)circle.circle_position.Position.DistanceFrom(neighbor.Position) / Proximity;
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
            foreach (var line in lines) args.SpriteBatch.DrawLine(line.Key.Position, line.Key.Size, Color.Lerp(Color.Transparent, Color.Red, line.Value));
            if (DrawCircles) foreach (var circle in circles) circle.Draw(circle_t, args);
        }

        void AddCircle(object sender, EventArgs args)
        {
            RectangleF area = Parent.Area.SizeOnly().ResizedBy(-120f, Directions2D.All);

            for (int i = 0; i < 50; i++) circles.Add(NeuronsCircle.RandomCircle(prox_list, area));
        }

        public void AddRandomCircle(RectangleF area)
        {
            circles.Add(NeuronsCircle.RandomCircle(prox_list, area));
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
