using Downunder.Utility;
using DownUnder.UI.Widgets.Behaviors.Behaviors.BehaviorObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class BorderFormat : WidgetBehavior
    {
        [DataMember] public GenericDirections2D<BorderSettings> BorderSettings { get; private set; } = new GenericDirections2D<BorderSettings>();

        private Widget _center_backing;

        public Widget Center {
            get => _center_backing;
            set {
                if (_center_backing != null) Parent.Remove(_center_backing);
                _center_backing = value;
                if (value != null) Parent.Add(_center_backing);
            }
        }

        public override object Clone() {
            BorderFormat c = new BorderFormat();
            c.BorderSettings = (GenericDirections2D<BorderSettings>)BorderSettings.Clone();
            return c;
        }

        protected override void ConnectToParent() {
            if (Center == null) Center = new Widget();
            Parent.OnResize += Align;
            Parent.OnUpdate += Update;
            Align(this, EventArgs.Empty);
        }

        protected override void DisconnectFromParent() {
            Parent.OnResize -= Align;
            Parent.OnUpdate -= Update;
        }

        public void Update(object sender, EventArgs args) {
            BorderSettings.Up.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Down.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Left.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Right.Update(Parent.UpdateData.ElapsedSeconds);
        }

        private void Align(object sender, EventArgs args)
        {

        }
    }
}
