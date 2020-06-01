//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using MonoGame.Extended;
//using Microsoft.Xna.Framework.Input;
//using DownUnder.UI.Widgets.DataTypes;
//using DownUnder.Utilities;

//namespace DownUnder.UI.Widgets.WidgetElements
//{
//    /// <summary> The blinking vertical line used in typing. Also handles highlighted text. </summary>
//    class TextCursor
//    {
//        #region Fields

//        /// <summary> The widget that owns this text cursor. </summary>
//        Label label;
        
//        /// <summary> Position of the caret. </summary>
//        int caret_position = 0;

//        int highlight_start = 0;

//        int highlight_end = 0;

//        /// <summary> If this is less than half _CaretBlinkTime then the caret won't be drawn. </summary>
//        float caret_blink_timer = 0f;

//        StringBuilder edit_text;

//        bool active_backing = false;

//        List<RectangleF> highlight_area;

//        List<RectangleF> text_area;

//        bool allow_draw = false;

//        bool clicking = false;

//        #endregion

//        #region Public Properties

//        public bool Active
//        {
//            get => active_backing;
//            set
//            {
//                if (active_backing && value)
//                {
//                    return;
//                }
//                if (value)
//                {
//                    label.PaletteUsage = BaseColorScheme.PaletteCategory.text_edit_widget;
//                    ActivateAndHighlight();
//                }
//                else
//                {
//                    label.PaletteUsage = BaseColorScheme.PaletteCategory.text_widget;
//                    edit_text?.Clear();
//                    caret_blink_timer = 0f;
//                    allow_draw = false;
//                }
//                active_backing = value;
//            }
//        }

//        public string Text
//        {
//            get
//            {
//                if (edit_text == null) return "";
//                return edit_text.ToString();
//            }
//        }

//        #endregion

//        #region Private Properties

//        /// <summary> The start of the highlighted text (if active). </summary>
//        int _HighlightPosition
//        {
//            get
//            {
//                if (highlight_start > highlight_end)
//                {
//                    return highlight_end;
//                }
//                else
//                {
//                    return highlight_start;
//                }
//            }
//        }

//        /// <summary> The length of the highlighted text (if active). </summary>
//        int _HighlightLength
//        {
//            get
//            {
//                if (highlight_start > highlight_end)
//                {
//                    return highlight_start - highlight_end;
//                }
//                else
//                {
//                    return highlight_end - highlight_start;
//                }
//            }
//        }

//        bool _CaretCurrentlyDrawn
//        {
//            get => caret_blink_timer / _CaretBlinkTime < 0.5f;
//        }

//        float _CaretBlinkTime => OSInterface.CaretBlinkTime;
        
//        int _CaretLine
//        {
//            get
//            {
//                if (caret_position == 0) return 0;
//                string source = edit_text.ToString().Substring(0, caret_position);
//                int count = 0;
//                foreach (char c in source)
//                    if (c == '\n') count++;
//                return count;
//            }
//        }

//        /// <summary> The number of lines in this text. </summary>
//        public int NumOfLines
//        {
//            get
//            {
//                string source = edit_text.ToString();
//                int count = 0;
//                foreach (char c in source)
//                    if (c == '\n') count++;
//                return count;
//            }
//        }

//        int _BeginningOfCurrentLine
//        {
//            get
//            {
//                for (int i = caret_position - 1; i >= 0; i--)
//                {
//                    if (edit_text[i] == '\n') return i + 1;
//                }
//                return 0;
//            }
//        }
        
//        int _EndOfCurrentLine
//        {
//            get
//            {
//                for (int i = caret_position; i < edit_text.Length; i++)
//                {
//                    if (edit_text[i] == '\n') return i;
//                }
//                return edit_text.Length;
//            }
//        }

//        /// <summary> Returns the start and end index of the word the caret is over. Includes the following space. </summary>
//        public Tuple<int, int> CurrentWord
//        {
//            get
//            {
//                int start = 0;

//                for (int i = caret_position - 1; i >= 0; i--)
//                {
//                    if (edit_text[i] == '\n' || edit_text[i] == ' ')
//                    {
//                        start = i + 1;
//                        break;
//                    }
//                }

//                for (int i = caret_position; i < edit_text.Length; i++)
//                {
//                    if (edit_text[i] == '\n' || edit_text[i] == ' ')
//                    {
//                        if (edit_text[i] == ' ') i++;
//                        return new Tuple<int, int>(start, i);
//                    }
//                }

