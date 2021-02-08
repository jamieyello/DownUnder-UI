using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets {
    /// <summary> An object that can own a widget. Typically a <see cref="DWindow"/> or a <see cref="Widget"/>. </summary>
    public interface IParent {
        SpriteFont WindowFont { get; }
        GraphicsDevice GraphicsDevice { get; }
        RectangleF Area { get; }
        float Width { get; }
        float Height { get; }
        Point2 Size { get; }
        Point2 PositionInRender { get; }
        IParent Parent { get; }
    }
}
