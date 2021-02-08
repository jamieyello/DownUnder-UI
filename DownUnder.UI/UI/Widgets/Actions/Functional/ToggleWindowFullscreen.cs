namespace DownUnder.UI.UI.Widgets.Actions.Functional
{
    public class ToggleWindowFullscreen : WidgetAction
    {
        protected override void ConnectEvents()
        {

        }

        protected override void DisconnectEvents()
        {

        }

        protected override void Initialize()
        {
            Parent.ParentDWindow.ToggleFullscreen();
            EndAction();
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return action.GetType() == GetType();
        }

        protected override bool Matches(WidgetAction action)
        {
            return action.GetType() == GetType();
        }
    }
}