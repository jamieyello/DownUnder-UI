using DownUnder.Utility;
using DownUnder.Utility.Images;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DownUnder.SpriteEngine.Field
{
    /// <summary>
    /// A collection of backgrounds.
    /// </summary>
    public class Field
    {
        List<BackGround> _backgrounds = new List<BackGround>();
        public Orientation orientation = new Orientation();

        /// <summary>
        /// A field with a single backround filled with a single tile.
        /// </summary>
        /// <param name="image_pool">Image pool to be used.</param>
        /// <param name="filler_tile">The name of the image set to be used.</param>
        /// <param name="width">How wide the background will be.</param>
        /// <param name="height">How high the background will be.</param>
        /// <returns></returns>
        public static Field UniformField(ImagePool image_pool, string filler_tile, int width, int height)
        {
            Field result = new Field();
            result._backgrounds.Add(new BackGround(image_pool, filler_tile, width, height));
            return result;
        }

        public void Draw(SpriteBatch sprite_batch)
        {
            foreach (BackGround background in _backgrounds)
            {
                background.Draw(sprite_batch, orientation);
            }
        }
    }
}
