﻿using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Contains a <see cref="WidgetBehavior"/> and a <see cref="BehaviorInheritancePolicy"/> describing how the <see cref="WidgetBehavior"/> should be implemented. </summary>
    [DataContract] public class GroupBehaviorPolicy : ICloneable
    {
        public enum BehaviorInheritancePolicy
        {
            /// <summary> Apply to all <see cref="Widget"/>s contained in this <see cref="Widget"/>. (<see cref="Widget.AllContainedWidgets"/>) </summary>
            all_children,
            /// <summary>
            /// Apply just to this <see cref="Widget"/>'s <see cref="Widget.Children"/>.
            /// </summary>
            direct_children
        }

        [DataMember] public WidgetBehavior Behavior;
        [DataMember] public BehaviorInheritancePolicy InheritancePolicy = BehaviorInheritancePolicy.all_children;

        public object Clone()
        {
            GroupBehaviorPolicy c = new GroupBehaviorPolicy();
            c.Behavior = (WidgetBehavior)Behavior.Clone();
            c.InheritancePolicy = InheritancePolicy;
            return c;
        }
    }
}