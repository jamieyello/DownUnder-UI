using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.Utility.Images
{
    internal class Image
    {
        public Texture2D texture;
        private bool[,] collision = new bool[0, 0];

        /// <summary>
        /// This takes an image and generates a bool collision map. (Where a pixel's blue value is less than 60, collision will be true)
        /// </summary>
        /// <param name="map"></param>
        public void SetCollision(Texture2D map)
        {
            collision = new bool[map.Width, map.Height];

            Color[] colors = new Color[map.Width * map.Height];
            map.GetData(colors);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    collision[x, y] = (colors[x + y * map.Width].B < 60);
                }
            }
        }

        public bool GetCollision(Vector2 point)
        {
            int x = (int)point.X * collision.GetLength(0) / texture.Width;
            int y = (int)point.X * collision.GetLength(1) / texture.Height;

            if (x < 0) return false;
            if (y < 0) return false;
            if (x > collision.GetLength(0)) return false;
            if (y > collision.GetLength(1)) return false;

            return collision[x, y];
        }
    }
}