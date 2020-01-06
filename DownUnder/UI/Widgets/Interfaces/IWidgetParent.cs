using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Interfaces
{
    public interface IWidgetParent
    {
        SpriteFont SpriteFont { get; }
        GraphicsDevice GraphicsDevice { get; }
    }
}
