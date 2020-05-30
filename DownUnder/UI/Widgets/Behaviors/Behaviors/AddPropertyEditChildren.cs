using DownUnder.Utility;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class AddPropertyEditChildren : WidgetBehavior
    {
        private object _edit_object_backing;

        public object EditObject
        {
            get => _edit_object_backing;
            set
            {
                if (value == _edit_object_backing) return;
                if (_edit_object_backing != null) throw new Exception($"Cannot re-set {nameof(EditObject)} after being set to something else.");
                _edit_object_backing = value;
                Properties = _edit_object_backing.GetType().GetProperties();
            }
        }

        public PropertyInfo[] Properties { get; private set; }

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
            _edit_object_backing = null;
        }

        private void AddChildren() {
            if (EditObject == null) throw new Exception($"{nameof(EditObject)} must be set before use.");
            if (Properties.Length == 0) throw new Exception($"No properties in {nameof(EditObject)} {EditObject.GetType().Name}.");

            Widget[] widgets = new Widget[Properties.Length * 2];

            for (int i = 0; i < Properties.Length * 2; i++)
            {
                Parent.Children.Add(new Widget());
            }

            return;

            Parallel.ForEach(Properties, (property, state, index) => {
                int i = (int)index;
                Widget widget = new Widget(Parent) { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 };
                widget.Behaviors.Add(new DrawText() { Text = "test" });
                widgets[i * 2] = widget;
            });

            for (int i = 0; i < widgets.Length; i++) {
                if (widgets[i] == null) Parent.Children.Add(new Widget(Parent) { SnappingPolicy = DiagonalDirections2D.None });
                else Parent.Children.Add(widgets[i]);
            }
        }
    }
}
