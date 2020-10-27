using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects
{
    class NeuronsCircle
    {
        ProxList list;
        public ProxListPosition circle_position;
        
        Point2 circle_size = new Point2(10f, 10f);
        Vector2 circle_offset = new Point2(20,35);
        Vector2 circle_momentum = new Point2();

        // Randoms
        Vector2 gravity_force = new Vector2(0.0005f, 0.0005f);
        Vector2 circle_origin = new Point2(50, 50);

        public NeuronsCircle(ProxList list)
        {
            this.list = list;
            circle_position = list.Add(new Point2());
        }

        public NeuronsCircle(ProxList list, Vector2 origin, Vector2 offset, Vector2 gravity)
        {
            this.list = list;
            circle_origin = origin;
            circle_offset = offset;
            gravity_force = gravity;
            circle_position = list.Add(circle_origin + circle_offset);
        }

        public static NeuronsCircle RandomCircle(ProxList list, RectangleF area)
        {
            Random random = new Random();

            float x_offset = (random.Next(10000) / 10000f) * list.InteractDiameter * 2 - list.InteractDiameter;
            float y_offset = (random.Next(10000) / 10000f) * list.InteractDiameter * 2 - list.InteractDiameter;

            Vector2 circle_origin = new Vector2(random.Next((int)area.Width), random.Next((int)area.Height));
            Vector2 circle_offset = new Vector2(x_offset, y_offset);
            Vector2 gravity_force = new Vector2(0.0005f + random.Next(1000) * 0.00001f, 0.0005f + random.Next(1000) * 0.00001f);

            NeuronsCircle result = new NeuronsCircle(list, circle_origin, circle_offset, gravity_force);
            int mixup = random.Next(400);
            for (int i = 0; i < mixup; i++) result.Update();
            return result;
        }

        public void Update()
        {
            float speed = 0.1f;

            circle_momentum -= circle_offset * gravity_force * speed;
            circle_offset += circle_momentum * speed;
            circle_position.Position = circle_origin + circle_offset;
        }

        public void Draw(Texture2D circle, WidgetDrawArgs args)
        {
            args.SpriteBatch.Draw(circle, circle_size.AsRectangleSize().WithCenter(circle_position.Position).ToRectangle(), Color.White);
        }
    }
}
