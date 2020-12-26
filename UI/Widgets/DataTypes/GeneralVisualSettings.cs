using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class GeneralVisualSettings
    {
        /// <summary> Defines the behavior of <see cref="Widget"/>s when being used by <see cref="WidgetBehavior"/>s. </summary>
        public enum VisualRoleType
        {
            default_widget = 0,
            text_widget = 1,
            text_edit_widget = 2,
            header_widget = 3,
            pop_up = 4,
            button = 5,
            background = 6
        }

        /// <summary> What this <see cref="Widget"/> should be regarded as regarding several <see cref="WidgetBehavior"/>. </summary>
        [DataMember]
        public VisualRoleType VisualRole { get; set; } = VisualRoleType.default_widget;

        /// <summary> The color palette of this <see cref="Widget"/>. </summary>
        [DataMember]
        public BaseColorScheme ColorScheme { get; set; } = BaseColorScheme.Dark;

        /// <summary> If set to true, colors will shift to their hovered colors on mouse-over. </summary>
        [DataMember] 
        public bool ChangeColorOnMouseOver { get; set; } = true;
        
        /// <summary> If set to false, the background color will not be drawn. </summary>
        [DataMember] 
        public bool DrawBackground { get; set; } = true;
        
        /// <summary> If set to true, an outline will be draw. (What sides are drawn is determined by OutlineSides) </summary>
        [DataMember]
        public bool DrawOutline { get; set; } = true;

        /// <summary> Which sides of the outline are drawn (top, bottom, left, right) if <see cref="DrawOutline"/> is true. </summary>
        [DataMember]
        public Directions2D OutlineSides { get; set; } = Directions2D.UDLR;
        /// <summary> Represents the corners this <see cref="Widget"/> will snap to within the <see cref="IParent"/>. </summary>

        public Color TextColor => ColorScheme.GetText(VisualRole).CurrentColor;
        public Color BackgroundColor => ColorScheme.GetBackground(VisualRole).CurrentColor;
        public Color OutlineColor => ColorScheme.GetOutline(VisualRole).CurrentColor;

        public void Update(float step, bool is_hovered)
        {
            ColorScheme.Update(VisualRole, step, ChangeColorOnMouseOver, is_hovered);
        }

        public static GeneralVisualSettings Invisible => new GeneralVisualSettings
        {
            ChangeColorOnMouseOver = false,
            DrawBackground = false,
            DrawOutline = false,
        };

        public object Clone()
        {
            GeneralVisualSettings c = new GeneralVisualSettings();
            c.VisualRole = VisualRole;
            c.ColorScheme = (BaseColorScheme)ColorScheme.Clone();
            c.ChangeColorOnMouseOver = ChangeColorOnMouseOver;
            c.DrawBackground = DrawBackground;
            c.DrawOutline = DrawOutline;
            c.OutlineSides = OutlineSides;
            return c;
        }
    }
}
