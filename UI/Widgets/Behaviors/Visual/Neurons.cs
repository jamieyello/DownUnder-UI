using DownUnder.UI.Widgets.Behaviors.Visual.NeuronsObjects;
using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class Neurons : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };
        Texture2D circle_t;
        ProxArrayContainer<NeuronsCircle>
        
        protected override void Initialize()
        {
            if (Parent.ParentWindow != null) LoadContent(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += LoadContent;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
            Parent.OnClick += AddCircle;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= LoadContent;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
        }

        void LoadContent(object sender, EventArgs args)
        {
            if (circle_t == null) circle_t = Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Tiny Circle");
        }

        void Update(object sender, EventArgs args)
        {
            foreach (var circle in circles) circle.Update();
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            foreach (var circle in circles) circle.Draw(circle_t, args);
        }

        void AddCircle(object sender, EventArgs args)
        {
            for (int i = 0; i < 500; i++) circles.Add(NeuronsCircle.RandomCircle(Parent.Area.SizeOnly(), 10));
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
