using System;
using System.Collections.Generic;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.Behaviors;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    /// <summary> When enabled the parent <see cref="Widget"/> will foward certain bahaviors to this object to force the parent <see cref="Widget"/> to be editable. </summary>
    public sealed class DesignerModeSettings {
        bool _is_developer_mode_enabled_backing;
        readonly Widget _parent;

        public DesignerModeSettings(Widget parent) {
            if (parent is not { })
                throw new Exception($"{nameof(DesignerModeSettings)} parent cannot be null.");

            _parent = parent;

            _parent.OnAddChild += (s, e) => {
                if (IsEditModeEnabled)
                    _parent.LastAddedWidget.DesignerObjects.IsEditModeEnabled = true;
            };

            AcceptedDropTypes.Add(typeof(Widget));
            AcceptedDropTypes.Add(typeof(WidgetBehavior));
        }

        public float AddWidgetSpacing { get; } = 8f;

        public bool IsEditModeEnabled {
            get => _is_developer_mode_enabled_backing;
            set {
                _is_developer_mode_enabled_backing = value;

                foreach (var child in _parent)
                    child.DesignerObjects.IsEditModeEnabled = value;
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

        public bool AcceptsDrops { get; } = true;
        public List<SerializableType> AcceptedDropTypes { get; } = new List<SerializableType>();

        public void HandleDrop(object drop) {
            if (drop is Widget w_drop) {
                w_drop.Area = GetAddWidgetArea(w_drop);
                _parent.Add(w_drop);
            }

            if (drop is WidgetBehavior b_drop)
                _parent.Behaviors.TryAdd(b_drop);
        }

        /// <summary> Get the area to be set of a <see cref="Widget"/> being dropped onto this <see cref="Widget"/> at a certain position. </summary>
        /// <param name="dragging_widget">The <see cref="Widget"/> to be added to this one.</param>
        /// <param name="center">The *center* position of the new <see cref="Widget"/> to be added. Uses this.Parent.CursorPosition by default.</param>
        public RectangleF GetAddWidgetArea(
            Widget dragging_widget,
            Point2? center = null
        ) {
            center ??= _parent.CursorPosition;

            return
                dragging_widget
               .Area
               .SizeOnly()
               .WithCenter(center.Value)
               .Rounded(_parent.DesignerObjects.AddWidgetSpacing);
        }

        #endregion
    }
}
