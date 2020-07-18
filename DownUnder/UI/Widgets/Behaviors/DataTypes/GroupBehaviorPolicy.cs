namespace DownUnder.UI.Widgets.Behaviors.DataTypes
{
    public class GroupBehaviorPolicy
    {
        public enum BehaviorInheritancePolicy
        {
            apply_to_compatible_children
        }

        public WidgetBehavior Behavior;
        public BehaviorInheritancePolicy InheritancePolicy = BehaviorInheritancePolicy.apply_to_compatible_children;
    }
}
