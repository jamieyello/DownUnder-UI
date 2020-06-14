using Downunder.Utility;
using DownUnder.UI.Widgets.Behaviors.Behaviors.BehaviorObjects;
using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract] public class BorderFormat : WidgetBehavior
    {
        public const string CENTER_TAG = "C";
        public const string TOP_TAG = "T";
        public const string BOTTOM_TAG = "B";
        public const string LEFT_TAG = "L";
        public const string RIGHT_TAG = "R";

        private Widget _center_backing;
        private Widget _top_backing;
        private Widget _bottom_backing;
        private Widget _left_backing;
        private Widget _right_backing;

        [DataMember] public GenericDirections2D<BorderSettings> BorderSettings { get; private set; } = new GenericDirections2D<BorderSettings>();
        [DataMember] public BorderSize Spacing { get; set; } = new BorderSize(5f);

        public Widget Center {
            get => _center_backing;
            set {
                if (_center_backing == value) return;
                if (_center_backing != null) {
                    _center_backing?.BehaviorTags.Remove(typeof(BorderFormat));
                    Parent.Remove(_center_backing);
                }
                _center_backing = value;
                if (value == null) return;
                value.BehaviorTags.Add(typeof(BorderFormat), CENTER_TAG);
                Parent.Add(value);
            }
        }

        public Widget TopBorder;
        public Widget BottomBorder;
        public Widget LeftBorder;
        public Widget RightBorder;

        public override object Clone() {
            BorderFormat c = new BorderFormat();
            c.BorderSettings = (GenericDirections2D<BorderSettings>)BorderSettings.Clone();
            c.Spacing = Spacing;
            return c;
        }

        protected override void ConnectToParent() {
            Parent.OnResize += Align;
            Parent.OnUpdate += Update;
            Align(this, EventArgs.Empty);
        }

        protected override void DisconnectFromParent() {
            Parent.OnResize -= Align;
            Parent.OnUpdate -= Update;
        }

        public void Update(object sender, EventArgs args) {
            BorderSettings.Up.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Down.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Left.Update(Parent.UpdateData.ElapsedSeconds);
            BorderSettings.Right.Update(Parent.UpdateData.ElapsedSeconds);
        }

        private void Align(object sender, EventArgs args)
        {
            BorderSize center_border = new BorderSize();

            center_border.Top = 
        }
    }
}
