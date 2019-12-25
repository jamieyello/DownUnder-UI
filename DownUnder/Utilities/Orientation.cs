using Microsoft.Xna.Framework;

namespace DownUnder.Utility
{
    public class Orientation
    {
        public Vector2 position = new Vector2(0, 0);
        public float rotation = 0f;
        public Vector2 scale = new Vector2(1, 1);

        public Orientation()
        {
        }

        public Orientation(float x, float y)
        {
            position = new Vector2(x, y);
        }

        public Orientation WithCamera(Camera camera)
        {
            Orientation new_orientation = (Orientation)Clone();
            new_orientation.position -= camera.Position();
            new_orientation.scale *= camera.Zoom();
            return new_orientation;
        }

        public static Orientation operator +(Orientation o1, Orientation o2)
        {
            return new Orientation()
            {
                position = o1.position + o2.position,
                rotation = o1.rotation,
                scale = o1.scale
            };
        }

        public static Orientation operator +(Orientation o, Vector2 v)
        {
            return new Orientation()
            {
                position = o.position + v,
                rotation = o.rotation,
                scale = o.scale
            };
        }

        // Weird text here to get rid of an incorrect VS error list message
        public Orientation Clone()
        {
#pragma warning disable IDE0017 // Simplify object initialization
            Orientation clone = new Orientation();
#pragma warning restore IDE0017 // Simplify object initialization
            clone.position = position;
            clone.rotation = rotation;
            clone.scale = scale;
            return clone;
        }
    }
}