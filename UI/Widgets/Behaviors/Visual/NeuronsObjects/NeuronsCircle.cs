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
        Point2 circle_size = new Point2(4f, 4f);
        Vector2 circle_offset = new Point2(20,35);
        Vector2 circle_momentum = new Point2();

        // Randoms
        Vector2 gravity_force = new Vector2(0.0005f, 0.0005f);
        Vector2 circle_origin = new Point2(50, 50);

        public NeuronsCircle()
        {
        }

        public static NeuronsCircle RandomCircle(RectangleF area, float max_radius)
        {
            NeuronsCircle result = new NeuronsCircle();
            Random random = new Random();

            result.circle_origin = new Vector2(random.Next((int)area.Width), random.Next((int)area.Height));
            float x_offset = (random.Next(10000) / 10000f) * max_radius * 2 - max_radius;
            float y_offset = (random.Next(10000) / 10000f) * max_radius * 2  - max_radius;
            result.circle_offset = new Vector2(x_offset, y_offset);
            result.gravity_force += new Vector2(random.Next(1000) * 0.00001f, random.Next(1000) * 0.00001f);
            int mixup = random.Next(400);
            for (int i = 0; i < mixup; i++) result.Update();
            return result;
        }

        public void Update()
        {
            float speed = 0.5f;

            circle_momentum -= circle_offset * gravity_force * speed;
            circle_offset += circle_momentum * speed;
        }

        public void Draw(Texture2D circle, WidgetDrawArgs args)
        {
            args.SpriteBatch.Draw(circle, circle_size.AsRectangleSize().WithCenter(circle_origin + circle_offset).ToRectangle(), Color.White);
        }
    }
}
