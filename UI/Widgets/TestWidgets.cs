﻿using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using MonoGame.Extended;

namespace TestContent
{
    public static class TestWidgets
    {
        public static Widget CurrentTest =>
            WidgetTransitionTest();
            //LoginLayoutEffects();
            //BGAnimationTest();

        public static Widget WidgetTransitionTest()
        {
            Widget result = new Widget();
            Widget button = BasicWidgets.Button("Send to container");
            Widget test_widget = new Widget
            {
                Area = new RectangleF(10, 40, 200, 200)
            };
            test_widget.Behaviors.Add(new GridFormat(2, 2));

            button.OnClick += (s, a) =>
            {
                //test_widget.Actions.Add(new ReplaceWidget(new Widget().WithAddedBehavior(new DrawText("New Widget")), InnerWidgetLocation.))
            };

            result.Add(button);
            result.Add(test_widget);
            return result;
        }

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

//            layout.Behaviors.Add(new DrawText
//            {
//                Text =
//@"Sometimes stuff will be drawn under widgets. Those widgets will sometimes use the graphics
//under itself (parent widgets) to use in its own Effects. This can be done by redrawing part
//of the parent's completed render directly under the Widget with an effect applied.

//This makes effects like blur and defraction possible and fairly easy to implement.",
//                XTextPositioning = DrawText.XTextPositioningPolicy.center,
//                YTextPositioning = DrawText.YTextPositioningPolicy.center
//            });

            Widget login_button = BasicWidgets.Button("Login");
            login_button.Position = new Point2(20, 20);
            login_button.Size = new Point2(100, 40);
            login_button.OnClick += (s, a) =>
            {
                if (layout["Login Window"] == null) layout.Add(LoginWindow());
            };

            layout.Add(login_button);
            layout.Behaviors.Add(new Neurons());
            layout.Behaviors.Add(ShadingBehavior.GlowingGreen);

            return layout;
        }

        public static Widget LoginWindow()
        {
            Widget window = new Widget { Size = new Point2(400, 300), Name = "Login Window" };
            window.VisualSettings.VisualRole = GeneralVisualSettings.VisualRoleType.pop_up;
            
            window.Behaviors.Add(new CenterContent());
            window.Behaviors.Add(new PinWidget { Pin = InnerWidgetLocation.Centered });
            window.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.975f), RectanglePart.Uniform(0.5f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Faster });

            Widget username_entry = BasicWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            Widget password_entry = BasicWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            
            username_entry.Area = new RectangleF(100, 0, 150, 40);
            password_entry.Area = new RectangleF(100, 60, 150, 40);

            Widget username_label = BasicWidgets.Label("Username:");
            username_label.Area = new RectangleF(0, 0, 60, 40);

            Widget password_label = BasicWidgets.Label("Password:");
            password_label.Area= new RectangleF(0, 60, 60, 40 );

            Widget login_button = BasicWidgets.Button("Login");
            login_button.Area = new RectangleF(100, 120, 150, 40);

            window.Add(username_label);
            window.Add(password_label);
            window.Add(username_entry);
            window.Add(password_entry);
            window.Add(login_button);

            return window;
        }

        public static Widget BGAnimationTest()
        {
            Widget result = new Widget();
            result.Behaviors.Add(new DrawText { Text = "Click me", XTextPositioning = DrawText.XTextPositioningPolicy.center, YTextPositioning = DrawText.YTextPositioningPolicy.center });
            result.Behaviors.Add(new Neurons());
            return result;
        }
    }
}
