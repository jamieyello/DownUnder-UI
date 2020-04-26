using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> When enabled the parent <see cref="Widget"/> will foward certain bahaviors to this object to force the parent <see cref="Widget"/> to be editable. </summary>
    public class DesignerModeSettings : INeedsWidgetParent, IAcceptsDrops {
        private bool _is_developer_mode_enabled_backing = false;
        private Widget _parent_backing;
        private float _add_widget_spacing_backing = 8f;

        public float AddWidgetSpacing {
            get => _add_widget_spacing_backing;
            set {
                _add_widget_spacing_backing = value;
                Parent.SignalAddWidgetSpacingChange();
            }
        }

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing == value) return;
                if (_parent_backing != null) throw new Exception("DeveloperObjects cannot be reused.");
                _parent_backing = value;

                if (_parent_backing is Layout parent) {
                    AcceptsDrops = true;
                    AcceptedDropTypes.Add(typeof(Widget));
                    parent.OnAddWidget += (sender, args) => {
                        if (IsEditModeEnabled) parent.LastAddedWidget.DesignerObjects.IsEditModeEnabled = true;
                    };
                }
            }
        }

        public bool IsEditModeEnabled {
            get => _is_developer_mode_enabled_backing;
            set {
                _is_developer_mode_enabled_backing = value;
                foreach (Widget child in Parent.Children) {
                    child.DesignerObjects.IsEditModeEnabled = value;
                }
            }
        }
        
        public Widget.UserResizePolicyType UserResizingPolicy { get; set; } = Widget.UserResizePolicyType.require_highlight;
        public Directions2D AllowedResizingDirections { get; set; } = Directions2D.UDLR;
        public bool AllowHighlight { get; set; } = true;
        public bool AllowDelete { get; set; } = true;
        public bool AllowCopy { get; set; } = true;
        public bool AllowCut { get; set; } = true;

        #region IAcceptsDrops Implementation

        public bool AcceptsDrops { get; private set; } = false;
        public List<Type> AcceptedDropTypes { get; private set; } = new List<Type>();

        public bool IsDropAcceptable(object drop) => Parent.IsDropAcceptable(drop);

        public void HandleDrop(object drop) {
            if (IsDropAcceptable(drop) && drop is Widget w_drop && Parent is Layout l_parent) {
                w_drop.Area = GetAddWidgetArea(w_drop);
                l_parent.Add(w_drop);
            }
        }
        
        /// <summary> Get the area to be set of a <see cref="Widget"/> being dropped onto this <see cref="Widget"/> at a certain position. </summary>
        /// <param name="dragging_widget">The <see cref="Widget"/> to be added to this one.</param>
        /// <param name="center">The *center* position of the new <see cref="Widget"/> to be added. Uses this.Parent.CursorPosition by default.</param>
        public RectangleF GetAddWidgetArea(Widget dragging_widget, Point2? center = null) {
            if (center == null) center = Parent.CursorPosition;
            return dragging_widget
                .Area
                .SizeOnly()
                .WithCenter(center.Value)
                .Rounded(Parent.DesignerObjects.AddWidgetSpacing);
        }

        #endregion
    }
}
