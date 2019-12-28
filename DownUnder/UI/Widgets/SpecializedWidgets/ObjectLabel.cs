using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.SpecializedWidgets
{
    /// <summary>
    /// This label represents any object, if object implements ILabelEdit then this can allow editing by the user during runtime.
    /// </summary>
    class ObjectLabel : Label
    {
        #region Private Fields

        private bool _text_editable = false;

        #endregion

        #region Public Properties

        #endregion

        #region Constructors

        public ObjectLabel(IWidgetParent parent, SpriteFont sprite_font, object obj)
            : base(parent, sprite_font, obj.ToString())
        {
            SetDefaults(obj);
        }

        private void SetDefaults(object obj)
        {
            OnConfirm += Confirm;

            if (obj is string)
            {
                _text_editable = true;
                TextEntryRules = TextEntryRuleSet.String;
            }
            else if (obj.GetType().IsNumeric())
            {
                _text_editable = true;
                if (obj.GetType().IsIntegral())
                {
                    Console.WriteLine($"object {obj} was detected as integral");
                    TextEntryRules = TextEntryRuleSet.Integer;
                }
                else
                {
                    Console.WriteLine($"object {obj} was detected a non-integral number");
                    TextEntryRules = TextEntryRuleSet.Double;
                }
            }
        }

        #endregion

        #region Events

        private void Confirm(object sender, EventArgs args)
        {
            
        }

        #endregion

        #region Overrides

        public override bool EditingEnabled
        {
            get => _text_editable && base.EditingEnabled;
            set
            {
                base.EditingEnabled = value;
            }
        }

        #endregion
    }
}
