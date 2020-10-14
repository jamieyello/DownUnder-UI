using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class WidgetDrawArgs : EventArgs
    {
        private readonly Widget _caller;
        public readonly SpriteBatch SpriteBatch;
        public readonly RenderTarget2D ParentRender;
        public readonly RectangleF AreaInRender;
        public readonly RectangleF DrawingArea;
        public readonly Point2 CursorPosition;
        public readonly RectangleF DrawingAreaUnscrolled;

        // Child area in render should be "area" normally
        public WidgetDrawArgs(Widget caller, RenderTarget2D parent_render, RectangleF drawing_area, RectangleF area_in_render, SpriteBatch sprite_batch, Point2 cursor_position)
        {
            _caller = caller;
            ParentRender = parent_render;
            AreaInRender = area_in_render;
            DrawingArea = drawing_area;
            SpriteBatch = sprite_batch;
            CursorPosition = cursor_position;
            DrawingAreaUnscrolled = DrawingArea.WithOffset(_caller.Scroll.Inverted());
        }

        /// <summary> Used to prevent <see cref="Effect"/>s from affecting further drawing. </summary>
        public void RestartDefault()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentWindow.RasterizerState);
        }

        public void StartDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentWindow.RasterizerState);
        }

        public void RestartImmediate()
        {
            EndDraw();
            StartImmediate();
        }

        public void StartImmediate()
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _caller.ParentWindow.RasterizerState);
        }

        public void EndDraw()
        {
            SpriteBatch.End();
        }
    }
}
