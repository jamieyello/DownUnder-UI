using System;
using System.Runtime.Serialization;
using DownUnder.UI;
using Microsoft.Xna.Framework;
using static DownUnder.GeneralVisualSettings;
using static DownUnder.GeneralVisualSettings.VisualRoleType;

namespace DownUnder {
    [DataContract]
    public sealed class BaseColorScheme : ICloneable {
        /// <summary> A reference to the widget that owns this palette. </summary>
        [DataMember] internal ElementColors[] BackGroundColors { get; set; }
        [DataMember] internal ElementColors[] TextColors { get; set; }
        [DataMember] internal ElementColors[] OutlineColors { get; set; }
        [DataMember] public ElementColors SideInnerScrollBar { get; set; }
        [DataMember] public ElementColors SideOuterScrollBar { get; set; }
        [DataMember] public ElementColors BottomInnerScrollBar { get; set; }
        [DataMember] public ElementColors BottomOuterScrollBar { get; set; }

        public BaseColorScheme()=> SetDefaults();

        void SetDefaults() {
            var length = Enum.GetValues(typeof(VisualRoleType)).Length;

            BackGroundColors = new ElementColors[length];
            TextColors = new ElementColors[length];
            OutlineColors = new ElementColors[length];
        }

        public ElementColors GetBackground(VisualRoleType category) => BackGroundColors[(int)category];
        public ElementColors GetText(VisualRoleType category) => TextColors[(int)category];
        public ElementColors GetOutline(VisualRoleType category) => OutlineColors[(int)category];

        public void SetBackground(VisualRoleType category, ElementColors colors) => BackGroundColors[(int)category] = colors;
        public void SetText(VisualRoleType category, ElementColors colors) => TextColors[(int)category] = colors;
        public void SetOutline(VisualRoleType category, ElementColors colors) => OutlineColors[(int)category] = colors;

        public void Update(
            VisualRoleType profile,
            float step,
            bool change_color_on_hover,
            bool is_hovered
        ) {
            GetBackground(profile).Update(step);
            GetText(profile).Update(step);
            GetOutline(profile).Update(step);

            if (change_color_on_hover)
                SetHovered(profile, is_hovered);
        }

        void SetHovered(VisualRoleType profile, bool hovered) {
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
            var c = new BaseColorScheme();

            var length = Enum.GetValues(typeof(VisualRoleType)).Length;
            for (var i = 0; i < length; i++) {
                c.BackGroundColors[i] = BackGroundColors[i]?.Clone();
                c.TextColors[i] = TextColors[i]?.Clone();
                c.OutlineColors[i] = OutlineColors[i]?.Clone();
            }

            c.SideInnerScrollBar = SideInnerScrollBar?.Clone();
            c.SideOuterScrollBar = SideOuterScrollBar?.Clone();
            c.BottomInnerScrollBar = BottomInnerScrollBar?.Clone();
            c.BottomOuterScrollBar = BottomOuterScrollBar?.Clone();

            return c;
        }

