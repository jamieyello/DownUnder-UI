using System;
using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects
{
    sealed class NeuronsCircle {
        ProxList _list;
        public readonly ProxListPosition circle_position;

        readonly Point2 circle_size = new Point2(10f, 10f);
        Vector2 circle_offset = new Point2(20,35);
        Vector2 circle_momentum = new Point2();

        // Randoms
        readonly Vector2 gravity_force = new Vector2(0.0005f, 0.0005f);
        readonly Vector2 circle_origin = new Point2(50, 50);

        public NeuronsCircle(ProxList list) {
            _list = list;
            circle_position = list.Add(new Point2());
        }

        public NeuronsCircle(ProxList list, Vector2 origin, Vector2 offset, Vector2 gravity) {
            _list = list;
            circle_origin = origin;
            circle_offset = offset;
            gravity_force = gravity;
            circle_position = list.Add(circle_origin + circle_offset);
        }

        public static NeuronsCircle RandomCircle(ProxList list, RectangleF area) {
            var random = new Random();

            var x_offset = random.Next(10000) / 10000f * list.InteractDiameter * 2 - list.InteractDiameter;
            var y_offset = random.Next(10000) / 10000f * list.InteractDiameter * 2 - list.InteractDiameter;

            var circle_origin = new Vector2(random.Next((int)area.Width), random.Next((int)area.Height)) + area.Position;
            var circle_offset = new Vector2(x_offset, y_offset);
            var gravity_force = new Vector2(0.0005f + random.Next(1000) * 0.00001f, 0.0005f + random.Next(1000) * 0.00001f);

            var result = new NeuronsCircle(list, circle_origin, circle_offset, gravity_force);
            var mixup = random.Next(100);

            for (var i = 0; i < mixup; i++)
                result.Update(0.5f);

            return result;
        }

        public void Update(float step) {
            circle_momentum -= circle_offset * gravity_force * step;
            circle_offset += circle_momentum * step;
            circle_position.Position = circle_origin + circle_offset;
        }

        public void Draw(Texture2D circle, WidgetDrawArgs args) =>
            args.SpriteBatch.Draw(
                circle,
                circle_size.AsRectangleSize().WithCenter(circle_position.Position).ToRectangle(),
                Color.White
            );
    }
}
