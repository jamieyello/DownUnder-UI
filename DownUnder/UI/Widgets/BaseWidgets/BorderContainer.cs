using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace DownUnder.UI.Widgets.BaseWidgets {
    /// <summary> A <see cref="Widget"/> that gives a border to one other <see cref="Widget"/>. One <see cref="Widget"/> can be contained on each of the four sides. </summary>
    [DataContract] public class BorderContainer : Widget {
        private Widget _widget;
        bool _update_area = true;
        private BorderSize _border_size_backing = new BorderSize(5f);

        [DataMember] public Widget TopBorderWidget { get; set; }
        [DataMember] public Widget DownBorderWidget { get; set; }
        [DataMember] public Widget LeftBorderWidget { get; set; }
        [DataMember] public Widget RightBorderWidget { get; set; }

        public BorderSize BorderSize {
            get => _border_size_backing; 
            set {
                _border_size_backing = value;
                if (_widget != null) ArangeContents(_widget.Size);
            }
        }

        public override WidgetList Children {
            get {
                WidgetList result = new WidgetList();
                if (_widget != null) result.Add(_widget);
                if (TopBorderWidget != null) result.Add(TopBorderWidget);
                if (DownBorderWidget != null) result.Add(DownBorderWidget);
                if (LeftBorderWidget != null) result.Add(LeftBorderWidget);
                if (RightBorderWidget != null) result.Add(RightBorderWidget);
                return result;
            }
        }

        public Widget ContainedWidget { 
            get => _widget; 
            set {
                _widget = value;
                if (_widget != null) _widget.Parent = this;
            } 
        }

        public override RectangleF Area { 
            get => base.Area;
            set {
                bool _previous_update_area = _update_area;
                _update_area = false;
                ArangeContents(value.Size);
                _update_area = _previous_update_area;
                base.Area = value;
            }
        }

        public BorderContainer(Widget widget = null, Widget parent = null) : base(parent) {
            ContainedWidget = widget;
            SetDefaults();
        }

        private void SetDefaults() {
            ChangeColorOnMouseOver = false;
            DesignerObjects.IsEditModeEnabled = true;
        }

        private void ArangeContents(Point2 new_size) {
            bool _previous_update_area = _update_area;
            _update_area = false;

            RectangleF center = new RectangleF(0f, 0f, new_size.X, new_size.Y).ResizedBy(-BorderSize);
            
            if (TopBorderWidget != null) {
                float widget_height = TopBorderWidget.Height;
                RectangleF widget_area = new RectangleF(
                    0f, 
                    0f, 
                    new_size.X,
                    widget_height);
                TopBorderWidget.Area = widget_area;
                center = center.ResizedBy(-widget_area.Height, Directions2D.U);
            }

            if (_widget != null) _widget.Area = center;
            _update_area = _previous_update_area;
            if (_update_area) base.SignalChildAreaChanged();
        }

        internal override void SignalChildAreaChanged() {
            if (!_update_area) return;
            ArangeContents(Size);
            base.SignalChildAreaChanged();
        }

        protected override object DerivedClone() {
            BorderContainer c = new BorderContainer();
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