        // TODO: these instantiate on every read
        /// <summary> The basic MonoGame-esque CornFlowerBlue theme. </summary>
        static BaseColorScheme Default { get {
            var r = new BaseColorScheme();

            // default_widget theme
            r.GetOutline(default_widget).DefaultColor = Color.Black;
            r.GetOutline(default_widget).SpecialColor = Color.Black;
            r.GetOutline(default_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
            r.GetBackground(default_widget).DefaultColor = Color.CornflowerBlue;
            r.GetBackground(default_widget).SpecialColor = Color.Yellow;
            r.GetBackground(default_widget).HoveredColor = Color.CornflowerBlue.ShiftBrightness(1.1f);
            r.GetText(default_widget).DefaultColor = Color.Black;
            r.GetText(default_widget).SpecialColor = Color.White;
            r.GetText(default_widget).HoveredColor = Color.Black;

            // text_widget theme
            r.GetOutline(text_widget).DefaultColor = Color.Black;
            r.GetOutline(text_widget).SpecialColor = Color.Black;
            r.GetOutline(text_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
            r.GetBackground(text_widget).DefaultColor = Color.White.ShiftBrightness(0.8f);
            r.GetBackground(text_widget).SpecialColor = Color.White;
            r.GetBackground(text_widget).HoveredColor = Color.White;
            r.GetText(text_widget).DefaultColor = Color.Black;
            r.GetText(text_widget).SpecialColor = Color.White;
            r.GetText(text_widget).HoveredColor = Color.Black;

            // text_edit_widget theme
            r.GetOutline(text_edit_widget).DefaultColor = Color.Black;
            r.GetOutline(text_edit_widget).SpecialColor = Color.Black;
            r.GetOutline(text_edit_widget).HoveredColor = Color.Black.ShiftBrightness(1.1f);
            r.GetBackground(text_edit_widget).DefaultColor = Color.White;
            r.GetBackground(text_edit_widget).SpecialColor = Color.White;
            r.GetBackground(text_edit_widget).HoveredColor = Color.White;
            r.GetText(text_edit_widget).DefaultColor = Color.Black;
            r.GetText(text_edit_widget).SpecialColor = Color.White;
            r.GetText(text_edit_widget).HoveredColor = Color.Black;

            // header_widget theme
            r.GetOutline(header_widget).DefaultColor = Color.Black.ShiftBrightness(1.3f);
            r.GetOutline(header_widget).SpecialColor = Color.Black;
            r.GetOutline(header_widget).HoveredColor = Color.Black.ShiftBrightness(1.35f);
            r.GetBackground(header_widget).DefaultColor = Color.Black.ShiftBrightness(1.1f);
            r.GetBackground(header_widget).SpecialColor = Color.Blue.ShiftBrightness(0.5f);
            r.GetBackground(header_widget).HoveredColor = Color.Black.ShiftBrightness(1.2f);
            r.GetText(header_widget).DefaultColor = Color.White.ShiftBrightness(0.8f);
            r.GetText(header_widget).SpecialColor = Color.White;
            r.GetText(header_widget).HoveredColor = Color.White;

            return r;
        } }

        /// <summary> A black theme. </summary>
        public static BaseColorScheme Dark { get {
            var r = new BaseColorScheme();

            // default_widget theme
            r.OutlineColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.Black.ShiftBrightness(1.35f), Color.Black.ShiftBrightness(1.6f), Color.Red.ShiftBrightness(0.2f));
            r.BackGroundColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.Black.ShiftBrightness(1.07f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
            r.TextColors[(int)VisualRoleType.default_widget] = new ElementColors(Color.White.ShiftBrightness(0.75f), Color.White, Color.White);

            // text_widget theme
            r.OutlineColors[(int)VisualRoleType.text_widget] = new ElementColors(Color.Black.ShiftBrightness(1.35f), Color.Black.ShiftBrightness(1.5f), Color.Red.ShiftBrightness(0.2f));
            r.BackGroundColors[(int)VisualRoleType.text_widget] = new ElementColors(Color.Black.ShiftBrightness(1.1f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
            r.TextColors[(int)text_widget] = new ElementColors(Color.White, Color.White.ShiftBrightness(0.75f), Color.White);

            // text_edit_widget theme
            r.OutlineColors[(int)text_edit_widget] = new ElementColors(Color.Black.ShiftBrightness(1.6f), Color.Black.ShiftBrightness(1.6f), Color.Red.ShiftBrightness(0.2f));
            r.BackGroundColors[(int)text_edit_widget] = new ElementColors(Color.Black.ShiftBrightness(1.15f), Color.Black.ShiftBrightness(1.15f), Color.Red.ShiftBrightness(0.4f));
            r.TextColors[(int)text_edit_widget] = new ElementColors(Color.White.ShiftBrightness(0.75f), Color.White, Color.White);

            // header_widget theme
            r.OutlineColors[(int)header_widget] = new ElementColors(Color.Black.ShiftBrightness(1.2f), Color.Black.ShiftBrightness(1.5f), Color.Red.ShiftBrightness(0.2f));
            r.BackGroundColors[(int)header_widget] = new ElementColors(Color.Black.ShiftBrightness(1.3f), Color.Red.ShiftBrightness(0.4f), Color.Black.ShiftBrightness(1.15f));
            r.TextColors[(int)header_widget] = new ElementColors(Color.Black, Color.White.ShiftBrightness(0.75f), Color.White);

            r.SideOuterScrollBar = new ElementColors(new Color(0, 0, 0, 0), new Color(0, 0, 0, 70), new Color(0, 0, 0, 0));
            r.SideInnerScrollBar = new ElementColors(new Color(100, 100, 100, 100), new Color(200, 200, 200, 100), new Color(255, 255, 255, 255));

            r.BottomInnerScrollBar = r.SideInnerScrollBar.Clone();
            r.BottomOuterScrollBar = r.SideOuterScrollBar.Clone();

            r.SetEmptyElementColors(default_widget);

            return r;
        } }

        public void SetEmptyElementColors(VisualRoleType replacement) =>
            SetEmptyElementColors(
                OutlineColors[(int)replacement],
                BackGroundColors[(int)replacement],
                TextColors[(int)replacement]
            );

        public void SetEmptyElementColors(
            ElementColors outline,
            ElementColors background,
            ElementColors text
        ) {
            // TODO: here, enum values are cast to ints
            // TODO: in other places, you count from 0 to length - 1
            // TODO: those are not the same thing
            foreach (var enum_ in Enum.GetValues(typeof(VisualRoleType))) {
                var i = (int)enum_;
                BackGroundColors[i] ??= background.Clone();
                OutlineColors[i] ??= outline.Clone();
                TextColors[i] ??= text.Clone();
            }
        }
    }
}
