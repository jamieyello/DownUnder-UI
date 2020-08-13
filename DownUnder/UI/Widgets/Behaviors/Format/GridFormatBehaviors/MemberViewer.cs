using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors
{
    public class MemberViewer : WidgetBehavior, ISubWidgetBehavior<GridFormat>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public GridFormat BaseBehavior => Parent.Behaviors.GetFirst<GridFormat>();

        private MemberInfo[] _members;

        public object RepresentedObject;

        public MemberViewer() { }
        public MemberViewer(object represented_object) 
        {
            RepresentedObject = represented_object;
        }

        protected override void Initialize()
        {
            _members = RepresentedObject.GetType().GetEditableMembers();
            AddMembers();
        }

        protected override void ConnectEvents()
        {
            
        }

        protected override void DisconnectEvents()
        {
            
        }

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
                BaseBehavior.AddRow(new Widget[] { new DrawText(member.Name).CreateWidget(), new RepresentMember(RepresentedObject, member.Name).CreateWidget() });
            }
        }
    }
}
