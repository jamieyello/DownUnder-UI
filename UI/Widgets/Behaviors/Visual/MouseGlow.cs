using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class MouseGlow : WidgetBehavior
    {
        public override string[] BehaviorIDs { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        Texture2D _circle;
        Point _position = new Point(0, 0);
        bool _follow = false;
        readonly ChangingValue<Color> _draw_color = new ChangingValue<Color>(Color.Transparent, Utilities.InterpolationSettings.Default);

        Color Color = new Color(100,100,100, 255);
        int Diameter { get; set; } = 1024;

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += LoadImage;
            Parent.OnDraw += DrawImage;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= LoadImage;
            Parent.OnDraw -= DrawImage;
            Parent.OnUpdate -= Update;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        void LoadImage(object sender, EventArgs args)
        {
            _circle = Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Blurred Circle 512");
        }

        void Update(object sender, EventArgs args)
        {
            if (Parent.IsPrimaryHovered)
            {
                _follow = true;
                _draw_color.SetTargetValue(Color);
            }
            else
            {
                _follow = false;
                _draw_color.SetTargetValue(Color.Transparent);
            }
            _draw_color.Update(Parent.UpdateData.ElapsedSeconds);
        }

        void DrawImage(object sender, WDrawEventArgs args)
        {
            if (_follow) _position = new Point((int)args.CursorPosition.X - Diameter / 2, (int)args.CursorPosition.Y - Diameter / 2);
            if (_draw_color.GetCurrent() != Color.Transparent) args.SpriteBatch.Draw(_circle, new Rectangle(_position.X, _position.Y, Diameter, Diameter), _draw_color.GetCurrent());
        }
    }
}
