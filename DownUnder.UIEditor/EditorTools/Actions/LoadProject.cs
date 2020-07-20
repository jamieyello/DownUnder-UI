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
    public class LoadProject : WidgetAction
    {
        protected override void ConnectEvents()
        {
            
        }

        protected override void DisconnectEvents()
        {
            
        }

        protected override void Initialize()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
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
            return action is LoadProject;
        }
    }
}
