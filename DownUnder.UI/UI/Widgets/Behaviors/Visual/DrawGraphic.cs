using System;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.UI.Widgets.DataTypes.AnimatedGraphics;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class DrawSwitchGraphic : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };
        bool IsToggled;

        public SwitchingGraphic Graphic;

        public DrawSwitchGraphic() { }

        public DrawSwitchGraphic(SwitchingGraphic graphic) =>
            Graphic = graphic;

        protected override void Initialize() {
            if (!Parent.IsGraphicsInitialized)
                return;

            Initialize(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnGraphicsInitialized += Initialize;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
            Parent.OnClick += ToggleAnimation;
        }

        protected override void DisconnectEvents() {
            Parent.OnGraphicsInitialized -= Initialize;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
            Parent.OnClick -= ToggleAnimation;
        }

        public override object Clone() =>
            new DrawSwitchGraphic {
                Graphic = Graphic.Clone()
            };

        void Initialize(object sender, EventArgs args) =>
            Graphic.Initialize(Parent.GraphicsDevice);

        void Update(object sender, EventArgs args) =>
            Graphic.Update(Parent.UpdateData.ElapsedSeconds);

        void Draw(object sender, WidgetDrawArgs args) =>
            Graphic.Draw(args);

        void ToggleAnimation(object sender, EventArgs args) {
            IsToggled = !IsToggled;
            Graphic.Progress.SetTargetValue(IsToggled ? 1f : 0f);
        }
    }
}