using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class DrawBGEffectsArgs : EventArgs
    {
        public DrawBGEffectsArgs(RenderTarget2D parent_render, RectangleF child_area_in_render)
        {
            ParentRender = parent_render;
            ChildAreaInRender = child_area_in_render;
        }

        public readonly RenderTarget2D ParentRender;
        public readonly RectangleF ChildAreaInRender;
    }
}
