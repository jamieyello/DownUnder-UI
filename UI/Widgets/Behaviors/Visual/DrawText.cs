using DownUnder.UI.Widgets.Behaviors.Functional;
using MonoGame.Extended;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    [DataContract]
    public class DrawText : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        /// <summary> Used by inheriting Behaviors to enable/disable normal text drawing. </summary>
        [DataMember] public bool EnableDefaultDraw = true;
        [DataMember] private string _text_backing = "";
        [DataMember] private Point2 _text_position_backing = new Point2();
        [DataMember] private XTextPositioningPolicy _x_position_policy = XTextPositioningPolicy.left;
        [DataMember] private YTextPositioningPolicy _y_position_policy = YTextPositioningPolicy.top;

        public DrawText() { }
        public DrawText(string text)
        {
            Text = text;
        }

        public enum XTextPositioningPolicy {
            none,
            left,
            center
        }

        public enum YTextPositioningPolicy
        {
            none,
            top,
            center
        }

        public string Text { get => _text_backing;
            set
            {
                byte[] bytes = Encoding.ASCII.GetBytes(value);
                char[] chars = Encoding.ASCII.GetChars(bytes);
                value = new string(chars);
                if (value == _text_backing) return;
                _text_backing = value;
                SetMinimumSize(this, EventArgs.Empty);
                AlignText(this, EventArgs.Empty);
            }
        }

        public Point2 TextPosition { get => _text_position_backing;
            set
            {
                if (value == _text_position_backing) return;
                _text_position_backing = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        public XTextPositioningPolicy XTextPositioning

        {
            get => _x_position_policy;
            set
            {
                if (value == _x_position_policy) return;
                _x_position_policy = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        public YTextPositioningPolicy YTextPositioning

        {
            get => _y_position_policy;
            set
            {
                if (value == _y_position_policy) return;
                _y_position_policy = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        [DataMember] public float SideSpacing { get; set; } = 3f;
        [DataMember] public bool ConstrainAreaToText { get; set; } = true;
        [DataMember] public bool Visible { get; set; } = true;

        protected override void Initialize()
        {
            if (Parent.IsGraphicsInitialized) SetMinimumSize(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnGraphicsInitialized += AlignText;
            Parent.OnGraphicsInitialized += SetMinimumSize;
            Parent.OnResize += AlignText;
            Parent.OnDraw += Draw;
            Parent.OnMinimumSizeSetPriority += OverrideMinimumSize;
        }

        protected override void DisconnectEvents() {
            Parent.OnGraphicsInitialized -= AlignText;
            Parent.OnGraphicsInitialized -= SetMinimumSize;
            Parent.OnResize -= AlignText;
            Parent.OnDraw -= Draw;
            Parent.OnMinimumSizeSetPriority -= OverrideMinimumSize;
        }

        public override object Clone() {
            DrawText c = new DrawText();
            c.Text = Text;
            c.TextPosition = TextPosition;
            c.XTextPositioning = XTextPositioning;
            c.YTextPositioning = YTextPositioning;
            c.SideSpacing = SideSpacing;
            c.ConstrainAreaToText = ConstrainAreaToText;
            return c;
        }

        public override Widget EditorWidgetRepresentation()
        {
            Widget result = base.EditorWidgetRepresentation();
            ((DrawText)result.Behaviors.Get<DragAndDropSource>().DragObject).Text = "New DrawText";
            return result;
        }

        private void AlignText(object sender, EventArgs args)
        {
            if (Parent == null || !Parent.IsGraphicsInitialized) return;
            string text = Text == "" ? "|" : Text;

            if (XTextPositioning == XTextPositioningPolicy.left) TextPosition = new Point2(SideSpacing, TextPosition.Y);
            if (XTextPositioning == XTextPositioningPolicy.center)
            {
                Point2 size = Parent.WindowFont.MeasureString(text);
                TextPosition = new Point2(Parent.Width / 2 - size.X / 2, TextPosition.Y);
            }

            if (YTextPositioning == YTextPositioningPolicy.top) TextPosition = new Point2(TextPosition.X, SideSpacing);
            if (YTextPositioning == YTextPositioningPolicy.center)
            {
                Point2 size = Parent.WindowFont.MeasureString(text);
                TextPosition = new Point2(TextPosition.X, Parent.Height / 2 - size.Y / 2);
            }
        }

        private void SetMinimumSize(object sender, EventArgs args)
        {
            if (ConstrainAreaToText && Parent != null) Parent.MinimumSize = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        private void OverrideMinimumSize(object sender, Point2SetOverrideArgs args)
        {
            //if (!Parent.IsGraphicsInitialized) return;
            //if (ConstrainAreaToText) args.Override = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        public Point2 GetTextMinimumArea()
        {
            if (!Parent.IsGraphicsInitialized) return new Point2(10, 10);
            RectangleF area = Parent.WindowFont.MeasureString(Text).ToPoint2().AsRectangleSize().ResizedBy(SideSpacing * 2, Directions2D.All);
            return new Point2(area.Right, area.Bottom);
        }

        private void Draw(object sender, WidgetDrawArgs args) {
            if (!Visible || !EnableDefaultDraw) return;
            ForceDrawText(args.DrawingArea.Position, Text);
        }

        public void ForceDrawText(Point2 origin, string text)
        {
            Parent.SpriteBatch.DrawString(Parent.WindowFont, text, origin.WithOffset(TextPosition).Floored(), Parent.VisualSettings.TextColor);
        }
    }
}
