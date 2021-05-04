using DownUnder.UI.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace DownUnder.UI.UI.Widgets.Actions.Functional
{
    public class ReplaceWidgetAsync : WidgetAction
    {
        public Widget LoadingScreen;
        volatile Widget loaded_widget = null;
        Func<IWidget> get_widget;
        WidgetTransitionAnimation animation;

        public ReplaceWidgetAsync(Func<IWidget> get_widget, WidgetTransitionAnimation animation = null)
        {
            this.get_widget = get_widget;
            this.animation = animation ?? UINavigator.DefaultTransition;
        }

        protected override void Initialize()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) => loaded_widget = get_widget.Invoke().Widget;
            worker.RunWorkerAsync();
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return action.GetType() == GetType();
        }

        protected override bool Matches(WidgetAction action)
        {
            return action.GetType() == GetType();
        }

        public static Widget GetDefaultLoadingScreen()
        {
            Widget result = new Widget();
            result.Behaviors.Add(new DrawText("Loading...") { XTextPositioning = DrawText.XTextPositioningPolicy.center, YTextPositioning = DrawText.YTextPositioningPolicy.center });
            return result;
        }

        void Update(object sender, EventArgs args)
        {
            if (loaded_widget == null) return;
            Parent.AnimatedReplace(loaded_widget, UINavigator.DefaultTransition, true);
            EndAction();
        }
    }
}
