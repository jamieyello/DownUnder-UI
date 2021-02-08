namespace DownUnder.UI.UI.Widgets.Behaviors {
    sealed class TemplateBehavior : WidgetBehavior {
        /// <summary> Allows the framework identify the purpose of your behavior. </summary>
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        /// <summary> Initialize your behavior's graphics here using <see cref="WidgetBehavior.Parent"/>. Called before ConnectEvents. </summary>
        protected override void Initialize() {
        }

        /// <summary> Connect all methods to the parent Widget's EventHandlers here. </summary>
        protected override void ConnectEvents() {
        }

        /// <summary> Disconnect all methods from the parent Widget's EventHandlers here. </summary>
        protected override void DisconnectEvents() {
        }

        /// <summary> Return the initial state of this WidgetBehavior to be used by another Widget. </summary>
        public override object Clone() =>
            new TemplateBehavior();
    }
}
