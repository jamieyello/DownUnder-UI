//using DownUnder.Utilities;
//using MonoGame.Extended;

//namespace DownUnder.UI.Widgets.BaseWidgets
//{
//    public class SpacedList : Layout {
//        private bool _disable_update_area = false;

//        public float ListSpacing { get; set; } = 0f;
//        /// <summary> Ther interpolation that will be used when aligning the <see cref="Widget"/>s. Set to null for none. </summary>
//        public InterpolationSettings? WidgetMovement { get; set; } = InterpolationSettings.Fast;

//        public SpacedList(Widget parent = null) : base(parent) => SetDefaults();
//        private void SetDefaults() {
//            OnListChange += (obj, sender) => SignalChildAreaChanged();
//            EmbedChildren = false;
//        }

//        public override RectangleF Area {
//            get => base.Area;
//            set {
//                base.Area = value;
//                _disable_update_area = true;
//                _widgets.AlignHorizontalWrap(area_backing.Width, false, ListSpacing, true, WidgetMovement);
//                _disable_update_area = false;
//            }
//        }

//        internal override void SignalChildAreaChanged() {
//            if (_disable_update_area) return;
//            _widgets.AlignHorizontalWrap(area_backing.Width, debug_output, ListSpacing);
//            base.SignalChildAreaChanged();
//        }
//    }
//}
