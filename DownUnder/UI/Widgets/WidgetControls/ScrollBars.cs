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
        private IScrollableWidget _owning_widget;
        private readonly Texture2D _white_dot;
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
        [DataMember] public UIPalette SideOuterBarPalette { get; private set; } = new UIPalette(Color.DarkGray);
        [DataMember] public UIPalette BottomOuterBarPalette { get; private set; } = new UIPalette(Color.DarkGray);
        [DataMember] public UIPalette SideInnerBarPalette { get; private set; } = new UIPalette(Color.LightGray, Color.White);
        [DataMember] public UIPalette BottomInnerBarPalette { get; private set; } = new UIPalette(Color.LightGray, Color.White);
        [DataMember] public float InnerBarSpacing { get; set; } = 4f;

        public ScrollBars(IScrollableWidget owning_widget, GraphicsDevice graphics_device)
        {
            if (!(owning_widget is Widget)) throw new Exception("Scrollbars can only be used with widgets.");
            _owning_widget = owning_widget;
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
            RectangleF widget_content_area = _owning_widget.ContentArea;
            RectangleF widget_area = ((Widget)_owning_widget).AreaInWindow;

            // Claculate size of bars. ---
            _outer_bar_bottom_area.Height = Thickness;
            _outer_side_bar_area.Width = Thickness;
            _inner_bottom_bar_area.Height = Thickness - InnerBarSpacing * 2;
            _inner_side_bar_area.Width = Thickness - InnerBarSpacing * 2;

            Point2 modifier = new Point2();

            // Calculate modifiers for the size of the inner bar.
            if (widget_area.Width.Rounded() < widget_content_area.Width.Rounded())
            {
                modifier.X = widget_area.Width / widget_content_area.Width;
                _BottomVisible = true;
            }
            else { modifier.X = 1f; _BottomVisible = false; }
            if (widget_area.Height.Rounded() < widget_content_area.Height.Rounded())
            {
                modifier.Y = widget_area.Height / widget_content_area.Height;
                _SideVisible = true;
            }
            else { modifier.Y = 1f; _SideVisible = false; }

            _outer_bar_bottom_area.Width = widget_area.Width;
            _outer_side_bar_area.Height = widget_area.Height;

            _inner_bottom_bar_area.Width = (widget_area.Width - InnerBarSpacing * 3) * modifier.X;
            _inner_side_bar_area.Height = (widget_area.Height - InnerBarSpacing * 3) * modifier.Y;

            // Calculate the positions of the bars. ---
            _outer_bar_bottom_area.X = widget_area.X;
            _outer_side_bar_area.Y = widget_area.Y;
            _outer_bar_bottom_area.Y = widget_area.Bottom - Thickness;
            _outer_side_bar_area.X = widget_area.Right - Thickness;

            _inner_side_bar_area.X = _outer_side_bar_area.X + InnerBarSpacing;
            _inner_side_bar_area.Y = _outer_side_bar_area.Y + InnerBarSpacing + _owning_widget.Scroll.Y.GetCurrent() * modifier.Y;
            _inner_bottom_bar_area.X = _outer_bar_bottom_area.X + InnerBarSpacing + _owning_widget.Scroll.X.GetCurrent() * modifier.X;
            _inner_bottom_bar_area.Y = _outer_bar_bottom_area.Y + InnerBarSpacing;

            #endregion Position

            #region Palette

            // Modify these so the width of the bar matches the outer rather
            // than the inner bar
            // Update palettes ---
            if (_inner_bottom_bar_area.Contains(ui_input_state.CursorPosition)) BottomInnerBarPalette.Hovered = true; else BottomInnerBarPalette.Hovered = false;
            if (_inner_side_bar_area.Contains(ui_input_state.CursorPosition)) SideInnerBarPalette.Hovered = true; else SideInnerBarPalette.Hovered = false;
            if (_outer_bar_bottom_area.Contains(ui_input_state.CursorPosition)) BottomOuterBarPalette.Hovered = true; else BottomOuterBarPalette.Hovered = false;
            if (_outer_side_bar_area.Contains(ui_input_state.CursorPosition)) SideOuterBarPalette.Hovered = true; else SideOuterBarPalette.Hovered = false;

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
                    _bottom_bar_initial_x_value = _owning_widget.Scroll.X.GetCurrent();
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
                    _side_bar_initial_y_value = _owning_widget.Scroll.Y.GetCurrent();
                }
            }
            else
            {
                SideInnerBarPalette.SpecialColorEnabled = false;
            }

            // Apply offset.
            if (_side_bar_held)
            {
                _owning_widget.Scroll.Y.SetTargetValue(
                    _side_bar_initial_y_value +
                    (ui_input_state.CursorPosition.Y - _side_bar_cursor_initial_y_value) / modifier.Y
                    , true);
            }
            if (_bottom_bar_held)
            {
                _owning_widget.Scroll.X.SetTargetValue(
                    _bottom_bar_initial_x_value +
                    (ui_input_state.CursorPosition.X - _bottom_bar_cursor_initial_x_value) / modifier.X
                    , true);
            }

            #endregion Grabbing and dragging the bars

            #region Restraining

            // Don't let the scrollbars go out of bounds.
            if (_owning_widget.Scroll.X.GetCurrent() < 0f) _owning_widget.Scroll.X.SetTargetValue(0f, true);
            if (_owning_widget.Scroll.Y.GetCurrent() < 0f) _owning_widget.Scroll.Y.SetTargetValue(0f, true);
            if (_owning_widget.Scroll.X.GetCurrent() > widget_content_area.Width - widget_area.Width) _owning_widget.Scroll.X.SetTargetValue(widget_content_area.Width - widget_area.Width, true);
            if (_owning_widget.Scroll.Y.GetCurrent() > widget_content_area.Height - widget_area.Height) _owning_widget.Scroll.Y.SetTargetValue(widget_content_area.Height - widget_area.Height, true);

            #endregion Restraining
        }
    }
}

