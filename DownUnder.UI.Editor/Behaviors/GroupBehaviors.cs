using DownUnder.UI.Widgets.Behaviors;

namespace DownUnder.UI.Editor.Behaviors
{
    public static class GroupBehaviors
    {
        public static GroupBehaviorCollection EditModeBehaviors
        {
            get
            {
                var result = new GroupBehaviorCollection
                {
                    new GroupBehaviorPolicy() { Behavior = new GetEditModeDropDowns(), InheritancePolicy = GroupBehaviorPolicy.BehaviorInheritancePolicy.all_children },
                };
                return result;
            }
        }
    }
}
