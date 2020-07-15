using DownUnder.Utility;

namespace DownUnder.Utilities
{
    public class RectanglePart
    {
        public GenericDirections2D<float> Indents { get; set; } = new GenericDirections2D<float>(1f);

        public RectanglePart() { }
        public RectanglePart(float up, float down, float left, float right) {
            Indents.Up = up;
            Indents.Down = down;
            Indents.Left = left;
            Indents.Right = right;
        }

        public static RectanglePart Uniform(float indent) => new RectanglePart(indent, indent, indent, indent);
        public static RectanglePart Uniform(float indent, Directions2D directions) => 
            new RectanglePart(
                directions.Up ? indent : 1f,
                directions.Down ? indent : 1f, 
                directions.Left ? indent : 1f, 
                directions.Right ? indent : 1f);
        public static RectanglePart Uniform(float indent, Directions2D directions, float others) =>
            new RectanglePart(
                directions.Up ? indent : others,
                directions.Down ? indent : others,
                directions.Left ? indent : others,
                directions.Right ? indent : others);

        public object Clone()
        {
            RectanglePart c = new RectanglePart();
            c.Indents = (GenericDirections2D<float>)Indents.Clone();
            return c;
        }
    }
}
