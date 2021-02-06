using DownUnder.UI.Widgets;
using DownUnder.UIEditor.Behaviors;
using DownUnder.UIEditor.CodeGeneration;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DownUnder.UIEditor.Test;

namespace DownUnder.UIEditor.DataTypes
{
    public class UIEditorStuff
    {
        public string WorkingFileBase;
        public string WorkingDUWDFile => WorkingFileBase + ".duwd";
        public string WorkingCSFile => WorkingFileBase + ".cs";
        public string WorkingXMLFile => WorkingFileBase + ".xml";

        string widget_code_class;
        string widget_code_namespace;

        public Widget WorkingProject;
        public string PartialCodeNamespace = typeof(MyWidgetCode).Namespace;
        public string PartialCodeClass = nameof(MyWidgetCode);

        public UIEditorStuff(string[] args)
        {
            if (args.Length > 0)
            {
                Load(args[0]);
            }
        }

        static string GetBasePath(string duwd_path) {
            var maybe_dir_name = Path.GetDirectoryName(duwd_path);
            if (!(maybe_dir_name is { } dir_name))
                throw new InvalidOperationException($"Failed to find directory from path '{duwd_path}'.");

            return Path.Combine(dir_name, Path.GetFileNameWithoutExtension(duwd_path));
        }

        public void SaveTT()
        {
            if (WorkingFileBase != null) SaveProject();
            else SaveAs();
        }

        public void SaveAs()
        {
            SaveFileDialog dialogue = new SaveFileDialog();
            dialogue.Filter = "DownUnder Widget Code files (*.duwd)|*.duwd|All files (*.*)|*.*";
            dialogue.RestoreDirectory = true;

            if (dialogue.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(dialogue.FileName) != ".duwd")
                {
                    MessageBox.Show("Not a valid name. File must have the .duwd extension.");
                    return;
                }

                if (Path.GetFileNameWithoutExtension(dialogue.FileName) == "")
                {
                    MessageBox.Show("Not a valid name.");
                    return;
                }

                widget_code_namespace = "DownUnder.UI.UserCode";
                widget_code_class = Path.GetFileNameWithoutExtension(dialogue.FileName);

                WorkingFileBase = GetBasePath(dialogue.FileName);
                try { CreateNewCSFile(); }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to create cs file when generating project;\n\n" + ex.Message);
                    return;
                }
                SaveProject();
            }
        }

        void CreateNewCSFile()
        {
            File.WriteAllText(WorkingCSFile, ProjectCodeInterface.MakeCSCode(widget_code_namespace, widget_code_class));
        }

        void SaveProject()
        {
            WorkingProject.DesignerObjects.IsEditModeEnabled = false;
            WorkingProject.Position = new Point2(0f, 0f);

            try {
                WorkingProject.Behaviors.GroupBehaviors.RemovePolicy(GroupBehaviors.EditModeBehaviors);
                foreach (Widget widget in WorkingProject.AllContainedWidgets)
                {
                    if (widget.Behaviors.HasBehaviorOfType(typeof(GetEditModeDropDowns))) throw new Exception();
                }
                WorkingProject.SaveToXML(WorkingXMLFile);
                File.WriteAllText(WorkingDUWDFile, ProjectCodeInterface.MakeDuwdCode(WorkingProject, widget_code_namespace, widget_code_class));
            } catch (Exception ex) { MessageBox.Show("Failed to save project. Error;\n\n" + ex.Message); }

            WorkingProject.DesignerObjects.IsEditModeEnabled = true;
            WorkingProject.Position = new Point2(20f, 20f);
        }

        public bool Load()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DownUnder Widget Code files (*.duwd)|*.duwd|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK) return Load(dialog.FileName);

            return false;
        }

        public bool Load(string duwd_path)
        {
            if (!File.Exists(duwd_path)) throw new Exception("Specified file not found.");
            WorkingFileBase = GetBasePath(duwd_path);
            return LoadProject();
        }

        bool LoadProject()
        {
            Widget loaded_widget = null;
            string[] names;
            try {
                if (File.Exists(WorkingCSFile)) names = ProjectCodeInterface.GetCodeNames(File.ReadAllText(WorkingCSFile));
                else throw new Exception("CS file " + WorkingCSFile + " not found.");

                bool update_duwdl = false;
                if (File.Exists(WorkingXMLFile))
                {
                    loaded_widget = Widget.LoadFromXML(WorkingXMLFile);
                    loaded_widget.DesignerObjects.IsEditModeEnabled = true;
                    loaded_widget.DesignerObjects.UserRepositionPolicy = Widget.UserResizePolicyType.disallow;
                    loaded_widget.DesignerObjects.AllowedResizingDirections = Directions2D.DR;
                    loaded_widget.Position = new Point2(20f, 20f);
                    loaded_widget.Behaviors.GroupBehaviors.ImplementPolicy(GroupBehaviors.EditModeBehaviors);
                    update_duwdl = true;
                }

                WorkingProject = loaded_widget;
                widget_code_class = names[0];
                widget_code_namespace = names[1];
                if (update_duwdl) ProjectCodeInterface.UpdateDUWDL(widget_code_class, widget_code_namespace, WorkingXMLFile);

                return true;
            } catch (Exception ex) { MessageBox.Show("Failed to load project. Error;\n\n" + ex.Message); }

            return false;
        }
    }
}
