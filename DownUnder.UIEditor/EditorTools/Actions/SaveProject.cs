using DownUnder.Content.Utilities.Serialization;
using DownUnder.UI.Widgets.Actions;
using DownUnder.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownUnder.UIEditor.EditorTools.Actions
{
    public class SaveProject : WidgetAction
    {
        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        protected override void Initialize()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XmlHelper.ToXmlFile(((MainWindow)Parent.ParentWindow).editor_objects.project, saveFileDialog1.FileName);
            }
            EndAction();
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return false;
        }

        protected override bool Matches(WidgetAction action)
        {
            return action is SaveProject;
        }
    }
}
