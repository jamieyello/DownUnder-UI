using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework.Graphics;
using System;
using DownUnder.UI.Widgets.BaseWidgets;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.SpecializedWidgets
{
    /// <summary> This label represents any object. Can be edited by the user during runtime. </summary>
    class ObjectLabel : Label {
        object _obj;
        private bool _text_editable = false;
        private bool _grid_insert_backing = false;
        private Grid _dropdown;

        private bool _GridInsert {
            get => _grid_insert_backing && Parent != null && Parent is Grid && PropertyGrid.IsCompatible(_obj);
            set => _grid_insert_backing = value;
        }

        public ObjectLabel(Widget parent, SpriteFont sprite_font, object obj) : base(parent, sprite_font, obj.ToString()) => SetDefaults(obj);
        private void SetDefaults(object obj) {
            OnConfirm += Confirm;
            OnDoubleClick += DoubleClickAction;
            TextEntryRules.IsSingleLine = true;
            EnterConfirms = true;
            
            if (obj is string) {
                _text_editable = true;
                TextEntryRules = TextEntryRuleSet.String;
            }
            else if (obj.GetType().IsNumeric()) {
                _text_editable = true;
                if (obj.GetType().IsIntegral()) {
                    //Console.WriteLine($"object {obj} was detected as integral");
                    TextEntryRules = TextEntryRuleSet.Integer;
                }
                else {
                    //Console.WriteLine($"object {obj} was detected a non-integral number");
                    TextEntryRules = TextEntryRuleSet.Double;
                }
            }
            else {
                _obj = obj;
                _text_editable = false;
                _GridInsert = true;
                EditingEnabled = false;
            }
        }
        
        private void Confirm(object sender, EventArgs args) { }

        private void DoubleClickAction(object sender, EventArgs args) {
            if (!_GridInsert) return;
            if (_dropdown == null)
            {
                Point index = ((PropertyGrid)Parent).IndexOf(this);
                if (index.Y == -1) throw new Exception("Error inserting new property grid, could not find self in parent.");

                _dropdown = new Grid(Parent, 2, 1);
                _dropdown.GetCell(0, 0).Width = 16f;
                _dropdown.GetCell(0, 0).IsFixedWidth = true;
                _dropdown.SetCell(1, 0, new PropertyGrid(_dropdown, _obj));

                ((PropertyGrid)Parent).InsertDivider(_dropdown, index.Y + 1);
            }
            else {
                if (!((PropertyGrid)ParentWidget).RemoveDivider(_dropdown)) throw new Exception("Failed to remove property grid from parent.");
                _dropdown.Dispose();
                _dropdown = null;
            }
        }
        
        public override bool EditingEnabled
        {
            get => base.EditingEnabled && _text_editable;
            set {
                if (_text_editable) base.EditingEnabled = value;
            }
        }
    }
}
