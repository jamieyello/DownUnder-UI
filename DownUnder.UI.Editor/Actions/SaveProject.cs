//using DownUnder.Content.Utilities.Serialization;
//using DownUnder.UI.Widgets;
//using DownUnder.UI.Widgets.Actions;
//using DownUnder.Widgets;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace DownUnder.UIEditor.EditorTools.Actions
//{
//    public class SaveProject : WidgetAction
//    {
//        protected override void ConnectEvents() { }

//        protected override void DisconnectEvents() { }

//        protected override void Initialize()
//        {
//            SaveFileDialog save_dialog = new SaveFileDialog
//            {
//                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
//                FilterIndex = 2,
//                RestoreDirectory = true
//            };

//            if (save_dialog.ShowDialog() == DialogResult.OK)
//            {
//                Widget project = ((MainWindow)Parent.ParentWindow).editor_objects.project;
//                project.DesignerObjects.IsEditModeEnabled = false;
//                XmlHelper.ToXmlFile(project, save_dialog.FileName);
//                project.DesignerObjects.IsEditModeEnabled = true;
//            }
//            EndAction();
//        }

//        protected override bool InterferesWith(WidgetAction action)
//        {
//            return false;
//        }

//        protected override bool Matches(WidgetAction action)
//        {
//            return action is SaveProject;
//        }
//    }
//}
