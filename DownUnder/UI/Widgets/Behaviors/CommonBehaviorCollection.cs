using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class CommonBehaviorCollection
    {
        private BehaviorAcceptancePolicy _common_behavior_rules_backing = new BehaviorAcceptancePolicy();
        private List<CommonBehaviorPolicy> _behavior_policies = new List<CommonBehaviorPolicy>();

        public Widget Parent { get; set; }
        
        public BehaviorAcceptancePolicy AcceptancePolicy
        {
            get => _common_behavior_rules_backing;
            set
            {
                _common_behavior_rules_backing = value;
            }
        }

        public CommonBehaviorCollection() { }
        public CommonBehaviorCollection(Widget parent)
        {
            Parent = parent;
        }

        public void AddPolicy(CommonBehaviorPolicy policy)
        {
            foreach (var policy_ in _behavior_policies)
            {
                if (policy_.Behavior.GetType() == policy.Behavior.GetType()) throw new System.Exception($"This {nameof(CommonBehaviorCollection)} already contains a {nameof(WidgetBehavior)} of type {policy.Behavior.GetType().Name}");
            }
            _behavior_policies.Add(policy);
            ImplementPolicy(policy);
        }

        private void ImplementPolicy(CommonBehaviorPolicy policy)
        {
            if (policy.InheritancePolicy == CommonBehaviorPolicy.BehaviorInheritancePolicy.apply_to_compatible_children)
            {
                foreach (Widget widget in Parent.AllContainedWidgets)
                {
                    if (widget.CommonBehaviors.AcceptancePolicy.IsBehaviorAllowed(policy.Behavior))
                    {
                        widget.Behaviors.TryAdd((WidgetBehavior)policy.Behavior.Clone());
                    }
                }
            }
        }

        internal List<CommonBehaviorPolicy> InheritedPolicies
        {
            get
            {
                var result = new List<CommonBehaviorPolicy>(_behavior_policies);
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.CommonBehaviors.InheritedPolicies);
                return result;
            }
        }
    }
}
