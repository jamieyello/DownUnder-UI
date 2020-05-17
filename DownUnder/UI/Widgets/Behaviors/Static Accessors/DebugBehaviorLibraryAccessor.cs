namespace DownUnder.UI.Widgets.Behaviors.Static_Accessors {
    public class DebugBehaviorLibraryAccessor {
        public WritePropertyToConsole WriteProperty(string nameof_property, string pre_text = "", string post_text = "") => new WritePropertyToConsole(nameof_property, pre_text, post_text);
    }
}
