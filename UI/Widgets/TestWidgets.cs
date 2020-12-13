using DownUnder;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.DataTypes.AnimatedGraphics;
using MonoGame.Extended;

namespace TestContent
{
    public static class TestWidgets
    {
        public static Widget CurrentTest =>
            //WidgetReorderTest();
            //WidgetTransitionTest();
            //LoginLayoutEffects();
            //BGAnimationTest();
            //GridDebug();
            //GraphicTest();
            SpacedGridTest();

        public static Widget SpacedGridTest()
        {
            Widget result = new Widget();
            result.Add(new Widget(new RectangleF(10, 10, 200, 200)).WithAddedBehavior(new GridFormat(2, 3, null, new Point2(10,10)), out var grid_format), out var grid_widget);
            grid_widget.UserResizePolicy = Widget.UserResizePolicyType.allow;
            grid_widget[0, 0].MinimumHeight = 30f;
            return result;
        }

        public static Widget GraphicTest()
        {
            Widget result = new Widget();
            result.Add(new Widget().WithAddedBehavior(SwitchingGraphic.PausePlayGraphic(), out var pause_play), out var inner);
            inner.Area = new RectangleF(50, 50, 650, 300);
            inner.UserResizePolicy = Widget.UserResizePolicyType.allow;
            inner.VisualSettings.DrawBackground = false;
            inner.VisualSettings.DrawOutline = false;
            //pause_play.Graphic.Progress.InterpolationSettings = InterpolationSettings.Faster;
            //inner.UserRepositionPolicy = Widget.UserResizePolicyType.allow;
            return result;
        }

        public static Widget GridDebug()
        {
            Widget result = new Widget();
            Widget grid_widget = new Widget().WithAddedBehavior(new GridFormat(1, 4));
            grid_widget[0].debug_output = true;
            grid_widget.Area = new RectangleF(50, 50, 100, 75);

            grid_widget[0, 0].Behaviors.Add(new DrawText("Test text 55555555555 5"));
            grid_widget[0, 1].Behaviors.Add(new DrawText("Test text 55555555555 5"));

            grid_widget.UserResizePolicy = Widget.UserResizePolicyType.allow;
            grid_widget.AllowedResizingDirections = Directions2D.All;
            result.Add(grid_widget);

            Widget second = new Widget();
            second.Area = new RectangleF(50, 150, 100, 75);
            second.Behaviors.Add(new GridFormat(3, 3, new Widget { UserResizePolicy = Widget.UserResizePolicyType.allow }));

            result.Add(second);

            return result;
        }

        public static Widget WidgetReorderTest()
        {
            Widget result = new Widget();
            Widget box1 = new Widget();
            Widget box2 = new Widget();
            box1.Area = new RectangleF(50,50,100,100);
            box2.Area = new RectangleF(100,100,100,100);

            result.Add(box1);
            result.Add(box2);

            box1.OnClick += (s, a) => box1.SendToFront();
            box2.OnClick += (s, a) => box2.SendToFront();

            return result;
        }

        public static Widget WidgetTransitionTest()
        {
            Widget result = new Widget();
            Widget button = CommonWidgets.Button("Send to container");
            Widget test_widget = new Widget
            {
                Area = new RectangleF(10, 40, 200, 200)
            };
            test_widget.Behaviors.Add(new GridFormat(2, 2));

            button.OnClick += (s, a) =>
            {
                test_widget.Actions.Add(new ReplaceWidget(new Widget().WithAddedBehavior(new DrawText("New Widget")), InnerWidgetLocation.OutsideLeft, InnerWidgetLocation.OutsideRight));
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
            layout.VisualSettings.DrawBackground = false;

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

            Widget login_button = CommonWidgets.Button("Login");
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

            Widget username_entry = CommonWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            Widget password_entry = CommonWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            
            username_entry.Area = new RectangleF(100, 0, 150, 40);
            password_entry.Area = new RectangleF(100, 60, 150, 40);

            Widget username_label = CommonWidgets.Label("Username:");
            username_label.Area = new RectangleF(0, 0, 60, 40);

            Widget password_label = CommonWidgets.Label("Password:");
            password_label.Area= new RectangleF(0, 60, 60, 40 );

            Widget login_button = CommonWidgets.Button("Login");
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
