using DownUnder.Utilities;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class SpacedListFormat : WidgetBehavior
    {
        [DataMember] public float ListSpacing { get; set; } = 25f;

        [DataMember] public InterpolationSettings? Interpolation { get; set; } = InterpolationSettings.Faster;

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
            Parent.Children.SetAreas(Parent.Children.GetHorizontalWrapAreas(Parent.Width, ListSpacing), Interpolation);
        }
    }
}
