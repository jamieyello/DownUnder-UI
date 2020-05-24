//using Downunder.Utility;
//using DownUnder.UI.Widgets.DataTypes;
//using DownUnder.UI.Widgets.Interfaces;
//using DownUnder.Utility;
//using MonoGame.Extended;
//using System;
//using System.Runtime.Serialization;

//namespace DownUnder.UI.Widgets.BaseWidgets {
//    /// <summary> A <see cref="Widget"/> that gives a border to one other <see cref="Widget"/>. One <see cref="Widget"/> can be contained on each of the four sides. </summary>
//    [DataContract] public sealed class BorderedContainer : Widget {
//        private Widget _widget;
//        bool _update_area = true;
//        private BorderSize _border_spacing_backing = new BorderSize(5f);

//        [DataMember] public GenericDirections2D<ContainerBorder> Borders { get; set; } = new GenericDirections2D<ContainerBorder>(new object[] { });
        
//        public BorderSize BorderSpacing {
//            get => _border_spacing_backing; 
//            set {
//                _border_spacing_backing = value;
//                if (_widget != null) ArangeContents(_widget.Size);
//            }
//        }
        
//        public override WidgetList Children {
//            get {
//                if (Borders.HasNull) return new WidgetList();
//                WidgetList result = new WidgetList();
//                if (_widget != null) result.Add(_widget);
//                if (Borders.Up.Widget != null) result.Add(Borders.Up.Widget);
//                if (Borders.Down.Widget != null) result.Add(Borders.Down.Widget);
//                if (Borders.Left.Widget != null) result.Add(Borders.Left.Widget);
//                if (Borders.Right.Widget != null) result.Add(Borders.Right.Widget);
//                return result;
//            }
//        }

//        public Widget ContainedWidget { 
//            get => _widget; 
//            set {
//                _widget = value;
//                if (_widget != null) _widget.Parent = this;
//            } 
//        }

//        public override RectangleF Area { 
//            get => base.Area;
//            set {
//                bool _previous_update_area = _update_area;
//                _update_area = false;
//                ArangeContents(value.Size);
//                _update_area = _previous_update_area;
//                base.Area = value;
//            }
//        }

//        public BorderedContainer(Widget widget = null, IParent parent = null) : base(parent) {
//            ContainedWidget = widget;
//            SetDefaults();
//        }

//        private void SetDefaults() {
//            Borders.Up.Parent = this;
//            Borders.Down.Parent = this;
//            Borders.Left.Parent = this;
//            Borders.Right.Parent = this;
//            ChangeColorOnMouseOver = false;
//        }

//        public void ArangeContents(Point2 new_size) {
//            //Console.WriteLine("Arranging contents");
//            if (Borders.HasNull) throw new Exception();
//            bool _previous_update_area = _update_area;
//            _update_area = false;

//            RectangleF center = new RectangleF(0f, 0f, new_size.X, new_size.Y).ResizedBy(-BorderSpacing);

//            RectangleF top_area = new RectangleF();
//            if (Borders.Up.Widget != null) {
//                Borders.Up.Widget.Area = new RectangleF(0f, 0f, new_size.X, Borders.Up.Widget.Height);
//                top_area = Borders.Up.Widget.Area;
//                center = center.ResizedBy(-top_area.Height, Directions2D.U);
//            }

//            RectangleF bottom_area = new RectangleF();
//            if (Borders.Down.Widget != null) {
//                bottom_area = Borders.Down.Widget.Area;
//                bottom_area = new RectangleF(0f, 0f, new_size.X, bottom_area.Height);
//                Borders.Down.Widget.Area = bottom_area;
//                bottom_area = Borders.Down.Widget.Area;
//                Borders.Down.Widget.Y = new_size.Y - bottom_area.Height;
//                center = center.ResizedBy(-bottom_area.Height, Directions2D.D);
//            }

//            if (Borders.Left.Widget != null) {
//                Borders.Left.Widget.Area = new RectangleF(0f, top_area.Height, Borders.Left.Widget.Width, new_size.Y - top_area.Height - bottom_area.Height);
//                RectangleF left_area = Borders.Left.Widget.Area;
//                center = center.ResizedBy(-left_area.Width, Directions2D.L);
//            }

//            if (Borders.Right.Widget != null) {
//                float right_width = Borders.Right.Widget.Width;
//                Borders.Right.Widget.Area = new RectangleF(new_size.X - right_width, top_area.Height, right_width, new_size.Y - top_area.Height - bottom_area.Height);
//                RectangleF right_area = Borders.Right.Widget.Area;
//                center = center.ResizedBy(-right_area.Width, Directions2D.R);
//            }

//            if (_widget != null) _widget.Area = center;
//            _update_area = _previous_update_area;
//        }

//        internal override void SignalChildAreaChanged() {
//            if (!_update_area) return;
//            ArangeContents(Size);
//            base.SignalChildAreaChanged();
//        }

//        protected override object DerivedClone() {
//            BorderedContainer c = new BorderedContainer();
//            c.BorderSpacing = BorderSpacing;
//            if (ContainedWidget != null) c.ContainedWidget = (Widget)ContainedWidget.Clone();
//            return c;
//        }

//        protected override void HandleChildDelete(Widget widget) {
//            if (widget != _widget) throw new Exception("Attempted to remove a Widget that was not contained in this Container.");
//            Widget r_widget = _widget;
//            ContainedWidget = null;
//            r_widget.Dispose();
//        }
//    }
//}
