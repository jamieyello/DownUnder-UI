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

        public void CheckAndAppend(StringBuilder string_builder, string text)
        {
            foreach (char c in text)
            {
                if (IsCharAllowed(c, string_builder)) string_builder.Append(c);
            }
        }

        public void ApplyFinalCheck(StringBuilder string_builder, out string text)
        {
            if (string_builder.Length == 0 && EmptyResultBecomesZero)
            {
                string_builder.Append('0');
            }

            text = string_builder.ToString();
        }

        public bool IsCharAllowed(char c, StringBuilder text)
        {
            if ( // Don't allow leading 0s (005.5)
                c == '0'
                && !AllowLeadingZeros
                && text.Length == 1
                && text.ToString() == "0"
                ) return false;

            if (c == '.' && AllowDecimal && !text.ToString().Contains('.')) return true;
            if (!char.IsNumber(c) && !AllowNonNumbers) return false;
            
            return true;
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
