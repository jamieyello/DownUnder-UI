using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class DrawText : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        /// <summary> Used by inheriting Behaviors to enable/disable normal text drawing. </summary>
        public bool EnableDefaultDraw = true;

        private string _text_backing = "";
        private Point2 _text_position_backing = new Point2();
        private TextPositioningPolicy _text_positioning_backing = TextPositioningPolicy.top_left;

        public DrawText() { }
        public DrawText(string text)
        {
            Text = text;
        }

        public enum TextPositioningPolicy {
            none,
            top_left,
            center
        }

        public string Text { get => _text_backing;
            set
            {
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

        public TextPositioningPolicy TextPositioning
        {
            get => _text_positioning_backing;
            set
            {
                if (value == _text_positioning_backing) return;
                _text_positioning_backing = value;
                //SetMinimumSize(this, EventArgs.Empty);
            }
        }

        public float SideSpacing { get; set; } = 3f;
        public bool ConstrainAreaToText { get; set; } = true;
        public bool Visible { get; set; } = true;

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
            c.TextPositioning = TextPositioning;
            c.SideSpacing = SideSpacing;
            c.ConstrainAreaToText = ConstrainAreaToText;
            return c;
        }

        public override Widget EditorWidgetRepresentation()
        {
            Widget result = base.EditorWidgetRepresentation();
            ((DrawText)result.Behaviors.GetFirst<DragAndDropSource>().DragObject).Text = "New DrawText";
            return result;
        }

        private void AlignText(object sender, EventArgs args)
        {
            if (Parent == null || !Parent.IsGraphicsInitialized) return;

            if (TextPositioning == TextPositioningPolicy.top_left) TextPosition = new Point2(SideSpacing, SideSpacing);
            if (TextPositioning == TextPositioningPolicy.center)
            {
                Point2 size = Parent.WindowFont.MeasureString(Text);
                TextPosition = Parent.Size.DividedBy(2f).WithOffset(size.DividedBy(2f).Inverted());
            }
        }

        private void SetMinimumSize(object sender, EventArgs args)
        {
            if (ConstrainAreaToText && Parent != null) Parent.MinimumSize = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        private void OverrideMinimumSize(object sender, Point2SetOverrideArgs args)
        {
            if (!Parent.IsGraphicsInitialized) return;
            if (ConstrainAreaToText) args.Override = Parent.MinimumSize.Max(GetTextMinimumArea());
        }

        public Point2 GetTextMinimumArea()
        {
            if (!Parent.IsGraphicsInitialized) return new Point2(10, 10);
            RectangleF area = Parent.WindowFont.MeasureString(Text).ToPoint2().AsRectangleSize(TextPosition).ResizedBy(SideSpacing, Directions2D.All);
            return new Point2(area.Right, area.Bottom);
        }

        private void Draw(object sender, WDrawEventArgs args) {
            if (!Visible || !EnableDefaultDraw) return;
            ForceDrawText(args.DrawingArea.Position, Text);
        }

        public void ForceDrawText(Point2 origin, string text)
        {
            Parent.SpriteBatch.DrawString(Parent.WindowFont, text, origin.WithOffset(TextPosition).Floored(), Parent.Theme.GetText(Parent.WidgetRole).CurrentColor);
        }
    }
}
