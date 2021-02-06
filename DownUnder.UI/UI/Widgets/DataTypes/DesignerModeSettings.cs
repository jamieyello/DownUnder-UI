using DownUnder.UI.Widgets.Behaviors;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> When enabled the parent <see cref="Widget"/> will foward certain bahaviors to this object to force the parent <see cref="Widget"/> to be editable. </summary>
    public class DesignerModeSettings {
        private bool _is_developer_mode_enabled_backing = false;
        private Widget _parent_backing;
        private float _add_widget_spacing_backing = 8f;

        public DesignerModeSettings()
        {
            AcceptsDrops = true;
            AcceptedDropTypes.Add(typeof(Widget));
            AcceptedDropTypes.Add(typeof(WidgetBehavior));
        }

        public float AddWidgetSpacing {
            get => _add_widget_spacing_backing;
            set {
                _add_widget_spacing_backing = value;
            }
        }

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing == value) return;
                if (_parent_backing != null) throw new Exception("DeveloperObjects cannot be reused.");
                _parent_backing = value;

                _parent_backing.OnAddChild += (sender, args) => {
                    if (IsEditModeEnabled) _parent_backing.LastAddedWidget.DesignerObjects.IsEditModeEnabled = true;
                };
            }
        }

        public bool IsEditModeEnabled {
            get => _is_developer_mode_enabled_backing;
            set {
                _is_developer_mode_enabled_backing = value;
                foreach (Widget child in Parent.Children)
                {
                    child.DesignerObjects.IsEditModeEnabled = value;
                }
            }
        }

        public Widget.UserResizePolicyType UserResizingPolicy { get; set; } = Widget.UserResizePolicyType.require_highlight;
        public Widget.UserResizePolicyType UserRepositionPolicy { get; set; } = Widget.UserResizePolicyType.allow;
        public Directions2D AllowedResizingDirections { get; set; } = Directions2D.UDLR;
        public bool AllowHighlight { get; set; } = true;
        public bool AllowDelete { get; set; } = true;
        public bool AllowCopy { get; set; } = true;
        public bool AllowCut { get; set; } = true;

        #region IAcceptsDrops Implementation

        public bool AcceptsDrops { get; private set; } = false;
        public List<SerializableType> AcceptedDropTypes { get; private set; } = new List<SerializableType>();

        public void HandleDrop(object drop) {
            if (drop is Widget w_drop)
            {
                w_drop.Area = GetAddWidgetArea(w_drop);
                Parent.Add(w_drop);
            }
            if (drop is WidgetBehavior b_drop)
            {
                Parent.Behaviors.TryAdd(b_drop);
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
