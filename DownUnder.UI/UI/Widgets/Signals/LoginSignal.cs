using System;

namespace DownUnder.UI.UI.Widgets.Signals
{
    public class LoginSignal : WidgetSignal
    {
        public readonly string Username;
        public readonly string Password;

        public enum LoginErrorType
        {
            none,
            username_not_found,
            password_incorrect,
            connection_problem
        }

        public LoginSignal(string username, string password, Action<WidgetResponse> response_handle) : base(response_handle)
        {
            Username = username;
            Password = password;
        }
    }
}
