using System;
using System.Linq;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors
{
    public class TextEntryRuleSet : ITextEntryRuleset
    {
        public bool AllowNonNumbers { get; set; } = false; // Update clone when adding new properties
        public bool AllowDecimal { get; set; } = true;
        public bool AllowLeadingZeros { get; set; } = true;
        public bool EmptyResultBecomesZero { get; set; } = false;
        public bool IsSingleLine { get; set; } = false;
        public bool AllowTab { get; set; } = true;
        public string TabReplacement { get; set; } = "    ";

        /// <summary> Inserts text into a stringbuilder if it follows the rules. Returns the number of inserted characters. </summary>
        public int CheckAndInsert(StringBuilder string_builder, string text, int index) {
            int result = 0;
            foreach (char c in text) {
                if (IsCharAllowed(c, string_builder, index)) {
                    if (c == '\t') {
                        string_builder.Insert(index, TabReplacement);
                        index += TabReplacement.Length;
                        result += TabReplacement.Length;
                    }
                    else
                    {
                        string_builder.Insert(index++, c);
                        result++;
                    }
                }
            }
            return result;
        }

        public bool IsCharAllowed(char c, StringBuilder text, int index) {
            if ( // Don't allow leading 0s (005.5)
                !AllowLeadingZeros
                && c == '0'
                && text.Length >= 1
                && text[0] == '0'
                && (index == 0 || index == 1)
                ) return false;

            if (c == '.' && AllowDecimal && !text.ToString().Contains('.')) return true;
            if (!char.IsNumber(c) && !AllowNonNumbers) return false;
            if (IsSingleLine && c == '\n') return false;
            if (!AllowTab && c == '\t') return false;

            return true;
        }

        public void ApplyFinalCheck(StringBuilder string_builder) {
            if (string_builder.Length == 0 && EmptyResultBecomesZero) string_builder.Append('0');
            if (!AllowNonNumbers && string_builder.Length > 0 && string_builder[0] == '.') string_builder.Insert(0, '0');
        }

        public static TextEntryRuleSet Integer => new TextEntryRuleSet() {
            AllowNonNumbers = false,
            AllowDecimal = false,
            AllowLeadingZeros = false,
            EmptyResultBecomesZero = true
        };
        
        public static TextEntryRuleSet Double => new TextEntryRuleSet() {
            AllowNonNumbers = false,
            AllowDecimal = true,
            AllowLeadingZeros = false,
            EmptyResultBecomesZero = true
        };

        public static TextEntryRuleSet String => new TextEntryRuleSet() {
            AllowNonNumbers = true,
            AllowDecimal = true,
            AllowLeadingZeros = true,
            EmptyResultBecomesZero = false
        };

        public object Clone()
        {
            TextEntryRuleSet result = new TextEntryRuleSet();

            result.AllowNonNumbers = AllowNonNumbers;
            result.AllowDecimal = AllowDecimal;
            result.AllowLeadingZeros = AllowLeadingZeros;
            result.EmptyResultBecomesZero = EmptyResultBecomesZero;
            result.IsSingleLine = IsSingleLine;

            return result;
        }
    }
}
