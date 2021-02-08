using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.Actions;
using DownUnder.UI.UI.Widgets.Actions.Functional;
using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional
{
    [DataContract]
    public sealed class PopInOut : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public enum PopInOutModeType {
            use_set_area,
            use_indent
        }

        WidgetAction closing_action;

        [DataMember] public PopInOutModeType Mode { get; set; } = PopInOutModeType.use_indent;
        [DataMember] public bool CloseOnClickOff { get; set; } = true;
        [DataMember] public bool DeleteOnClose { get; set; } = true;
        [DataMember] public RectangleF OpeningArea { get; set; }
        [DataMember] public RectangleF ClosingArea { get; set; }
        [DataMember] public InterpolationSettings OpeningMotion { get; set; } = InterpolationSettings.Default;
        [DataMember] public InterpolationSettings ClosingMotion { get; set; } = InterpolationSettings.Default;
        [DataMember] public RectanglePart OpeningIndent { get; set; } = new RectanglePart();
        [DataMember] public RectanglePart ClosingIndent { get; set; } = new RectanglePart();

        public PopInOut() {
        }

        public PopInOut(
            RectanglePart opening_indent,
            RectanglePart closing_indent = null
        ) {
            OpeningIndent = (RectanglePart)opening_indent.Clone();
            ClosingIndent = closing_indent != null ? (RectanglePart)closing_indent.Clone() : (RectanglePart)opening_indent.Clone();
            Mode = PopInOutModeType.use_indent;
        }

        protected override void Initialize() {
            Parent.MinimumSize = new Point2(1f, 1f);
            Parent.Behaviors.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NoUserScrolling;

            if (Parent.ParentWidget is { })
                Open(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnClickOff += ClickOff;
            Parent.OnParentWidgetSet += Open;
            Parent.OnUpdate += CheckForClose;
        }

        protected override void DisconnectEvents() {
            Parent.OnClickOff -= ClickOff;
            Parent.OnParentWidgetSet -= Open;
            Parent.OnUpdate -= CheckForClose;
        }

        public override object Clone() {
            var c = new PopInOut();
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

        void Open(object sender, EventArgs args) {
            var area = Parent.Area;
            Parent.Area = Mode == PopInOutModeType.use_set_area ? OpeningArea : area.ResizedBy(OpeningIndent);
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), area, OpeningMotion) { DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override });
        }

        void ClickOff(object sender, EventArgs args) {
            if (CloseOnClickOff)
                Close();
        }

        void CheckForClose(object sender, EventArgs args) {
            if (!DeleteOnClose)
                return;

            if (closing_action is { } && closing_action.IsCompleted)
                Parent.Delete();
        }

        public void Close() {
            var closing_area = Mode == PopInOutModeType.use_set_area
                ? ClosingArea
                : Parent.Area.ResizedBy(ClosingIndent);

            Parent.Actions.Add(
                new PropertyTransitionAction<RectangleF>(
                    nameof(Widget.Area),
                    closing_area,
                    ClosingMotion
                ) {
                    DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override
                },
                out closing_action
            );
        }
    }
}
