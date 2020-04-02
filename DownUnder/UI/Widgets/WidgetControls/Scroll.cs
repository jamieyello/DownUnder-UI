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

namespace DownUnder.UI.Widgets.WidgetControls
{
    /// <summary> Scrollbars used commonly in widgets with inner areas. Modifies ScrollX and ScrollY. </summary>
    [DataContract]
    public class Scroll
    {
        private ChangingValue<float> X { get; } = new ChangingValue<float>(0f);
        private ChangingValue<float> Y { get; } = new ChangingValue<float>(0f);
        private ChangingValue<float> SideBarWidth = new ChangingValue<float>();
        private ChangingValue<float> BottomBarHeight = new ChangingValue<float>();

        private IScrollableWidget _iscrollable_parent;
        private Widget _parent;
        private Texture2D _white_dot;
        private RectangleF _outer_bar_bottom_area = new RectangleF();
        private RectangleF _inner_bottom_bar_area = new RectangleF();
        private RectangleF _outer_side_bar_area = new RectangleF();
        private RectangleF _inner_side_bar_area = new RectangleF();
        private RectangleF _bottom_right_square_area = new RectangleF();

        // These are used for grabbing and dragging the inner scroll bars.
        private bool _side_bar_held;

        private float _bottom_bar_cursor_initial_x_value;
        private float _bottom_bar_initial_x_value;
        private bool _bottom_bar_held;
        private float _side_bar_cursor_initial_y_value;
        private float _side_bar_initial_y_value;

        private float _thickness_backing;
        private bool _bottom_visible_backing;

        private bool _BottomVisible { get; set; }
        private bool _SideVisible { get; set; }

        [DataMember] public float Thickness
        {
            get => _thickness_backing;
            set
            {
                _thickness_backing = value;
                _bottom_right_square_area.Size = new Size2(Thickness, Thickness);
            }
        }
        [DataMember] public ElementColors SideOuterBarPalette { get; private set; }
        [DataMember] public ElementColors BottomOuterBarPalette { get; private set; }
        [DataMember] public ElementColors SideInnerBarPalette { get; private set; }
        [DataMember] public ElementColors BottomInnerBarPalette { get; private set; }
        [DataMember] public ElementColors BottomRightSquarePalette { get; private set; }

        [DataMember] public float InnerBarSpacing { get; set; } = 4f;

        public float SidebarHideSpeed
        {
            get => SideBarWidth.TransitionSpeed;
            set
            {
                SideBarWidth.TransitionSpeed = value;
                BottomBarHeight.TransitionSpeed = value;
            }
        }

        public Scroll(IScrollableWidget parent, GraphicsDevice graphics_device)
        {
            _iscrollable_parent = parent;
            _parent = (Widget)parent;
            _white_dot = DrawingTools.Dot(graphics_device, Color.White);

            SideInnerBarPalette = (ElementColors)_parent.Theme.InnerScrollBar.Clone();
            BottomInnerBarPalette = (ElementColors)_parent.Theme.InnerScrollBar.Clone();
            SideOuterBarPalette = (ElementColors)_parent.Theme.OuterScrollBar.Clone();
            BottomOuterBarPalette = (ElementColors)_parent.Theme.OuterScrollBar.Clone();
            BottomRightSquarePalette = (ElementColors)_parent.Theme.OuterScrollBar.Clone();

            Thickness = 20f;
            SidebarHideSpeed = 4f;
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
                sprite_batch.Draw(_white_dot, _outer_bar_bottom_area.ToRectangle(), BottomOuterBarPalette.CurrentColor);
                sprite_batch.Draw(_white_dot, _inner_bottom_bar_area.ToRectangle(), BottomInnerBarPalette.CurrentColor);
            }

            if (_SideVisible || _BottomVisible)
            {
                sprite_batch.Draw(_white_dot, _bottom_right_square_area.ToRectangle(), BottomRightSquarePalette.CurrentColor);
            }
        }

