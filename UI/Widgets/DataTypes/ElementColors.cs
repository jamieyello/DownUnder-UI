using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes {
    /// <summary> The color settings of any UI element. </summary>
    [DataContract] public class ElementColors {
        [DataMember] private ChangingValue<Color> _changing_color = new ChangingValue<Color>() { TransitionSpeed = 4f };
        private bool _special_color_enabled_backing = false;

        /// <summary> Color to shift to when nothing of interest is happening. </summary>
        [DataMember] public Color DefaultColor { get; set; } = Color.Black;
        /// <summary> Color to shift to when this element is hovered over. </summary>
        [DataMember] public Color HoveredColor { get; set; } = Color.White;
        /// <summary> Color to toggle when SpecialColorEnabled is enabled. No transition. </summary>
        [DataMember] public Color SpecialColor { get; set; } = Color.Black;
        /// <summary> Whether this element currently shifts towards the hovered color or the default color. </summary>
        public bool Hovered { get; set;}

        /// <summary> The type of interpolation to be used when shifting between colors. </summary>
        public InterpolationType Interpolation { get => _changing_color.Interpolation; set => _changing_color.Interpolation = value; }
        /// <summary> How fast colors should shift. (1 = immediate) </summary>
        public float TransitionSpeed { get => _changing_color.TransitionSpeed; set => _changing_color.TransitionSpeed = value; }
        /// <summary> The color that this element should display at this moment. </summary>
        public Color CurrentColor { get => _changing_color.GetCurrent(); }
        /// <summary> Returns true if this is in the middle of transitioning to a different color. </summary>
        public bool IsTransitioning { get => _changing_color.IsTransitioning; }

        /// <summary> When set to true this will display the special color. </summary>
        public bool SpecialColorEnabled {
            get => _special_color_enabled_backing;
            set {
                _special_color_enabled_backing = value;
                if (value) _changing_color.SetTargetValue(SpecialColor, true);
            }
        }

        public ElementColors() {}

        public ElementColors(Color solid_color) {
            DefaultColor = solid_color;
            HoveredColor = solid_color;
        }

        public ElementColors(Color default_color, Color hovered_color) {
            DefaultColor = default_color;
            HoveredColor = hovered_color;
        }

        public ElementColors(Color default_color, Color hovered_color, Color special_color) {
            DefaultColor = default_color;
            HoveredColor = hovered_color;
            SpecialColor = special_color;
        }

        /// <summary> Force the animation to complete immediately. </summary>
        public void ForceComplete() => Update(1f);

        public void Update(float step) {
            if (SpecialColorEnabled) _changing_color.SetTargetValue(SpecialColor);
            else {
                if (Hovered) _changing_color.SetTargetValue(HoveredColor);
                else _changing_color.SetTargetValue(DefaultColor);
            }
            
            _changing_color.Update(step);
        }
        
        /// <summary> Set the default and the hovered color to one solid color. </summary>
        public void SetSolidColor(Color color) {
            DefaultColor = color;
            HoveredColor = color;
        }

        public object Clone() {
            ElementColors result = new ElementColors();
            result._changing_color = (ChangingValue<Color>)_changing_color.Clone();
            result.DefaultColor = DefaultColor;
            result.HoveredColor = HoveredColor;
            result.SpecialColor = SpecialColor;
            result.Interpolation = Interpolation;
            result.TransitionSpeed = TransitionSpeed;
            return result;
        }
    }
}