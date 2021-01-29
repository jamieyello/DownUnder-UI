using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    public class ScrollBase : WidgetBehavior
    {
        private ChangingValue<Point2> _offset = new ChangingValue<Point2>(new InterpolationSettings(Utilities.InterpolationType.fake_sin, 3f));
        Point2 _scroll_range;
        public Directions2D AvailableScrollDirections { get; private set; }
        public Point2 OverScroll { get; private set; } = new Point2();

        /// <summary> Invoked before updating any limiting logic. </summary>
        public event EventHandler OnUpdate;

        /// <summary> Invoked ofter updating limitting logic. </summary>
        public event EventHandler OnPostUpdate;

        public bool HardLock = true;
        /// <summary> Modifier for the range of a single scroll. Default is 0.4f. </summary>
        public float ScrollStep = 0.4f;
        public float ScrollSpeed
        {
            get => _offset.TransitionSpeed;
            set => _offset.TransitionSpeed = value;
        }
        public InterpolationType Interpolation
        {
            get => _offset.Interpolation;
            set => _offset.Interpolation = value;
        }

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.SCROLL_FUNCTION };

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
            Parent.OnResize += HandleResize;
            Parent.OnPreUpdate += PreUpdate;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
            Parent.OnResize -= HandleResize;
            Parent.OnPreUpdate += PreUpdate;
        }

        public override object Clone()
        {
            ScrollBase c = new ScrollBase();
            c._offset = _offset.Clone();
            c.HardLock = HardLock;
            c.ScrollStep = ScrollStep;
            return c;
        }

        void PreUpdate(object sender, PreUpdateArgs args)
        {
            UpdateScrollRange();
            UpdateAvailableScrollDirections();
            if (!args.Flags.hovered_over) return;
            foreach (Focus f in Parent.ParentDWindow.ScrollableWidgetFocus[AvailableScrollDirections]) f.AddFocus(Parent);
        }

        /// <summary> Called by <see cref="Widget"/> to update only once per frame. </summary>
        public void UpdateScrollRange()
        {
            RectangleF? area_coverage = Parent.Children.AreaCoverage;
            if (area_coverage == null)
            {
                _scroll_range = new Point2();
                return;
            }
            
            RectangleF coverage = area_coverage.Value.Union(Parent.Area.SizeOnly());
            _scroll_range = new Point2(coverage.Width - Parent.Area.Width, coverage.Height - Parent.Area.Height).Inverted();
        }

        void Update(object sender, EventArgs args)
        {
            OverScroll = new Point2();
            OnUpdate?.Invoke(this, EventArgs.Empty);
            if (!_offset.IsTransitioning) return;

            _offset.Update(Parent.UpdateData.ElapsedSeconds);
            Point2 movement = (_offset.Current - _offset.Previous);
            Parent.Scroll = Parent.Scroll.WithOffset(movement);
            if (HardLock) HardLockScroll();
            OnPostUpdate?.Invoke(this, EventArgs.Empty);
        }

        void HandleResize(object sender, EventArgs args)
        {
            if (HardLock) HardLockScroll();
        }

        public void AddOffset(Point2 offset)
        {
            _offset.SetTargetValue(_offset.Target.WithOffset(offset * new Vector2(ScrollStep, ScrollStep)));
        }

        void HardLockScroll()
        {
            if (Parent.Scroll.X > 0f)
            {
                OverScroll = OverScroll.WithX(Parent.Scroll.X);
                Parent.Scroll.X = 0f;
            }
            if (Parent.Scroll.Y > 0f)
            {
                OverScroll = OverScroll.WithY(Parent.Scroll.Y);
                Parent.Scroll.Y = 0f;
            }
            if (Parent.Scroll.X < _scroll_range.X)
            {
                OverScroll = OverScroll.WithX(Parent.Scroll.X - _scroll_range.X);
                Parent.Scroll.X = _scroll_range.X;
            }
            if (Parent.Scroll.Y < _scroll_range.Y)
            {
                OverScroll = OverScroll.WithY(Parent.Scroll.Y - _scroll_range.Y);
                Parent.Scroll.Y = _scroll_range.Y;
            }
        }

        void UpdateAvailableScrollDirections()
        {
            AvailableScrollDirections = new Directions2D()
            {
                Up = Parent.Scroll.Y < 0f,
                Down = Parent.Scroll.Y > _scroll_range.Y,
                Left = Parent.Scroll.X < 0f,
                Right = Parent.Scroll.X > _scroll_range.X
            };
        }
    }
}
