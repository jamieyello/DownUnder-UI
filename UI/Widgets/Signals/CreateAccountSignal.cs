using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class CreateAccountSignal : WidgetSignal
    {
        public CreateAccountSignal(string username, string password, string email, Action<WidgetResponse> handle_response) : base(handle_response)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public readonly string Username;
        public readonly string Password;
        public readonly string Email;

        public static CreateAccountSignal Empty(Action<WidgetResponse> handle_response) => new CreateAccountSignal("", "", "", handle_response);
    }
}
