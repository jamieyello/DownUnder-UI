using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes {
    [DataContract] public class ContainerBorder {
        public enum BorderWidgetOccupyPolicy {
            occupy_space,
            overlay
        }

        [DataMember] public BorderWidgetOccupyPolicy OccupyPolicy { get; set; } = BorderWidgetOccupyPolicy.overlay;
        [DataMember] public Widget Widget { get; set; }
    }
}
