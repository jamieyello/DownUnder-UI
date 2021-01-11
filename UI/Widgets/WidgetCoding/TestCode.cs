using DownUnder.UI.Widgets.WidgetCoding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public partial class TestCode : WidgetCode
    {
        public TestCode()
        {
            InitializeComponent();
        }

        void MyWidget_OnClick(object sender, EventArgs args)
        {
            Debug.WriteLine("Button clicked");
        }
    }
}
