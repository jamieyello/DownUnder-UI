using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary> A <see cref="Widget"/> that gives a border to one other <see cref="Widget"/>. One <see cref="Widget"/> can be contained on each of the four sides. </summary>
    [DataContract] public class BorderContainer : Widget {
        private Widget _widget;
        bool _update_area = true;
        private BorderSize _border_size_backing = new BorderSize(5f);

        [DataMember] public GenericDirections2D<ContainerBorder> Borders { get; set; } = new GenericDirections2D<ContainerBorder>(new object[] { });

        public BorderSize BorderSize {
            get => _border_size_backing; 
            set {
                _border_size_backing = value;
                if (_widget != null) ArangeContents(_widget.Size);
            }
        }

        public override WidgetList Children {
            get {
                if (Borders.HasNull) return new WidgetList();
                WidgetList result = new WidgetList();
                if (_widget != null) result.Add(_widget);
                if (Borders.Up.Widget != null) result.Add(Borders.Up.Widget);
                if (Borders.Down.Widget != null) result.Add(Borders.Down.Widget);
                if (Borders.Left.Widget != null) result.Add(Borders.Left.Widget);
                if (Borders.Right.Widget != null) result.Add(Borders.Right.Widget);
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
            if (Borders.HasNull) return;
            bool _previous_update_area = _update_area;
            _update_area = false;

            RectangleF center = new RectangleF(0f, 0f, new_size.X, new_size.Y).ResizedBy(-BorderSize);
            
            if (Borders.Down.Widget != null) {
                float widget_height = Borders.Down.Widget.Height;
                RectangleF widget_area = new RectangleF(
                    0f, 
                    0f, 
                    new_size.X,
                    widget_height);
                Borders.Down.Widget.Area = widget_area;
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
            if (ContainedWidget != null) c.ContainedWidget = (Widget)ContainedWidget.Clone();
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
