using System.Text;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual.DrawTextBehaviors.DataTypes {
    public sealed class TextEntryRuleSet : ITextEntryRuleset {
        bool AllowNonNumbers { get; set; } // Update clone when adding new properties
        bool AllowDecimal { get; set; } = true;
        bool AllowLeadingZeros { get; set; } = true;
        bool EmptyResultBecomesZero { get; set; }
        bool IsSingleLine { get; set; }
        bool AllowTab { get; } = true;
        string TabReplacement { get; } = "    ";

        /// <summary> Inserts text into a stringbuilder if it follows the rules. Returns the number of inserted characters. </summary>
        public int CheckAndInsert(StringBuilder string_builder, string text, int index) {
            var result = 0;
            foreach (var c in text) {
                if (!IsCharAllowed(c, string_builder, index))
                    continue;

                if (c == '\t') {
                    string_builder.Insert(index, TabReplacement);
                    index += TabReplacement.Length;
                    result += TabReplacement.Length;
                    continue;
                }

                string_builder.Insert(index++, c);
                result += 1;
            }
            return result;
        }

        bool IsCharAllowed(char c, StringBuilder text, int index) {
            if ( // Don't allow leading 0s (005.5)
                !AllowLeadingZeros
                && c == '0'
                && text.Length >= 1
                && text[0] == '0'
                && (index == 0 || index == 1)
            ) return false;

            if (c == '.' && AllowDecimal && !text.ToString().Contains('.'))
                return true;

            if (!char.IsNumber(c) && !AllowNonNumbers)
                return false;

            if (IsSingleLine && c == '\n')
                return false;

            if (!AllowTab && c == '\t')
                return false;

            return true;
        }

        public void ApplyFinalCheck(StringBuilder string_builder) {
            if (string_builder.Length == 0 && EmptyResultBecomesZero)
                string_builder.Append('0');

            if (!AllowNonNumbers && string_builder.Length > 0 && string_builder[0] == '.')
                string_builder.Insert(0, '0');
        }

        public static TextEntryRuleSet Integer => new TextEntryRuleSet {
            AllowNonNumbers = false,
            AllowDecimal = false,
            AllowLeadingZeros = false,
            EmptyResultBecomesZero = true
        };

        public static TextEntryRuleSet Double => new TextEntryRuleSet {
            AllowNonNumbers = false,
            AllowDecimal = true,
            AllowLeadingZeros = false,
            EmptyResultBecomesZero = true
        };

        public static TextEntryRuleSet String => new TextEntryRuleSet {
            AllowNonNumbers = true,
            AllowDecimal = true,
            AllowLeadingZeros = true,
            EmptyResultBecomesZero = false
        };

        public object Clone() =>
            new TextEntryRuleSet {
                AllowNonNumbers = AllowNonNumbers,
                AllowDecimal = AllowDecimal,
                AllowLeadingZeros = AllowLeadingZeros,
                EmptyResultBecomesZero = EmptyResultBecomesZero,
                IsSingleLine = IsSingleLine
            };
    }
}