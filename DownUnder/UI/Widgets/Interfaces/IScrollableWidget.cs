using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.WidgetControls;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Interfaces
{
    /// <summary>
    /// Widgets that have an inner area.
    /// </summary>
    public interface IScrollableWidget
    {
        ScrollBars ScrollBars { get; }
        RectangleF ContentArea { get; set; }
        float ContentWidth { get; set; }
        float ContentHeight { get; set; }
        Point2 ContentPosition { get; set; }
        Point2 ContentSize { get; set; }
        float ContentX { get; set; }
        float ContentY { get; set; }
        Scroll Scroll { get; }
    }
}