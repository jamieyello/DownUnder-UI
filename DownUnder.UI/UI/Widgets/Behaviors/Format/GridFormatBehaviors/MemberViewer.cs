using System.Reflection;
using DownUnder.UI.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using DownUnder.UI.Utilities.Extensions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Format.GridFormatBehaviors
{
    public class MemberViewer : WidgetBehavior, ISubWidgetBehavior<GridFormat>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public GridFormat BaseBehavior => Parent.Behaviors.Get<GridFormat>();

        private MemberInfo[] _members;
        private float _row_height;

        public object RepresentedObject;

        public MemberViewer() { }
        public MemberViewer(object represented_object, float row_height = 30f)
        {
            RepresentedObject = represented_object;
            _row_height = row_height;
        }

        protected override void Initialize()
        {
            _members = RepresentedObject.GetType().GetEditableMembers();
            AddMembers();
        }

        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        public override object Clone()
        {
            MemberViewer c = new MemberViewer();
            c.RepresentedObject = RepresentedObject;
            return c;
        }

        private void AddMembers()
        {
            foreach (var member in _members)
            {
                BaseBehavior.AddRow(new Widget[] {
                    new Widget { Height = _row_height, IsFixedHeight = true }.WithAddedBehavior(new DrawText(member.Name) { YTextPositioning = DrawText.YTextPositioningPolicy.center }),
                    new Widget { Height = _row_height, IsFixedHeight = true }.WithAddedBehavior(new RepresentMember(RepresentedObject, member.Name), out var rep_mem)
                });
                rep_mem.BaseBehavior.YTextPositioning = DrawText.YTextPositioningPolicy.center;
            }
        }
    }
}
