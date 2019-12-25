using DownUnder.Utility;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class Scroll
    {
        public ChangingValue<float> X { get; } = new ChangingValue<float>(0f);
        public ChangingValue<float> Y { get; } = new ChangingValue<float>(0f);

        public Vector2 ToVector2()
        {
            return new Vector2(X.GetCurrent(), Y.GetCurrent());
        }
    }
}