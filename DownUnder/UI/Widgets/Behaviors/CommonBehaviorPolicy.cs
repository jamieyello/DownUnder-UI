using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class CommonBehaviorPolicy
    {
        public enum BehaviorInheritancePolicy
        {
            apply_to_compatible_children
        }

        public WidgetBehavior Behavior;
        public BehaviorInheritancePolicy InheritancePolicy = BehaviorInheritancePolicy.apply_to_compatible_children;
    }
}
