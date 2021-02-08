using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.DataTypes;

namespace DownUnder.UI.UI.Widgets.Behaviors {
    /// <summary> Contains a <see cref="WidgetBehavior"/> and a <see cref="BehaviorInheritancePolicy"/> describing how the <see cref="WidgetBehavior"/> should be implemented. </summary>
    [DataContract] public sealed class GroupBehaviorPolicy : ICloneable {
        public enum BehaviorInheritancePolicy {
            /// <summary> Apply to all <see cref="Widget"/>s contained in this <see cref="Widget"/>. (<see cref="Widget.AllContainedWidgets"/>) </summary>
            all_children,
            /// <summary> Apply just to this <see cref="Widget"/>'s <see cref="Widget.Children"/>. </summary>
            direct_children
        }

        [DataMember] public WidgetBehavior Behavior { get; set; }
        [DataMember] public BehaviorInheritancePolicy InheritancePolicy { get; set; } = BehaviorInheritancePolicy.all_children;
        [DataMember] public GeneralVisualSettings.VisualRoleType? NecessaryVisualRole { get; set; }

        public GroupBehaviorPolicy() {
        }

        GroupBehaviorPolicy(
            GroupBehaviorPolicy source
        ) {
            Behavior = (WidgetBehavior)source.Behavior.Clone();
            InheritancePolicy = source.InheritancePolicy;
            NecessaryVisualRole = source.NecessaryVisualRole;
        }

        public object Clone() =>
            new GroupBehaviorPolicy(this);

        public bool ConflictsWith(GroupBehaviorPolicy policy) =>
            policy.Behavior.GetType() == Behavior.GetType()
            && policy.NecessaryVisualRole == NecessaryVisualRole;
    }
}
