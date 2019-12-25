using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    // [Attribute] to describe the fact that the editor should represent this object
    // visually with a custom window.
    [DataContract]
    public class UIPalette
    {
        [DataMember] private ChangingValue<Color> _changing_color = new ChangingValue<Color>();
        private bool _special_color_enabled_backing = false;

        [DataMember] public Color DefaultColor { get; set; } = Color.Black;
        [DataMember] public Color HoveredColor { get; set; } = Color.White;
        [DataMember] public Color SpecialColor { get; set; } = Color.Black;
        [DataMember] public InterpolationType Interpolation { get => _changing_color.Interpolation; set => _changing_color.Interpolation = value; }
        [DataMember] public float TransitionSpeed { get => _changing_color.TransitionSpeed; set => _changing_color.TransitionSpeed = value; }
        public bool Hovered { get; set;}

        public Color CurrentColor { get => _changing_color.GetCurrent(); }
        public bool IsTransitioning { get => _changing_color.IsTransitioning; }

        public bool SpecialColorEnabled
        {
            get => _special_color_enabled_backing;
            set
            {
                _special_color_enabled_backing = value;
                if (value)
                {
                    _changing_color.SetTargetValue(SpecialColor, true);
                }
            }
        }

        #region Constructors

        public UIPalette()
        {
        }

        public UIPalette(Color solid_color)
        {
            DefaultColor = solid_color;
            HoveredColor = solid_color;
        }

        public UIPalette(Color default_color, Color hovered_color)
        {
            DefaultColor = default_color;
            HoveredColor = hovered_color;
        }

        public UIPalette(Color default_color, Color hovered_color, Color special_color)
        {
            DefaultColor = default_color;
            HoveredColor = hovered_color;
            SpecialColor = special_color;
        }

        #endregion Constructors

        public void ForceComplete() => Update(1f);

        public void Update(float step)
        {
            if (SpecialColorEnabled)
            {
                _changing_color.SetTargetValue(SpecialColor);
            }
            else
            {
                if (Hovered) _changing_color.SetTargetValue(HoveredColor);
                else _changing_color.SetTargetValue(DefaultColor);

            }
            
            _changing_color.Update(step);
        }
        

        public void SetSolidColor(Color color)
        {
            DefaultColor = color;
            HoveredColor = color;
        }

        public object Clone()
        {
            UIPalette result = new UIPalette();
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