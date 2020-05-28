using DownUnder.Utility;
using System;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class AddPropertyEditChildren : WidgetBehavior
    {
        public object EditObject;

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            AddChildren();
        }

        protected override void DisconnectFromParent()
        {
            throw new NotImplementedException();
        }

        private void AddChildren()
        {
            var properties = EditObject.GetType().GetProperties();
            if (properties.Length == 0) throw new Exception("No properties in object.");

            Widget[] widgets = new Widget[properties.Length * 2];

            Parallel.ForEach(properties, (property, state, index) =>
            {
            int i = (int)index;
            widgets[i * 2] = new Widget(null) { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 };
            });

            for (int i = 0; i < widgets.Length; i++)
            {
                if (widgets[i] == null) Parent.Children.Add(new Widget());
                else Parent.Children.Add(widgets[i]);
            }
        }
    }
}
