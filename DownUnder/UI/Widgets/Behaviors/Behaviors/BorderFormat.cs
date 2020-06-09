using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    class BorderFormat : WidgetBehavior
    {
        private Widget _center_backing;

        public Widget Center
        {
            get => _center_backing;
            set
            {
                if (_center_backing != null) Parent.Remove(_center_backing);
                _center_backing = value;
                if (value != null) Parent.Add(_center_backing);
            }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            if (Center == null) Center = new Widget();
        }

        protected override void DisconnectFromParent()
        {
            throw new NotImplementedException();
        }
    }
}
