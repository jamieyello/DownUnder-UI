using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets {
    [DataContract] class Container : Widget {
        private Widget _widget;
        bool _update_area = true;

        [DataMember] public BorderSize BorderSize { get; set; }
        public override WidgetList Children => new WidgetList() { _widget };

        public Widget ContainedWidget { 
            get => _widget; 
            set {
                _widget = value;
                _widget.Parent = this;
            } 
        }

        public override RectangleF Area { 
            get => base.Area;
            set {
                bool _previous_update_area = _update_area;
                _update_area = false;
                SetWidgetArea(value.Size);
                _update_area = _previous_update_area;
                base.Area = value;
            }
        }

        public Container(Widget widget = null, Widget parent = null) : base(parent) {
            ContainedWidget = widget;
        }

        private void SetWidgetArea(Point2 new_size) {
            _widget.Area = new RectangleF(0f, 0f, new_size.X, new_size.Y).ResizedBy(-BorderSize);
        }

        internal override void SignalChildAreaChanged() {
            if (_update_area) base.SignalChildAreaChanged();
        }

        protected override object DerivedClone() {
            Container c = new Container();
            c.BorderSize = BorderSize;
            c.ContainedWidget = (Widget)ContainedWidget.Clone();
            return c;
        }

        protected override void HandleChildDelete(Widget widget) {
            if (widget != _widget) throw new Exception("Attempted to remove a Widget that was not contained in this Container.");
            Widget r_widget = _widget;
            ContainedWidget = null;
            r_widget.Dispose();
        }
    }
}