//using DownUnder.UI.Widgets.DataTypes;
//using DownUnder.UI.Widgets.Interfaces;
//using DownUnder.Utility;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
//using System;
//using System.Runtime.Serialization;

//// Bookmark: This is mostly functional. Limit ScrollX and ScrollY for testing and implementation.
//// Todo: Add ImmediateValue/TargetValue to ChangingValue, replacing GetCurrent?
//// Todo: Move the bar before calculating position to optimize responsiveness.
//// Todo: contain this in scroll?

//namespace DownUnder.UI.Widgets.WidgetControls
//{
//    /// <summary>
//    /// Scrollbars used commonly in widgets with inner areas. Modifies ScrollX and ScrollY,.
//    /// </summary>
//    [DataContract]
//    public class ScrollBars
//    {
//        private IScrollableWidget _iscrollable_parent;
//        private Widget _parent;
//        private Texture2D _white_dot;
//        private bool _BottomVisible { get; set; } = false;
//        private bool _SideVisible { get; set; } = false;
//        private RectangleF _outer_bar_bottom_area = new RectangleF();
//        private RectangleF _inner_bottom_bar_area = new RectangleF();
//        private RectangleF _outer_side_bar_area = new RectangleF();
//        private RectangleF _inner_side_bar_area = new RectangleF();

//        // These are used for grabbing and dragging the inner scroll bars.
//        private bool _side_bar_held;

//        private float _bottom_bar_cursor_initial_x_value;
//        private float _bottom_bar_initial_x_value;
//        private bool _bottom_bar_held;
//        private float _side_bar_cursor_initial_y_value;
//        private float _side_bar_initial_y_value;

//        [DataMember] public float Thickness { get; set; } = 20f;
//        [DataMember] public UIPalette SideOuterBarPalette { get; private set; } = new UIPalette(Color.DarkGray);
//        [DataMember] public UIPalette BottomOuterBarPalette { get; private set; } = new UIPalette(Color.DarkGray);
//        [DataMember] public UIPalette SideInnerBarPalette { get; private set; } = new UIPalette(Color.LightGray, Color.White);
//        [DataMember] public UIPalette BottomInnerBarPalette { get; private set; } = new UIPalette(Color.LightGray, Color.White);
//        [DataMember] public float InnerBarSpacing { get; set; } = 4f;

