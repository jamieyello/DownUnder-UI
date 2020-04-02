using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities.DrawingEffects;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class DrawPixelGrid : WidgetBehavior
    {
        PixelGrid _pixel_grid_effect = new PixelGrid();

        public DrawPixelGrid()
        {

        }

        /// <summary>
        /// All Events should be added here.
        /// </summary>
        protected override void ConnectEvents()
        {
            _pixel_grid_effect.Size = Parent.Size;
            Parent.OnDraw += Draw;
            Parent.OnUpdate += Update;
        }

        /// <summary>
        /// All Events added in ConnectEvents should be removed here.
        /// </summary>
        internal override void DisconnectEvents()
        {
            Parent.OnDraw -= Draw;
            Parent.OnUpdate -= Update;
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
    }
}
