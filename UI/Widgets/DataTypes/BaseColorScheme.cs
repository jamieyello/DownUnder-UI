using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using static DownUnder.UI.Widgets.DataTypes.GeneralVisualSettings;

namespace DownUnder.UI.Widgets.DataTypes
{
    [DataContract] public class BaseColorScheme : ICloneable {
        /// <summary> A reference to the widget that owns this palette. </summary>
        [DataMember] public ElementColors[] BackGroundColors { get; set; }
        [DataMember] public ElementColors[] TextColors { get; set; }
        [DataMember] public ElementColors[] OutlineColors { get; set; }
        [DataMember] public ElementColors SideInnerScrollBar { get; set; }
        [DataMember] public ElementColors SideOuterScrollBar { get; set; }
        [DataMember] public ElementColors BottomInnerScrollBar { get; set; }
        [DataMember] public ElementColors BottomOuterScrollBar { get; set; }

        public BaseColorScheme()=> SetDefaults();

        private void SetDefaults() {
            int length = Enum.GetValues(typeof(VisualRoleType)).Length;

            BackGroundColors = new ElementColors[length];
            TextColors = new ElementColors[length];
            OutlineColors = new ElementColors[length];
        }

        public ElementColors GetBackground(VisualRoleType category) => BackGroundColors[(int)category];
        public ElementColors GetText(VisualRoleType category) => TextColors[(int)category];
        public ElementColors GetOutline(VisualRoleType category) => OutlineColors[(int)category];

        public void Update(VisualRoleType profile, float step, bool change_color_on_hover, bool is_hovered) {
            GetBackground(profile).Update(step);
            GetText(profile).Update(step);
            GetOutline(profile).Update(step);
            if (change_color_on_hover) SetHovered(profile, is_hovered);
        }

        private void SetHovered(VisualRoleType profile, bool hovered)
        {
            GetBackground(profile).Hovered = hovered;
            GetText(profile).Hovered = hovered;
            GetOutline(profile).Hovered = hovered;
        }

        public void ForceComplete() {
            foreach (VisualRoleType category in Enum.GetValues(typeof(VisualRoleType))) {
                BackGroundColors[(int)category].ForceComplete();
                TextColors[(int)category].ForceComplete();
                OutlineColors[(int)category].ForceComplete();
            }
        }

        public object Clone() {
            BaseColorScheme c = new BaseColorScheme();

            int length = Enum.GetValues(typeof(VisualRoleType)).Length;
            for (int i = 0; i < length; i++)
            {
                c.BackGroundColors[i] = (ElementColors)BackGroundColors[i]?.Clone();
                c.TextColors[i] = (ElementColors)TextColors[i]?.Clone();
                c.OutlineColors[i] = (ElementColors)OutlineColors[i]?.Clone();
            }

            c.SideInnerScrollBar = (ElementColors)SideInnerScrollBar?.Clone();
            c.SideOuterScrollBar = (ElementColors)SideOuterScrollBar?.Clone();
            c.BottomInnerScrollBar = (ElementColors)BottomInnerScrollBar?.Clone();
            c.BottomOuterScrollBar = (ElementColors)BottomOuterScrollBar?.Clone();

            return c;
        }

        /// <summary> The basic MonoGame-esque CornFlowerBlue theme. </summary>
        private static BaseColorScheme Default {
            get {
                BaseColorScheme r = new BaseColorScheme();

                // default_widget theme
                r.GetOutline(VisualRoleType.default_widget).DefaultColor = Color.Black;
                r.GetOutline(VisualRoleType.default_widget).SpecialColor = Color.Black;
                r.GetOutline(VisualRoleType.default_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(VisualRoleType.default_widget).DefaultColor = Color.CornflowerBlue;
                r.GetBackground(VisualRoleType.default_widget).SpecialColor = Color.Yellow;
                r.GetBackground(VisualRoleType.default_widget).HoveredColor = Color.CornflowerBlue.ShiftBrightness(1.1f);
                r.GetText(VisualRoleType.default_widget).DefaultColor = Color.Black;
                r.GetText(VisualRoleType.default_widget).SpecialColor = Color.White;
                r.GetText(VisualRoleType.default_widget).HoveredColor = Color.Black;

                // text_widget theme
                r.GetOutline(VisualRoleType.text_widget).DefaultColor = Color.Black;
                r.GetOutline(VisualRoleType.text_widget).SpecialColor = Color.Black;
                r.GetOutline(VisualRoleType.text_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(VisualRoleType.text_widget).DefaultColor = Color.White.ShiftBrightness(0.8f);
                r.GetBackground(VisualRoleType.text_widget).SpecialColor = Color.White;
                r.GetBackground(VisualRoleType.text_widget).HoveredColor = Color.White;
                r.GetText(VisualRoleType.text_widget).DefaultColor = Color.Black;
                r.GetText(VisualRoleType.text_widget).SpecialColor = Color.White;
                r.GetText(VisualRoleType.text_widget).HoveredColor = Color.Black;

                // text_edit_widget theme
                r.GetOutline(VisualRoleType.text_edit_widget).DefaultColor = Color.Black;
                r.GetOutline(VisualRoleType.text_edit_widget).SpecialColor = Color.Black;
                r.GetOutline(VisualRoleType.text_edit_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(VisualRoleType.text_edit_widget).DefaultColor = Color.White;
                r.GetBackground(VisualRoleType.text_edit_widget).SpecialColor = Color.White;
                r.GetBackground(VisualRoleType.text_edit_widget).HoveredColor = Color.White;
                r.GetText(VisualRoleType.text_edit_widget).DefaultColor = Color.Black;
                r.GetText(VisualRoleType.text_edit_widget).SpecialColor = Color.White;
                r.GetText(VisualRoleType.text_edit_widget).HoveredColor = Color.Black;

                // header_widget theme
                r.GetOutline(VisualRoleType.header_widget).DefaultColor = Color.Black.ShiftBrightness(1.3f);
                r.GetOutline(VisualRoleType.header_widget).SpecialColor = Color.Black;
                r.GetOutline(VisualRoleType.header_widget).HoveredColor = Color.Black.ShiftBrightness(1.35f);
                r.GetBackground(VisualRoleType.header_widget).DefaultColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(VisualRoleType.header_widget).SpecialColor = Color.Blue.ShiftBrightness(0.5f);
                r.GetBackground(VisualRoleType.header_widget).HoveredColor = Color.Black.ShiftBrightness(1.2f);
                r.GetText(VisualRoleType.header_widget).DefaultColor = Color.White.ShiftBrightness(0.8f);
                r.GetText(VisualRoleType.header_widget).SpecialColor = Color.White;
                r.GetText(VisualRoleType.header_widget).HoveredColor = Color.White;

                return r;
            }
        }

        /// <summary> A black theme. </summary>
        public static BaseColorScheme Dark {
            get {
                BaseColorScheme r = new BaseColorScheme();

                // default_widget theme
                r.OutlineColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.Black.ShiftBrightness(1.35f), Color.Black.ShiftBrightness(1.6f), Color.Red.ShiftBrightness(0.2f));
                r.BackGroundColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.Black.ShiftBrightness(1.07f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
                r.TextColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.White.ShiftBrightness(0.75f), Color.White, Color.White);

                // text_widget theme
                r.OutlineColors[(int)VisualRoleType.text_widget] = new ElementColors(Color.Black.ShiftBrightness(1.35f), Color.Black.ShiftBrightness(1.5f), Color.Red.ShiftBrightness(0.2f));
                r.BackGroundColors[(int)VisualRoleType.text_widget] = new ElementColors(Color.Black.ShiftBrightness(1.1f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
                r.TextColors[(int)VisualRoleType.text_widget] = new ElementColors(Color.White, Color.White.ShiftBrightness(0.75f), Color.White);

                // text_edit_widget theme
                r.OutlineColors[(int)VisualRoleType.text_edit_widget] = new ElementColors(Color.Black.ShiftBrightness(1.6f), Color.Black.ShiftBrightness(1.6f), Color.Red.ShiftBrightness(0.2f));
                r.BackGroundColors[(int)VisualRoleType.text_edit_widget] = new ElementColors(Color.Black.ShiftBrightness(1.15f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
                r.TextColors[(int)VisualRoleType.text_edit_widget] = new ElementColors(Color.White.ShiftBrightness(0.75f), Color.White, Color.White);

                // header_widget theme
                r.OutlineColors[(int)VisualRoleType.header_widget] = new ElementColors(Color.Black.ShiftBrightness(1.2f), Color.Black.ShiftBrightness(1.5f), Color.Red.ShiftBrightness(0.2f));
                r.BackGroundColors[(int)VisualRoleType.header_widget] = new ElementColors(Color.Black.ShiftBrightness(1.3f), Color.Red.ShiftBrightness(0.4f), Color.Black.ShiftBrightness(1.15f));
                r.TextColors[(int)VisualRoleType.header_widget] = new ElementColors(Color.Black, Color.White.ShiftBrightness(0.75f), Color.White);

                r.SideOuterScrollBar = new ElementColors(new Color(0, 0, 0, 0), new Color(0, 0, 0, 70), new Color(0, 0, 0, 0));
                r.SideInnerScrollBar = new ElementColors(new Color(100, 100, 100, 100), new Color(200, 200, 200, 100), new Color(255, 255, 255, 255));

                r.BottomInnerScrollBar = (ElementColors)r.SideInnerScrollBar.Clone();
                r.BottomOuterScrollBar = (ElementColors)r.SideOuterScrollBar.Clone();

                r.SetEmptyElementColors(VisualRoleType.default_widget);

                return r;
            }
        }

        public void SetEmptyElementColors(VisualRoleType replacement)
        {
            SetEmptyElementColors(OutlineColors[(int)replacement], BackGroundColors[(int)replacement], TextColors[(int)replacement]);
        }

        public void SetEmptyElementColors(ElementColors outline, ElementColors background, ElementColors text)
        {
            foreach (var enum_ in Enum.GetValues(typeof(VisualRoleType)))
            {
                if (BackGroundColors[(int)(VisualRoleType)enum_] == null) BackGroundColors[(int)(VisualRoleType)enum_] = (ElementColors)background.Clone();
                if (OutlineColors[(int)(VisualRoleType)enum_] == null) OutlineColors[(int)(VisualRoleType)enum_] = (ElementColors)outline.Clone();
                if (TextColors[(int)(VisualRoleType)enum_] == null) TextColors[(int)(VisualRoleType)enum_] = (ElementColors)text.Clone();
            }
        }
    }
}
