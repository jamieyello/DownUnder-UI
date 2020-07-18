using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors.DataTypes
{
    public class GroupBehaviorAcceptancePolicy// : ICloneable
    {
        public List<string> DisallowedIDs { get; set; } = new List<string>();

        internal bool IsBehaviorAllowed(WidgetBehavior behavior)
        {
            foreach (string id in behavior.BehaviorIDs)
            {
                if (DisallowedIDs.Contains(id)) return false;
            }

            return true;
        }

        public static GroupBehaviorAcceptancePolicy operator +(GroupBehaviorAcceptancePolicy p1, GroupBehaviorAcceptancePolicy p2)
        {
            var result = new GroupBehaviorAcceptancePolicy();
            result.DisallowedIDs.AddRange(p1.DisallowedIDs);
            result.DisallowedIDs.AddRange(p2.DisallowedIDs);
            return result;
        }

        // same as +
        public static GroupBehaviorAcceptancePolicy operator |(GroupBehaviorAcceptancePolicy p1, GroupBehaviorAcceptancePolicy p2)
        {
            var result = new GroupBehaviorAcceptancePolicy();
            result.DisallowedIDs.AddRange(p1.DisallowedIDs);
            result.DisallowedIDs.AddRange(p2.DisallowedIDs);
            return result;
        }

        public static GroupBehaviorAcceptancePolicy NonScrollable => new GroupBehaviorAcceptancePolicy() { DisallowedIDs = new List<string>() { DownUnderBehaviorIDs.SCROLL_FUNCTION } };
    }
}
