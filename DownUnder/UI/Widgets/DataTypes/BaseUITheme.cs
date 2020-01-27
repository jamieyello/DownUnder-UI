using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
{
    [DataContract]
    public class BaseUITheme
    {
        /// <summary> The colors used for the background color. </summary>
        [DataMember] public virtual ElementColors Background { get; set; } = new ElementColors(Color.CornflowerBlue, Color.CornflowerBlue.ShiftBrightness(1.1f));

        /// <summary> The colors used for any text in this widget. </summary>
        [DataMember] public virtual ElementColors Text { get; set; } = new ElementColors(Color.Black, Color.Black);

        /// <summary> The colors used for the outline of this widget. (if enabled) </summary>
        [DataMember] public virtual ElementColors Outline { get; set; } = new ElementColors();

        [DataMember] public virtual ElementColors TextEditBackground { get; set; } = new ElementColors(Color.White);

        public void Update(Widget parent, GameTime game_time)
        {
            if (parent.IsPrimaryHovered && parent.ChangeColorOnMouseOver)
            {
                Background.Hovered = true;
                Text.Hovered = true;
                Outline.Hovered = true;
                TextEditBackground.Hovered = true;
            }
            else
            {
                Background.Hovered = false;
                Text.Hovered = false;
                Outline.Hovered = false;
                TextEditBackground.Hovered = false;
            }

            Text.Update(game_time.GetElapsedSeconds());
            Background.Update(game_time.GetElapsedSeconds());
            Outline.Update(game_time.GetElapsedSeconds());
        }

        public object Clone()
        {
            BaseUITheme c = new BaseUITheme();
            
            c.Background = (ElementColors)Background.Clone();
            c.Text = (ElementColors)Text.Clone();
            c.Outline = (ElementColors)Outline.Clone();
            c.TextEditBackground = (ElementColors)TextEditBackground.Clone();

            return c;
        }
    }
}
