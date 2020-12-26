using DownUnder.Utilities;
using System;
using System.Collections.Generic;

namespace DownUnder
{
    /// <summary> Value designed to smoothly transition between two states. </summary>
    public class ChangingValue<T> : ICloneable
    {
        InterpolationSettings interpolation_settings_backing;
        static readonly Func<object, object, float, object> interpolation_method = InterpolationFuncs.GetLerpFunc<T>();
        Func<float, float> plot_method;

        [NonSerialized] private T initial_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T current_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T target_value = (T)Activator.CreateInstance(typeof(T));
        [NonSerialized] private T previous_value = (T)Activator.CreateInstance(typeof(T));

        public float Progress { get; private set; } = 0f; // From 0f to 1f

        /// <summary> Speed modifier of the value. Default is 1f. </summary>
        public float TransitionSpeed 
        { 
            get => interpolation_settings_backing.TransitionSpeed; 
            set => interpolation_settings_backing.TransitionSpeed = value;
        }

        public InterpolationType Interpolation
        {
            get => interpolation_settings_backing.Interpolation;
            set
            {
                if (interpolation_settings_backing.Interpolation != value) plot_method = InterpolationFuncs.GetPlotFunc(value);
                interpolation_settings_backing.Interpolation = value;
            }
        }

        public bool IsTransitioning { get => Progress != 1f; }

        public InterpolationSettings InterpolationSettings {
            set {
                interpolation_settings_backing = value;
                plot_method = InterpolationFuncs.GetPlotFunc(value.Interpolation);
            }
        }

        public ChangingValue() =>
            InterpolationSettings = new InterpolationSettings();
        

        public ChangingValue(InterpolationSettings interpolation) => 
            InterpolationSettings = interpolation;

        public ChangingValue(T value) {
            initial_value = value;
            current_value = value;
            target_value = value;
            InterpolationSettings = new InterpolationSettings();
        }

        public ChangingValue(T initial_value, T target_value, InterpolationSettings interpolation)
        {
            this.initial_value = initial_value;
            current_value = initial_value;
            this.target_value = target_value;
            InterpolationSettings = interpolation;
        }
        public ChangingValue(T initial_value, T current_value, T target_value, InterpolationSettings interpolation)
        {
            this.initial_value = initial_value;
            this.current_value = current_value;
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
            previous_value = current_value;
            current_value = (T)interpolation_method.Invoke(initial_value, target_value, plot_method.Invoke(Progress));
        }

        public T Initial => initial_value;
        public T Current 
        { 
            get => current_value;
            set => SetTargetValue(value, true);
        }
        public T Target
        {
            get => target_value;
            set => SetTargetValue(value);
        }
        public T Previous => previous_value;

        object ICloneable.Clone() => Clone();
        public ChangingValue<T> Clone(bool reset_progress = false) {
            var c = new ChangingValue<T>(initial_value, current_value, target_value, interpolation_settings_backing);

            if (!reset_progress) 
            {
                c.Progress = Progress;
                c.previous_value = previous_value;
            }

            return c;
        }


    }
}