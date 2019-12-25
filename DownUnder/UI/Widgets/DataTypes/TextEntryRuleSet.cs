using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class TextEntryRuleSet
    {
        public bool AllowNonNumbers { get; set; } = false;
        public bool AllowDecimal { get; set; } = true;
        public bool AllowLeadingZeros { get; set; } = true;
        public bool EmptyResultBecomesZero { get; set; } = false;

        /// <summary>
        /// Inserts text into a stringbuilder if it follows the rules. Returns the number of inserted characters.
        /// </summary>
        /// <param name="string_builder"></param>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int CheckAndInsert(StringBuilder string_builder, string text, int index)
        {
            int result = 0;
            foreach (char c in text)
            {
                if (IsCharAllowed(c, string_builder, index))
                {
                    string_builder.Insert(index++, c);
                    result++;
                }
            }
            return result;
        }

        public bool IsCharAllowed(char c, StringBuilder text, int index)
        {
            if ( // Don't allow leading 0s (005.5)
                c == '0'
                && !AllowLeadingZeros
                && text.Length >= 1
                && text[0] == '0'
                && (index == 0 || index == 1)
                ) return false;

            if (c == '.' && AllowDecimal && !text.ToString().Contains('.')) return true;
            if (!char.IsNumber(c) && !AllowNonNumbers) return false;

            return true;
        }

        public void ApplyFinalCheck(StringBuilder string_builder, out string text)
        {
            if (string_builder.Length == 0 && EmptyResultBecomesZero)
            {
                string_builder.Append('0');
            }

            text = string_builder.ToString();
        }

        public static TextEntryRuleSet Integer
        {
            get
            {
                return new TextEntryRuleSet()
                {
                    AllowNonNumbers = false,
                    AllowDecimal = false,
                    AllowLeadingZeros = false,
                    EmptyResultBecomesZero = true
                };
            }
        }

        public static TextEntryRuleSet Double
        {
            get
            {
                return new TextEntryRuleSet()
                {
                    AllowNonNumbers = false,
                    AllowDecimal = true,
                    AllowLeadingZeros = false,
                    EmptyResultBecomesZero = true
                };
            }
        }

        public static TextEntryRuleSet String
        {
            get
            {
                return new TextEntryRuleSet()
                {
                    AllowNonNumbers = true,
                    AllowDecimal = true,
                    AllowLeadingZeros = true,
                    EmptyResultBecomesZero = false
                };
            }
        }
    }
}
