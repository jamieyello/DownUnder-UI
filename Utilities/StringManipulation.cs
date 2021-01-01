using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DownUnder.Utilities
{
    public static class StringManipulation
    {
        //https://blogs.msdn.microsoft.com/csharpfaq/2004/03/12/what-character-escape-sequences-are-available/
        /// <summary>
        /// Replaces all characters characters in a string that need /escapes with their literal string equivelent.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String InsertLiterals(string text)
        {
            return text
                .Replace("\"", "\\\"")
                .Replace("\\", "\\\\")
                .Replace("\0", "\\0")
                .Replace("\a", "\\a")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\v", "\\v");
        }

        // https://stackoverflow.com/questions/532892/can-i-multiply-a-string-in-c
        public static string Multiply(this string source, int multiplier)
        {
            return Enumerable.Range(1, multiplier)
                     .Aggregate(new StringBuilder(multiplier * source.Length),
                           (sb, n) => sb.Append(source))
                     .ToString();
        }

        public static void TrimEnd(ref StringBuilder string_builder, char[] char_array)
        {
            foreach (char char_ in char_array)
            {
                TrimEnd(ref string_builder, char_);
            }
        }

        public static void TrimEnd(ref StringBuilder string_builder, char char_)
        {
            while (string_builder.Length != 0 && string_builder[string_builder.Length - 1] == char_)
            {
                string_builder.Length--;
            }
        }

        public static String StringListToString(List<String> string_list)
        {
            String result = "";
            foreach (var string_ in string_list)
            {
                result += string_ + "\n";
            }
            return result;
        }

        public static string IndentCSCode(string code)
        {
            StringBuilder sb_code = new StringBuilder(code);
            IndentCSCode(sb_code);
            return sb_code.ToString();
        }

        public static void IndentCSCode(StringBuilder code)
        {
            int tab_count = 0;
            // Parse string in reverse
            for (int i = code.Length - 1; i > -1; i--)
            {
                if (code[i] == '}') tab_count++;
                if (code[i] == '\n') code.Insert(i + 1, "    ", tab_count);
                if (code[i] == '{')
                {
                    if (tab_count > 0) tab_count--;
                }
            }
        }
    }
}