using DownUnder.Utilities;
using DownUnder.Utility;
using System;
using System.Reflection;

// https://www.youtube.com/watch?v=J4nM-F1kxs8

namespace DownUnder.UI.Widgets.Actions.Actions {
    /// <summary> Transition a given property to a given value over time. </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyTransitionAction<T> : WidgetAction {
        private ChangingValue<T> _changing_value;
        private readonly T _target_value;
        private PropertyInfo _property_info;
        public readonly string PropertyName;

        public T TargetValue { get; set; }
        public InterpolationSettings Interpolation;

        public PropertyTransitionAction(string nameof_property, T target_value, InterpolationSettings? interpolation = null) {
            PropertyName = nameof_property;
            _target_value = target_value;
            if (interpolation != null) Interpolation = interpolation.Value;
            else Interpolation = new InterpolationSettings(InterpolationType.fake_sin, 1f);
        }

        internal override bool Matches(WidgetAction action) => (action is PropertyTransitionAction<T> p_action) ? PropertyName == p_action.PropertyName :  false;
        
        public override object InitialClone() => new PropertyTransitionAction<T>(PropertyName, _target_value);
        
        protected override void ConnectToParent() {
            _property_info = typeof(Widget).GetProperty(PropertyName);
            _changing_value = new ChangingValue<T>((T)_property_info.GetValue(Parent));
            _changing_value.InterpolationSettings = Interpolation;

            _changing_value.SetTargetValue(_target_value);
            Parent.OnUpdate += Update;
        }

        internal override void DisconnectFromParent() {
            Parent.OnUpdate -= Update;
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
