// Enums are usually pretty self-explanitory.
namespace DownUnder.Input
{
    public enum ControllerType : int
    {
        null_ = 0,
        xbox = 1,
        keyboard = 2,
        mouse = 3
    }

    public enum InputType : int
    {
        trigger = 0,
        analog = 1
    }

    public enum MouseButtons : int
    {
        x_movement,
        y_movement,
        left_button,
        right_button
    }
}