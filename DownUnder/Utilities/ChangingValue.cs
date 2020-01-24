using System;
using System.Collections.Generic;

namespace DownUnder.Utility
{
    /// <summary>
    /// Value designed to smoothly transition between two states.
    /// </summary>
    public class ChangingValue<T>
    {
        [NonSerialized] private T initial_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T current_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T target_value = (T)Activator.CreateInstance(typeof(T));

        private float interpolation_progress = 0f; // From 0f to 1f

        public InterpolationType Interpolation { get; set; } = InterpolationType.fake_sin;
        public float TransitionSpeed { get; set; } = 1f;
        public bool IsTransitioning { get => interpolation_progress != 1f; }

        public ChangingValue()
        {
        }

        public ChangingValue(T value)
        {
            initial_value = value;
            current_value = value;
            target_value = value;
        }

        public void SetTargetValue(T target_value, float transition_speed, bool instant = false)
        {
            TransitionSpeed = transition_speed;
            SetTargetValue(target_value);
        }

        public void SetTargetValue(T target_value, bool instant = false)
        {
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
            if (instant)
            {
                ForceComplete();
            }
            else
            {
                interpolation_progress = 0f;
            }
        }

        public void ForceComplete()
        {
            Update(1f);
        }

        public void Update(float step)
        {
            interpolation_progress += TransitionSpeed * step;

            if (interpolation_progress < 0f) interpolation_progress = 0f;
            if (interpolation_progress > 1f) interpolation_progress = 1f;

            current_value = DownUnder.Utility.Interpolation.GetMiddle(initial_value, target_value, interpolation_progress, Interpolation);
        }

        public T GetCurrent()
        {
            return current_value;
        }

        public object Clone()
        {
            var obj = new ChangingValue<T>();
            obj.interpolation_progress = interpolation_progress;
            obj.Interpolation = Interpolation;
            obj.TransitionSpeed = TransitionSpeed;

            return obj;
        }
    }
}