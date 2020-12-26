using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class MouseGlow : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_MID_PERFORMANCE };
        Texture2D _circle;
        Point _position = new Point(0, 0);
        bool _follow = false;
        readonly ChangingValue<Color> _draw_color = new ChangingValue<Color>(Color.Transparent, InterpolationSettings.Default);

        public enum MouseGlowActivationPolicy
        {
            /// <summary> Will always glow. </summary>
            always_active,
            /// <summary> Will never glow. </summary>
            disabled,
            /// <summary> Will glow if the cursor is anywhere above the <see cref="Widget"/>'s area. </summary>
            hovered_over,
            /// <summary> (Default) Will only glow when this is the primary hovered widget. </summary>
            primary_hovered
        }

        public Color Color = new Color(100,100,100, 255);
        public int Diameter { get; set; } = 1024;
        public InterpolationSettings ShineSpeed = InterpolationSettings.Faster;
        public InterpolationSettings ShineOffSpeed = InterpolationSettings.Default;
        public MouseGlowActivationPolicy ActivationPolicy = MouseGlowActivationPolicy.primary_hovered;

        protected override void Initialize()
        {
            if (Parent.ParentDWindow != null) LoadImage(Parent, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += LoadImage;
            Parent.OnDrawBackground += DrawImage;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= LoadImage;
            Parent.OnDrawBackground -= DrawImage;
            Parent.OnUpdate -= Update;
        }

        public override object Clone()
        {
            MouseGlow c = new MouseGlow();
            c.Color = Color;
            c.Diameter = Diameter;
            c.ShineSpeed = ShineSpeed;
            c.ShineOffSpeed = ShineOffSpeed;
            c.ActivationPolicy = ActivationPolicy;
            return c;
        }

        void LoadImage(object sender, EventArgs args)
        {
            if (_circle == null) _circle = Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Blurred Circle 512");
        }

        void Update(object sender, EventArgs args)
        {
            if ((ActivationPolicy == MouseGlowActivationPolicy.primary_hovered && Parent.IsPrimaryHovered)
                || (ActivationPolicy == MouseGlowActivationPolicy.hovered_over && Parent.IsHoveredOver)
                || ActivationPolicy == MouseGlowActivationPolicy.always_active)
            {
                _follow = true;
                _draw_color.InterpolationSettings = ShineSpeed;
                _draw_color.SetTargetValue(Color);
            }
            else
            {
                _follow = false;
                _draw_color.InterpolationSettings = ShineOffSpeed;
                _draw_color.SetTargetValue(Color.Transparent);
            }
            _draw_color.Update(Parent.UpdateData.ElapsedSeconds);
        }

        void DrawImage(object sender, WidgetDrawArgs args)
        {
            if (_follow) _position = new Point((int)args.CursorPosition.X - Diameter / 2, (int)args.CursorPosition.Y - Diameter / 2);
            if (_draw_color.Current != Color.Transparent) args.SpriteBatch.Draw(_circle, new Rectangle(_position.X, _position.Y, Diameter, Diameter), _draw_color.Current);
        }

        public static MouseGlow SubtleGray => new MouseGlow() { Diameter = 8192 * 2, Color = new Color(30, 30, 30, 30) };
    }
}
