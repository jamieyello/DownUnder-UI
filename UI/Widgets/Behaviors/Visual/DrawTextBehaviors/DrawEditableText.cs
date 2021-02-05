using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors
{
    public class DrawEditableText : WidgetBehavior, ISubWidgetBehavior<DrawText>
    {
        public DrawText BaseBehavior => Parent.Behaviors.Get<DrawText>();

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        private bool Active => active_backing;

        float caret_blink_time = 1f;
        StringBuilder edit_text = new StringBuilder();
        int caret_position = 0;
        int highlight_start = 0;
        int highlight_end = 0;
        /// <summary> If this is less than half _CaretBlinkTime then the caret won't be drawn. </summary>
        float caret_blink_timer = 0f;
        bool active_backing = false;
        List<RectangleF> highlight_area;
        List<RectangleF> text_area;
        bool allow_draw = false;
        bool clicking = false;
        bool editing_enabled_backing = true;

        public DrawEditableTextSettings Settings { get; set; } = new DrawEditableTextSettings();

        public bool EditingEnabled
        {
            get => editing_enabled_backing;
            set
            {
                if (!value && Active)
                {
                    editing_enabled_backing = value;
                    active_backing = value;
                    DeactivateEditing();
                    return;
                }
                editing_enabled_backing = value;
            }
        }

        public void ActivateEditing()
        {
            if (active_backing) return;
            edit_text.Clear();
            edit_text.Append(BaseBehavior.Text);
            if (Settings.HighlightTextOnActivation) HighlightRange(0, edit_text.Length);
            else MoveCaretTo(Parent.ParentDWindow.WindowFont.IndexFromPoint(edit_text.ToString(), Parent.CursorPosition, true));
            BaseBehavior.EnableDefaultDraw = false;
            active_backing = true;
        }

        public void DeactivateEditing(bool keep_changes = false)
        {
            if (!active_backing) return;
            if (keep_changes) BaseBehavior.Text = edit_text.ToString();
            //BaseBehavior.TextEntryRules.ApplyFinalCheck(edit_text); ??
            edit_text.Clear();
            BaseBehavior.EnableDefaultDraw = true;
            active_backing = false;
            caret_blink_timer = 0f;
        }

        /// <summary> The start of the highlighted text (if active). </summary>
        int _HighlightPosition => (highlight_start > highlight_end) ? highlight_end : highlight_start;

        /// <summary> The length of the highlighted text (if active). </summary>
        int _HighlightLength => (highlight_start > highlight_end) ? highlight_start - highlight_end : highlight_end - highlight_start;

        bool _CaretCurrentlyDrawn => caret_blink_timer / caret_blink_time < 0.5f;

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

        /// <summary> The number of lines in this text. </summary>
        public int NumOfLines
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

        int _BeginningOfCurrentLine {
            get {
                for (int i = caret_position - 1; i >= 0; i--)
                    if (edit_text[i] == '\n') return i + 1;
                return 0;
            }
        }

        int _EndOfCurrentLine {
            get {
                for (int i = caret_position; i < edit_text.Length; i++)
                    if (edit_text[i] == '\n') return i;
                return edit_text.Length;
            }
        }

        /// <summary> Returns the start and end index of the word the caret is over. Includes the following space. </summary>
        public Tuple<int, int> CurrentWord {
            get {
                int start = 0;

                for (int i = caret_position - 1; i >= 0; i--) {
                    if (edit_text[i] == '\n' || edit_text[i] == ' ') {
                        start = i + 1;
                        break;
                    }
                }

                for (int i = caret_position; i < edit_text.Length; i++) {
                    if (edit_text[i] == '\n' || edit_text[i] == ' ') {
                        if (edit_text[i] == ' ') i++;
                        return new Tuple<int, int>(start, i);
                    }
                }

                return new Tuple<int, int>(start, edit_text.Length);
            }
        }

        public DrawEditableText() : base() {

        }

        protected override void Initialize()
        {
            Parent.OnParentWindowSet += (s, a) => caret_blink_time = DWindow.OS.CaretBlinkTime;
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
            Parent.OnClick += ClickAction;
            Parent.OnDoubleClick += DoubleClickAction;
            Parent.OnTripleClick += TripleClickAction;
            Parent.OnConfirm += ConfirmAction;
            Parent.OnSelectOff += FocusOffAction;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
            Parent.OnClick -= ClickAction;
            Parent.OnDoubleClick -= DoubleClickAction;
            Parent.OnTripleClick -= TripleClickAction;
            Parent.OnConfirm -= ConfirmAction;
            Parent.OnSelectOff -= FocusOffAction;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public void Update(object sender, EventArgs args)
        {
            if (!Parent.UpdateData.UIInputState.PrimaryClick) clicking = false;
            if (!Settings.RequireDoubleClick && Parent.IsPrimaryHovered) Parent.ParentDWindow.UICursor = MouseCursor.IBeam;
            if (!Active) return;
            Vector2 offset = Parent.PositionInWindow.ToVector2().Floored();
            UIInputState inp = Parent.UpdateData.UIInputState;

            if (clicking) MoveCaretTo(Parent.ParentDWindow.WindowFont.IndexFromPoint(edit_text.ToString(), Parent.CursorPosition, true));

            // Movement of the caret
            if (inp.TextCursorMovement.Left && caret_position != 0) MoveCaretTo(caret_position - 1);
            if (inp.TextCursorMovement.Right && caret_position != edit_text.Length) MoveCaretTo(caret_position + 1);
            if (inp.TextCursorMovement.Up)
            {
                if (_CaretLine == 0) MoveCaretTo(0);
                else
                {
                    MoveCaretTo(
                        Parent.ParentDWindow.WindowFont.IndexFromPoint
                        (
                            edit_text.ToString(),
                            Parent.ParentDWindow.WindowFont.GetCharacterPosition(edit_text.ToString(), caret_position) - new Vector2(0f, Parent.ParentDWindow.WindowFont.MeasureString("|").Y),
                            true
                        )
                    );
                }
            }

            if (inp.TextCursorMovement.Down)
            {
                if (_CaretLine == NumOfLines) MoveCaretTo(edit_text.Length);
                else
                {
                    MoveCaretTo(
                        Parent.ParentDWindow.WindowFont.IndexFromPoint
                        (
                            edit_text.ToString(),
                            Parent.ParentDWindow.WindowFont.GetCharacterPosition(edit_text.ToString(), caret_position) + new Vector2(0f, Parent.ParentDWindow.WindowFont.MeasureString("|").Y),
                            true
                        )
                    );
                }
            }

            if (inp.Home) MoveCaretTo(_BeginningOfCurrentLine);
            if (inp.End) MoveCaretTo(_EndOfCurrentLine);

            // Editing the text
            bool text_changed = false;

            if (inp.BackSpace)
            {
                if (_HighlightLength != 0)
                {
                    text_changed |= DeleteHighlightedText();
                }
                else if (edit_text.Length > 0 && caret_position != 0)
                {
                    edit_text.Remove(caret_position - 1, 1);
                    MoveCaretTo(caret_position - 1);
                    text_changed = true;
                }
            }

            if (inp.Delete)
            {
                if (_HighlightLength != 0)
                {
                    text_changed |= DeleteHighlightedText();
                }
                else if (edit_text.Length > 0 && caret_position != edit_text.Length)
                {
                    edit_text.Remove(caret_position, 1);
                    MoveCaretTo(caret_position);
                    text_changed = true;
                }
            }

            if (inp.SelectAll) HighlightRange(0, edit_text.Length);

            if (inp.Copy || inp.Cut)
            {
                if (_HighlightLength > 0)
                {
                    char[] t = new char[_HighlightLength];
                    edit_text.CopyTo(_HighlightPosition, t, 0, _HighlightLength);
                    DWindow.OS.CopyTextToClipBoard(new string(t));
                }
                if (inp.Cut) text_changed |= DeleteHighlightedText();
            }

            if (inp.Paste) text_changed |= InsertText(DWindow.OS.GetTextFromClipboard(), caret_position, true);

            // Insert typed text
            text_changed |= InsertText(Parent.UpdateData.UIInputState.Text, caret_position, true);

            if (text_changed)
            {
                if (Settings.LiveUpdate) BaseBehavior.Text = edit_text.ToString();
            }

            text_area = Parent.ParentDWindow.WindowFont.MeasureStringAreas(edit_text.ToString());
            highlight_area = Parent.ParentDWindow.WindowFont.MeasureSubStringAreas(edit_text.ToString(), _HighlightPosition, _HighlightLength, true);
            caret_blink_timer += Parent.UpdateData.ElapsedSeconds;

            bool over_highlighted_text = false;
            foreach (RectangleF text in highlight_area) {
                text.Offset(offset);
                if (text.Contains(Parent.CursorPosition)) over_highlighted_text = true;
            }

            if (Parent.IsPrimaryHovered && (!over_highlighted_text || clicking)) Parent.ParentDWindow.UICursor = MouseCursor.IBeam;
            if (caret_blink_timer >= caret_blink_time) caret_blink_timer -= caret_blink_time;

            allow_draw = true;
        }

        private void MoveCaretTo(int index, bool no_highlight = false)
        {
            if (caret_position != index) caret_blink_timer = 0f;
            caret_position = index;
            if (!Parent.UpdateData.UIInputState.Shift && !clicking || no_highlight) highlight_start = caret_position;
            highlight_end = caret_position;
        }

        private void HighlightRange(int start, int end)
        {
            caret_position = end;
            highlight_start = start;
            highlight_end = end;
            caret_blink_timer = 0f;
        }

        private bool InsertText(string text, int index, bool no_highlight = false)
        {
            bool text_changed = false;
            if (text == "") return text_changed;
            if (DeleteHighlightedText())
            {
                index = caret_position;
                text_changed = true;
            }
            int added_chars = Settings.TextEntryRules.CheckAndInsert(edit_text, text, index);
            if (added_chars != 0)
            {
                MoveCaretTo(index + added_chars, no_highlight);
                text_changed = true;
            }
            return text_changed;
        }

        private bool DeleteHighlightedText()
        {
            int highlight_length = _HighlightLength;
            if (highlight_length == 0) return false;
            int highlight_position = _HighlightPosition;
            edit_text.Remove(highlight_position, highlight_length);
            highlight_start = highlight_position;
            highlight_end = highlight_position;
            caret_position = highlight_position;
            return true;
        }

        /// <summary> Is called when the parent is clicked. </summary>
        private void ClickAction(object sender, EventArgs args)
        {
            if (Active)
            {
                MoveCaretTo(Parent.ParentDWindow.WindowFont.IndexFromPoint(edit_text.ToString(), Parent.CursorPosition, true));
                clicking = true;
            }

            if (!Settings.RequireDoubleClick) ActivateEditing();
        }

        /// <summary> Is called when the parent is double clicked. </summary>
        private void DoubleClickAction(object sender, EventArgs args)
        {
            if (!EditingEnabled) return;

            if (Active)
            {
                clicking = false;
                Tuple<int, int> word = CurrentWord;
                //Console.WriteLine(word);
                HighlightRange(word.Item1, word.Item2);
                return;
            }

            ActivateEditing();
        }

        private void ConfirmAction(object sender, EventArgs args)
        {
            DeactivateEditing(true);
        }

        private void FocusOffAction(object sender, EventArgs args)
        {
            DeactivateEditing(false);
        }

        /// <summary> Is called when the parent is triple clicked. </summary>
        private void TripleClickAction(object sender, EventArgs args)
        {
            if (Active)
            {
                clicking = false;
                HighlightRange(_BeginningOfCurrentLine, _EndOfCurrentLine);
                return;
            }
        }

        public void Draw(object sender, WidgetDrawArgs args)
        {
            if (!Active) return;
            if (!allow_draw) return;
            Vector2 offset = args.DrawingArea.Position.WithOffset(BaseBehavior.TextPosition).Floored();

            if (_HighlightLength > 0)
            {
                foreach (var rect in highlight_area)
                {
                    rect.Offset(offset);
                    Parent.SpriteBatch.FillRectangle(rect, Color.LightBlue);
                }
            }

            BaseBehavior.ForceDrawText(args.DrawingArea.Position, edit_text.ToString());

            if (_CaretCurrentlyDrawn)
            {
                Vector2 position = Parent.ParentDWindow.WindowFont.GetCharacterPosition(edit_text.ToString(), caret_position) + offset + new Vector2(1, 0);
                Vector2 position2 = position + new Vector2(0, 20);
                Parent.SpriteBatch.DrawLine(position, position2, Parent.VisualSettings.TextColor, 1);
            }
        }
    }
}
