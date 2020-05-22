using DownUnder.Utilities;
using DownUnder.Utility;
using DownUnder.UI.Widgets.BaseWidgets;
using System;
using System.Threading.Tasks;
using DownUnder.UI.Widgets.SpecializedWidgets;

namespace DownUnder.UI.Widgets.Actions
{
    /// <summary> Creates a <see cref="Widget"/> asyncronously and slides it into the given <see cref="Layout"/>. </summary>
    public class AsyncLayoutSlide : WidgetAction
    {
        Task<Widget> _new_widget_task;
        Directions2D _direction;
        InterpolationSettings _interpolation;

        public AsyncLayoutSlide(Task<Widget> new_widget_task, Directions2D direction, InterpolationSettings? interpolation = null)
        {
            _new_widget_task = new_widget_task;
            _direction = direction;
            _interpolation = interpolation == null ? InterpolationSettings.Fast : interpolation.Value;
        }

        protected override void ConnectToParent() 
        {
            Console.WriteLine("yh");
            if (!(Parent is Layout)) throw new Exception($"{nameof(AsyncLayoutSlide)} can only be used with {nameof(Layout)}.");
            SetNewPropertyGridAsync();
        }

        protected override void DisconnectFromParent() { }

        public override object InitialClone()
        {
            throw new NotImplementedException();
        }

        protected override bool InterferesWith(WidgetAction action) => action is AsyncLayoutSlide || action is AutoLayoutSlide;

        private async Task SetNewPropertyGridAsync()
        {
            Console.WriteLine("start");
            Widget new_widget = await _new_widget_task;
            Console.WriteLine("end");

            AutoLayoutSlide slide = new AutoLayoutSlide(new_widget, _direction, _interpolation) { DuplicatePolicy = DuplicatePolicyType.wait };
            Parent.Actions.Add(slide);
            EndAction();
        }

        protected override bool Matches(WidgetAction action)
        {
            throw new NotImplementedException();
        }
    }
}
