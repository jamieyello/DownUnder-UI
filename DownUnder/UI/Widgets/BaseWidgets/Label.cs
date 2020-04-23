using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

// Todo: add TextArea property

namespace DownUnder.UI.Widgets.BaseWidgets
{
    [DataContract] public class Label : Widget
    {
        #region Fields
        
        /// <summary> The <see cref="TextCursor"/> used by this <see cref="Label"/>. Handles all user text modification. </summary>
        private TextCursor _text_cursor;

        /// <summary> Used to prevent the <see cref="Parent"/> from updating its area. </summary>
        protected bool _disable_area_update = true;

        // various backings
        private string _text_backing = "";
        private bool _editing_enabled_backing = false;

        #endregion Fields

        #region Public Properties

        /// <summary> Set to enable/disable user editing of this text. </summary>
        [DataMember] public virtual bool EditingEnabled
        {
            get => _editing_enabled_backing;
            set
            {
                _text_cursor.Active = value ? false : _text_cursor.Active;
                _editing_enabled_backing = value;
            }
        }

        /// <summary> The displayed text of this <see cref="Label"/>. </summary>
        [DataMember] public string Text
        {
            get
            {
                if (_text_cursor.Active)
                {
                    return _text_cursor.Text;
                }
                else
                {
                    return _text_backing;
                }
            }
            set
            {
                _text_backing = value;
                SignalChildAreaChanged();
            }
        }

        /// <summary> What kind of text is allowed to be entered in this <see cref="Label"/>. </summary>
        [DataMember] public TextEntryRuleSet TextEntryRules { get; set; } = TextEntryRuleSet.String;

        /// <summary> Whet set to true the area of this <see cref="Label"/> will try to cover any text within. </summary>
        [DataMember] public bool ConstrainAreaToText { get; set; } = true;

        /// <summary> Area of the text within the <see cref="Label"/>. </summary>
        public RectangleF TextArea => IsGraphicsInitialized ? SpriteFont.MeasureString(Text).ToRectSize() : Position.ToRectPosition(1, 1);

        /// <summary> True if the user is editting text. </summary>
        public bool IsBeingEdited => _text_cursor == null ? false : _text_cursor.Active;

        #endregion Public Properties

        #region Constructors

        public Label(IParent parent = null)
            : base(parent)
        {
            SetDefaults();
            _disable_area_update = false;
        }

        public Label(IParent parent, string text)
            : base(parent)
        {
            SetDefaults();
            Text = text;
            _disable_area_update = false;
        }

        public Label(IParent parent, SpriteFont sprite_font, string text = "")
            : base(parent)
        {
            SpriteFont = sprite_font;
            SetDefaults();
            Text = text;
            _disable_area_update = false;
        }

        private void SetDefaults()
        {
            PaletteUsage = BaseColorScheme.PaletteCategory.text_widget;
            EnterConfirms = true;
            
            OnDraw += DrawText;
            OnSelectOff += DisableEditing;
            OnUpdate += Update;
            OnConfirm += ConfirmEdit;
            if (IsGraphicsInitialized) InitializeCursor(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeCursor;
            if (IsGraphicsInitialized) SetDefaultArea(this, EventArgs.Empty);
            else OnGraphicsInitialized += SetDefaultArea;
        }

        #endregion Constructors

        #region Overrides

        /// <summary> When set to true pressing enter while this <see cref="Label"/> is the primarily selected one will trigger confirmation events. </summary>
        public override bool EnterConfirms
        {
            get => TextEntryRules.IsSingleLine;
            set => TextEntryRules.IsSingleLine = value;
        }

        /// <summary> Minimum size allowed when setting this <see cref="Label"/>'s area. (in terms of pixels on a 1080p monitor) </summary>
        public override Point2 MinimumSize
        {
            get => !ConstrainAreaToText || !IsGraphicsInitialized ? base.MinimumSize : base.MinimumSize.Max(SpriteFont.MeasureString(Text));
            set => base.MinimumSize = value;
        }

        protected override object DerivedClone()
        {
            Label result = new Label();
                
            result.Text = Text;
            result.Name = Name;
            result.TextEntryRules = (TextEntryRuleSet)TextEntryRules.Clone();
            result.EditingEnabled = EditingEnabled;
            result.ConstrainAreaToText = ConstrainAreaToText;

            return result;
        }

        public override WidgetList Children
        {
            get => new WidgetList();
        }

        internal override void SignalChildAreaChanged()
        {
            if (_disable_area_update) return;
            base.SignalChildAreaChanged();
        }

        #endregion Overrides

        #region Event Handlers

        /// <summary>
        /// Invoked when the text has been edited.
        /// </summary>
        public EventHandler<EventArgs> OnTextEdited;

        #endregion

        #region Events

        private void Update(object sender, EventArgs args)
        {
            _text_cursor.Update();
        }

        private void DisableEditing(object sender, EventArgs args)
        {
            _text_cursor.Active = false;
        }

        private void DrawText(object sender, EventArgs args)
        {
            _text_cursor.Draw();
            SpriteBatch.DrawString(SpriteFont, Text, DrawingArea.Position.Floored(), Theme.GetText(PaletteUsage).CurrentColor);
        }

        private void ConfirmEdit(object sender, EventArgs args)
        {
            if (IsBeingEdited)
            {
                _text_cursor.ApplyFinalCheck();
                Text = _text_cursor.Text;
                DisableEditing(this, EventArgs.Empty);
                OnTextEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        private void InitializeCursor(object sender, EventArgs args)
        {
            _text_cursor = new TextCursor(this);
        }

        private void SetDefaultArea(object sender, EventArgs args)
        {
            Area = Area.WithMinimumSize(TextArea.Size);
            ParentWidget?.SignalChildAreaChanged();
        }

        protected override void HandleChildRemoval(Widget widget)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}