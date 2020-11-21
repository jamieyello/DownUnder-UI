using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class WidgetDrawArgs : EventArgs
    {
        private readonly Widget _caller;
        public readonly SpriteBatch SpriteBatch;
        public readonly RectangleF AreaInRender;
        public readonly RectangleF DrawingArea;
        public readonly Point2 CursorPosition;
        public readonly RectangleF DrawingAreaUnscrolled;
        public readonly GraphicsDevice GraphicsDevice;

        // Child area in render should be "area" normally
        public WidgetDrawArgs(Widget caller, GraphicsDevice graphics, RectangleF drawing_area, RectangleF area_in_render, SpriteBatch sprite_batch, Point2 cursor_position)
        {
            _caller = caller;
            AreaInRender = area_in_render;
            DrawingArea = drawing_area;
            SpriteBatch = sprite_batch;
            CursorPosition = cursor_position;
            GraphicsDevice = graphics;
            DrawingAreaUnscrolled = DrawingArea.WithOffset(_caller.Scroll.Inverted());
        }

        /// <summary> Used to prevent <see cref="Effect"/>s from affecting further drawing. </summary>
        public void RestartDefault()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState);
        }

        public void StartDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState);
        }

        public void RestartImmediate()
        {
            EndDraw();
            StartImmediate();
        }

        public void StartImmediate()
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState);
        }

        public void EndDraw()
        {
            SpriteBatch.End();
        }

        public RenderTarget2D GetBackgroundRender()
        {
            RestartImmediate();
            Rectangle scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
            _caller.ParentDWindow.SwapBackBuffer(GraphicsDevice, SpriteBatch);
            SpriteBatch.GraphicsDevice.ScissorRectangle = scissor_area;
            RestartDefault();
            return _caller.ParentDWindow.OtherBuffer;
        }
    }
}
