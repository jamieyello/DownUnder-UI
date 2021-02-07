using System;
using System.Runtime.Serialization;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.Behaviors.Format
{
    [DataContract] public class SpacedListFormat : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        [DataMember] public float ListSpacing { get; set; } = 25f;
        [DataMember] public InterpolationSettings? Interpolation { get; set; } = InterpolationSettings.Faster;

        public override object Clone()
        {
            SpacedListFormat c = new SpacedListFormat();
            c.ListSpacing = ListSpacing;
            if (Interpolation != null) c.Interpolation = Interpolation.Value;
            return c;
        }

        protected override void Initialize()
        {
            Align(this, EventArgs.Empty);
            Parent.EmbedChildren = false;
        }

        protected override void ConnectEvents()
        {
            Parent.OnListChange += Align;
            Parent.OnAreaChange += Align;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnListChange -= Align;
            Parent.OnAreaChange -= Align;
        }

        private void Align(object sender, EventArgs args)
        {
            Parent.Children.SetAreas(Parent.Children.GetHorizontalWrapAreas(Parent.Width, ListSpacing), Interpolation);
        }
    }
}
