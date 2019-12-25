using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.SpriteEngine
{
    public class SpriteEngine
    {
        public Field.Field Field { get; set; }

        public void Draw(SpriteBatch sprite_batch)
        {
            Field.Draw(sprite_batch);
        }
    }
}
