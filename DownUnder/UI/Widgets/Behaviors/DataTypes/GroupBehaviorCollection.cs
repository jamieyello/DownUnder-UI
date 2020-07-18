using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.DataTypes
{
    [DataContract] public class GroupBehaviorCollection
    {
        private GroupBehaviorAcceptancePolicy _common_behavior_rules_backing = new GroupBehaviorAcceptancePolicy();
        private List<GroupBehaviorPolicy> _behavior_policies = new List<GroupBehaviorPolicy>();

        public Widget Parent { get; set; }
        
        public GroupBehaviorAcceptancePolicy AcceptancePolicy
        {
            get => _common_behavior_rules_backing;
            set
            {
                Parent.Behaviors.RemoveIDed(value.DisallowedIDs);
                _common_behavior_rules_backing = value;
            }
        }

        public GroupBehaviorCollection() { }
        public GroupBehaviorCollection(Widget parent)
        {
            Parent = parent;
        }

        public void AddPolicy(GroupBehaviorPolicy policy)
        {
            foreach (var policy_ in _behavior_policies)
            {
                if (policy_.Behavior.GetType() == policy.Behavior.GetType()) throw new System.Exception($"This {nameof(GroupBehaviorCollection)} already contains a {nameof(WidgetBehavior)} of type {policy.Behavior.GetType().Name}");
            }
            _behavior_policies.Add(policy);
            ImplementPolicy(policy);
        }

        private void ImplementPolicy(GroupBehaviorPolicy policy)
        {
            if (policy.InheritancePolicy == GroupBehaviorPolicy.BehaviorInheritancePolicy.apply_to_compatible_children)
            {
                foreach (Widget widget in Parent.AllContainedWidgets)
                {
                    if (widget.GroupBehaviors.AcceptancePolicy.IsBehaviorAllowed(policy.Behavior))
                    {
                        widget.Behaviors.TryAdd((WidgetBehavior)policy.Behavior.Clone());
                    }
                }
            }
        }

        internal List<GroupBehaviorPolicy> InheritedPolicies
        {
            get
            {
                var result = new List<GroupBehaviorPolicy>(_behavior_policies);
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.GroupBehaviors.InheritedPolicies);
                return result;
            }
        }
    }
}
