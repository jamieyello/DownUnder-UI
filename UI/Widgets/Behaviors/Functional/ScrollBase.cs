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
        private ChangingValue<Point2> Offset = new ChangingValue<Point2>(new InterpolationSettings(Utilities.InterpolationType.fake_sin, 3f));
        public bool HardLock = true;
        /// <summary> Modifier for the range of a single scroll. Default is 0.4f. </summary>
        public float ScrollStep = 0.4f;
        public float ScrollSpeed
        {
            get => Offset.TransitionSpeed;
            set => Offset.TransitionSpeed = value;
        }
        public InterpolationType Interpolation
        {
            get => Offset.Interpolation;
            set => Offset.Interpolation = value;
        }

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.SCROLL_FUNCTION };

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate += Update;
        }

        public override object Clone()
        {
            ScrollBase c = new ScrollBase();
            c.Offset = Offset.Clone();
            c.HardLock = HardLock;
            c.ScrollStep = ScrollStep;
            return c;
        }

        /// <summary> Invoked before updating any limiting logic. </summary>
        public event EventHandler OnPreUpdate;

        void Update(object sender, EventArgs args)
        {
            OnPreUpdate?.Invoke(this, EventArgs.Empty);
            if (!Offset.IsTransitioning) return;

            Offset.Update(Parent.UpdateData.ElapsedSeconds);
            Point2 movement = (Offset.Current - Offset.Previous);
            Parent.Scroll = Parent.Scroll.WithOffset(movement);
            if (HardLock) HardLockScroll();
        }

        public void AddOffset(Point2 offset)
        {
            Offset.SetTargetValue(Offset.Target.WithOffset(offset * new Vector2(ScrollStep, ScrollStep)));
        }

        void HardLockScroll()
        {
            if (Parent.Scroll.X > 0f) Parent.Scroll.X = 0f;
            if (Parent.Scroll.Y > 0f) Parent.Scroll.Y = 0f;
            Point2 scroll_range = GetScrollRange().Inverted();
            if (Parent.Scroll.X < scroll_range.X) Parent.Scroll.X = scroll_range.X;
            if (Parent.Scroll.Y < scroll_range.Y) Parent.Scroll.Y = scroll_range.Y;
        }

        Point2 GetScrollRange()
        {
            RectangleF? area_coverage = Parent.Children.AreaCoverage;
            if (area_coverage == null) return new Point2();
            RectangleF coverage = area_coverage.Value.Union(Parent.Area.SizeOnly());
            return new Point2(coverage.Width - Parent.Area.Width, coverage.Height - Parent.Area.Height);
        }
    }
}
