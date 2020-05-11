using DownUnder.Utility;
using System;
using System.Reflection;

// https://www.youtube.com/watch?v=J4nM-F1kxs8

namespace DownUnder.UI.Widgets.Actions.Actions
{
    public class PropertyTransitionAction<T> : WidgetAction
    {
        private ChangingValue<T> _changing_value;
        private T _target_value;
        private PropertyInfo _property_info;
        private string _nameof_property;

        public T TargetValue { get; set; }

        public PropertyTransitionAction(string nameof_property, T target_value) {
            _nameof_property = nameof_property;
            _target_value = target_value;
        }

        public override object InitialClone() => new PropertyTransitionAction<T>(_nameof_property, _target_value);
        
        protected override void ConnectToParent() {
            _property_info = typeof(Widget).GetProperty(_nameof_property);
            _changing_value = new ChangingValue<T>((T)_property_info.GetValue(Parent));
            Parent.OnUpdate += Update;
            Console.WriteLine("Connected");
        }

        internal override void DisconnectFromParent() {
            Parent.OnUpdate -= Update;
            Console.WriteLine("Disconnected");
        }

        private void Update(object sender, EventArgs args) {
            if (!_changing_value.IsTransitioning) {
                EndAction();
                return;
            }

            _changing_value.Update(((Widget)sender).UpdateData.ElapsedSeconds);
            _property_info.SetValue(Parent, _changing_value.GetCurrent());
            if (!_changing_value.IsTransitioning) EndAction();
        }
    }
}