//        public ScrollBars(IScrollableWidget owning_widget, GraphicsDevice graphics_device)
//        {
//            if (!(owning_widget is Widget)) throw new Exception("Scrollbars can only be used with widgets.");
//            _iscrollable_parent = owning_widget;
//            _parent = (Widget)owning_widget;
//            _white_dot = DrawingTools.Dot(graphics_device, Color.White);
//        }

//        public void Draw(SpriteBatch sprite_batch)
//        {
//            if (_SideVisible)
//            {
//                sprite_batch.Draw(_white_dot, _outer_side_bar_area.ToRectangle(), SideOuterBarPalette.CurrentColor);
//                sprite_batch.Draw(_white_dot, _inner_side_bar_area.ToRectangle(), SideInnerBarPalette.CurrentColor);
//            }

//            if (_BottomVisible)
//            {
//                sprite_batch.Draw(_white_dot, _outer_bar_bottom_area.ToRectangle(), SideOuterBarPalette.CurrentColor);
//                sprite_batch.Draw(_white_dot, _inner_bottom_bar_area.ToRectangle(), BottomInnerBarPalette.CurrentColor);
//            }
//        }

//        public void Update(float step, UIInputState ui_input_state)
//        {
//            #region Position / Size
//            RectangleF widget_content_area = _iscrollable_parent.ContentArea.SizeOnly();
//            RectangleF widget_area = _parent.Area;

//            // Claculate size of bars. ---
//            _outer_bar_bottom_area.Height = Thickness;
//            _outer_side_bar_area.Width = Thickness;
//            _inner_bottom_bar_area.Height = Thickness - InnerBarSpacing * 2;
//            _inner_side_bar_area.Width = Thickness - InnerBarSpacing * 2;

//            Point2 modifier = new Point2();

//            // Calculate modifiers for the size of the inner bar.
//            if (widget_area.Width.Rounded() < widget_content_area.Width.Rounded())
//            {
//                modifier.X = widget_area.Width / widget_content_area.Width;
//                _BottomVisible = true;
//            }
//            else { modifier.X = 1f; _BottomVisible = false; }
//            if (widget_area.Height.Rounded() < widget_content_area.Height.Rounded())
//            {
//                modifier.Y = widget_area.Height / widget_content_area.Height;
//                _SideVisible = true;
//            }
//            else { modifier.Y = 1f; _SideVisible = false; }

//            _outer_bar_bottom_area.Width = widget_area.Width;
//            _outer_side_bar_area.Height = widget_area.Height;

//            _inner_bottom_bar_area.Width = (widget_area.Width - InnerBarSpacing * 3) * modifier.X;
//            _inner_side_bar_area.Height = (widget_area.Height - InnerBarSpacing * 3) * modifier.Y;

//            // Calculate the positions of the bars. ---
//            _outer_bar_bottom_area.X = widget_area.X;
//            _outer_side_bar_area.Y = widget_area.Y;
//            _outer_bar_bottom_area.Y = widget_area.Bottom - Thickness;
//            _outer_side_bar_area.X = widget_area.Right - Thickness;

//            _inner_side_bar_area.X = _outer_side_bar_area.X + InnerBarSpacing;
//            _inner_side_bar_area.Y = _outer_side_bar_area.Y + InnerBarSpacing + _iscrollable_parent.Scroll.Y.GetCurrent() * modifier.Y;
//            _inner_bottom_bar_area.X = _outer_bar_bottom_area.X + InnerBarSpacing + _iscrollable_parent.Scroll.X.GetCurrent() * modifier.X;
//            _inner_bottom_bar_area.Y = _outer_bar_bottom_area.Y + InnerBarSpacing;

//            #endregion Position

//            #region Palette

