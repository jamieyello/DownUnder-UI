using DownUnder.Utilities;
using System;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class BehaviorAcceptancePolicy// : ICloneable
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

        public static BehaviorAcceptancePolicy operator |(BehaviorAcceptancePolicy p1, BehaviorAcceptancePolicy p2)
        {
            var result = new BehaviorAcceptancePolicy();
            result.DisallowedIDs.AddRange(p1.DisallowedIDs);
            result.DisallowedIDs.AddRange(p2.DisallowedIDs);
            return result;
        }

        public static BehaviorAcceptancePolicy NonScrollable => new BehaviorAcceptancePolicy() { DisallowedIDs = new List<string>() { DownUnderBehaviorIDs.SCROLL_FUNCTION } };
    }
}
