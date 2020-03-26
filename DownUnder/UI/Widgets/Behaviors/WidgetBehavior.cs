using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A <see cref="WidgetBehavior"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. </summary>
    public abstract class WidgetBehavior : INeedsWidgetParent
    {
        Widget _parent_backing;

        public Widget Parent
        {
            get => _parent_backing;
            set
            {
                if (_parent_backing != null)
                {
                    if (_parent_backing == value) return;
                    throw new Exception("WidgetBehaviors cannot be reused.");
                }
                _parent_backing = value;
                ConnectEvents();
            }
        }

        public bool HasParent { get => Parent != null; }
        
        protected abstract void ConnectEvents();
        internal abstract void DisconnectEvents();
    }
}
