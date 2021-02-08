using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.UI.Widgets.Behaviors
{
    [DataContract] public class GroupBehaviorManager
    {
        [DataMember] private GroupBehaviorAcceptancePolicy _common_behavior_rules_backing = new GroupBehaviorAcceptancePolicy();
        [DataMember] private List<GroupBehaviorPolicy> _behavior_policies = new List<GroupBehaviorPolicy>();
        [DataMember] Widget Parent { get; set; }

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

        /// <summary> Returns a matching policy if the given <see cref="GroupBehaviorPolicy"/> has a matching <see cref="GroupBehaviorPolicy.Behavior"/> <see cref="Type"/> and <see cref="GroupBehaviorPolicy.InheritancePolicy"/> pair. </summary>
        public GroupBehaviorPolicy GetMatchingPolicy(GroupBehaviorPolicy policy)
        {
            foreach (var _policy in _behavior_policies)
            {
                if (_policy.Behavior.GetType() == policy.Behavior.GetType() && _policy.NecessaryVisualRole == policy.NecessaryVisualRole) return _policy;
            }
            return null;
        }

        //public void AddPolicy(IEnumerable<GroupBehaviorPolicy> policies)
        //{
        //    foreach (GroupBehaviorPolicy policy in policies) AddPolicy(policy);
        //}

        //public void AddPolicy(GroupBehaviorPolicy policy)
        //{
        //    //if (GetMatchingPolicy(policy) != null) return;
        //    //_behavior_policies.Add(policy);
        //    ImplementPolicy(policy);
        //}

        public void RemovePolicy(IEnumerable<GroupBehaviorPolicy> policies, bool remove_behavior = true, bool recursive = true)
        {
            foreach (var policy in policies) RemovePolicy(policy, remove_behavior, recursive);
        }

        /// <summary> Removes the matching policy without disabling the behavior. Does not match by reference, but through <see cref="GetMatchingPolicy(GroupBehaviorPolicy)"/>. </summary>
        /// <param name="policy"> The <see cref="GroupBehaviorPolicy"/> to search for. </param>
        /// <param name="remove_behavior"> When set to true, the matching <see cref="WidgetBehavior"/>s will be removed as well. </param>
        /// <param name="recursive"> If true, iterate through all contained <see cref="Widget"/>s to do the same. </param>
        public void RemovePolicy(GroupBehaviorPolicy policy, bool remove_behavior = true, bool recursive = true)
        {
            if (Parent.ParentWidget?.Behaviors.GroupBehaviors.GetMatchingPolicy(policy) != null) throw new Exception($"Cannot remove a policy that originates from a parent {nameof(Widget)}.");
            GroupBehaviorPolicy _policy = GetMatchingPolicy(policy);
            if (_policy == null) return;

            if (!_behavior_policies.Remove(_policy)) throw new Exception($"Unexpected behavior in {nameof(GroupBehaviorManager)}.{nameof(GroupBehaviorManager.RemovePolicy)}.");
            if (remove_behavior && !Parent.Behaviors.RemoveType(_policy.Behavior.GetType())) throw new Exception($"Unexpected behavior in {nameof(GroupBehaviorManager)}.{nameof(GroupBehaviorManager.RemovePolicy)}.");
            if (recursive)
            {
                foreach (Widget widget in Parent.Children) widget.Behaviors.GroupBehaviors.RemovePolicy(policy, remove_behavior, recursive);
            }
        }

        /// <summary> Internal method. Avoid calling. </summary>
        internal void UpdatePolicies()
        {
            ImplementPolicy(_behavior_policies);
        }

        public void ImplementPolicy(IEnumerable<GroupBehaviorPolicy> policies)
        {
            foreach (GroupBehaviorPolicy policy in policies) ImplementPolicy(policy);
        }

        public void ImplementPolicy(GroupBehaviorPolicy policy)
        {
            if (GetMatchingPolicy(policy) == null) _behavior_policies.Add(policy);
            TryAddBehavior(policy);

            if (policy.InheritancePolicy == GroupBehaviorPolicy.BehaviorInheritancePolicy.all_children)
            {
                foreach (Widget widget in Parent.Children) widget.Behaviors.GroupBehaviors.ImplementPolicy(policy);
            }
            else if (policy.InheritancePolicy == GroupBehaviorPolicy.BehaviorInheritancePolicy.direct_children)
            {
                foreach (Widget widget in Parent.Children) widget.Behaviors.GroupBehaviors.TryAddBehavior(policy);
            }
        }

        void TryAddBehavior(GroupBehaviorPolicy policy)
        {
            if (AcceptancePolicy.IsBehaviorAllowed(Parent, policy)) Parent.Behaviors.TryAdd((WidgetBehavior)policy.Behavior.Clone());
        }

        internal List<GroupBehaviorPolicy> InheritedPolicies
        {
            get
            {
                var result = new List<GroupBehaviorPolicy>(_behavior_policies);
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.Behaviors.GroupBehaviors._InheritedPolicies);
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
                if (Parent.ParentWidget != null) result.AddRange(Parent.ParentWidget.Behaviors.GroupBehaviors._InheritedPolicies);
                return result;
            }
        }
    }
}
