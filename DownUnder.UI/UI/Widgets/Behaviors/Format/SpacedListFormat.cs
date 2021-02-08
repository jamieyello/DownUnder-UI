using System;
using System.Runtime.Serialization;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.Behaviors.Format {
    [DataContract]
    public sealed class SpacedListFormat : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        [DataMember] public float ListSpacing { get; set; } = 25f;
        [DataMember] public InterpolationSettings? Interpolation { get; set; } = InterpolationSettings.Faster;

        public override object Clone() {
            var c = new SpacedListFormat {
                ListSpacing = ListSpacing
            };

            if (Interpolation is { })
                c.Interpolation = Interpolation.Value;

            return c;
        }

        protected override void Initialize() {
            Align(this, EventArgs.Empty);
            Parent.EmbedChildren = false;
        }

        protected override void ConnectEvents() {
            Parent.OnListChange += Align;
            Parent.OnAreaChange += Align;
        }

        protected override void DisconnectEvents() {
            Parent.OnListChange -= Align;
            Parent.OnAreaChange -= Align;
        }

        void Align(object sender, EventArgs args) =>
            Parent.Children.SetAreas(
                Parent.Children.GetHorizontalWrapAreas(
                    Parent.Width,
                    ListSpacing
                ),
                Interpolation
            );
    }
}
