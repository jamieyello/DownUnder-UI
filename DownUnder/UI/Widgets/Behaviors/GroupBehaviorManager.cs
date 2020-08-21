using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class GroupBehaviorManager : IIsWidgetChild
    {
        [DataMember] private GroupBehaviorAcceptancePolicy _common_behavior_rules_backing = new GroupBehaviorAcceptancePolicy();
        [DataMember] private List<GroupBehaviorPolicy> _behavior_policies = new List<GroupBehaviorPolicy>();

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

        public GroupBehaviorManager() { }
        public GroupBehaviorManager(Widget parent)
        {
            Parent = parent;
        }

        public void AddPolicy(GroupBehaviorPolicy policy)
        {
            foreach (var policy_ in _behavior_policies)
            {
                if (policy_.Behavior.GetType() == policy.Behavior.GetType()) throw new System.Exception($"This {nameof(GroupBehaviorManager)} already contains a {nameof(WidgetBehavior)} of type {policy.Behavior.GetType().Name}");
            }
            _behavior_policies.Add(policy);
            ImplementPolicy(policy);
        }
        public void AddPolicy(IEnumerable<GroupBehaviorPolicy> policies)
        {
            foreach (GroupBehaviorPolicy policy in policies) AddPolicy(policy);
        }

        private void ImplementPolicy(GroupBehaviorPolicy policy)
        {
            WidgetList widgets;

            if (policy.InheritancePolicy == GroupBehaviorPolicy.BehaviorInheritancePolicy.all_children) widgets = Parent.AllContainedWidgets;
            else widgets = Parent.Children;
            
            foreach (Widget widget in widgets)
            {
                if (widget.GroupBehaviors.AcceptancePolicy.IsBehaviorAllowed(policy.Behavior))
                {
                    widget.Behaviors.TryAdd((WidgetBehavior)policy.Behavior.Clone());
                }
            }
        }

        internal List<GroupBehaviorPolicy> InheritedPolicies
        {
            get
            {
                var result = new List<GroupBehaviorPolicy>(_behavior_policies);
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.GroupBehaviors._InheritedPolicies);
                return result;
            }
        }

        private List<GroupBehaviorPolicy> _InheritedPolicies
        {
            get
            {
                var result = new List<GroupBehaviorPolicy>(_behavior_policies);
                for (int i = result.Count - 1; i >= 0; i--) {
                    if (result[0].InheritancePolicy == GroupBehaviorPolicy.BehaviorInheritancePolicy.direct_children) result.RemoveAt(i);
                }
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.GroupBehaviors._InheritedPolicies);
                return result;
            }
        }
    }
}
