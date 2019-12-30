using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using DownUnder.UI.Widgets.DataTypes;

// Todo: Check MeasureStrings offset parameter, it's broken maybe

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

        int highlight_start = 0;

        int highlight_end = 0;

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
                    edit_text?.Clear();
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

        /// <summary>
        /// The start of the highlighted text (if active).
        /// </summary>
        int _HighlightPosition
        {
            get
            {
                if (highlight_start > highlight_end)
                {
                    return highlight_end;
                }
                else
                {
                    return highlight_start;
                }
            }
        }

        /// <summary>
        /// The length of the highlighted text (if active).
        /// </summary>
        int _HighlightLength
        {
            get
            {
                if (highlight_start > highlight_end)
                {
                    return highlight_start - highlight_end;
                }
                else
                {
                    return highlight_end - highlight_start;
                }
            }
        }

        float _CaretBlinkTime
        {
            get => SystemInformation.CaretBlinkTime / 500f;
        }

        bool _CaretCurrentlyDrawn
        {
            get => caret_blink_timer / _CaretBlinkTime < 0.5f;
        }

        int _CaretLine
        {
            get
            {
                if (caret_position == 0) return 0;
                string source = edit_text.ToString().Substring(0, caret_position);
                int count = 0;
                foreach (char c in source)
                    if (c == '\n') count++;
                return count;
            }
        }

        int _NumOfLines
        {
            get
            {
                string source = edit_text.ToString();
                int count = 0;
                foreach (char c in source)
                    if (c == '\n') count++;
                return count;
            }
        }

        int _BeginningOfCurrentLine
        {
            get
            {
                for (int i = caret_position - 1; i >= 0; i--)
                {
                    if (edit_text[i] == '\n') return i + 1;
                }
                return 0;
            }
        }
        
        int _EndOfCurrentLine
        {
            get
            {
                for (int i = caret_position; i < edit_text.Length; i++)
                {
                    if (edit_text[i] == '\n') return i;
                }
                return edit_text.Length;
            }
        }

        #endregion

        #region Constructors

        public TextCursor(Label label)
        {
            this.label = label;
            label.OnDoubleClick += DoubleClickAction;
            label.OnClick += ClickAction;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (!Active) return;
            Vector2 offset = label.PositionInWindow.ToVector2().Floored();
            UIInputState inp = label.UpdateData.UIInputState;
            //Console.WriteLine("_HighlightPosition = " + _HighlightPosition + " _HighlightLength = " + _HighlightLength);

            // Movement of the caret
            if (inp.TextCursorMovement.Left && caret_position != 0)
            {
                MoveCaretTo(caret_position - 1);
            }

            if (inp.TextCursorMovement.Right && caret_position != edit_text.Length)
            {
                MoveCaretTo(caret_position + 1);
            }
            
            if (inp.TextCursorMovement.Up)
            {
                if (_CaretLine == 0)
                {
                    MoveCaretTo(0);
                }
                else
                {
                    MoveCaretTo(
                        label.DrawingData.sprite_font.IndexFromPoint
                        (
                            edit_text.ToString(), 
                            label.DrawingData.sprite_font.GetCharacterPosition(edit_text.ToString(), caret_position) - new Vector2(0f, label.DrawingData.sprite_font.MeasureString("|").Y), 
                            true
                        )
                    );
                }
            }

            if (inp.TextCursorMovement.Down)
            {
                if (_CaretLine == _NumOfLines)
                {
                    MoveCaretTo(edit_text.Length);
                }
                else
                {
                    MoveCaretTo(
                        label.DrawingData.sprite_font.IndexFromPoint
                        (
                            edit_text.ToString(),
                            label.DrawingData.sprite_font.GetCharacterPosition(edit_text.ToString(), caret_position) + new Vector2(0f, label.DrawingData.sprite_font.MeasureString("|").Y),
                            true
                        )
                    );
                }
            }

            if (inp.Home)
            {
                MoveCaretTo(_BeginningOfCurrentLine);
            }

            if (inp.End)
            {
                MoveCaretTo(_EndOfCurrentLine);
            }

            // Editing the text
            if (inp.BackSpace)
            {
                if (edit_text.Length > 0 && caret_position != 0)
                {
                    edit_text.Remove(caret_position - 1, 1);
                    MoveCaretTo(caret_position - 1);
                }
            }

            if (inp.Delete)
            {
                if (edit_text.Length > 0 && caret_position != edit_text.Length)
                {
                    edit_text.Remove(caret_position, 1);
                    MoveCaretTo(caret_position);
                }
            }

            int added_chars = label.TextEntryRules.CheckAndInsert(edit_text, label.UpdateData.UIInputState.Text, caret_position);
            if (added_chars != 0)
            {
                MoveCaretTo(caret_position + added_chars);
            }

            text_area = label.DrawingData.sprite_font.MeasureStringAreas(edit_text.ToString());
            highlight_area = label.DrawingData.sprite_font.MeasureSubStringAreas(edit_text.ToString(), _HighlightPosition, _HighlightLength, true);
            caret_blink_timer += label.UpdateData.GameTime.GetElapsedSeconds();

            bool over_highlighted_text = false;
            foreach (RectangleF text in highlight_area)
            {
                text.Offset(offset);
                if (text.Contains(label.UpdateData.UIInputState.CursorPosition)) over_highlighted_text = true;
            }
            
            if (label.IsPrimaryHovered && !over_highlighted_text)
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

            if (_HighlightLength > 0)
            {
                foreach (var rect in highlight_area)
                {
                    rect.Offset(offset);
                    label.DrawingData.sprite_batch.FillRectangle(rect, Color.LightBlue);
                }
            }

            if (_CaretCurrentlyDrawn)
            {
                Vector2 position = label.DrawingData.sprite_font.GetCharacterPosition(edit_text.ToString(), caret_position) + offset + new Vector2(1, 0);
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
            highlight_start = 0;
            highlight_end = edit_text.Length;
        }

        public void ApplyFinalCheck()
        {
            label.TextEntryRules.ApplyFinalCheck(edit_text);
        }

        #endregion

        #region Private Methods
        
        private void MoveCaretTo(int index)
        {
            caret_position = index;
            if (!label.UpdateData.UIInputState.Shift) highlight_start = caret_position;
            highlight_end = caret_position;
            caret_blink_timer = 0f;
        }

        #endregion

        #region Events

        /// <summary>
        /// Is called when this widget is double clicked. Activates editing (if enabled)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DoubleClickAction(object sender, EventArgs args)
        {
            if (!label.EditingEnabled)
            {
                return;
            }

            if (Active)
            {
                return;
            }

            Active = true;
        }

        private void ClickAction(object sender, EventArgs args)
        {
            if (!Active) return;
            MoveCaretTo(label.DrawingData.sprite_font.IndexFromPoint(edit_text.ToString(), label.UpdateData.UIInputState.CursorPosition, true));
        }

        #endregion

    }
}
