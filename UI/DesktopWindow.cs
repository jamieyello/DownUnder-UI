using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.Utility;
using Microsoft.Xna.Framework;

namespace DownUnder.UI
{
    public class DesktopWindow : DWindow
    {
        BorderFormat _border_format = new BorderFormat();

        public DesktopWindow(GraphicsDeviceManager graphics, Game game, Widget display_widget = null) : base(graphics, game)
        {
            MainWidget.Behaviors.Add(_border_format);
            _border_format.TopBorder = CommonWidgets.WindowHandle(MainWidget);
            _border_format.Center = new Widget { };
            DisplayWidget = display_widget ?? new Widget();
        }

        public Widget DisplayWidget
        {
            get => _border_format.Center.Children.Count == 0 ? null : _border_format.Center.Children[0];
            set
            {
                if (_border_format.Center.Children.Count != 0)
                {
                    _border_format.Center.RemoveAt(0);
                }

                value.SnappingPolicy = DiagonalDirections2D.All;
                _border_format.Center.Add(value);
            }
        }
    }
}
