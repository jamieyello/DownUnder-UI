using System;
using System.Runtime.Serialization;
using DownUnder.UI.Widgets.Behaviors;

namespace DownUnder.UI.Widgets.Actions {
    /// <summary> Acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. Differs from <see cref="WidgetBehavior"/> as this removes itself on finishing execution. </summary>
    [DataContract]
    public abstract class WidgetAction : ICloneable {
        Widget _parent_backing;

        public enum DuplicatePolicyType {
            /// <summary> Override any existing duplicate <see cref="WidgetAction"/>. </summary>
            @override,
            /// <summary> Execute alongside any duplicate <see cref="WidgetAction"/>. </summary>
            parallel,
            /// <summary> Execute after any duplicate <see cref="WidgetAction"/> is done. </summary>
            wait,
            /// <summary> Do not execute if a duplicate <see cref="WidgetAction"/> is being executed. </summary>
            cancel
        }

        public enum DuplicateDefinitionType {
            /// <summary> The most strict standard for catching duplicate actions. Defines a duplicate where the existing <see cref="WidgetAction"/> does a similar task. </summary>
            interferes_with,
            /// <summary> Defines a duplicate where the existing <see cref="WidgetAction"/> is attempting to reach the same end result. </summary>
            matches_result
        }

        /// <summary> How this <see cref="WidgetAction"/> will execute if a duplicate action is already being executed. </summary>
        [DataMember] public DuplicatePolicyType DuplicatePolicy { get; set; } = DuplicatePolicyType.wait;

        /// <summary> How this <see cref="WidgetAction"/> will determine if an existing <see cref="WidgetAction"/> is a duplicate. </summary>
        [DataMember] public DuplicateDefinitionType DuplicateDefinition { get; set; } = DuplicateDefinitionType.interferes_with;

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing is { }) {
                    if (_parent_backing == value)
                        return;

                    throw new Exception($"{nameof(WidgetAction)}s cannot be reused. Call {nameof(InitialClone)} to create a copy.");
                }
                _parent_backing = value;
                Initialize();
                ConnectEvents();
            }
        }

        /// <summary> The action that this <see cref="WidgetAction"/> overrode. (if any) </summary>
        public WidgetAction OverrodeAction { get; internal set; }

        public bool HasParent => Parent is { };
        public bool IsCompleted { get; private set; }

        public bool IsDuplicate(WidgetAction action) {
            if (DuplicateDefinition == DuplicateDefinitionType.interferes_with)
                return InterferesWith(action);

            if (DuplicateDefinition == DuplicateDefinitionType.matches_result)
                return Matches(action);

            throw new Exception();
        }

        protected abstract void Initialize();
        protected abstract void ConnectEvents();
        protected abstract void DisconnectEvents();
        protected abstract bool InterferesWith(WidgetAction action);
        protected abstract bool Matches(WidgetAction action);

        object ICloneable.Clone() =>
            InitialClone();

        public virtual object InitialClone() {
            var type = GetType();

            var maybe_clone = (WidgetAction)Activator.CreateInstance(type);
            if (!(maybe_clone is { } clone))
                throw new InvalidOperationException($"Failed to create an instance of type '{type.Name}'.");

            clone.DuplicatePolicy = DuplicatePolicy;
            clone.DuplicateDefinition = DuplicateDefinition;
            return clone;
        }

        protected void EndAction() {
            DisconnectEvents();
            _parent_backing.Actions.Remove(this);
            IsCompleted = true;
        }
    }
}
