using DownUnder.Utilities;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class SpacedListFormat : WidgetBehavior
    {
        [DataMember] public float ListSpacing { get; set; } = 0f;

        [DataMember] public InterpolationSettings? WidgetMovement { get; set; } = InterpolationSettings.Fast;

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            Parent.OnListChange += Align;
            Parent.OnAreaChange += Align;
            Align(this, EventArgs.Empty);
            Parent.EmbedChildren = false;
        }

        protected override void DisconnectFromParent()
        {
            Parent.OnListChange -= Align;
            Parent.OnAreaChange -= Align;
        }

        private void Align(object sender, EventArgs args)
        {
            Parent.Children.AlignHorizontalWrap(Parent.Width, true, ListSpacing);
        }
    }
}
