using System;
using System.Collections.Generic;

namespace DownUnder
{
    /// <summary> Value designed to smoothly transition between two states. </summary>
    public class ChangingValue<T> : ICloneable
    {
        [NonSerialized] private T initial_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T current_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T target_value = (T)Activator.CreateInstance(typeof(T));

        public float Progress { get; private set; } = 0f; // From 0f to 1f
        public float ProgressPlotted => Interpolation.Plot(Progress, UsedInterpolation);

        public InterpolationType UsedInterpolation { get; set; } = InterpolationType.fake_sin;
        /// <summary> Speed modifier of the value. Default is 1f. </summary>
        public float TransitionSpeed { get; set; } = 1f;
        public bool IsTransitioning { get => Progress != 1f; }
        public InterpolationSettings InterpolationSettings {
            get => new InterpolationSettings(UsedInterpolation, TransitionSpeed);
            set {
                UsedInterpolation = value.Interpolation;
                TransitionSpeed = value.TransitionSpeed;
            }
        }

        public ChangingValue() {}

        public ChangingValue(T value) {
            initial_value = value;
            current_value = value;
            target_value = value;
        }

        public ChangingValue(T initial_value, T target_value, InterpolationSettings interpolation)
        {
            this.initial_value = initial_value;
            current_value = initial_value;
            this.target_value = target_value;
            InterpolationSettings = interpolation;
        }

        public ChangingValue(T target_value, InterpolationSettings interpolation) {
            InterpolationSettings = interpolation;
            SetTargetValue(target_value);
        }

        public void SetTargetValue(T target_value, float transition_speed, bool instant = false) {
            TransitionSpeed = transition_speed;
            SetTargetValue(target_value);
        }

        public void SetTargetValue(T target_value, bool instant = false) {
            // Return if target value is met
            if (EqualityComparer<T>.Default.Equals(target_value, current_value)) return;

            // Return if target value is current target value (already transitioning to this point)
            if (EqualityComparer<T>.Default.Equals(target_value, this.target_value))
            {
                if (instant) ForceComplete();
                return;
            }

            this.target_value = target_value;
            initial_value = current_value;

            // If instant is true, the interpolation will immediately finish.
            if (instant) ForceComplete();
            else Progress = 0f;
        }

        public void ForceComplete() => Update(1f);
        
        public void Update(float step)
        {
            if (Progress >= 1f) {
                Progress = 1f;
                current_value = target_value;
                return;
            }
            if (Progress < 0f) {
                Progress = 0f;
                current_value = initial_value;
                return;
            }

            Progress += TransitionSpeed * step;
            current_value = Interpolation.GetMiddle(initial_value, target_value, Progress, UsedInterpolation);
        }

        public T GetCurrent() => current_value;

        object ICloneable.Clone() => Clone();
        public ChangingValue<T> Clone(bool reset_progress = false) {
            var c = new ChangingValue<T>();
            if (!reset_progress) c.Progress = Progress;
            c.UsedInterpolation = UsedInterpolation;
            c.TransitionSpeed = TransitionSpeed;

            c.initial_value = initial_value;
            c.current_value = current_value;
            c.target_value = target_value;

            return c;
        }


    }
}