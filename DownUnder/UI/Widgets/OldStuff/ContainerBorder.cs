//using DownUnder.UI.Widgets.BaseWidgets;
//using DownUnder.UI.Widgets.Interfaces;
//using System;
//using System.Runtime.Serialization;

//namespace DownUnder.UI.Widgets.DataTypes {
//    [DataContract] public class ContainerBorder : INeedsWidgetParent {
//        BorderedContainer _parent_backing;
//        Widget _widget_backing;

//        public enum BorderWidgetOccupyPolicy {
//            occupy_space,
//            overlay
//        }

//        [DataMember] public BorderWidgetOccupyPolicy OccupyPolicy { get; set; } = BorderWidgetOccupyPolicy.overlay;
//        [DataMember] public Widget Widget { 
//            get => _widget_backing;
//            set {
//                if (value == _widget_backing) return;
//                _widget_backing = value;
//                _parent_backing.ArangeContents(_parent_backing.Size);
//            }
//        }

//        public Widget Parent { 
//            get => _parent_backing;
//            set {
//                if (_parent_backing != null && _parent_backing != value) throw new Exception("ContainerBorder cannot be reused by different parents.");
//                _parent_backing = (BorderedContainer)value;
//            }
//        }
//    }
//}
