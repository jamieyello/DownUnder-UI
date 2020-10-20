using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using static DownUnder.UI.Widgets.DataTypes.GeneralVisualSettings;
using static DownUnder.UI.Widgets.Widget;

namespace DownUnder.UI.Widgets.DataTypes {
    [DataContract] public class BaseColorScheme : ICloneable {
        /// <summary> A reference to the widget that owns this palette. </summary>
        [DataMember] public List<ElementColors> BackGroundColors { get; set; } = new List<ElementColors>();
        [DataMember] public List<ElementColors> TextColors { get; set; } = new List<ElementColors>();
        [DataMember] public List<ElementColors> OutlineColors { get; set; } = new List<ElementColors>();
        [DataMember] public ElementColors SideInnerScrollBar { get; set; } = new ElementColors();
        [DataMember] public ElementColors SideOuterScrollBar { get; set; } = new ElementColors();
        [DataMember] public ElementColors BottomInnerScrollBar { get; set; } = new ElementColors();
        [DataMember] public ElementColors BottomOuterScrollBar { get; set; } = new ElementColors();

        public BaseColorScheme()=> SetDefaults();
        
        private void SetDefaults() {
            foreach (VisualRoleType _ in Enum.GetValues(typeof(VisualRoleType))) {
                BackGroundColors.Add(new ElementColors(Color.LimeGreen));
                TextColors.Add(new ElementColors());
                OutlineColors.Add(new ElementColors());
            }
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
            c.BackGroundColors.Clear();
            c.OutlineColors.Clear();
            c.TextColors.Clear();

            foreach (VisualRoleType category in Enum.GetValues(typeof(VisualRoleType))) {
                c.BackGroundColors.Add((ElementColors)BackGroundColors[(int)category].Clone());
                c.TextColors.Add((ElementColors)TextColors[(int)category].Clone());
                c.OutlineColors.Add((ElementColors)OutlineColors[(int)category].Clone());
            }

            c.SideInnerScrollBar = (ElementColors)SideInnerScrollBar.Clone();
            c.SideOuterScrollBar = (ElementColors)SideOuterScrollBar.Clone();
            c.BottomInnerScrollBar = (ElementColors)BottomInnerScrollBar.Clone();
            c.BottomOuterScrollBar = (ElementColors)BottomOuterScrollBar.Clone();

            return c;
        }

        /// <summary> The basic MonoGame-esque CornFlowerBlue theme. </summary>
        public static BaseColorScheme Default {
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
                ElementColors e = r.GetOutline(VisualRoleType.default_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.35f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.2f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.6f);
                e = r.GetBackground(VisualRoleType.default_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.07f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.4f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.15f);
                e = r.GetText(VisualRoleType.default_widget);
                e.DefaultColor = Color.White.ShiftBrightness(0.75f);
                e.SpecialColor = Color.White;
                e.HoveredColor = Color.White;

                // text_widget theme
                e = r.GetOutline(VisualRoleType.text_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.35f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.2f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.5f);
                e = r.GetBackground(VisualRoleType.text_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.1f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.4f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.15f);
                e = r.GetText(VisualRoleType.text_widget);
                e.DefaultColor = Color.White.ShiftBrightness(0.75f);
                e.SpecialColor = Color.White;
                e.HoveredColor = Color.White;

                // text_edit_widget theme
                e = r.GetOutline(VisualRoleType.text_edit_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.6f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.2f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.6f);
                e = r.GetBackground(VisualRoleType.text_edit_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.15f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.4f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.15f);
                e = r.GetText(VisualRoleType.text_edit_widget);
                e.DefaultColor = Color.White.ShiftBrightness(0.75f);
                e.SpecialColor = Color.White;
                e.HoveredColor = Color.White;

                // header_widget theme
                e = r.GetOutline(VisualRoleType.header_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.2f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.2f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.5f);
                e = r.GetBackground(VisualRoleType.header_widget);
                e.DefaultColor = Color.Black.ShiftBrightness(1.3f);
                e.SpecialColor = Color.Red.ShiftBrightness(0.4f);
                e.HoveredColor = Color.Black.ShiftBrightness(1.15f);
                e = r.GetText(VisualRoleType.header_widget);
                e.DefaultColor = Color.Black;
                e.SpecialColor = Color.White;
                e.HoveredColor = Color.White.ShiftBrightness(0.75f);

                r.SideOuterScrollBar.DefaultColor = new Color(0, 0, 0, 0);
                r.SideOuterScrollBar.SpecialColor = new Color(0, 0, 0, 0);
                r.SideOuterScrollBar.HoveredColor = new Color(0, 0, 0, 70);

                r.SideInnerScrollBar.DefaultColor = new Color(100, 100, 100, 100);
                r.SideInnerScrollBar.SpecialColor = new Color(255, 255, 255, 255);
                r.SideInnerScrollBar.HoveredColor = new Color(200, 200, 200, 100);

                r.BottomInnerScrollBar = (ElementColors)r.SideInnerScrollBar.Clone();
                r.BottomOuterScrollBar = (ElementColors)r.SideOuterScrollBar.Clone();

                ElementColors default_colors = r.GetBackground(VisualRoleType.default_widget);
                r.SetEmptyElementColors(default_colors);
                
                return r;
            }
        }

        public void SetEmptyElementColors(ElementColors default_colors)
        {
            foreach (var enum_ in Enum.GetValues(typeof(VisualRoleType)))
            {
                if (BackGroundColors[(int)(VisualRoleType)enum_] == null) BackGroundColors[(int)(VisualRoleType)enum_] = (ElementColors)default_colors.Clone();
                if (OutlineColors[(int)(VisualRoleType)enum_] == null) OutlineColors[(int)(VisualRoleType)enum_] = (ElementColors)default_colors.Clone();
                if (TextColors[(int)(VisualRoleType)enum_] == null) TextColors[(int)(VisualRoleType)enum_] = (ElementColors)default_colors.Clone();
            }

            SideInnerScrollBar = SideInnerScrollBar ?? (ElementColors)default_colors.Clone();
            SideOuterScrollBar = SideOuterScrollBar ?? (ElementColors)default_colors.Clone();
            BottomInnerScrollBar = BottomInnerScrollBar ?? (ElementColors)default_colors.Clone();
            BottomOuterScrollBar = BottomOuterScrollBar ?? (ElementColors)default_colors.Clone();
        }
    }
}
