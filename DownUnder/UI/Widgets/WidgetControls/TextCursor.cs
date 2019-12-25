using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

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

                active_backing = value;
                if (!value)
                {
                    caret_blink_timer = 0f;
                    allow_draw = false;
                }
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

        #region Public Methods

        public TextCursor(Label label, StringBuilder edit_text)
        {
            this.label = label;
            Console.WriteLine($"blink time = {_CaretBlinkTime}");
            this.edit_text = edit_text;
        }

        public void Update()
        {
            if (!Active) return;

            if (label.UpdateData.UIInputState.Back)
            {
                if (edit_text.Length > 0)
                {
                    edit_text.Remove(edit_text.Length - 1, 1);
                }
            }

            label.TextEntryRules.CheckAndAppend(edit_text, label.UpdateData.UIInputState.Text);

            text_area = label.DrawingData.sprite_font.MeasureStringAreas(edit_text.ToString());
            highlight_area = label.DrawingData.sprite_font.MeasureSubStringAreas(edit_text.ToString(), highlight_position, highlight_length);
            caret_blink_timer += label.UpdateData.GameTime.GetElapsedSeconds();

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
                Vector2 position = label.DrawingData.sprite_font.GetCharacterPosition(edit_text.ToString(), caret_position, true) + offset;
                Vector2 position2 = position + new Vector2(0, 20);
                label.DrawingData.sprite_batch.DrawLine(position, position2, Color.Black);
            }
        }

        public void ActivateAndHighlight()
        {
            Active = true;
            caret_position = edit_text.Length;
            highlight_position = 0;
            highlight_length = edit_text.Length;
        }

        #endregion

        #region Private Methods
        
        #endregion

    }
}
