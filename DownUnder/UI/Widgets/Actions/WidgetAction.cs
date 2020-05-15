using DownUnder.UI.Widgets.Interfaces;
using DownUnder.UI.Widgets.Behaviors;
using System;

namespace DownUnder.UI.Widgets.Actions {
    /// <summary> A <see cref="WidgetAction"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. Differs from <see cref="WidgetBehavior"/> as this deletes itself on finishing execution. </summary>
    public abstract class WidgetAction : INeedsWidgetParent {
        Widget _parent_backing;
        public enum DuplicatePolicy {
            /// <summary> Override any existing matching <see cref="WidgetAction"/>. </summary>
            override_,
            /// <summary> Execute alongside any matching <see cref="WidgetAction"/>. </summary>
            parallel,
            /// <summary> Execute after any matching <see cref="WidgetAction"/> are done. </summary>
            wait,
            /// <summary> Do not execute if a matching <see cref="WidgetAction"/> is being executed. </summary>
            cancel
        }

        /// <summary> When this <see cref="WidgetAction"/> will execute if a similar action is already being executed. </summary>
        public DuplicatePolicy Policy { get; set; } = DuplicatePolicy.parallel;

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing != null) {
                    if (_parent_backing == value) return;
                    throw new Exception("WidgetBehaviors cannot be reused. Call Clone() to create a copy.");
                }
                _parent_backing = value;
                ConnectToParent();
            }
        }

        public bool HasParent => Parent != null;
        public bool IsCompleted { get; protected set; } = false;

        protected abstract void ConnectToParent();
        internal abstract void DisconnectFromParent();
        public abstract object InitialClone();
        internal abstract bool Matches(WidgetAction action);

        protected void EndAction() {
            IsCompleted = true;
            _parent_backing.Actions.Remove(this);
        }
    }
}
