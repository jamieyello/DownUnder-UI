using System;
using System.Reflection;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Outputs the ToString() value of a given <see cref="Widget"/> property to the console. </summary>
    public class WritePropertyToConsole : WidgetBehavior
    {
        private readonly string _nameof_property;
        private string _pre_text;
        private string _post_text;
        private PropertyInfo _property_info;

        public WritePropertyToConsole(string nameof_property, string pre_text = "", string post_text = "") {
            _nameof_property = nameof_property;
            _pre_text = pre_text;
            _post_text = post_text;
        }

        public override object Clone() => new WritePropertyToConsole(_nameof_property, _pre_text, _post_text);
        
        protected override void ConnectToParent() {
            _property_info = typeof(Widget).GetProperty(_nameof_property);
            Parent.OnUpdate += WriteLine;
        }

        protected override void DisconnectFromParent() => Parent.OnUpdate -= WriteLine;

        private void WriteLine(object sender, EventArgs args) => Console.WriteLine(_pre_text + _property_info.GetValue(Parent).ToString() + _post_text);
    }
}
