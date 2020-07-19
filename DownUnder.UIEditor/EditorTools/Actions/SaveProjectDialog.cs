using DownUnder.UI.Widgets.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownUnder.UIEditor.EditorTools.Actions
{
    class SaveProjectDialog : WidgetAction
    {
        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        protected override void Initialize()
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    myStream.Close();
                }
            }
            EndAction();
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return false;
        }

        protected override bool Matches(WidgetAction action)
        {
            return action is SaveProjectDialog;
        }
    }
}
