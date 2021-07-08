using DownUnder.UI.Editor.Test;
using DownUnder.UI.Widgets;

namespace DownUnder.UI.Editor.Widgets
{
    public class EditorTestWidgets
    {
        public static Widget CurrentTest() =>
            CodeGenTest();

        public static Widget CodeGenTest()
        {
            Widget result = new Widget();
            Widget button = CommonWidgets.Button("Do test", new MonoGame.Extended.RectangleF(30, 30, 0, 0));
            button.OnClick += (s, a) =>
            {
                //ProjectCodeInterface.AddEventVoid(@"C:\Users\jamie\Desktop\test\MyWidgetCode.cs", nameof(Widget.OnClick), "MyWidgetName");
            };
            result.Add(button);
            return result;
        }

        public static Widget WorkflowTest()
        {
            Widget result = new Widget();
            MyWidgetCode myWidgetCode = new MyWidgetCode();
            result.Add(myWidgetCode.Base);
            return result;
        }
    }
}
