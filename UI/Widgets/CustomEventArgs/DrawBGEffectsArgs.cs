using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class DrawBGEffectsArgs : EventArgs
    {
        private readonly Widget _caller;
        public readonly SpriteBatch SpriteBatch;
        public readonly RenderTarget2D ParentRender;
        public readonly RectangleF ChildAreaInRender;

        public DrawBGEffectsArgs(Widget caller, RenderTarget2D parent_render, RectangleF child_area_in_render, SpriteBatch sprite_batch)
        {
            _caller = caller;
            ParentRender = parent_render;
            ChildAreaInRender = child_area_in_render;
            SpriteBatch = sprite_batch;
        }

        /// <summary> Used to prevent <see cref="Effect"/>s from affecting further drawing. </summary>
        public void RestartDraw()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentWindow.RasterizerState);
        }
    }
}
