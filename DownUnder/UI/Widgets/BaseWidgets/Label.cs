using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

// Todo: add TextArea property

namespace DownUnder.UI.Widgets.BaseWidgets {
    [DataContract] public class Label : Widget {
        /// <summary> The <see cref="TextCursor"/> used by this <see cref="Label"/>. Handles all user text modification. </summary>
        private TextCursor _text_cursor;
        /// <summary> Used to prevent the <see cref="Parent"/> from updating its area. </summary>
        protected bool _disable_area_update = true;
        private string _text_backing = "";
        private bool _editing_enabled_backing = false;
        /// <summary> Invoked when the text has been edited. </summary>
        public EventHandler<EventArgs> OnTextEdited;

        /// <summary> What kind of text is allowed to be entered in this <see cref="Label"/>. </summary>
        [DataMember] public TextEntryRuleSet TextEntryRules { get; set; } = TextEntryRuleSet.String;
        /// <summary> Whet set to true the area of this <see cref="Label"/> will try to cover any text within. </summary>
        [DataMember] public bool ConstrainAreaToText { get; set; } = true;
        /// <summary> Area of the text within the <see cref="Label"/>. </summary>
        public RectangleF TextArea => IsGraphicsInitialized ? SpriteFont.MeasureString(Text).ToRectSize() : Position.ToRectPosition(1, 1);
        /// <summary> True if the user is editting text. </summary>
        public bool IsBeingEdited => _text_cursor == null ? false : _text_cursor.Active;

        /// <summary> Set to enable/disable user editing of this text. </summary>
        [DataMember] public virtual bool EditingEnabled {
            get => _editing_enabled_backing;
            set {
                if (_text_cursor != null) _text_cursor.Active = value ? false : _text_cursor.Active;
                _editing_enabled_backing = value;
            }
        }

        /// <summary> The displayed text of this <see cref="Label"/>. </summary>
        [DataMember] public string Text {
            get => _text_cursor != null && _text_cursor.Active ? _text_cursor.Text : _text_backing;
            set {
                _text_backing = value;
                SignalChildAreaChanged();
            }
        }

        public Label(IParent parent = null) : base(parent) {
            SetDefaults();
            _disable_area_update = false;
        }

        public Label(IParent parent, string text) : base(parent) {
            SetDefaults();
            Text = text;
            _disable_area_update = false;
        }

        public Label(IParent parent, SpriteFont sprite_font, string text = "") : base(parent) {
            SpriteFont = sprite_font;
            SetDefaults();
            Text = text;
            _disable_area_update = false;
        }

        private void SetDefaults() {
            PaletteUsage = BaseColorScheme.PaletteCategory.text_widget;
            EnterConfirms = true;
            
            OnDraw += DrawText;
            OnSelectOff += (obj, sender) => _text_cursor.Active = false;
            OnUpdate += (obj, sender) => _text_cursor.Update();
            OnConfirm += ConfirmEdit;
            if (IsGraphicsInitialized) _text_cursor = new TextCursor(this);
            else OnGraphicsInitialized += (obj, sender) => _text_cursor = new TextCursor(this);
            if (IsGraphicsInitialized) SetDefaultArea(this, EventArgs.Empty);
            else OnGraphicsInitialized += SetDefaultArea;
        }

        public override WidgetList Children => new WidgetList();
        protected override void HandleChildDelete(Widget widget) => throw new NotImplementedException();

        /// <summary> When set to true pressing enter while this <see cref="Label"/> is the primarily selected one will trigger confirmation events. </summary>
        public override bool EnterConfirms {
            get => TextEntryRules.IsSingleLine;
            set => TextEntryRules.IsSingleLine = value;
        }

        /// <summary> Minimum size allowed when setting this <see cref="Label"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public override Point2 MinimumSize {
            get => !ConstrainAreaToText || !IsGraphicsInitialized  ? base.MinimumSize : base.MinimumSize.Max(SpriteFont.MeasureString(Text));
            set => base.MinimumSize = value;
        }
        
        protected override object DerivedClone() {
            Label result = new Label();
                
            result.Text = Text;
            result.Name = Name;
            result.TextEntryRules = (TextEntryRuleSet)TextEntryRules.Clone();
            result.EditingEnabled = EditingEnabled;
            result.ConstrainAreaToText = ConstrainAreaToText;

            return result;
        }

        internal override void SignalChildAreaChanged() {
            if (_disable_area_update) return;
            base.SignalChildAreaChanged();
        }

        private void DrawText(object sender, EventArgs args) {
            _text_cursor.Draw();
            SpriteBatch.DrawString(SpriteFont, Text, DrawingArea.Position.Floored(), Theme.GetText(PaletteUsage).CurrentColor);
        }

        private void ConfirmEdit(object sender, EventArgs args) {
            if (IsBeingEdited) {
                _text_cursor.ApplyFinalCheck();
                Text = _text_cursor.Text;
                _text_cursor.Active = false;
                OnTextEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetDefaultArea(object sender, EventArgs args) {
            Area = Area.WithMinimumSize(TextArea.Size);
            ParentWidget?.SignalChildAreaChanged();
        }
    }
}