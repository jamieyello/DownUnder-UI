using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class DrawingData : IDisposable
    {
        /// <summary>
        /// Local sprite font used for text display.
        /// </summary>
        public SpriteFont sprite_font;

        public GraphicsDevice graphics_device;

        public RenderTarget2D local_render_target;

        /// <summary>
        /// Local texture used for drawing of various solid rectangles.
        /// </summary>
        public Texture2D white_dot;

        public SpriteBatch sprite_batch;

        public void Dispose()
        {
            local_render_target?.Dispose();
            white_dot?.Dispose();
        }
    }
}
