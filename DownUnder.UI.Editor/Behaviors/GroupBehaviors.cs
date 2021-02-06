using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UIEditor.Behaviors
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
