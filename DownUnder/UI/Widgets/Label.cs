using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

// Todo: add TextArea property

namespace DownUnder.UI.Widgets
{
    [DataContract]
    public class Label : Widget
    {
        #region Fields

        private bool _is_being_edited;
        private string _text_backing = "";
        private readonly StringBuilder _edit_text = new StringBuilder();
        private bool _text_edit_enabled = false;
        private readonly UIPalette _text_edit_background_pallete = new UIPalette();
        private TextCursor _text_cursor;

        #endregion Fields

        #region Public Properties

        [DataMember] public virtual bool EditingEnabled
        {
            get => _text_edit_enabled;
            set
            {
                _is_being_edited = value ? false : _is_being_edited;
                _text_edit_enabled = value;
            }
        }

        [DataMember] public string Text
        {
            get
            {
                if (_is_being_edited)
                {
                    return _edit_text.ToString();
                }
                else
                {
                    return _text_backing;
                }
            }
            set
            {
                _text_backing = value;
                UpdateArea(true);
            }
        }

        [DataMember] public TextEntryRuleSet TextEntryRules { get; set; } = TextEntryRuleSet.String;

        /// <summary>
        /// Area of the text within the label.
        /// </summary>
        public RectangleF TextArea
        {
            get
            {
                if (IsGraphicsInitialized)
                {
                    return new RectangleF(Position, DrawingData.sprite_font.MeasureString(Text));
                }
                
                return new RectangleF(Position, new Point2(1, 1));
            } 
        }

        public RectangleF TextAreaInWindow
        {
            get
            {
                RectangleF result = TextArea;
                result.Offset(PositionInWindow);
                return result;
            }
        }

        public bool IsBeingEdited
        {
            get => _is_being_edited;
        }

        #endregion Public Properties

        #region Constructors

        public Label(IWidgetParent parent = null)
            : base(parent)
        {
            SetDefaults();
            if (IsGraphicsInitialized)
            {
                InitializeGraphics();
            }
        }

        public Label(IWidgetParent parent, SpriteFont sprite_font, string text = "")
            : base(parent)
        {
            DrawingData.sprite_font = sprite_font;
            SetDefaults();
            Text = text;
            if (IsGraphicsInitialized)
            {
                InitializeGraphics();
            }
        }

        public Label(IWidgetParent parent, string text)
            : base(parent)
        {
            SetDefaults();
            Text = text;
            if (IsGraphicsInitialized)
            {
                InitializeGraphics();
            }
        }

        private void SetDefaults()
        {
            DrawBackground = true;
            BackgroundColor.DefaultColor = Color.LightGray;
            _text_edit_background_pallete.DefaultColor = Color.White;
            _text_edit_background_pallete.HoveredColor = Color.Yellow;
            _text_edit_background_pallete.ForceComplete();
            Area = new RectangleF(0, 0, 50, 20);

            OnDoubleClick += ActivateEditing;
            OnDraw += DrawText;
            OnSelectOff += DisableEditing;
            OnUpdate += Update;
            OnConfirm += Confirm;
            OnGraphicsInitialized += InitializeTextCursor;
        }

        #endregion Constructors

        #region Overrides

        public override UIPalette BackgroundColor
        {
            get
            {
                if (_is_being_edited)
                {
                    return _text_edit_background_pallete;
                }
                else
                {
                    return base.BackgroundColor;
                }
            }
            protected set => base.BackgroundColor = value;
        }

        public override bool DrawBackground
        {
            get
            {
                if (_is_being_edited)
                {
                    return true;
                }
                else
                {
                    return base.DrawBackground;
                }
            }
            set => base.DrawBackground = value;
        }

        protected override object DerivedClone()
        {
            Label result = new Label
            {
                Text = Text,
                Name = Name
            };
            return result;
        }

        public override List<Widget> GetChildren()
        {
            return new List<Widget>();
        }

        #endregion Overrides

        #region Event Handlers

        // unused atm
        public EventHandler<EventArgs> TextEdited;

        #endregion

        #region Events

        private void Update(object sender, EventArgs args)
        {
            _text_cursor.Update();
        }

        private void DisableEditing(object sender, EventArgs args)
        {
            if (!_is_being_edited)
            {
                return;
            }

            _is_being_edited = false;
        }

        private void DrawText(object sender, EventArgs args)
        {
            _text_cursor.Draw();
            DrawingData.sprite_batch.DrawString(DrawingData.sprite_font, Text, PositionInWindow.ToVector2().Floored(), TextColor.CurrentColor);
        }

        private void ActivateEditing(object sender, EventArgs args)
        {
            if (!EditingEnabled)
            {
                return;
            }

            _edit_text.Clear();
            TextEntryRules.CheckAndAppend(_edit_text, Text);
            TextEntryRules.CheckAndAppend(_edit_text, UpdateData.UIInputState.Text); // Meta defining responsiveness
            _is_being_edited = true;
            _text_cursor.ActivateAndHighlight();
        }

        private void Confirm(object sender, EventArgs args)
        {
            if (_is_being_edited)
            {
                TextEntryRules.ApplyFinalCheck(_edit_text, out string result);
                Text = result;
                _edit_text.Clear();
                DisableEditing(this, EventArgs.Empty);
            }
        }

        private void InitializeTextCursor(object sender, EventArgs args)
        {
            _text_cursor = new TextCursor(this, _edit_text);
        }

        #endregion
    }
}