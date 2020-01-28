using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    [DataContract]
    public class BaseColorScheme
    {
        /// <summary> Defines the behavior of the theme when updating as well as what colors a widget accesses in the theme. </summary>
        public enum PaletteCatagory : int
        {
            default_ = 0,
            text_edit = 1,
            header = 2
        }

        [DataMember] public List<ElementColors> BackGrounds { get; set; } = new List<ElementColors>();
        [DataMember] public List<ElementColors> Texts { get; set; } = new List<ElementColors>();
        [DataMember] public List<ElementColors> Outlines { get; set; } = new List<ElementColors>();

        public BaseColorScheme()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            foreach (PaletteCatagory catagory in Enum.GetValues(typeof(PaletteCatagory)))
            {
                BackGrounds.Add(new ElementColors());
                Texts.Add(new ElementColors());
                Outlines.Add(new ElementColors());
            }
        }

        public ElementColors GetBackground(Widget parent)
        {
            return GetBackground(parent.PaletteUsage);
        }

        public ElementColors GetBackground(PaletteCatagory catagory)
        {
            return BackGrounds[(int)catagory];
        }

        public ElementColors GetText(Widget parent)
        {
            return GetText(parent.PaletteUsage);
        }

        public ElementColors GetText(PaletteCatagory catagory)
        {
            return Texts[(int)catagory];
        }

        public ElementColors GetOutline(Widget parent)
        {
            return GetOutline(parent.PaletteUsage);
        }

        public ElementColors GetOutline(PaletteCatagory catagory)
        {
            return Outlines[(int)catagory];
        }

        public void Update(Widget parent, GameTime game_time)
        {
            GetBackground(parent.PaletteUsage).Update(game_time.GetElapsedSeconds());
            GetText(parent.PaletteUsage).Update(game_time.GetElapsedSeconds());
            GetOutline(parent.PaletteUsage).Update(game_time.GetElapsedSeconds());

            if (parent.ChangeColorOnMouseOver)
            {
                SetHovered(parent, parent.IsPrimaryHovered);
            }

            switch (parent.PaletteUsage)
            {
                case PaletteCatagory.default_:
                    break;

                case PaletteCatagory.header:
                    break;

                case PaletteCatagory.text_edit:
                    break;

                default:
                    break;
            }
        }

        private void SetHovered(Widget parent, bool hovered)
        {
            GetBackground(parent.PaletteUsage).Hovered = hovered;
            GetText(parent.PaletteUsage).Hovered = hovered;
            GetOutline(parent.PaletteUsage).Hovered = hovered;
        }

        private void ForceComplete()
        {
            foreach (PaletteCatagory catagory in Enum.GetValues(typeof(PaletteCatagory)))
            {
                BackGrounds[(int)catagory].ForceComplete();
                Texts[(int)catagory].ForceComplete();
                Outlines[(int)catagory].ForceComplete();
            }
        }

        public object Clone()
        {
            BaseColorScheme c = new BaseColorScheme();

            foreach (PaletteCatagory catagory in Enum.GetValues(typeof(PaletteCatagory)))
            {
                c.BackGrounds.Add((ElementColors)BackGrounds[(int)catagory].Clone());
                c.Texts.Add((ElementColors)Texts[(int)catagory].Clone());
                c.Outlines.Add((ElementColors)Outlines[(int)catagory].Clone());
            }

            return c;
        }

        public static BaseColorScheme Default
        {
            get
            {
                BaseColorScheme r = new BaseColorScheme();

                // default_ theme
                r.GetOutline(PaletteCatagory.default_).DefaultColor = Color.Black;
                r.GetOutline(PaletteCatagory.default_).SpecialColor = Color.Black;
                r.GetOutline(PaletteCatagory.default_).HoveredColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(PaletteCatagory.default_).DefaultColor = Color.CornflowerBlue;
                r.GetBackground(PaletteCatagory.default_).SpecialColor = Color.Yellow;
                r.GetBackground(PaletteCatagory.default_).HoveredColor = Color.CornflowerBlue.ShiftBrightness(1.1f);
                r.GetText(PaletteCatagory.default_).DefaultColor = Color.Black;
                r.GetText(PaletteCatagory.default_).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.default_).HoveredColor = Color.Black;

                // text_edit theme
                r.GetOutline(PaletteCatagory.text_edit).DefaultColor = Color.Black;
                r.GetOutline(PaletteCatagory.text_edit).SpecialColor = Color.Black;
                r.GetOutline(PaletteCatagory.text_edit).HoveredColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(PaletteCatagory.text_edit).DefaultColor = Color.White;
                r.GetBackground(PaletteCatagory.text_edit).SpecialColor = Color.White;
                r.GetBackground(PaletteCatagory.text_edit).HoveredColor = Color.White;
                r.GetText(PaletteCatagory.text_edit).DefaultColor = Color.Black;
                r.GetText(PaletteCatagory.text_edit).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.text_edit).HoveredColor = Color.Black;

                // header theme
                r.GetOutline(PaletteCatagory.header).DefaultColor = Color.Black.ShiftBrightness(1.3f);
                r.GetOutline(PaletteCatagory.header).SpecialColor = Color.Black;
                r.GetOutline(PaletteCatagory.header).HoveredColor = Color.Black.ShiftBrightness(1.35f);
                r.GetBackground(PaletteCatagory.header).DefaultColor = Color.Black.ShiftBrightness(1.1f);
                r.GetBackground(PaletteCatagory.header).SpecialColor = Color.Blue.ShiftBrightness(0.5f);
                r.GetBackground(PaletteCatagory.header).HoveredColor = Color.Black.ShiftBrightness(1.2f);
                r.GetText(PaletteCatagory.header).DefaultColor = Color.White.ShiftBrightness(0.8f);
                r.GetText(PaletteCatagory.header).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.header).HoveredColor = Color.White;

                r.ForceComplete();

                return r;
            }
        }
        public bool crash = true;
        public static BaseColorScheme Test
        {
            get
            {
                BaseColorScheme r = new BaseColorScheme();
                r.crash = false;

                // default_ theme
                r.GetOutline(PaletteCatagory.default_).DefaultColor = Color.White;
                r.GetOutline(PaletteCatagory.default_).SpecialColor = Color.White;
                r.GetOutline(PaletteCatagory.default_).HoveredColor = Color.White;
                r.GetBackground(PaletteCatagory.default_).DefaultColor = Color.White;
                r.GetBackground(PaletteCatagory.default_).SpecialColor = Color.White;
                r.GetBackground(PaletteCatagory.default_).HoveredColor = Color.White;
                r.GetText(PaletteCatagory.default_).DefaultColor = Color.White;
                r.GetText(PaletteCatagory.default_).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.default_).HoveredColor = Color.White;

                // text_edit theme
                r.GetOutline(PaletteCatagory.text_edit).DefaultColor = Color.White;
                r.GetOutline(PaletteCatagory.text_edit).SpecialColor = Color.White;
                r.GetOutline(PaletteCatagory.text_edit).HoveredColor = Color.White;
                r.GetBackground(PaletteCatagory.text_edit).DefaultColor = Color.White;
                r.GetBackground(PaletteCatagory.text_edit).SpecialColor = Color.White;
                r.GetBackground(PaletteCatagory.text_edit).HoveredColor = Color.White;
                r.GetText(PaletteCatagory.text_edit).DefaultColor = Color.White;
                r.GetText(PaletteCatagory.text_edit).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.text_edit).HoveredColor = Color.White;

                // header theme
                r.GetOutline(PaletteCatagory.header).DefaultColor = Color.White;
                r.GetOutline(PaletteCatagory.header).SpecialColor = Color.White;
                r.GetOutline(PaletteCatagory.header).HoveredColor = Color.White;
                r.GetBackground(PaletteCatagory.header).DefaultColor = Color.White;
                r.GetBackground(PaletteCatagory.header).SpecialColor = Color.White;
                r.GetBackground(PaletteCatagory.header).HoveredColor = Color.White;
                r.GetText(PaletteCatagory.header).DefaultColor = Color.White;
                r.GetText(PaletteCatagory.header).SpecialColor = Color.White;
                r.GetText(PaletteCatagory.header).HoveredColor = Color.White;

                r.crash = true;
                return r;
            }
        }
    }
}
