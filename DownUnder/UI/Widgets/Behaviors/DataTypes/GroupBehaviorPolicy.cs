using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.DataTypes
{
    [DataContract] public class GroupBehaviorPolicy
    {
        public enum BehaviorInheritancePolicy
        {
            apply_to_compatible_children
        }

        [DataMember] public WidgetBehavior Behavior;
        [DataMember] public BehaviorInheritancePolicy InheritancePolicy = BehaviorInheritancePolicy.apply_to_compatible_children;
    }
}