//            // Modify these so the width of the bar matches the outer rather
//            // than the inner bar
//            // Update palettes ---
//            if (_inner_bottom_bar_area.Contains(_parent.CursorPosition)) BottomInnerBarPalette.Hovered = true; else BottomInnerBarPalette.Hovered = false;
//            if (_inner_side_bar_area.Contains(_parent.CursorPosition)) SideInnerBarPalette.Hovered = true; else SideInnerBarPalette.Hovered = false;
//            if (_outer_bar_bottom_area.Contains(_parent.CursorPosition)) BottomOuterBarPalette.Hovered = true; else BottomOuterBarPalette.Hovered = false;
//            if (_outer_side_bar_area.Contains(_parent.CursorPosition)) SideOuterBarPalette.Hovered = true; else SideOuterBarPalette.Hovered = false;

//            SideOuterBarPalette.Update(step);
//            BottomOuterBarPalette.Update(step);
//            SideInnerBarPalette.Update(step);
//            BottomInnerBarPalette.Update(step);

//            #endregion Palette

//            #region Grabbing and dragging the bars

//            // Release the bars if the cursor isn't clicking.
//            if (!ui_input_state.PrimaryClick)
//            {
//                _bottom_bar_held = false;
//                _side_bar_held = false;
//            }

//            // Hold/Continue holding the bars.
//            if ((BottomInnerBarPalette.Hovered && ui_input_state.PrimaryClickTriggered && _BottomVisible) || _bottom_bar_held)
//            {
//                BottomInnerBarPalette.SpecialColorEnabled = true;
//                if (!_bottom_bar_held)
//                {
//                    _bottom_bar_held = true;
//                    _bottom_bar_cursor_initial_x_value = ui_input_state.CursorPosition.X;
//                    _bottom_bar_initial_x_value = _iscrollable_parent.Scroll.X.GetCurrent();
//                }
//            }
//            else
//            {
//                BottomInnerBarPalette.SpecialColorEnabled = false;
//            }
//            if ((SideInnerBarPalette.Hovered && ui_input_state.PrimaryClickTriggered && _SideVisible) || _side_bar_held)
//            {
//                SideInnerBarPalette.SpecialColorEnabled = true;
//                if (!_side_bar_held)
//                {
//                    _side_bar_held = true;
//                    _side_bar_cursor_initial_y_value = ui_input_state.CursorPosition.Y;
//                    _side_bar_initial_y_value = _iscrollable_parent.Scroll.Y.GetCurrent();
//                }
//            }
//            else
//            {
//                SideInnerBarPalette.SpecialColorEnabled = false;
//            }

//            // Apply offset.
//            if (_side_bar_held)
//            {
//                _iscrollable_parent.Scroll.Y.SetTargetValue(
//                    _side_bar_initial_y_value +
//                    (ui_input_state.CursorPosition.Y - _side_bar_cursor_initial_y_value) / modifier.Y
//                    , true);
//            }
//            if (_bottom_bar_held)
//            {
//                _iscrollable_parent.Scroll.X.SetTargetValue(
//                    _bottom_bar_initial_x_value +
//                    (ui_input_state.CursorPosition.X - _bottom_bar_cursor_initial_x_value) / modifier.X
//                    , true);
//            }

//            #endregion Grabbing and dragging the bars

//            #region Restraining

//            // Don't let the scrollbars go out of bounds.
//            if (_iscrollable_parent.Scroll.X.GetCurrent() < 0f) _iscrollable_parent.Scroll.X.SetTargetValue(0f, true);
//            if (_iscrollable_parent.Scroll.Y.GetCurrent() < 0f) _iscrollable_parent.Scroll.Y.SetTargetValue(0f, true);
//            if (_iscrollable_parent.Scroll.X.GetCurrent() > widget_content_area.Width - widget_area.Width) _iscrollable_parent.Scroll.X.SetTargetValue(widget_content_area.Width - widget_area.Width, true);
//            if (_iscrollable_parent.Scroll.Y.GetCurrent() > widget_content_area.Height - widget_area.Height) _iscrollable_parent.Scroll.Y.SetTargetValue(widget_content_area.Height - widget_area.Height, true);

//            #endregion Restraining
//        }
//    }
//}