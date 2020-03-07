using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

// Bookmark: This is mostly functional. Limit ScrollX and ScrollY for testing and implementation.
// Todo: Add ImmediateValue/TargetValue to ChangingValue, replacing GetCurrent?
// Todo: Move the bar before calculating position to optimize responsiveness.
// Todo: contain this in scroll?

namespace DownUnder.UI.Widgets.WidgetControls
{
    /// <summary>
    /// Scrollbars used commonly in widgets with inner areas. Modifies ScrollX and ScrollY,.
    /// </summary>
    [DataContract]
    public class ScrollBars
    {
        private IScrollableWidget _iscrollable_parent;
        private Widget _parent;
        private Texture2D _white_dot;
        private bool _BottomVisible { get; set; } = false;
        private bool _SideVisible { get; set; } = false;
        private RectangleF _outer_bar_bottom_area = new RectangleF();
        private RectangleF _inner_bottom_bar_area = new RectangleF();
        private RectangleF _outer_side_bar_area = new RectangleF();
        private RectangleF _inner_side_bar_area = new RectangleF();

        // These are used for grabbing and dragging the inner scroll bars.
        private bool _side_bar_held;

        private float _bottom_bar_cursor_initial_x_value;
        private float _bottom_bar_initial_x_value;
        private bool _bottom_bar_held;
        private float _side_bar_cursor_initial_y_value;
        private float _side_bar_initial_y_value;

        [DataMember] public float Thickness { get; set; } = 20f;
        [DataMember] public ElementColors SideOuterBarPalette { get; private set; } = new ElementColors(Color.DarkGray);
        [DataMember] public ElementColors BottomOuterBarPalette { get; private set; } = new ElementColors(Color.DarkGray);
        [DataMember] public ElementColors SideInnerBarPalette { get; private set; } = new ElementColors(Color.LightGray, Color.White);
        [DataMember] public ElementColors BottomInnerBarPalette { get; private set; } = new ElementColors(Color.LightGray, Color.White);
        [DataMember] public float InnerBarSpacing { get; set; } = 4f;

        public ScrollBars(IScrollableWidget owning_widget, GraphicsDevice graphics_device)
        {
            if (!(owning_widget is Widget)) throw new Exception("Scrollbars can only be used with widgets.");
            _iscrollable_parent = owning_widget;
            _parent = (Widget)owning_widget;
            _white_dot = DrawingTools.Dot(graphics_device, Color.White);
        }

        public void Draw(SpriteBatch sprite_batch)
        {
            if (_SideVisible)
            {
                sprite_batch.Draw(_white_dot, _outer_side_bar_area.ToRectangle(), SideOuterBarPalette.CurrentColor);
                sprite_batch.Draw(_white_dot, _inner_side_bar_area.ToRectangle(), SideInnerBarPalette.CurrentColor);
            }

            if (_BottomVisible)
            {
                sprite_batch.Draw(_white_dot, _outer_bar_bottom_area.ToRectangle(), SideOuterBarPalette.CurrentColor);
                sprite_batch.Draw(_white_dot, _inner_bottom_bar_area.ToRectangle(), BottomInnerBarPalette.CurrentColor);
            }
        }

