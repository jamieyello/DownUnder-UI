using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

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
            Widget layout = new Widget { ChangeColorOnMouseOver = true };
            //layout.DrawingMode = Widget.DrawingModeType.use_render_target;

            layout.Behaviors.Add(new DrawText
            {
                Text =
@"Sometimes stuff will be drawn under widgets. Those widgets will sometimes use the graphics
under itself (parent widgets) to use in its own Effects. DownUnder does this by rendering a
transparent rectangle below a widget before drawing it and applying any defined Effects.

This makes effects like blur and defraction possible and fairly easy to implement.",
                TextPositioning = DrawText.TextPositioningPolicy.center
            });

            Widget login_button = BasicWidgets.GenericButton("Login");
            login_button.Position = new Point2(20, 20);
            login_button.OnClick += (s, a) =>
            {
                Widget login_popup = LoginWindow();
                login_popup.DrawBackground = false;
                if (layout["Login Window"] == null) layout.Add(login_popup);
            };

            layout.Add(login_button);

            return layout;
        }

        public static Widget LoginLayout()
        {
            Widget layout = new Widget { ChangeColorOnMouseOver = false };

            Widget login_button = BasicWidgets.GenericButton("Login");
            login_button.Position = new Point2(20, 20);
            login_button.OnClick += (s, a) => 
            {
                if (layout["Login Window"] == null) layout.Add(LoginWindow());
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

            window.ChangeColorOnMouseOver = true;

            Widget username_entry = new Widget { Position = new Point2(100, 0), Width = 150, Height = 40 };
            Widget password_entry = new Widget { Position = new Point2(100, 60), Width = 150, Height = 40 };
            Widget username_label = new Widget { Position = new Point2(0, 0), Width = 60, Height = 40, DrawBackground = false, DrawOutline = false };
            Widget password_label = new Widget { Position = new Point2(0, 60), Width = 60, Height = 40, DrawBackground = false, DrawOutline = false };
            Widget login_button = new Widget { Area = new RectangleF(100, 120, 150, 40) };

            username_entry.Behaviors.Add(new ShadingBehavior { ShadeColor = Color.White, BorderWidth = 10, BorderVisibility = 0.05f, GradientVisibility = new Point2(0.05f, 0.03f) });
            password_entry.Behaviors.Add(new ShadingBehavior { ShadeColor = Color.White, BorderWidth = 10, BorderVisibility = 0.05f, GradientVisibility = new Point2(0.05f, 0.03f) });
            ShadingBehavior blue = ShadingBehavior.SubtleBlue;
            blue.BorderVisibility = 0.3f;
            blue.GradientVisibility = new Point2(0.1f, 0.1f);
            window.Behaviors.Add(blue);
            window.Behaviors.Add(new BGEffectTest());

            username_label.Behaviors.Add(new DrawText { Text = "Username: " });
            password_label.Behaviors.Add(new DrawText { Text = "Password: " });
            login_button.Behaviors.Add(new DrawText { Text = "Login", TextPositioning = DrawText.TextPositioningPolicy.center });

            username_entry.Behaviors.Add(new DrawEditableText());
            password_entry.Behaviors.Add(new DrawEditableText());

            username_entry.Behaviors.GetFirst<DrawText>().ConstrainAreaToText = false;
            password_entry.Behaviors.GetFirst<DrawText>().ConstrainAreaToText = false;

            window.Add(username_label);
            //window.Add(password_label);
            window.Add(username_entry);
            window.Add(password_entry);
            window.Add(login_button);

            return window;
        }
    }
}
