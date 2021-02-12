using System;
using System.Runtime.Serialization;
using System.Text;
using DownUnder.UI.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.Utilities.CommonNamespace;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    [DataContract]
    public sealed class DrawText : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        /// <summary> Used by inheriting Behaviors to enable/disable normal text drawing. </summary>
        [DataMember] public bool EnableDefaultDraw = true;
        [DataMember] string _text_backing = "";
        [DataMember] Point2 _text_position_backing;
        [DataMember] XTextPositioningPolicy _x_position_policy = XTextPositioningPolicy.left;
        [DataMember] YTextPositioningPolicy _y_position_policy = YTextPositioningPolicy.top;

        public DrawText() {
        }

        public DrawText(string text) =>
            Text = text;

        public enum XTextPositioningPolicy {
            none,
            left,
            center
        }

        public enum YTextPositioningPolicy {
            none,
            top,
            center
        }

        public string Text {
            get => _text_backing;
            set {
                // what is the point of these conversions?
                var bytes = Encoding.ASCII.GetBytes(value);
                var chars = Encoding.ASCII.GetChars(bytes);

                value = new string(chars);
                if (value == _text_backing)
                    return;

                _text_backing = value;
                SetMinimumSize(this, EventArgs.Empty);
                AlignText(this, EventArgs.Empty);
            }
        }

        public Point2 TextPosition {
            get => _text_position_backing;
            set {
                if (value == _text_position_backing)
                    return;

                _text_position_backing = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        public XTextPositioningPolicy XTextPositioning {
            get => _x_position_policy;
            set {
                if (value == _x_position_policy)
                    return;

                _x_position_policy = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        public YTextPositioningPolicy YTextPositioning {
            get => _y_position_policy;
            set {
                if (value == _y_position_policy)
                    return;

                _y_position_policy = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        [DataMember] public float SideSpacing { get; set; } = 3f;
        [DataMember] public bool ConstrainAreaToText { get; set; } = true;
        [DataMember] public bool Visible { get; set; } = true;

        protected override void Initialize() {
            if (!Parent.IsGraphicsInitialized)
                return;

            SetMinimumSize(this, EventArgs.Empty);
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

        public override object Clone() =>
            new DrawText {
                Text = Text,
                TextPosition = TextPosition,
                XTextPositioning = XTextPositioning,
                YTextPositioning = YTextPositioning,
                SideSpacing = SideSpacing,
                ConstrainAreaToText = ConstrainAreaToText
            };

        public override Widget EditorWidgetRepresentation() {
            var result = base.EditorWidgetRepresentation();
            ((DrawText)result.Behaviors.Get<DragAndDropSource>().DragObject).Text = "New DrawText";
            return result;
        }

        void AlignText(object sender, EventArgs args) {
            if (Parent == null || !Parent.IsGraphicsInitialized)
                return;

            var text = Text == "" ? "|" : Text;

            if (XTextPositioning == XTextPositioningPolicy.left)
                TextPosition = new Point2(SideSpacing, TextPosition.Y);

            if (XTextPositioning == XTextPositioningPolicy.center) {
                Point2 size = Parent.WindowFont.MeasureString(text);
                TextPosition = new Point2(Parent.Width / 2 - size.X / 2, TextPosition.Y);
            }

            if (YTextPositioning == YTextPositioningPolicy.top)
                TextPosition = new Point2(TextPosition.X, SideSpacing);

            if (YTextPositioning == YTextPositioningPolicy.center) {
                Point2 size = Parent.WindowFont.MeasureString(text);
                TextPosition = new Point2(TextPosition.X, Parent.Height / 2 - size.Y / 2);
            }
        }

        void SetMinimumSize(object sender, EventArgs args) {
            if (!ConstrainAreaToText || Parent == null)
                return;

            Parent.MinimumSize = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        void OverrideMinimumSize(object sender, Point2SetOverrideArgs args) {
            //if (!Parent.IsGraphicsInitialized) return;
            //if (ConstrainAreaToText) args.Override = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        public Point2 GetTextMinimumArea() {
            if (!Parent.IsGraphicsInitialized)
                return new Point2(10, 10);

            var area =
                Parent
                .WindowFont
                .MeasureString(Text)
                .ToPoint2()
                .AsRectangleSize()
                .ResizedBy(SideSpacing * 2, Directions2D.All);

            return new Point2(area.Right, area.Bottom);
        }

        void Draw(object sender, WidgetDrawArgs args) {
            if (!Visible || !EnableDefaultDraw)
                return;

            ForceDrawText(args.DrawingArea.Position, Text);
        }

        public void ForceDrawText(Point2 origin, string text) =>
            Parent.SpriteBatch.DrawString(
                Parent.WindowFont,
                text,
                origin.WithOffset(TextPosition).Floored(),
                Parent.VisualSettings.TextColor.ShiftBrightness(Parent.IsActive ? 1f : 0.5f)
            );
    }
}
