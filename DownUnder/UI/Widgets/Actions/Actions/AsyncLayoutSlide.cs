using DownUnder.Utilities;
using DownUnder.Utility;
using DownUnder.UI.Widgets.BaseWidgets;
using System;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Actions
{
    /// <summary> Creates a <see cref="Widget"/> asyncronously and slides it into the given <see cref="Layout"/>. </summary>
    class AsyncLayoutSlide : WidgetAction
    {
        public AsyncLayoutSlide(Func<Widget> new_widget, Directions2D direction, InterpolationSettings? interpolation = null)
        {

        }

        public override void DisconnectFromParent()
        {
            throw new NotImplementedException();
        }

        public override object InitialClone()
        {
            throw new NotImplementedException();
        }

        public override bool Matches(WidgetAction action)
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            throw new NotImplementedException();
        }
    }
}
