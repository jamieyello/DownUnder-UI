using DownUnder.UI.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.UI.Widgets.Behaviors.Format.GridFormatBehaviors
{
    class WebList : WidgetBehavior, ISubWidgetBehavior<GridFormat>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public GridFormat BaseBehavior => Parent.Behaviors.Get<GridFormat>();
        List<Widget> pre_load;
        SerializableType web_list_access_type;

        IWebListAccesPoint web_list_acces_point_backing;
        IWebListAccesPoint WebListAccesPoint 
        { 
            get => web_list_acces_point_backing;
            set
            {
                web_list_access_type = value.GetType();
                web_list_acces_point_backing = value;
            }
        }

        public WebList(Type i_web_list_access)
        {
            WebListAccesPoint = (IWebListAccesPoint)Activator.CreateInstance(i_web_list_access);
            pre_load = WebListAccesPoint.GetWeblistWidgets(0);
        }

        protected override void Initialize()
        {
            foreach (var widget in pre_load) BaseBehavior.AddRow(widget);
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
            var c = new WebList();
            return c;
        }
    }
}
