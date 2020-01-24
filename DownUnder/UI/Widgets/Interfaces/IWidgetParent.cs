using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Interfaces
{
    /// <summary> An object that can own a widget. Typically a DWindow or a Widget. </summary>
    public interface IWidgetParent
    {
        SpriteFont SpriteFont { get; }
        GraphicsDevice GraphicsDevice { get; }
        float Width { get; }
        float Height { get; }
        Point2 Size { get; }
    }
}
