using System;

// Return value used by Action.GetButton
namespace DownUnder.Input {
    public sealed class PseudoButton {
        readonly bool _pressed;
        readonly float _analog;
        readonly bool _triggered;

        public PseudoButton() {
        }

        public PseudoButton(
            bool pressed,
            float analog,
            bool triggered
        ) {
            _pressed = pressed;
            _analog = analog;
            _triggered = triggered;
        }

        public static PseudoButton operator |(PseudoButton ab1, PseudoButton ab2) =>
            new PseudoButton(
                ab1._pressed || ab2._pressed,
                Math.Max(ab1._analog, ab2._analog),
                ab1._triggered || ab2._triggered
            );

        public static PseudoButton operator &(PseudoButton ab1, PseudoButton ab2) =>
            new PseudoButton(
                ab1._pressed && ab2._pressed,
                Math.Min(ab1._analog, ab2._analog),
                ab1._triggered && ab2._triggered
            );
    }
}