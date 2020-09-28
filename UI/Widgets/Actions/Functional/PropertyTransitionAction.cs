using DownUnder.Utilities;
using DownUnder.Utility;
using System;
using System.Reflection;
using System.Runtime.Serialization;

// https://www.youtube.com/watch?v=J4nM-F1kxs8

namespace DownUnder.UI.Widgets.Actions.Functional
{
    /// <summary> Transition a given property to a given value over time. </summary>
    /// <typeparam name="T"> Type of the targeted property. </typeparam>
    [DataContract] public class PropertyTransitionAction<T> : WidgetAction {
        private ChangingValue<T> _changing_value;
        private PropertyInfo _property_info;
        
        [DataMember] public string PropertyName;
        [DataMember] public InterpolationSettings Interpolation = InterpolationSettings.Default;
        [DataMember] public T TargetValue;

        public bool IsTransitioning => _changing_value != null && _changing_value.IsTransitioning;

        public PropertyTransitionAction(string nameof_property, T target_value, InterpolationSettings? interpolation = null) {
            PropertyName = nameof_property;
            TargetValue = target_value;
            if (interpolation != null) Interpolation = interpolation.Value;

            DuplicateDefinition = DuplicateDefinitionType.matches_result;
            DuplicatePolicy = DuplicatePolicyType.cancel;
        }

        protected override bool InterferesWith(WidgetAction action) => (action is PropertyTransitionAction<T> p_action) && PropertyName == p_action.PropertyName;

        protected override bool Matches(WidgetAction action)
        {
            return action is PropertyTransitionAction<T> action_t
                && action_t.TargetValue.Equals(TargetValue);
        }

        public override object InitialClone()
        {
            PropertyTransitionAction<T> c = (PropertyTransitionAction<T>)base.InitialClone();
            c.PropertyName = PropertyName;
            c.TargetValue = TargetValue;
            c.Interpolation = Interpolation;

            return c;
        }

        protected override void Initialize()
        {
            _property_info = typeof(Widget).GetProperty(PropertyName);
            _changing_value = new ChangingValue<T>((T)_property_info.GetValue(Parent));
            _changing_value.InterpolationSettings = Interpolation;

            _changing_value.SetTargetValue(TargetValue);
        }

        protected override void ConnectEvents() {
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents() {
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
