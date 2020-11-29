using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DownUnder.UI
{
    public class DesktopWindow : DWindow
    {
        BorderFormat _border_format = new BorderFormat();

        public DesktopWindow(GraphicsDeviceManager graphics, Game game, Widget display_widget = null) : base(graphics, game)
        {
            MainWidget.Behaviors.Add(_border_format);
            MainWidget.VisualSettings.DrawBackground = false;
            _border_format.BorderOccupy.Up.TransitionSpeed = 4f;
            _border_format.BorderOccupy.Up.Interpolation = InterpolationType.fake_sin;

            _border_format.TopBorder = CommonWidgets.WindowHandle(MainWidget);
            _border_format.TopBorder.ParentDWindow.OnToggleFullscreen += (s, a) =>
            {
                if (_border_format.TopBorder.ParentDWindow.IsFullscreen)
                {
                    DisableTopBar();
                    MainWidget.VisualSettings.DrawOutline = false;
                }
                else
                {
                    EnableTopBar();
                    MainWidget.VisualSettings.DrawOutline = true;
                }
            };
            
            _border_format.Center = new Widget { };
            _border_format.Center.VisualSettings = GeneralVisualSettings.Invisible;
            DisplayWidget = display_widget ?? new Widget { };
        }

        public override Widget DisplayWidget
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

        public void EnableTopBar()
        {
            _border_format.BorderOccupy.Up.SetTargetValue(1f);
            _border_format.TopBorder.VisualSettings.DrawOutline = true;
            _border_format.TopBorder.VisualSettings.DrawBackground = true;
            _border_format.TopBorder.Behaviors.GetFirst<DrawText>().Visible = true;
            _border_format.TopBorder.PassthroughMouse = false;
        }

        public void DisableTopBar()
        {
            _border_format.BorderOccupy.Up.SetTargetValue(0f);
            _border_format.TopBorder.VisualSettings.DrawOutline = false;
            _border_format.TopBorder.VisualSettings.DrawBackground = false;
            _border_format.TopBorder.Behaviors.GetFirst<DrawText>().Visible = false;
            _border_format.TopBorder.PassthroughMouse = true;
        }
    }
}
