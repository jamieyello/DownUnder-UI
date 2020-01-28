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
        RectangleF ContentArea { get; }
        Scroll Scroll { get; }
        bool FitToContentArea { get; set; }
    }
}