        public void Update(float step, UIInputState ui_input_state)
        {
            #region Position / Size
            RectangleF widget_content_area = _iscrollable_parent.ContentArea;
            RectangleF drawing_area = _parent.DrawingArea;
            RectangleF widget_area = _parent.Area;
            RectangleF area_in_window = _parent.AreaInWindow;

            if (_SideVisible) { SideBarWidth.SetTargetValue(Thickness); } else { SideBarWidth.SetTargetValue(0f); }
            if (_BottomVisible) { BottomBarHeight.SetTargetValue(Thickness); } else { BottomBarHeight.SetTargetValue(0f); }
            SideBarWidth.Update(step);
            BottomBarHeight.Update(step);

            // Calculate size of bars.
            _outer_bar_bottom_area.Height = BottomBarHeight.GetCurrent();
            _outer_side_bar_area.Width = SideBarWidth.GetCurrent();
            _inner_bottom_bar_area.Height = _outer_bar_bottom_area.Height - InnerBarSpacing * 2;
            _inner_side_bar_area.Width = _outer_side_bar_area.Width - InnerBarSpacing * 2;

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

            _outer_bar_bottom_area.Width = drawing_area.Width - Thickness;
            _outer_side_bar_area.Height = drawing_area.Height - Thickness;

            _inner_bottom_bar_area.Width = (drawing_area.Width) * modifier.X;
            _inner_side_bar_area.Height = (drawing_area.Height) * modifier.Y;

            _bottom_right_square_area.Position = _outer_bar_bottom_area.TopRight();
            _bottom_right_square_area.Size = new Size2(Thickness, Thickness);

            // Calculate the positions of the bars.
            _outer_bar_bottom_area.X = area_in_window.X;
            _outer_side_bar_area.Y = area_in_window.Y;
            _outer_bar_bottom_area.Y = area_in_window.Bottom - _outer_bar_bottom_area.Height;
            _outer_side_bar_area.X = area_in_window.Right - _outer_side_bar_area.Width;

            _inner_side_bar_area.X = _outer_side_bar_area.X + InnerBarSpacing;
            _inner_side_bar_area.Y = _outer_side_bar_area.Y + InnerBarSpacing + Y.GetCurrent() * modifier.Y;
            _inner_bottom_bar_area.X = _outer_bar_bottom_area.X + InnerBarSpacing + X.GetCurrent() * modifier.X;
            _inner_bottom_bar_area.Y = _outer_bar_bottom_area.Y + InnerBarSpacing;

            #endregion Position

            #region Palette

            // Update palettes
            Point2 cursor_position = ui_input_state.CursorPosition;
            if (_parent.DrawMode == Widget.DrawingMode.use_render_target) cursor_position = _parent.CursorPosition;

            BottomInnerBarPalette.Hovered = 
                _inner_bottom_bar_area
                .ResizedBy(InnerBarSpacing, Directions2D.UpDown)
                .Contains(cursor_position);
            SideInnerBarPalette.Hovered = 
                _inner_side_bar_area
                .ResizedBy(InnerBarSpacing, Directions2D.LeftRight)
                .Contains(cursor_position);
            BottomOuterBarPalette.Hovered = _bottom_bar_held || _outer_bar_bottom_area.Contains(cursor_position);
            SideOuterBarPalette.Hovered = _side_bar_held || _outer_side_bar_area.Contains(cursor_position);

            SideOuterBarPalette.Update(step);
            BottomOuterBarPalette.Update(step);
            SideInnerBarPalette.Update(step);
            BottomInnerBarPalette.Update(step);
            BottomRightSquarePalette.Update(step);

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
                    _bottom_bar_initial_x_value = X.GetCurrent();
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
                    _side_bar_initial_y_value = Y.GetCurrent();
                }
            }
            else
            {
                SideInnerBarPalette.SpecialColorEnabled = false;
            }

            // Apply offset.
            if (_side_bar_held)
            {
                Y.SetTargetValue(
                    _side_bar_initial_y_value +
                    (ui_input_state.CursorPosition.Y - _side_bar_cursor_initial_y_value) / modifier.Y
                    , true);
            }
            if (_bottom_bar_held)
            {
                X.SetTargetValue(
                    _bottom_bar_initial_x_value +
                    (ui_input_state.CursorPosition.X - _bottom_bar_cursor_initial_x_value) / modifier.X
                    , true);
            }

            #endregion Grabbing and dragging the bars

            #region Restraining

            // Don't let the scrollbars go out of bounds.
            if (X.GetCurrent() < 0f) X.SetTargetValue(0f, true);
            if (Y.GetCurrent() < 0f) Y.SetTargetValue(0f, true);
            if (X.GetCurrent() > widget_content_area.Width - widget_area.Width)
                X.SetTargetValue(widget_content_area.Width - widget_area.Width, true);
            if (Y.GetCurrent() > widget_content_area.Height - widget_area.Height)
                Y.SetTargetValue(widget_content_area.Height - widget_area.Height, true);

            #endregion Restraining
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X.GetCurrent(), Y.GetCurrent());
        }

        public Point2 ToPoint2()
        {
            return new Point2(X.GetCurrent(), Y.GetCurrent());
        }
    }
}