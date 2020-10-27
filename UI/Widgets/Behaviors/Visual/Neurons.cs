using DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects;
using DownUnder.UI.Widgets.DataTypes;
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
        float proximity = 100f;

        protected override void Initialize()
        {
            prox_list = new ProxList(proximity, Widget.MAXIMUM_WIDGET_SIZE, Widget.MAXIMUM_WIDGET_SIZE);
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
            Parallel.ForEach(circles, (circle) => circle.Update());

            // Make lines
            lines.Clear();

            Parallel.ForEach(circles, (circle) => 
            {
                foreach (var neighbor in prox_list.GetNeighbors(circle.circle_position))
                {
                    // use x * y to determine the first point in the rectangle (smaller first) to avoid duplicate lines
                    if (circle.circle_position.Position.Product() < neighbor.Position.Product())
                    {
                        lines.TryAdd(new RectangleF
                            (
                            circle.circle_position.X,
                            circle.circle_position.Y,
                            neighbor.X,
                            neighbor.Y
                            ), (float)circle.circle_position.Position.DistanceFrom(neighbor.Position) / proximity / 2f);
                    }
                    else
                    {
                        lines.TryAdd(new RectangleF
                            (
                            neighbor.X,
                            neighbor.Y,
                            circle.circle_position.X,
                            circle.circle_position.Y
                            ), (float)circle.circle_position.Position.DistanceFrom(neighbor.Position) / proximity / 2f);
                    }
                }
            });
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            foreach (var neighbor in neighbors)
            {
                args.SpriteBatch.DrawLine(cursor.Position, neighbor.Position, Color.White);
            }
            foreach (var line in lines) args.SpriteBatch.DrawLine(line.Key.Position, line.Key.Size, Color.Lerp(Color.Red, Color.Transparent, line.Value));
            foreach (var circle in circles) circle.Draw(circle_t, args);
        }

        void AddCircle(object sender, EventArgs args)
        {
            for (int i = 0; i < 50; i++) circles.Add(NeuronsCircle.RandomCircle(prox_list, Parent.Area.SizeOnly()));
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
