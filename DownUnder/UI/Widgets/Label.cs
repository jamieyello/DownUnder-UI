using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.WidgetControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// Todo: add TextArea property

namespace DownUnder.UI.Widgets
{
    [DataContract]
    public class Label : Widget
    {
        #region Fields
        
        private string _text_backing = "";
        private bool _text_edit_enabled = false;
        private TextCursor _text_cursor;
        //private UIPalette _edit_text

        #endregion Fields

        #region Public Properties

        [DataMember] public virtual bool EditingEnabled
        {
            get => _text_edit_enabled;
            set
            {
                _text_cursor.Active = value ? false : _text_cursor.Active;
                _text_edit_enabled = value;
            }
        }

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
                    return new RectangleF(Position, SpriteFont.MeasureString(Text));
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
            get
            {
                if (_text_cursor == null) return false;
                return _text_cursor.Active;
            }
        }

        /// <summary>
        /// Palette of the background while editing text.
        /// </summary>
        public UIPalette TextEditBackgroundPalette { get; internal set; } = new UIPalette();

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
            SpriteFont = sprite_font;
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
            EnterConfirms = false;
            BackgroundColor.DefaultColor = Color.White.ShiftBrightness(0.84f);
            BackgroundColor.HoveredColor = Color.White;
            TextEditBackgroundPalette.DefaultColor = Color.White;
            TextEditBackgroundPalette.HoveredColor = Color.White;
            TextEditBackgroundPalette.ForceComplete();
            
            OnDraw += DrawText;
            OnSelectOff += DisableEditing;
            OnUpdate += Update;
            OnConfirm += ConfirmEdit;
            OnGraphicsInitialized += InitializeGraphics;
        }

        #endregion Constructors

        #region Overrides

        public override UIPalette BackgroundColor
        {
            get
            {
                if (IsBeingEdited)
                {
                    return TextEditBackgroundPalette;
                }
                else
                {
                    return base.BackgroundColor;
                }
            }
            internal set => base.BackgroundColor = value;
        }

        public override bool DrawBackground
        {
            get
            {
                if (IsBeingEdited)
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
            Label result = new Label();
                
            result.Text = Text;
            result.Name = Name;
            result.TextEntryRules = (TextEntryRuleSet)TextEntryRules.Clone();
            result.EditingEnabled = EditingEnabled;
            result.TextEditBackgroundPalette = (UIPalette)TextEditBackgroundPalette.Clone();

            return result;
        }

        public override List<Widget> Children
        {
            get => new List<Widget>();
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
            sprite_batch.DrawString(SpriteFont, Text, new Vector2(), TextColor.CurrentColor);
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

        private void InitializeGraphics(object sender, EventArgs args)
        {
            _text_cursor = new TextCursor(this);
        }

        #endregion
    }
}