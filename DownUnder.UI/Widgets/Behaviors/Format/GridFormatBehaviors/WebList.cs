using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors
{
    public class WebList : WidgetBehavior, ISubWidgetBehavior<GridFormat>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public GridFormat BaseBehavior => Parent.Behaviors.Get<GridFormat>();
        List<Widget> pre_load;
        SerializableType web_list_access_type;

        IWebList web_list_acces_point_backing;
        IWebList WebListAccesPoint 
        { 
            get => web_list_acces_point_backing;
            set
            {
                web_list_access_type = value.GetType();
                web_list_acces_point_backing = value;
            }
        }

        public WebList(Type i_web_list_access, params object[] parameters)
        {
            WebListAccesPoint = (IWebList)Activator.CreateInstance(i_web_list_access, parameters);
            pre_load = WebListAccesPoint.GetWeblistWidgets(0);
        }

        protected override void Initialize()
        {
            foreach (var widget in pre_load) BaseBehavior.AddRow(widget);
        }

        protected override void ConnectEvents()
        {
            
        }

        protected override void DisconnectEvents()
        {
            
        }

        public override object Clone()
        {
            var c = new WebList(WebListAccesPoint.GetType());
            return c;
        }
    }
}
