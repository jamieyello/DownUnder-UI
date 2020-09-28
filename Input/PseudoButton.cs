using System;

// Return value used by Action.GetButton
namespace DownUnder.Input
{
    public class PseudoButton
    {
        public bool pressed = false;
        public float analog = 0f;
        public bool triggered = false;

        public PseudoButton()
        {
        }

        public PseudoButton(bool pressed_, float analog_, bool triggered_)
        {
            pressed = pressed_;
            analog = analog_;
            triggered = triggered_;
        }

        public static PseudoButton operator |(PseudoButton ab1, PseudoButton ab2)
        {
            return new PseudoButton(
                ab1.pressed || ab2.pressed,
                Math.Max(ab1.analog, ab2.analog),
                ab1.triggered || ab2.triggered
            );
        }

        public static PseudoButton operator &(PseudoButton ab1, PseudoButton ab2)
        {
            return new PseudoButton(
                ab1.pressed && ab2.pressed,
                Math.Min(ab1.analog, ab2.analog),
                ab1.triggered && ab2.triggered
            );
        }
    }
}