using DownUnder.UI.Widgets.Actions;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class PopInOut : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        [DataMember] public bool DeleteOnClickOff { get; set; } = true;
        [DataMember] Directions2D StartSides { get; set; } = Directions2D.U;
        [DataMember] public float StartingPercentage { get; set; } = -1f;
        [DataMember] Directions2D EndingSides { get; set; } = Directions2D.U;
        [DataMember] public float EndingPercentage { get; set; } = -1f;
        [DataMember] public InterpolationSettings OpeningMotion { get; set; } = InterpolationSettings.Faster;
        [DataMember] public InterpolationSettings ClosingMotion { get; set; } = InterpolationSettings.Fast;

        protected override void Initialize()
        {
            Parent.MinimumSize = new Point2(1f, 1f);
            Parent.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NonScrollable;
            if (Parent.IsGraphicsInitialized) Open(this, EventArgs.Empty);
            else Parent.OnGraphicsInitialized += Open;
        }

        protected override void ConnectEvents()
        {
            Parent.OnClickOff += ClickOff;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnClickOff -= ClickOff;
            Parent.OnGraphicsInitialized -= Open;
        }

        public override object Clone()
        {
            PopInOut c = new PopInOut();
            c.DeleteOnClickOff = DeleteOnClickOff;
            c.StartSides = StartSides;
            c.StartingPercentage = StartingPercentage;
            c.EndingSides = EndingSides;
            c.EndingPercentage = EndingPercentage;
            c.OpeningMotion = OpeningMotion;
            c.ClosingMotion = ClosingMotion;
            return c;
        }

        private void Open(object sender, EventArgs args)
        {
            RectangleF area = Parent.Area;
            Parent.Area = area.ResizedByPercent(StartingPercentage, StartSides.FlippedXY());
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), area, OpeningMotion) { DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override });
        }

        private void ClickOff(object sender, EventArgs args)
        {
            if (DeleteOnClickOff) Close();
        }

        public void Close()
        {
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), Parent.Area.ResizedByPercent(EndingPercentage, EndingSides.FlippedXY()), InterpolationSettings.Fast) { DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override }, out var close);
            close.OnCompletion += (s, a) => Parent.Delete();
        }
    }
}
