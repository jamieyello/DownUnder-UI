using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class LoginSignal : WidgetSignal
    {
        public enum LoginErrorType
        {
            none,
            username_not_found,
            password_incorrect,
            connection_problem
        }

        public LoginSignal(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public readonly string Username;
        public readonly string Password;
    }
}
