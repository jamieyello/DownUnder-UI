using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities.DrawingEffects;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class DrawPixelGrid : WidgetBehavior
    {
        PixelGrid _pixel_grid_effect = new PixelGrid();

        public float PixelGlowRadius { get; set; } = 40f;

        /// <summary>
        /// All Events should be added here.
        /// </summary>
        protected override void ConnectEvents()
        {
            _pixel_grid_effect.Size = Parent.Size;
            _pixel_grid_effect.Spacing = Parent.DeveloperObjects.AddWidgetSpacing;
            Parent.OnDraw += Draw;
            Parent.OnUpdate += Update;
            Parent.OnAddWidgetSpacingChange += UpdateWidgetGrid;
        }

        /// <summary>
        /// All Events added in ConnectEvents should be removed here.
        /// </summary>
        internal override void DisconnectEvents()
        {
            Parent.OnDraw -= Draw;
            Parent.OnUpdate -= Update;
            Parent.OnAddWidgetSpacingChange -= UpdateWidgetGrid;
        }

        private void Update(object sender, EventArgs args)
        {
            if (Parent is IScrollableWidget s_parent)
            {
                _pixel_grid_effect.Size = s_parent.ContentArea.Size;
            }
            else
            {
                _pixel_grid_effect.Size = Parent.Size;
            }

            if (Parent.IsPrimaryHovered && Parent.ParentWindow.DraggingObject is Widget widget)
            {
                RectangleF area = widget.Area
                    .WithCenter(Parent.UpdateData.UIInputState.CursorPosition)
                    .Rounded(Parent.DeveloperObjects.AddWidgetSpacing)
                    .WithOffset(Parent.Position.Inverted());

                foreach (GridPixel pixel in _pixel_grid_effect.GetAllPixels())
                {
                    float closeness = (PixelGlowRadius - (float)area.DistanceFrom(pixel.Position)) / PixelGlowRadius;

                    pixel.ChangingColor.SetTargetValue(Color.Lerp(Color.DarkBlue, Color.Blue, closeness), true);
                    if (closeness > 0f) pixel.Size = 1f + closeness;
                    else pixel.Size = 1f;
                }
            }
            else
            {
                foreach (GridPixel pixel in _pixel_grid_effect.GetAllPixels())
                {
                    pixel.Size = 1f;
                    pixel.ChangingColor.SetTargetValue(Color.Black);
                }
            }

            _pixel_grid_effect.Update(Parent.UpdateData.ElapsedSeconds);
        }

        private void Draw(object sender, EventArgs args)
        {
            if (Parent is IScrollableWidget s_parent)
            {
                _pixel_grid_effect.Draw(Parent.SpriteBatch, Parent.PositionInWindow - s_parent.Scroll.Inverted()); // Point2 doesn't have + in this version of ME, update later
            }
            else
            {
                _pixel_grid_effect.Draw(Parent.SpriteBatch, Parent.PositionInWindow);
            }
        }

        private void UpdateWidgetGrid(object sender, EventArgs args)
        {
            _pixel_grid_effect.UpdateGridDimensions();
        }
    }
}
