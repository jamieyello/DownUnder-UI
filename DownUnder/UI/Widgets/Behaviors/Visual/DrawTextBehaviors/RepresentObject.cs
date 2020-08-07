using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors
{
    public class RepresentObject : WidgetBehavior, ISubWidgetBehavior<DrawText>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public DrawText BaseBehavior => Parent.Behaviors.GetFirst<DrawText>();

        private PropertyInfo _property;

        public string NameOfProperty;
        public object RepresentedObject;

        public RepresentObject() { }
        public RepresentObject(object obj, string nameof_property)
        {
            RepresentedObject = obj;
            NameOfProperty = nameof_property;
        }

        protected override void Initialize()
        {
            _property = RepresentedObject.GetType().GetProperty(NameOfProperty);
        }

        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        public override object Clone()
        {
            return new RepresentObject(RepresentedObject, NameOfProperty);
        }

        private void UpdateText(object sender, EventArgs args)
        {
            BaseBehavior.Text = _property.GetValue(RepresentedObject).ToString();
        }

        public void UpdateText()
        {
            UpdateText(this, EventArgs.Empty);
        }
    }
}
