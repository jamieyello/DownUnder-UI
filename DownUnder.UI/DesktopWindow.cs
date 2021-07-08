using Microsoft.Xna.Framework;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Visual;
using static DownUnder.UI.DesktopWindow.TopBarPolicyType;
using DownUnder.UI.Utilities;

namespace DownUnder.UI
{
    public sealed class DesktopWindow : DWindow {
        readonly BorderFormat _border_format = new BorderFormat();
        TopBarPolicyType _top_bar_policy;
        readonly bool _initialized;

        public enum TopBarPolicyType {
            enable,
            disable,
            fullscreen_toggle
        }

        public TopBarPolicyType TopBarPolicy {
            get => _top_bar_policy;
            set {
                _top_bar_policy = value;
                if (value == disable) DisableTopBar();
                else if (value == enable) EnableTopBar();
                else if (value == fullscreen_toggle) ToggleTopBarWithFullscreen();
            }
        }

        /// <summary> Get the <see cref="ChangingValue{float}"/> animation value of the top border expanding/retracting. </summary>
        public ChangingValue<float> GetTopBorderAnimation() =>
            _border_format.BorderOccupy.Up;

        /// <summary> A <see cref="DWindow"/> suited for desktop environments. </summary>
        public DesktopWindow(
            GraphicsDeviceManager graphics,
            Game game,
            IOSInterface os_interface,
            Widget display_widget = null,
            GroupBehaviorCollection group_behaviors = null
        ) : base(graphics, game, os_interface) {
            MainWidget.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);
            MainWidget.Behaviors.Add(_border_format);
            MainWidget.VisualSettings.DrawBackground = false;
            _border_format.BorderOccupy.Up.TransitionSpeed = 4f;
            _border_format.BorderOccupy.Up.Interpolation = InterpolationType.fake_sin;

            _border_format.TopBorder = InternalWidgets.WindowHandle(MainWidget);
            _border_format.TopBorder.ParentDWindow.OnToggleFullscreen += (s, a) => ToggleTopBarWithFullscreen();

            _border_format.Center = new Widget {
                VisualSettings = GeneralVisualSettings.Invisible
            };

            _border_format.Center.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);

            _initialized = true;

            DisplayWidget = display_widget ?? new Widget();
            DisplayWidget.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);
            TopBarPolicy = disable;
            _border_format.BorderOccupy.Up.ForceComplete();
            if (group_behaviors != null) MainWidget.Behaviors.GroupBehaviors.ImplementPolicy(group_behaviors);
        }

        public override Widget DisplayWidget {
            get => _border_format.Center.Children.Count == 0 ? null : _border_format.Center.Children[0];
            set {
                if (_border_format.Center.Children.Count != 0)
                    _border_format.Center.RemoveAt(0);

                value.SnappingPolicy = DiagonalDirections2D.All;
                _border_format.Center.Add(value);
            }
        }

        void ToggleTopBarWithFullscreen() {
            if (TopBarPolicy != fullscreen_toggle)
                return;

            if (_border_format.TopBorder.ParentDWindow.IsFullscreen)
                DisableTopBar();
            else
                EnableTopBar();
        }

        void EnableTopBar() {
            if (!_initialized)
                return;

            _border_format.BorderOccupy.Up.SetTargetValue(1f);
            _border_format.TopBorder.VisualSettings.DrawOutline = true;
            _border_format.TopBorder.VisualSettings.DrawBackground = true;
            _border_format.TopBorder.Behaviors.Get<DrawText>().Visible = true;
            _border_format.TopBorder.PassthroughMouse = false;
            MainWidget.VisualSettings.DrawOutline = true;
        }

        void DisableTopBar() {
            if (!_initialized)
                return;

            _border_format.BorderOccupy.Up.SetTargetValue(0f);
            _border_format.TopBorder.VisualSettings.DrawOutline = false;
            _border_format.TopBorder.VisualSettings.DrawBackground = false;
            _border_format.TopBorder.Behaviors.Get<DrawText>().Visible = false;
            _border_format.TopBorder.PassthroughMouse = true;
            MainWidget.VisualSettings.DrawOutline = false;
        }
    }
}