        public void Update(float step, UIInputState ui_input_state)
        {
            #region Position / Size
            RectangleF widget_content_area = _iscrollable_parent.ContentArea;
            RectangleF drawing_area = _parent.DrawingArea;
            RectangleF widget_area = _parent.Area;
            RectangleF area_in_window = _parent.AreaInWindow;

            // Claculate size of bars. ---
            _outer_bar_bottom_area.Height = Thickness;
            _outer_side_bar_area.Width = Thickness;
            _inner_bottom_bar_area.Height = Thickness - InnerBarSpacing * 2;
            _inner_side_bar_area.Width = Thickness - InnerBarSpacing * 2;

            Point2 modifier = new Point2();

            // Calculate modifiers for the size of the inner bar.
            if (drawing_area.Width.Rounded() < widget_content_area.Width.Rounded())
            {
                modifier.X = widget_area.Width / widget_content_area.Width;
                _BottomVisible = true;
            }
            else { modifier.X = 1f; _BottomVisible = false; }
            if (drawing_area.Height.Rounded() < widget_content_area.Height.Rounded())
            {
                modifier.Y = widget_area.Height / widget_content_area.Height;
                _SideVisible = true;
            }
            else { modifier.Y = 1f; _SideVisible = false; }

            _outer_bar_bottom_area.Width = drawing_area.Width;
            _outer_side_bar_area.Height = drawing_area.Height;

            _inner_bottom_bar_area.Width = (drawing_area.Width - InnerBarSpacing * 3) * modifier.X;
            _inner_side_bar_area.Height = (drawing_area.Height - InnerBarSpacing * 3) * modifier.Y;

            // Calculate the positions of the bars. ---
            _outer_bar_bottom_area.X = area_in_window.X;
            _outer_side_bar_area.Y = area_in_window.Y;
            _outer_bar_bottom_area.Y = area_in_window.Bottom - Thickness;
            _outer_side_bar_area.X = area_in_window.Right - Thickness;

            _inner_side_bar_area.X = _outer_side_bar_area.X + InnerBarSpacing;
            _inner_side_bar_area.Y = _outer_side_bar_area.Y + InnerBarSpacing + _iscrollable_parent.Scroll.Y.GetCurrent() * modifier.Y;
            _inner_bottom_bar_area.X = _outer_bar_bottom_area.X + InnerBarSpacing + _iscrollable_parent.Scroll.X.GetCurrent() * modifier.X;
            _inner_bottom_bar_area.Y = _outer_bar_bottom_area.Y + InnerBarSpacing;

            #endregion Position

            #region Palette

            // Modify these so the width of the bar matches the outer rather
            // than the inner bar
            // Update palettes ---
            Point2 cursor_position = ui_input_state.CursorPosition;
            if (_parent.DrawMode == Widget.DrawingMode.use_render_target) cursor_position = _parent.CursorPosition;
            if (_inner_bottom_bar_area.Contains(cursor_position)) BottomInnerBarPalette.Hovered = true; else BottomInnerBarPalette.Hovered = false;
            if (_inner_side_bar_area.Contains(cursor_position)) SideInnerBarPalette.Hovered = true; else SideInnerBarPalette.Hovered = false;
            if (_outer_bar_bottom_area.Contains(cursor_position)) BottomOuterBarPalette.Hovered = true; else BottomOuterBarPalette.Hovered = false;
            if (_outer_side_bar_area.Contains(cursor_position)) SideOuterBarPalette.Hovered = true; else SideOuterBarPalette.Hovered = false;

            SideOuterBarPalette.Update(step);
            BottomOuterBarPalette.Update(step);
            SideInnerBarPalette.Update(step);
            BottomInnerBarPalette.Update(step);

            #endregion Palette

            #region Grabbing and dragging the bars

            // Release the bars if the cursor isn't clicking.
            if (!ui_input_state.PrimaryClick)
            {
                _bottom_bar_held = false;
                _side_bar_held = false;
            }

            // Hold/Continue holding the bars.
            if ((BottomInnerBarPalette.Hovered && ui_input_state.PrimaryClickTriggered && _BottomVisible) || _bottom_bar_held)
            {
                BottomInnerBarPalette.SpecialColorEnabled = true;
                if (!_bottom_bar_held)
                {
                    _bottom_bar_held = true;
                    _bottom_bar_cursor_initial_x_value = ui_input_state.CursorPosition.X;
                    _bottom_bar_initial_x_value = _iscrollable_parent.Scroll.X.GetCurrent();
                }
            }
            else
            {
                BottomInnerBarPalette.SpecialColorEnabled = false;
            }
            if ((SideInnerBarPalette.Hovered && ui_input_state.PrimaryClickTriggered && _SideVisible) || _side_bar_held)
            {
                SideInnerBarPalette.SpecialColorEnabled = true;
                if (!_side_bar_held)
                {
                    _side_bar_held = true;
                    _side_bar_cursor_initial_y_value = ui_input_state.CursorPosition.Y;
                    _side_bar_initial_y_value = _iscrollable_parent.Scroll.Y.GetCurrent();
                }
            }
            else
            {
                SideInnerBarPalette.SpecialColorEnabled = false;
            }

            // Apply offset.
            if (_side_bar_held)
            {
                _iscrollable_parent.Scroll.Y.SetTargetValue(
                    _side_bar_initial_y_value +
                    (ui_input_state.CursorPosition.Y - _side_bar_cursor_initial_y_value) / modifier.Y
                    , true);
            }
            if (_bottom_bar_held)
            {
                _iscrollable_parent.Scroll.X.SetTargetValue(
                    _bottom_bar_initial_x_value +
                    (ui_input_state.CursorPosition.X - _bottom_bar_cursor_initial_x_value) / modifier.X
                    , true);
            }

            #endregion Grabbing and dragging the bars

            #region Restraining

            // Don't let the scrollbars go out of bounds.
            if (_iscrollable_parent.Scroll.X.GetCurrent() < 0f) _iscrollable_parent.Scroll.X.SetTargetValue(0f, true);
            if (_iscrollable_parent.Scroll.Y.GetCurrent() < 0f) _iscrollable_parent.Scroll.Y.SetTargetValue(0f, true);
            if (_iscrollable_parent.Scroll.X.GetCurrent() > widget_content_area.Width - widget_area.Width)
                _iscrollable_parent.Scroll.X.SetTargetValue(widget_content_area.Width - widget_area.Width, true);
            if (_iscrollable_parent.Scroll.Y.GetCurrent() > widget_content_area.Height - widget_area.Height)
                _iscrollable_parent.Scroll.Y.SetTargetValue(widget_content_area.Height - widget_area.Height, true);

            #endregion Restraining

            // Validate
            if (_parent.debug_output)
            {
                Console.WriteLine();
                Console.WriteLine($"widget_area {widget_area}");
                Console.WriteLine($"DisplayArea {_parent.VisibleArea}");
                Console.WriteLine($"area_in_window {area_in_window}");
                Console.WriteLine($"widget_content_area {widget_content_area}");
                Console.WriteLine($"drawing_area {drawing_area}");
                Console.WriteLine($"modifier {modifier}");
                Console.WriteLine($"Scroll.Y {_iscrollable_parent.Scroll.Y.GetCurrent()}");
                Console.WriteLine();
            }
        }
    }
}