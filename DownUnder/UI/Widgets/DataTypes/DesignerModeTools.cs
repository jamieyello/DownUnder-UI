using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> When enabled the parent <see cref="Widget"/> will foward certain bahaviors to this object to force the parent <see cref="Widget"/> to be editable. </summary>
    public class DesignerModeTools
        : INeedsWidgetParent, IAcceptsDrops
    {
        private bool _is_developer_mode_enabled_backing = false;
        private Widget _last_added_widget_backing;
        private Widget _parent_backing;

        private float _add_widget_spacing_backing = 8f;

        public float AddWidgetSpacing
        {
            get => _add_widget_spacing_backing;
            set
            {
                _add_widget_spacing_backing = value;
                Parent.SignalAddWidgetSpacingChange();
            }
        }

        public Widget Parent
        {
            get => _parent_backing;
            set
            {
                if (_parent_backing == value) return;
                if (_parent_backing != null) throw new Exception("DeveloperObjects cannot be reused.");
                _parent_backing = value;

                if (_parent_backing is Layout)
                {
                    AcceptsDrops = true;
                    AcceptedDropTypes.Add(typeof(Widget));
                }
            }
        }

        public bool IsEditModeEnabled
        {
            get => _is_developer_mode_enabled_backing;
            set
            {
                _is_developer_mode_enabled_backing = value;
                foreach (Widget child in Parent.Children)
                {
                    child.DesignerObjects.IsEditModeEnabled = value;
                }
            }
        }

        /// <summary> Is set when a child's <see cref="Widget.ParentWidget"/> is set./ </summary>
        public Widget LastAddedChild
        {
            get => _last_added_widget_backing;
            set
            {
                _last_added_widget_backing = value;
                value.DesignerObjects.IsEditModeEnabled = IsEditModeEnabled || value.DesignerObjects.IsEditModeEnabled;
            }
        }

        public bool AllowUserResizing { get; set; } = true;

        public DiagonalDirections2D AllowedResizingDirections { get; set; } = DiagonalDirections2D.TL_TR_BL_BR;

        public bool AllowHighlight { get; set; } = true;

        #region IAcceptsDrops Implementation

        public bool AcceptsDrops { get; private set; } = false;

        public List<Type> AcceptedDropTypes { get; private set; } = new List<Type>();

        public bool IsDropAcceptable(object drop)
        {
            return Parent.IsDropAcceptable(drop);
        }

        public void HandleDrop(object drop)
        {
            if (IsDropAcceptable(drop))
            {
                if (drop is Widget w_drop)
                {
                    if (Parent is Layout l_parent)
                    {
                        w_drop.Area = GetAddWidgetArea(w_drop);
                        l_parent.Add(w_drop);
                    }
                }
            }
        }
        
        /// <summary> Get the area to be set of a <see cref="Widget"/> being dropped onto this <see cref="Widget"/> at a certain position. </summary>
        /// <param name="dragging_widget">The <see cref="Widget"/> to be added to this one.</param>
        /// <param name="position">The *center* position of the new <see cref="Widget"/> to be added. Uses this.Parent.CursorPosition by default.</param>
        public RectangleF GetAddWidgetArea(Widget dragging_widget, Point2? position = null)
        {
            if (position == null) position = Parent.CursorPosition;
            return dragging_widget
                .Area
                .SizeOnly()
                .WithCenter(position.Value)
                .Rounded(Parent.DesignerObjects.AddWidgetSpacing);
        }

        #endregion
    }
}
