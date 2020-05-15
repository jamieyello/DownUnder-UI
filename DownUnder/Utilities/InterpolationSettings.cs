using DownUnder.Utility;

namespace DownUnder.Utilities
{
    public struct InterpolationSettings
    {
        public InterpolationType Interpolation;
        public float TransitionSpeed;

        public InterpolationSettings(InterpolationType interpolation, float transition_speed)
        {
            Interpolation = interpolation;
            TransitionSpeed = transition_speed;
        }

        public static InterpolationSettings Default => new InterpolationSettings(InterpolationType.fake_sin, 1f);
        public static InterpolationSettings Fast => new InterpolationSettings(InterpolationType.fake_sin, 2.5f);
    }
}
