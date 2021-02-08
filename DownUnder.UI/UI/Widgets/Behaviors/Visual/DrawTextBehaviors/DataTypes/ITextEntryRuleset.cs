using System;
using System.Text;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual.DrawTextBehaviors.DataTypes
{
    /// <summary> Controls what text is entered from the text the user gives. </summary>
    public interface ITextEntryRuleset : ICloneable
    {
        int CheckAndInsert(StringBuilder string_builder, string text, int index);
        void ApplyFinalCheck(StringBuilder string_builder);
    }
}
