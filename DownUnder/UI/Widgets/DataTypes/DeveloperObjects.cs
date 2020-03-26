using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class DeveloperObjects 
        : INeedsWidgetParent, IAcceptsDrops
    {
        private bool _is_developer_mode_enabled_backing = false;
        private Widget _last_added_widget_backing;
        private Widget _parent_backing;

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

        public bool IsDeveloperModeEnabled
        {
            get => _is_developer_mode_enabled_backing;
            set
            {
                _is_developer_mode_enabled_backing = value;
                foreach (Widget child in Parent.Children)
                {
                    child.DeveloperObjects.IsDeveloperModeEnabled = value;
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
                value.DeveloperObjects.IsDeveloperModeEnabled = IsDeveloperModeEnabled || value.DeveloperObjects.IsDeveloperModeEnabled;
            }
        }

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
                        l_parent.Add(w_drop);
                    }
                }
            }
        }

        #endregion
    }
}
