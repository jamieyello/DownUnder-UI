using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.Utilities.DrawingEffects
{
    public class GridPixel : ICloneable
    {
        #region Private Fields
        
        #endregion

        #region Public Properties

        public Point2 Position { get; set; }

        public ChangingValue<Color> Color { get; set; } = new ChangingValue<Color>();

        public float Size { get; set; } = 1f;

        #endregion

        #region Public Methods

        public void Update(float step)
        {
            Color.Update(step);
        }

        public void Draw(SpriteBatch sprite_batch, Point2 offset)
        {
            sprite_batch.DrawPoint(Position.X + offset.X, Position.Y + offset.Y, Color.GetCurrent(), Size);
        }

        #endregion

        #region ICloneable Implementation

        public object Clone()
        {
            GridPixel c = new GridPixel();
            c.Color = (ChangingValue<Color>)Color.Clone();
            c.Position = Position;
            c.Size = Size;
            return c;
        }

        #endregion
    }
}
