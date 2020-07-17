using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.Utilities;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    [DataContract] public class PopInOut : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public enum PopInOutModeType
        {
            use_set_area,
            use_indent
        }

        private WidgetAction closing_action;

        [DataMember] public PopInOutModeType Mode { get; set; } = PopInOutModeType.use_set_area;
        [DataMember] public bool CloseOnClickOff { get; set; } = true;
        [DataMember] public bool DeleteOnClose { get; set; } = true;
        [DataMember] public RectangleF OpeningArea { get; set; } = new RectangleF();
        [DataMember] public RectangleF ClosingArea { get; set; } = new RectangleF();
        [DataMember] public InterpolationSettings OpeningMotion { get; set; } = InterpolationSettings.Faster;
        [DataMember] public InterpolationSettings ClosingMotion { get; set; } = InterpolationSettings.Fast;
        [DataMember] public RectanglePart OpeningIndent { get; set; } = new RectanglePart();
        [DataMember] public RectanglePart ClosingIndent { get; set; } = new RectanglePart();

        public PopInOut() { }
        public PopInOut(RectanglePart opening_indent, RectanglePart closing_indent = null)
        {
            OpeningIndent = (RectanglePart)opening_indent.Clone();
            ClosingIndent = closing_indent != null ? (RectanglePart)closing_indent.Clone() : (RectanglePart)opening_indent.Clone();
            Mode = PopInOutModeType.use_indent;
        }

        protected override void Initialize()
        {
            Parent.MinimumSize = new Point2(1f, 1f);
            Parent.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NonScrollable;
            if (Parent.IsGraphicsInitialized) Open(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnClickOff += ClickOff;
            Parent.OnPostGraphicsInitialized += Open;
            Parent.OnUpdate += CheckForClose;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnClickOff -= ClickOff;
            Parent.OnPostGraphicsInitialized -= Open;
            Parent.OnUpdate -= CheckForClose;
        }

        public override object Clone()
        {
            PopInOut c = new PopInOut();
            c.Mode = Mode;
            c.CloseOnClickOff = CloseOnClickOff;
            c.DeleteOnClose = DeleteOnClose;
            c.OpeningArea = OpeningArea;
            c.ClosingArea = ClosingArea;
            c.OpeningMotion = OpeningMotion;
            c.ClosingMotion = ClosingMotion;
            c.OpeningIndent = (RectanglePart)OpeningIndent.Clone();
            c.ClosingIndent = (RectanglePart)ClosingIndent.Clone();
            return c;
        }

        private void Open(object sender, EventArgs args)
        {
            RectangleF area = Parent.Area;
            Parent.Area = Mode == PopInOutModeType.use_set_area ? OpeningArea : area.ResizedBy(OpeningIndent);
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), area, OpeningMotion) { DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override });
        }

        private void ClickOff(object sender, EventArgs args)
        {
            if (CloseOnClickOff) Close();
        }

        private void CheckForClose(object sender, EventArgs args)
        {
            if (!DeleteOnClose) return;
            if (closing_action != null && closing_action.IsCompleted) Parent.Delete();
        }

        public void Close()
        {
            RectangleF closing_area = Mode == PopInOutModeType.use_set_area ? ClosingArea : Parent.Area.ResizedBy(ClosingIndent);
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), closing_area, ClosingMotion) { DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override }, out closing_action);
        }
    }
}
