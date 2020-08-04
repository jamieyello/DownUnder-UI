using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Contains a <see cref="WidgetBehavior"/> and a <see cref="BehaviorInheritancePolicy"/> describing how the <see cref="WidgetBehavior"/> should be implemented. </summary>
    [DataContract] public class GroupBehaviorPolicy : ICloneable
    {
        public enum BehaviorInheritancePolicy
        {
            apply_to_compatible_children
        }

        [DataMember] public WidgetBehavior Behavior;
        [DataMember] public BehaviorInheritancePolicy InheritancePolicy = BehaviorInheritancePolicy.apply_to_compatible_children;

        public object Clone()
        {
            GroupBehaviorPolicy c = new GroupBehaviorPolicy();
            c.Behavior = (WidgetBehavior)Behavior.Clone();
            c.InheritancePolicy = InheritancePolicy;
            return c;
        }
    }
}