//                return new Tuple<int, int>(start, edit_text.Length);
//            }
//        }

//        #endregion

//        #region Constructors

//        public TextCursor(Label label)
//        {
//            this.label = label;
//            label.OnClick += ClickAction;
//            label.OnDoubleClick += DoubleClickAction;
//            label.OnTripleClick += TripleClickAction;
//        }

//        #endregion

//        #region Public Methods

//        public void Update()
//        {
//            if (!label.UpdateData.UIInputState.PrimaryClick) clicking = false;
//            if (!Active) return;
//            Vector2 offset = label.PositionInWindow.ToVector2().Floored();
//            UIInputState inp = label.UpdateData.UIInputState;

//            if (clicking)
//            {
//                MoveCaretTo(label.SpriteFont.IndexFromPoint(edit_text.ToString(), label.CursorPosition, true));
//            }

//            // Movement of the caret
//            if (inp.TextCursorMovement.Left && caret_position != 0)
//            {
//                MoveCaretTo(caret_position - 1);
//            }

//            if (inp.TextCursorMovement.Right && caret_position != edit_text.Length)
//            {
//                MoveCaretTo(caret_position + 1);
//            }
            
//            if (inp.TextCursorMovement.Up)
//            {
//                if (_CaretLine == 0)
//                {
//                    MoveCaretTo(0);
//                }
//                else
//                {
//                    MoveCaretTo(
//                        label.SpriteFont.IndexFromPoint
//                        (
//                            edit_text.ToString(), 
//                            label.SpriteFont.GetCharacterPosition(edit_text.ToString(), caret_position) - new Vector2(0f, label.SpriteFont.MeasureString("|").Y), 
//                            true
//                        )
//                    );
//                }
//            }

//            if (inp.TextCursorMovement.Down)
//            {
//                if (_CaretLine == NumOfLines)
//                {
//                    MoveCaretTo(edit_text.Length);
//                }
//                else
//                {
//                    MoveCaretTo(
//                        label.SpriteFont.IndexFromPoint
//                        (
//                            edit_text.ToString(),
//                            label.SpriteFont.GetCharacterPosition(edit_text.ToString(), caret_position) + new Vector2(0f, label.SpriteFont.MeasureString("|").Y),
//                            true
//                        )
//                    );
//                }
//            }

//            if (inp.Home)
//            {
//                MoveCaretTo(_BeginningOfCurrentLine);
//            }

//            if (inp.End)
//            {
//                MoveCaretTo(_EndOfCurrentLine);
//            }

//            // Editing the text
//            if (inp.BackSpace)
//            {
//                if (_HighlightLength != 0)
//                {
//                    DeleteHighlightedText();
//                }
//                else if (edit_text.Length > 0 && caret_position != 0)
//                {
//                    edit_text.Remove(caret_position - 1, 1);
//                    MoveCaretTo(caret_position - 1);
//                }
//            }

//            if (inp.Delete)
//            {
//                if (_HighlightLength != 0)
//                {
//                    DeleteHighlightedText();
//                }
//                else if (edit_text.Length > 0 && caret_position != edit_text.Length)
//                {
//                    edit_text.Remove(caret_position, 1);
//                    MoveCaretTo(caret_position);
//                }
//            }

//            if (inp.SelectAll)
//            {
//                HighlightRange(0, edit_text.Length);
//            }

//            if (inp.Copy || inp.Cut)
//            {
//                if (_HighlightLength > 0)
//                {
//                    char[] t = new char[_HighlightLength];
//                    edit_text.CopyTo(_HighlightPosition, t, 0, _HighlightLength);
//                    OSInterface.CopyTextToClipBoard(new string(t));
//                }
//                if (inp.Cut) DeleteHighlightedText();
//            }

//            if (inp.Paste)
//            {
//                InsertText(OSInterface.GetTextFromClipboard(), caret_position, true);
//            }

//            // Insert typed text
//            InsertText(label.UpdateData.UIInputState.Text, caret_position, true);
            
//            text_area = label.SpriteFont.MeasureStringAreas(edit_text.ToString());
//            highlight_area = label.SpriteFont.MeasureSubStringAreas(edit_text.ToString(), _HighlightPosition, _HighlightLength, true);
//            caret_blink_timer += label.UpdateData.ElapsedSeconds;

