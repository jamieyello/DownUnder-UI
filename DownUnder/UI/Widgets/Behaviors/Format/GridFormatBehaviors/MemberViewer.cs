using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors
{
    class MemberViewer : WidgetBehavior, ISubWidgetBehavior<GridFormat>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public GridFormat BaseBehavior => Parent.Behaviors.GetFirst<GridFormat>();

        private MemberInfo[] _members;

        public object RepresentedObject;

        protected override void Initialize()
        {
            _members = RepresentedObject.GetType().GetEditableMembers();
        }

        protected override void ConnectEvents()
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectEvents()
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
