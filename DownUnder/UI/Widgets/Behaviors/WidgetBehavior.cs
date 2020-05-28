using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors {
    /// <summary> A <see cref="WidgetBehavior"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. </summary>
    public abstract class WidgetBehavior : INeedsWidgetParent, ICloneable {
        Widget _parent_backing;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;

        public bool HasParent => Parent != null;

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing != null) {
                    if (_parent_backing == value) return;
                    throw new Exception("WidgetBehaviors cannot be reused. Use Clone() to create a copy first.");
                }
                _parent_backing = value;
                ConnectToParent();
                OnConnect?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void Disconnect()
        {
            DisconnectFromParent();
            OnDisconnect?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void ConnectToParent();
        protected abstract void DisconnectFromParent();
        public abstract object Clone();

        
    }
}
