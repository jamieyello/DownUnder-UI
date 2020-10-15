using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Threading;

namespace TestContent
{
    public static class TestWidgets
    {
        public static Widget CurrentTest =>
            LoginLayoutEffects();

        public static Widget CenterTest()
        {
            Widget layout = new Widget();
            layout.Behaviors.Add(new CenterContent());

            Widget inner = new Widget { Size = new Point2(300, 200) };
            Widget second_inner = new Widget { Position = new Point2(350, 40) };
            Widget third_inner = new Widget { Position = new Point2(600, 40) };

            layout.Add(inner);
            layout.Add(second_inner);
            layout.Add(third_inner);

            return layout;
        }

        public static Widget LoginLayoutEffects()
        {
            Widget layout = new Widget { };
            layout.VisualSettings.ChangeColorOnMouseOver = false;
            //layout.DrawingMode = Widget.DrawingModeType.use_render_target;
            //layout.Behaviors.Add(MouseGlow.SubtleGray);
            layout.Behaviors.Add(new DrawText
            {
                Text =
@"Sometimes stuff will be drawn under widgets. Those widgets will sometimes use the graphics
under itself (parent widgets) to use in its own Effects. This can be done by redrawing part
of the parent's completed render directly under the Widget with an effect applied.

This makes effects like blur and defraction possible and fairly easy to implement.",
                XTextPositioning = DrawText.XTextPositioningPolicy.center,
                YTextPositioning = DrawText.YTextPositioningPolicy.center
            });

            Widget login_button = BasicWidgets.GenericButton("Login");
            login_button.Position = new Point2(20, 20);
            login_button.Size = new Point2(100, 40);
            login_button.OnClick += (s, a) =>
            {
                Widget login_popup = LoginWindow();
                login_popup.VisualSettings.DrawBackground = false;
                if (layout["Login Window"] == null) layout.Add(login_popup);
            };

            layout.Add(login_button);

            return layout;
        }

        public static Widget LoginWindow()
        {
            Widget window = new Widget { Size = new Point2(450, 320), Name = "Login Window" };
            window.Behaviors.Add(new CenterContent());
            window.Behaviors.Add(new PinWidget { Pin = InnerWidgetLocation.Centered });
            window.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.975f), RectanglePart.Uniform(0.1f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Fast });
            window.Behaviors.Add(MouseGlow.SubtleGray);
            window.Behaviors.GetFirst<MouseGlow>().ActivationPolicy = MouseGlow.MouseGlowActivationPolicy.hovered_over;

            window.VisualSettings.ChangeColorOnMouseOver = true;

            Widget username_entry = BasicWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            Widget password_entry = BasicWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);

            username_entry.Area = new RectangleF(100, 0, 150, 40);
            password_entry.Area = new RectangleF(100, 60, 150, 40);

            Widget username_label = new Widget { Position = new Point2(0, 0), Width = 60, Height = 40};
            username_label.VisualSettings.DrawBackground = false;
            username_label.VisualSettings.DrawOutline = false;

            Widget password_label = new Widget { Position = new Point2(0, 60), Width = 60, Height = 40 };
            password_label.VisualSettings.DrawBackground = false;
            password_label.VisualSettings.DrawOutline = false;

            Widget login_button = BasicWidgets.GenericButton("Login");
            login_button.Area = new RectangleF(100, 120, 150, 40);

            ShadingBehavior blue = ShadingBehavior.SubtleBlue;
            blue.BorderVisibility = 0.3f;
            blue.GradientVisibility = new Point2(0.2f, 0.2f);
            window.Behaviors.Add(blue);
            window.Behaviors.Add(new BlurBackground());

            username_label.Behaviors.Add(new DrawText { Text = "Username: ", YTextPositioning = DrawText.YTextPositioningPolicy.center });
            password_label.Behaviors.Add(new DrawText { Text = "Password: ", YTextPositioning = DrawText.YTextPositioningPolicy.center });

            username_entry.Behaviors.GetFirst<DrawText>().ConstrainAreaToText = false;
            password_entry.Behaviors.GetFirst<DrawText>().ConstrainAreaToText = false;

            window.Add(username_label);
            window.Add(password_label);
            window.Add(username_entry);
            window.Add(password_entry);
            window.Add(login_button);

            return window;
        }
    }
}
