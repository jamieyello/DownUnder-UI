using DownUnder.Utilities;
using System;
using System.Runtime.Serialization;

namespace DownUnder
{
    [DataContract] public struct InterpolationSettings
    {
        [DataMember] public InterpolationType Interpolation 
        { 
            get; 
            set; 
        }

        [DataMember] public float TransitionSpeed { get; set; }

        public InterpolationSettings(InterpolationType interpolation, float transition_speed)
        {
            Interpolation = interpolation;
            TransitionSpeed = transition_speed;
        }

        /// <summary> Completed in 1 second. </summary>
        public static InterpolationSettings Default => new InterpolationSettings(InterpolationType.fake_sin, 1f);
        public static InterpolationSettings Fast => new InterpolationSettings(InterpolationType.fake_sin, 2.5f);
        public static InterpolationSettings Faster => new InterpolationSettings(InterpolationType.fake_sin, 5f);
        public static InterpolationSettings Fastest => new InterpolationSettings(InterpolationType.fake_sin, 10f);
        public static InterpolationSettings ExtremeSpeed => new InterpolationSettings(InterpolationType.fake_sin, 20f);
    }
}
