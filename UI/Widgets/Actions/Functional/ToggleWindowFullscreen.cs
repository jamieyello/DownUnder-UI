namespace DownUnder.UI.Widgets.Actions.Functional
{
    class ToggleWindowFullscreen : WidgetAction
    {
        protected override void ConnectEvents()
        {
            
        }

        protected override void DisconnectEvents()
        {
            
        }

        protected override void Initialize()
        {
            Parent.ParentDWindow.GraphicsManager.HardwareModeSwitch = false;
            Parent.ParentDWindow.GraphicsManager.ToggleFullScreen();
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