//            bool over_highlighted_text = false;
//            foreach (RectangleF text in highlight_area)
//            {
//                text.Offset(offset);
//                if (text.Contains(label.CursorPosition)) over_highlighted_text = true;
//            }
            
//            if (label.IsPrimaryHovered && (!over_highlighted_text || clicking))
//            {
//                label.ParentWindow.UICursor = MouseCursor.IBeam;
//            }

//            if (caret_blink_timer >= _CaretBlinkTime)
//            {
//                caret_blink_timer -= _CaretBlinkTime;
//            }

//            allow_draw = true;
//        }

//        public void Draw()
//        {
//            if (!Active) return;
//            if (!label.IsBeingEdited) return;
//            if (!allow_draw) return;
//            Vector2 offset = label.TextArea.Position;

//            if (_HighlightLength > 0)
//            {
//                foreach (var rect in highlight_area)
//                {
//                    rect.Offset(offset);
//                    label.SpriteBatch.FillRectangle(rect, Color.LightBlue);
//                }
//            }

//            if (_CaretCurrentlyDrawn)
//            {
//                Vector2 position = label.SpriteFont.GetCharacterPosition(edit_text.ToString(), caret_position) + offset + new Vector2(1, 0);
//                Vector2 position2 = position + new Vector2(0, 20);
//                label.SpriteBatch.DrawLine(position, position2, label.Theme.TextColor.CurrentColor, 1);
//            }
//        }

//        /// <summary> Called when Active is set to true. </summary>
//        private void ActivateAndHighlight()
//        {
//            edit_text = new StringBuilder(label.Text);
//            HighlightRange(0, edit_text.Length);
//        }

//        public void ApplyFinalCheck()
//        {
//            label.TextEntryRules.ApplyFinalCheck(edit_text);
//        }

//        #endregion

//        #region Private Methods
        
//        private void MoveCaretTo(int index, bool no_highlight = false)
//        {
//            if (caret_position != index) caret_blink_timer = 0f;
//            caret_position = index;
//            if (!label.UpdateData.UIInputState.Shift && !clicking || no_highlight) highlight_start = caret_position;
//            highlight_end = caret_position;
//        }

//        private void HighlightRange(int start, int end)
//        {
//            caret_position = end;
//            highlight_start = start;
//            highlight_end = end;
//            caret_blink_timer = 0f;
//        }

//        private void InsertText(string text, int index, bool no_highlight = false)
//        {
//            if (text == "") return;
//            if (DeleteHighlightedText()) index = caret_position;
//            int added_chars = label.TextEntryRules.CheckAndInsert(edit_text, text, index);
//            if (added_chars != 0)
//            {
//                MoveCaretTo(index + added_chars, no_highlight);
//            }
//        }

//        private bool DeleteHighlightedText()
//        {
//            int highlight_length = _HighlightLength;
//            if (highlight_length == 0) return false;
//            int highlight_position = _HighlightPosition;
//            edit_text.Remove(highlight_position, highlight_length);
//            highlight_start = highlight_position;
//            highlight_end = highlight_position;
//            caret_position = highlight_position;
//            return true;
//        }

//        #endregion

//        #region Events

//        /// <summary> Is called when the label is clicked. </summary>
//        private void ClickAction(object sender, EventArgs args)
//        {
//            if (!Active) return;
//            MoveCaretTo(label.SpriteFont.IndexFromPoint(edit_text.ToString(), label.CursorPosition, true));
//            clicking = true;
//        }

//        /// <summary> Is called when the label is double clicked. </summary>
//        private void DoubleClickAction(object sender, EventArgs args)
//        {
//            if (!label.EditingEnabled) return;

//            if (Active)
//            {
//                clicking = false;
//                Tuple<int, int> word = CurrentWord;
//                Console.WriteLine(word);
//                HighlightRange(word.Item1, word.Item2);
//                return;
//            }

//            Active = true;
//        }

//        /// <summary> Is called when the label is triple clicked. </summary>
//        private void TripleClickAction(object sender, EventArgs args)
//        {
//            if (Active)
//            {
//                clicking = false;
//                HighlightRange(_BeginningOfCurrentLine, _EndOfCurrentLine);
//                return;
//            }
//        }

//        #endregion
//    }
//}
