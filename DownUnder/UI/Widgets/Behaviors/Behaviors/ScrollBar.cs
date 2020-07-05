using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class ScrollBar : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.SCROLL_FUNCTION };

        private Texture2D _white_dot;

        private ChangingValue<float> X { get; } = new ChangingValue<float>(0f);
        private ChangingValue<float> Y { get; } = new ChangingValue<float>(0f);
        private readonly ChangingValue<float> SideBarWidth = new ChangingValue<float>();
        private readonly ChangingValue<float> BottomBarHeight = new ChangingValue<float>();

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

        private bool _BottomVisible { get; set; }
        private bool _SideVisible { get; set; }

        [DataMember]
        public float Thickness
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

        public float SidebarHideSpeed {
            get => SideBarWidth.TransitionSpeed;
            set {
                SideBarWidth.TransitionSpeed = value;
                BottomBarHeight.TransitionSpeed = value;
            }
        }

        public ScrollBar()
        {
            Thickness = 20f;
            SidebarHideSpeed = 2f;
        }

        public override object Clone()
        {
            return new ScrollBar();
        }

        protected override void Initialize()
        {
            SideInnerBarPalette = (ElementColors)Parent.Theme.InnerScrollBar.Clone();
            BottomInnerBarPalette = (ElementColors)Parent.Theme.InnerScrollBar.Clone();
            SideOuterBarPalette = (ElementColors)Parent.Theme.OuterScrollBar.Clone();
            BottomOuterBarPalette = (ElementColors)Parent.Theme.OuterScrollBar.Clone();
            BottomRightSquarePalette = (ElementColors)Parent.Theme.OuterScrollBar.Clone();
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
            Parent.OnDrawOverlay += Draw;
            if (Parent.IsGraphicsInitialized) InitializeGraphics(this, EventArgs.Empty);
            else Parent.OnGraphicsInitialized += InitializeGraphics;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
            Parent.OnDrawOverlay -= Draw;
            Parent.OnGraphicsInitialized -= InitializeGraphics;
        }

        private void InitializeGraphics(object sender, EventArgs args)
        {
            _white_dot = DrawingTools.Dot(Parent.GraphicsDevice, Color.White);
        }

        private void Draw(object sender, EventArgs args)
        {
            Parent.SpriteBatch.Draw(_white_dot, _outer_side_bar_area.ToRectangle(), SideOuterBarPalette.CurrentColor);
            Parent.SpriteBatch.Draw(_white_dot, _inner_side_bar_area.ToRectangle(), SideInnerBarPalette.CurrentColor);

            Parent.SpriteBatch.Draw(_white_dot, _outer_bar_bottom_area.ToRectangle(), BottomOuterBarPalette.CurrentColor);
            Parent.SpriteBatch.Draw(_white_dot, _inner_bottom_bar_area.ToRectangle(), BottomInnerBarPalette.CurrentColor);

            if (_SideVisible || _BottomVisible) Parent.SpriteBatch.Draw(_white_dot, _bottom_right_square_area.ToRectangle(), BottomRightSquarePalette.CurrentColor);
        }

        private void Update(object sender, EventArgs args)
        {
            #region Position / Size

            RectangleF widget_content_area = Parent.ContentArea;
            RectangleF drawing_area = Parent.DrawingAreaUnscrolled;
            RectangleF widget_area = Parent.Area;

            if (_SideVisible) { SideBarWidth.SetTargetValue(Thickness); } else { SideBarWidth.SetTargetValue(0f); }
            if (_BottomVisible) { BottomBarHeight.SetTargetValue(Thickness); } else { BottomBarHeight.SetTargetValue(0f); }
            SideBarWidth.Update(Parent.UpdateData.ElapsedSeconds);
            BottomBarHeight.Update(Parent.UpdateData.ElapsedSeconds);

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
            _outer_bar_bottom_area.X = drawing_area.X;
            _outer_side_bar_area.Y = drawing_area.Y;
            _outer_bar_bottom_area.Y = drawing_area.Bottom - _outer_bar_bottom_area.Height;
            _outer_side_bar_area.X = drawing_area.Right - _outer_side_bar_area.Width;

            _inner_side_bar_area.X = _outer_side_bar_area.X + InnerBarSpacing;
            _inner_side_bar_area.Y = _outer_side_bar_area.Y + InnerBarSpacing + Y.GetCurrent() * modifier.Y;
            _inner_bottom_bar_area.X = _outer_bar_bottom_area.X + InnerBarSpacing + X.GetCurrent() * modifier.X;
            _inner_bottom_bar_area.Y = _outer_bar_bottom_area.Y + InnerBarSpacing;

            #endregion Position

            #region Palette

            // Update palettes
            Point2 cursor_position = Parent.UpdateData.UIInputState.CursorPosition;
            if (Parent.DrawingMode == Widget.DrawingModeType.use_render_target) cursor_position = Parent.CursorPosition;

            BottomInnerBarPalette.Hovered =
                _inner_bottom_bar_area
                .ResizedBy(InnerBarSpacing, Directions2D.UD)
                .Contains(cursor_position);
            SideInnerBarPalette.Hovered =
                _inner_side_bar_area
                .ResizedBy(InnerBarSpacing, Directions2D.LR)
                .Contains(cursor_position);
            BottomOuterBarPalette.Hovered = _bottom_bar_held || _outer_bar_bottom_area.Contains(cursor_position);
            SideOuterBarPalette.Hovered = _side_bar_held || _outer_side_bar_area.Contains(cursor_position);

            SideOuterBarPalette.Update(Parent.UpdateData.ElapsedSeconds);
            BottomOuterBarPalette.Update(Parent.UpdateData.ElapsedSeconds);
            SideInnerBarPalette.Update(Parent.UpdateData.ElapsedSeconds);
            BottomInnerBarPalette.Update(Parent.UpdateData.ElapsedSeconds);
            BottomRightSquarePalette.Update(Parent.UpdateData.ElapsedSeconds);

            #endregion Palette

            #region Grabbing and dragging the bars

            // Release the bars if the cursor isn't clicking.
            if (!Parent.UpdateData.UIInputState.PrimaryClick)
            {
                _bottom_bar_held = false;
                _side_bar_held = false;
            }

            // Hold/Continue holding the bars.
            if ((BottomInnerBarPalette.Hovered && Parent.UpdateData.UIInputState.PrimaryClickTriggered && _BottomVisible) || _bottom_bar_held)
            {
                BottomInnerBarPalette.SpecialColorEnabled = true;
                if (!_bottom_bar_held)
                {
                    _bottom_bar_held = true;
                    _bottom_bar_cursor_initial_x_value = Parent.UpdateData.UIInputState.CursorPosition.X;
                    _bottom_bar_initial_x_value = X.GetCurrent();
                }
            }
            else BottomInnerBarPalette.SpecialColorEnabled = false;

            if ((SideInnerBarPalette.Hovered && Parent.UpdateData.UIInputState.PrimaryClickTriggered && _SideVisible) || _side_bar_held)
            {
                SideInnerBarPalette.SpecialColorEnabled = true;
                if (!_side_bar_held)
                {
                    _side_bar_held = true;
                    _side_bar_cursor_initial_y_value = Parent.UpdateData.UIInputState.CursorPosition.Y;
                    _side_bar_initial_y_value = Y.GetCurrent();
                }
            }
            else SideInnerBarPalette.SpecialColorEnabled = false;

            // Apply offset.
            if (_side_bar_held)
            {
                Y.SetTargetValue(
                    _side_bar_initial_y_value +
                    (Parent.UpdateData.UIInputState.CursorPosition.Y - _side_bar_cursor_initial_y_value) / modifier.Y
                    , true);
            }
            if (_bottom_bar_held)
            {
                X.SetTargetValue(
                    _bottom_bar_initial_x_value +
                    (Parent.UpdateData.UIInputState.CursorPosition.X - _bottom_bar_cursor_initial_x_value) / modifier.X
                    , true);
            }

            #endregion Grabbing and dragging the bars

            // Don't let the scrollbars go out of bounds.
            if (X.GetCurrent() < 0f) X.SetTargetValue(0f, true);
            if (Y.GetCurrent() < 0f) Y.SetTargetValue(0f, true);
            if (X.GetCurrent() > widget_content_area.Width - widget_area.Width)
                X.SetTargetValue(widget_content_area.Width - widget_area.Width, true);
            if (Y.GetCurrent() > widget_content_area.Height - widget_area.Height)
                Y.SetTargetValue(widget_content_area.Height - widget_area.Height, true);

            Parent.Scroll = new Point2(-X.GetCurrent(), -Y.GetCurrent());
        }
    }
}
