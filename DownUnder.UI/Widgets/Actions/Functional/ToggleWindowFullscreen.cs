namespace DownUnder.UI.Widgets.Actions.Functional {
    public sealed class ToggleWindowFullscreen : WidgetAction {
        protected override void ConnectEvents() {
        }

        protected override void DisconnectEvents() {
        }

        protected override void Initialize() {
            Parent.ParentDWindow.ToggleFullscreen();
            EndAction();
        }

        protected override bool InterferesWith(WidgetAction action) =>
            action.GetType() == GetType();

        protected override bool Matches(WidgetAction action) =>
            action.GetType() == GetType();
    }
}