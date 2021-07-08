using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors {
    [DataContract]
    public sealed class GroupBehaviorAcceptancePolicy { // : ICloneable
        [DataMember] public List<string> DisallowedIDs { get; set; } = new List<string>();

        internal bool IsBehaviorAllowed(Widget widget, GroupBehaviorPolicy policy) {
            foreach (var id in policy.Behavior.BehaviorIDs) {
                if (DisallowedIDs.Contains(id))
                    return false;
            }

            return
                policy.NecessaryVisualRole == null
                || policy.NecessaryVisualRole.Value == widget.VisualSettings.VisualRole;
        }

        public static GroupBehaviorAcceptancePolicy operator +(
            GroupBehaviorAcceptancePolicy p1,
            GroupBehaviorAcceptancePolicy p2
        ) {
            var result = new GroupBehaviorAcceptancePolicy();
            result.DisallowedIDs.AddRange(p1.DisallowedIDs);
            result.DisallowedIDs.AddRange(p2.DisallowedIDs);
            return result;
        }

        // same as +
        public static GroupBehaviorAcceptancePolicy operator |(
            GroupBehaviorAcceptancePolicy p1,
            GroupBehaviorAcceptancePolicy p2
        ) {
            var result = new GroupBehaviorAcceptancePolicy();
            result.DisallowedIDs.AddRange(p1.DisallowedIDs);
            result.DisallowedIDs.AddRange(p2.DisallowedIDs);
            return result;
        }

        public static GroupBehaviorAcceptancePolicy NoUserScrolling =>
            new GroupBehaviorAcceptancePolicy {
                DisallowedIDs = new List<string> {
                    DownUnderBehaviorIDs.SCROLL_FUNCTION
                }
            };
    }
}
