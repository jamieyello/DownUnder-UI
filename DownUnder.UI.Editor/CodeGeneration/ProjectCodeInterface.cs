using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.WidgetCoding;
using DownUnder.UI.Utilities.Serialization;

namespace DownUnder.UI.Editor.CodeGeneration {
    public static class ProjectCodeInterface {
        const string DefaultDuwdCode = "// This is generated code. Modifications will be overwritten.\nusing DownUnder.UI.Widgets;\n\nnamespace SafeNamespaceNameDUUI\n{\n    public partial class SafeWidgetCodeClassName : WidgetCode\n    {\nSafePropertiesName\n    }\n}";
        const string DefaultCSCode = "using DownUnder.UI.Widgets;\nusing DownUnder.UI.Widgets.WidgetCoding;\nusing System;\nusing System.Collections.Generic;\nusing System.Text;\n\nnamespace SafeNamespaceNameDUUI\n{\n    public partial class SafeWidgetCodeClassName : WidgetCode\n    {\n        public SafeWidgetCodeClassName() : base()\n        {\n        }\n    }\n}";

        public static string MakeCSCode(
            string code_namespace,
            string class_name
        ) {
            var result = new StringBuilder(DefaultCSCode);

            if (class_name.IsNullOrEmpty())
                throw new Exception("Class name cannot be empty.");

            if (code_namespace.IsNullOrEmpty())
                throw new Exception("Namespace name cannot be empty.");

            result.Replace("SafeWidgetCodeClassName", class_name);
            result.Replace("SafeNamespaceNameDUUI", code_namespace);

            return result.ToString();
        }

        public static string MakeDuwdCode(
            Widget base_widget,
            Type widget_code_type
        ) {
            if (!typeof(WidgetCode).IsAssignableFrom(widget_code_type))
                throw new Exception($"{nameof(WidgetCode)} is not assignable from passed type.");

            if (widget_code_type.Namespace == null)
                throw new Exception($"{nameof(WidgetCode)} does not have a usable namespace, cannot make partial code.");

            return MakeDuwdCode(base_widget, widget_code_type.Namespace, widget_code_type.Name);
        }

        public static string MakeDuwdCode(
            Widget base_widget,
            string code_namespace,
            string class_name
        ) {
            var result = new StringBuilder(DefaultDuwdCode);

            if (class_name.IsNullOrEmpty())
                throw new Exception("Class name cannot be empty.");

            if (code_namespace.IsNullOrEmpty())
                throw new Exception("Namespace name cannot be empty.");

            result.Replace("SafeWidgetCodeClassName", class_name);
            result.Replace("SafeNamespaceNameDUUI", code_namespace);

            var w_properties = new StringBuilder();
            foreach (var widget in base_widget.AllContainedWidgets) {
                if (widget == base_widget)
                    continue;
                w_properties.Append($"        /// <summary> Auto generated accessor for {widget.Name}. </summary>\n        public Widget {widget.Name} => Base[\"{widget.Name}\"];\n");
            }
            result.Replace("SafePropertiesName", w_properties.ToString());
            return result.ToString();
        }

        public static bool CodeHasEvent(
            StringBuilder cs,
            string widget_name,
            string event_name
        ) =>
            cs
            .ToString()
            .Replace(" ", "")
            .Contains($"void{widget_name}_{event_name}");

        public static void AddEventVoid(
            StringBuilder cs,
            string widget_name,
            string event_name
        ) {
            var args_type_name = typeof(Widget).GetEventHandlerTypeOrThrow(event_name);

            // Look for the second last closing bracket
            var closing_bracket_index = -1;
            var closing_bracket_count = 0;

            for (var i = cs.Length - 1; i >= 0; i--) {
                if (cs[i] == '}')
                    closing_bracket_count += 1;

                if (closing_bracket_count != 2)
                    continue;

                var indent_count = 0;
                while (cs[i - 1 - indent_count] == ' ' && indent_count < 4)
                    indent_count += 1;

                closing_bracket_index = i - indent_count;
                break;
            }

            cs.Insert(
                closing_bracket_index,
                $"\n        void {widget_name}_{event_name}(object sender, {args_type_name} args)\n        {{\n            \n        }}\n"
            );
        }

        /// <summary> Returns [0] the class name, and [1] the namespace name. </summary>
        public static string[] GetCodeNames(string cs) {
            var result = new string[2];

            // Find class name
            var keyword = "partial class ";
            var start_index = cs.IndexOf(keyword);
            if (start_index == -1)
                throw new Exception("Could not find the string \"" + keyword + "\" in given cs code.");
            start_index += keyword.Length;
            var end_index = cs.IndexOf(' ', start_index);
            result[0] = cs.Substring(start_index, end_index - start_index);

            // Find namespace name
            keyword = "namespace ";
            start_index = cs.IndexOf(keyword);
            if (start_index == -1) throw new Exception("Could not find the string \"" + keyword + "\" in given cs code.");
            start_index += keyword.Length;
            end_index = cs.IndexOf('{', start_index);
            result[1] = cs.Substring(start_index, end_index - start_index).Replace("\n", "").Replace("\r", "");

            return result;
        }

        public static bool UpdateDUWDL(
            string class_name,
            string namespace_name,
            string xml_path
        ) {
            var duwdl_path = FindParentFile(xml_path, ".duwdl");
            var csproj_path = FindParentFile(xml_path, ".csproj");

            if (duwdl_path == "" || csproj_path == "")
                return false;

            var maybe_project_folder = Path.GetDirectoryName(csproj_path);
            if (!(maybe_project_folder is { } project_folder))
                throw new InvalidOperationException($"Failed to find directory with path '{csproj_path}'.");

            var key = namespace_name + " " + class_name;
            var value = Path.GetRelativePath(project_folder, xml_path);
            var duwdl = XmlHelper.FromXmlFile<Dictionary<string, string>>(duwdl_path);

            if (duwdl.ContainsKey(key))
                duwdl.Remove(key);

            duwdl.Add(key, value);
            XmlHelper.ToXmlFile(duwdl, duwdl_path);
            return true;
        }

        static string FindParentFile(
            string file_path,
            string target_parent_extension
        ) {
            var root = Path.GetPathRoot(file_path);
            var folder = Path.GetDirectoryName(file_path);

            while (folder != root) {
                var files = Directory.GetFiles(folder);
                foreach (var file in files) {
                    if (Path.GetExtension(file) != target_parent_extension)
                        continue;

                    return file;
                }
                folder = Path.GetDirectoryName(folder);
            }

            return "";
        }
    }
}
