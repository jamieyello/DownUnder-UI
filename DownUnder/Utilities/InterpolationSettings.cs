using DownUnder.Utility;
using System.Runtime.Serialization;

namespace DownUnder.Utilities
{
    [DataContract] public struct InterpolationSettings
    {
        [DataMember] public InterpolationType Interpolation { get; set; }
        [DataMember] public float TransitionSpeed { get; set; }

        public InterpolationSettings(InterpolationType interpolation, float transition_speed)
        {
            Interpolation = interpolation;
            TransitionSpeed = transition_speed;
        }

        public static InterpolationSettings Default => new InterpolationSettings(InterpolationType.fake_sin, 1f);
        public static InterpolationSettings Fast => new InterpolationSettings(InterpolationType.fake_sin, 2.5f);
    }
}
