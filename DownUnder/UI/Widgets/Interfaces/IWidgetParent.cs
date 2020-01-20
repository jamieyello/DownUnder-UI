using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Interfaces
{
    public interface IWidgetParent
    {
        SpriteFont SpriteFont { get; }
        GraphicsDevice GraphicsDevice { get; }
        float Width { get; }
        float Height { get; }
        Point2 Size { get; }
    }
}
