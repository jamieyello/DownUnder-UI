using DownUnder.UI.Widgets.DataTypes.AnimatedGraphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    class DrawGraphic : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        public AnimatedGraphic Graphic;

        public DrawGraphic() { }
        public DrawGraphic(AnimatedGraphic graphic)
        {
            Graphic = graphic;
        }

        protected override void Initialize()
        {
            if (Parent.IsGraphicsInitialized) Initialize(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnGraphicsInitialized += Initialize;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnGraphicsInitialized -= Initialize;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
        }

        public override object Clone()
        {
            var result = new DrawGraphic();
            result.Graphic = (AnimatedGraphic)Graphic.Clone();
            return result;
        }

        void Initialize(object sender, EventArgs args)
        {
            Graphic.InitializeExternal(Parent.GraphicsDevice);
        }

        void Update(object sender, EventArgs args)
        {
            Graphic.UpdateExternal(Parent.UpdateData.ElapsedSeconds);
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            Graphic.DrawExternal(args);
        }
    }
}
