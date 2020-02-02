using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using DownUnder.UI.Widgets.BaseWidgets;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.SpecializedWidgets
{
    /// <summary>
    /// This label represents any object, if object implements ILabelEdit then this can allow editing by the user during runtime.
    /// </summary>
    class PropertyGridObjectLabel : Label
    {
        #region Private Fields

        private bool _text_editable = false;

        private bool _grid_insert = false;

        object obj;

        #endregion
        
        #region Public Properties

        #endregion

        #region Constructors

        public PropertyGridObjectLabel(PropertyGrid parent, SpriteFont sprite_font, object obj)
            : base(parent, sprite_font, obj.ToString())
        {
            SetDefaults(obj);
        }

        private void SetDefaults(object obj)
        {
            OnConfirm += Confirm;
            OnDoubleClick += DoubleClickAction;
            TextEntryRules.IsSingleLine = true;
            EnterConfirms = true;
            
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
                    //Console.WriteLine($"object {obj} was detected as integral");
                    TextEntryRules = TextEntryRuleSet.Integer;
                }
                else
                {
                    //Console.WriteLine($"object {obj} was detected a non-integral number");
                    TextEntryRules = TextEntryRuleSet.Double;
                }
            }
            else
            {
                this.obj = obj;
                _text_editable = false;
                _grid_insert = true;
                EditingEnabled = false;
            }
        }

        #endregion

        #region Events

        private void Confirm(object sender, EventArgs args)
        {
            
        }

        private void DoubleClickAction(object sender, EventArgs args)
        {
            if (_grid_insert)
            {
                var property_grid = new PropertyGrid(Parent, obj);
                Point index = ((PropertyGrid)Parent).IndexOf(this);
                if (index.Y == -1) throw new Exception("Error inserting new property grid.");
                ((PropertyGrid)Parent).InsertDivider(property_grid, index.Y);
            }
        }

        #endregion

        #region Overrides

        public override bool EditingEnabled
        {
            get => base.EditingEnabled && _text_editable;
            set
            {
                if (_text_editable) base.EditingEnabled = value;
            }
        }

        #endregion
    }
}
