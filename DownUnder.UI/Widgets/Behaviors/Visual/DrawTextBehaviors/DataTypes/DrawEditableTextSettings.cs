using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors.DataTypes {
    [DataContract]
    public sealed class DrawEditableTextSettings {
        /// <summary> What kind of text is allowed to be entered in this <see cref = "DrawEditableText"/>. </summary >
        [DataMember] public ITextEntryRuleset TextEntryRules { get; set; } = TextEntryRuleSet.String;

        ///// <summary> If set to true, Enter will nit be handled and will instead confirm this <see cref="Widget"/>. </summary>
        //[DataMember] public bool EnterConfirms;

        /// <summary> If set to true, tabs will not be handled and will exit the parent <see cref="Widget"/>. </summary>
        [DataMember] public bool TabExits;

        /// <summary> If set to true the underlying <see cref="DrawText.Text"/> will be updated to match this one while editing. </summary>
        [DataMember] public bool LiveUpdate = true;

        /// <summary> When set to true the user will be required to double click to activate text editing. </summary>
        [DataMember] public bool RequireDoubleClick;

        /// <summary> When set to true all text will be highlighted on edit start. </summary>
        [DataMember] public bool HighlightTextOnActivation;
    }
}