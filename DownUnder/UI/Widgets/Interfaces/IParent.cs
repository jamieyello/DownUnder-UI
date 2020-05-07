using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Interfaces {
    /// <summary> An object that can own a widget. Typically a <see cref="DWindow"/> or a <see cref="Widget"/>. </summary>
    public interface IParent {
        SpriteFont SpriteFont { get; }
        GraphicsDevice GraphicsDevice { get; }
        RectangleF Area { get; }
        float Width { get; }
        float Height { get; }
        Point2 Size { get; }
    }
}
