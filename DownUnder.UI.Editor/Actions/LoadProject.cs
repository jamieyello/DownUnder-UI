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
//    public class LoadProject : WidgetAction
//    {
//        protected override void ConnectEvents()
//        {
            
//        }

//        protected override void DisconnectEvents()
//        {
            
//        }

//        protected override void Initialize()
//        {
//            var fileContent = string.Empty;
//            var filePath = string.Empty;

//            using (OpenFileDialog openFileDialog = new OpenFileDialog())
//            {
//                //openFileDialog.InitialDirectory = "c:\\";
//                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
//                openFileDialog.FilterIndex = 2;
//                openFileDialog.RestoreDirectory = true;

//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    //Get the path of specified file
//                    filePath = openFileDialog.FileName;

//                    Widget widget = XmlHelper.FromXmlFile<Widget>(filePath);
//                    Console.WriteLine("LoadProject children count " + widget.Children.Count);

//                    ((MainWindow)Parent.ParentWindow).SetProject(widget);
//                }
//            }

//            EndAction();
//        }

//        protected override bool InterferesWith(WidgetAction action)
//        {
//            return false;
//        }

//        protected override bool Matches(WidgetAction action)
//        {
//            return action is LoadProject;
//        }
//    }
//}
