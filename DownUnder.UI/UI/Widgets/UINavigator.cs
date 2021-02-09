using System.Collections.Generic;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets {
    public sealed class UINavigator {
        readonly DWindow _parent;
        readonly Stack<Widget> _underlying_widgets = new Stack<Widget>();

        public WidgetTransitionAnimation DefaultTransition { get; } = WidgetTransitionAnimation.Slide(Direction2D.left);
        public WidgetTransitionAnimation DefaultBackTransition { get; } = WidgetTransitionAnimation.Slide(Direction2D.right);

        public UINavigator(DWindow parent) =>
            _parent = parent;

        public void NavigateTo(Widget widget) {
            _underlying_widgets.Push(_parent.DisplayWidget);
            _parent.DisplayWidget.AnimatedReplace(widget, DefaultTransition, false);
        }

        public void NavigateBack() {
            if (_underlying_widgets.Count == 0)
                return;

            _parent.DisplayWidget.AnimatedReplace(_underlying_widgets.Pop(), DefaultBackTransition);
        }
    }
}
