﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    /// <summary> Provides some utilities to be used while drawing. </summary>
    public sealed class WidgetDrawArgs : EventArgs {
        readonly Widget _caller;
        public readonly SpriteBatch SpriteBatch;
        public readonly RectangleF AreaInRender;
        public readonly RectangleF DrawingArea;
        public readonly Point2 CursorPosition;
        public readonly RectangleF DrawingAreaUnscrolled;
        public readonly GraphicsDevice GraphicsDevice;

        // Child area in render should be "area" normally
        public WidgetDrawArgs(
            Widget caller,
            GraphicsDevice graphics,
            RectangleF drawing_area,
            RectangleF area_in_render,
            SpriteBatch sprite_batch,
            Point2 cursor_position
        ) {
            _caller = caller;
            AreaInRender = area_in_render;
            DrawingArea = drawing_area;
            SpriteBatch = sprite_batch;
            CursorPosition = cursor_position;
            GraphicsDevice = graphics;
            DrawingAreaUnscrolled = DrawingArea.WithOffset(_caller.Scroll.Inverted());
        }

        /// <summary> Used to prevent <see cref="Effect"/>s from affecting further drawing. </summary>
        public void RestartDefault() {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState);
        }

        public void StartDraw() =>
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState);

        public void RestartImmediate(Effect effect = null) {
            EndDraw();
            StartImmediate(effect);
        }

        public void StartImmediate(Effect effect = null) =>
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _caller.ParentDWindow.RasterizerState, effect);

        public void EndDraw() =>
            SpriteBatch.End();

        /// <summary> Complete the current <see cref="SpriteBatch.Draw()"/> and return the render target to the default state without clearing graphics. </summary>
        public void ResetRenderTarget() =>
            GetBackgroundRender(); // Calling this is the same performace-wise

        /// <summary> Complete the current <see cref="SpriteBatch.Draw()"/> and return the current buffer. </summary>
        public RenderTarget2D GetBackgroundRender() {
            RestartImmediate();
            var scissor_area = SpriteBatch.GraphicsDevice.ScissorRectangle;
            _caller.ParentDWindow.SwapBackBuffer(GraphicsDevice, SpriteBatch);
            SpriteBatch.GraphicsDevice.ScissorRectangle = scissor_area;
            RestartDefault();
            return _caller.ParentDWindow.OtherBuffer;
        }

        /// <summary> Get the projection <see cref="Matrix"/> of the given <see cref="Widget"/> to draw graphics inside of. </summary>
        public Matrix GetStretchedProjection() {
            var target_size = ((Texture2D)SpriteBatch.GraphicsDevice.GetRenderTargets()[0].RenderTarget).Bounds.Size.ToVector2();
            var area_size = (Vector2)DrawingArea.Size;

            var window_offset = AreaInRender.Position.ToVector2() * 2 / target_size - new Vector2(1f, 1f);
            var area_offset = area_size / target_size;
            var resize = area_size / target_size;

            return
                Matrix.CreateScale(
                    resize.X,
                    resize.Y,
                    1f
                )
                * Matrix.CreateTranslation(
                    area_offset.X + window_offset.X,
                    -area_offset.Y - window_offset.Y,
                    0f
                );
        }
    }
}