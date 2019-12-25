using DownUnder.Utility.Images;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.SpriteEngine.Entities
{
    public abstract class Entity
    {
        protected List<ImageSetIndex> images = new List<ImageSetIndex>();
        protected int image_size;

        public Vector2 Location { get; set; } = new Vector2();

        public Entity(ImagePool image_pool)
        {
            
        }

        public virtual void Update(float step)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
