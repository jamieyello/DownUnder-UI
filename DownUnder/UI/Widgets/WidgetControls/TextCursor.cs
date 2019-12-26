using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.Widgets.WidgetControls
{
    /// <summary>
    /// The blinking vertical line used in typing. Also handles highlighted text.
    /// </summary>
    class TextCursor
    {
        #region Fields

        /// <summary>
        /// The widget that owns this text cursor.
        /// </summary>
        Label label;
        
        /// <summary>
        /// Position of the caret.
        /// </summary>
        int caret_position = 0;

        /// <summary>
        /// The start of the highlighted text (if active).
        /// </summary>
        int highlight_position = 0;

        /// <summary>
        /// The length of the highlighted text (if active).
        /// </summary>
        int highlight_length = 0;

        /// <summary>
        /// If this is less than half _CaretBlinkTime then the caret won't be drawn.
        /// </summary>
        float caret_blink_timer = 0f;

        StringBuilder edit_text;

        bool active_backing = false;

        List<RectangleF> highlight_area;

        List<RectangleF> text_area;

        bool allow_draw = false;

        #endregion

        #region Public Properties

        public bool Active
        {
            get => active_backing;
            set
            {
                if (active_backing && value)
                {
                    return;
                }
                if (value)
                {
                    ActivateAndHighlight();
                }
                else
                {
                    edit_text.Clear();
                    caret_blink_timer = 0f;
                    allow_draw = false;
                }
                active_backing = value;
            }
        }

        public string Text
        {
            get
            {
                if (edit_text == null) return "";
                return edit_text.ToString();
            }
        }

        #endregion

        #region Private Properties

        float _CaretBlinkTime
        {
            get => SystemInformation.CaretBlinkTime / 500f;
        }

        bool _CaretCurrentlyDrawn
        {
            get => caret_blink_timer / _CaretBlinkTime < 0.5f;
        }

        #endregion

        #region Constructors

        public TextCursor(Label label)
        {
            this.label = label;
            Console.WriteLine($"blink time = {_CaretBlinkTime}");
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (!Active) return;
            Vector2 offset = label.PositionInWindow.ToVector2().Floored();

            // Movement of the caret
            if (label.UpdateData.UIInputState.TextCursorMovement.Left && caret_position != 0)
            {
                caret_position--;
                highlight_position = caret_position;
                highlight_length = 0;
                caret_blink_timer = 0f;
            }

            if (label.UpdateData.UIInputState.TextCursorMovement.Right && caret_position != edit_text.Length)
            {
                caret_position++;
                highlight_position = caret_position;
                highlight_length = 0;
                caret_blink_timer = 0f;
            }

            // Editting the text
            if (label.UpdateData.UIInputState.Back)
            {
                if (edit_text.Length > 0 && caret_position != 0)
                {
                    edit_text.Remove(caret_position - 1, 1);
                    caret_position--;
                    highlight_position = caret_position;
                    highlight_length = 0;
                    caret_blink_timer = 0f;
                }
            }

            int added_chars = label.TextEntryRules.CheckAndInsert(edit_text, label.UpdateData.UIInputState.Text, caret_position);
            if (added_chars != 0)
            {
                caret_position += added_chars;
                highlight_position = caret_position;
                highlight_length = 0;
                caret_blink_timer = 0f;
            }

            text_area = label.DrawingData.sprite_font.MeasureStringAreas(edit_text.ToString());
            highlight_area = label.DrawingData.sprite_font.MeasureSubStringAreas(edit_text.ToString(), highlight_position, highlight_length);
            caret_blink_timer += label.UpdateData.GameTime.GetElapsedSeconds();

            bool over_text = false;
            foreach (RectangleF text in text_area)
            {
                text.Offset(offset);
                if (text.Contains(label.UpdateData.UIInputState.CursorPosition)) over_text = true;
            }
            bool over_highlighted_text = false;
            foreach (RectangleF text in highlight_area)
            {
                text.Offset(offset);
                if (text.Contains(label.UpdateData.UIInputState.CursorPosition)) over_highlighted_text = true;
            }
            
            if (over_text && !over_highlighted_text)
            {
                label.ParentWindow.UICursor = MouseCursor.IBeam;
            }

            if (caret_blink_timer >= _CaretBlinkTime)
            {
                caret_blink_timer -= _CaretBlinkTime;
            }
            
            if (label.IsPrimaryHovered)
            {
                //Console.WriteLine($"Hovering over {label.Text}, TextAreaInWindow = {label.TextAreaInWindow}");
            }

            allow_draw = true;
        }

        public void Draw()
        {
            if (!Active) return;
            if (!label.IsBeingEdited) return;
            if (!allow_draw) return;
            Vector2 offset = label.PositionInWindow.ToVector2().Floored();

            if (highlight_length > 0)
            {
                foreach (var rect in highlight_area)
                {
                    rect.Offset(offset);
                    label.DrawingData.sprite_batch.FillRectangle(rect, Color.LightBlue);
                }
            }

            if (_CaretCurrentlyDrawn)
            {
                Vector2 position = label.DrawingData.sprite_font.GetCharacterPosition(edit_text.ToString(), caret_position, true) + offset + new Vector2(1, 0);
                Vector2 position2 = position + new Vector2(0, 20);
                label.DrawingData.sprite_batch.DrawLine(position, position2, Color.Black);
            }
        }

        /// <summary>
        /// Called when Active is set to true.
        /// </summary>
        private void ActivateAndHighlight()
        {
            edit_text = new StringBuilder(label.Text);
            caret_position = edit_text.Length;
            highlight_position = 0;
            highlight_length = edit_text.Length;
        }

        public void ApplyFinalCheck()
        {
            label.TextEntryRules.ApplyFinalCheck(edit_text);
        }

        #endregion

        #region Private Methods
        
        #endregion

    }
}
