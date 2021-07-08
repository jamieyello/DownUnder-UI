using System;

namespace DownUnder.UI.Widgets.Signals {
    public sealed class CreateAccountSignal : WidgetSignal {
        public string Username { get; }
        public string Password { get; }
        public string Email { get; }

        public CreateAccountSignal(
            string username,
            string password,
            string email,
            Action<WidgetResponse> handle_response
        ) : base(handle_response) {
            Username = username;
            Password = password;
            Email = email;
        }

        public static CreateAccountSignal Empty(
            Action<WidgetResponse> handle_response
        ) =>
            new CreateAccountSignal("", "", "", handle_response);
    }
}
