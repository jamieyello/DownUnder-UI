using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using static DownUnder.UI.Widgets.Actions.WidgetAction;

namespace DownUnder.UI.Widgets.Actions {
    [DataContract]
    public sealed class ActionManager {
        [DataMember] readonly List<WidgetAction> _actions = new List<WidgetAction>();
        readonly List<WidgetAction> _qued_actions = new List<WidgetAction>();

        public WidgetAction this[int index] { get => ((IList<WidgetAction>)_actions)[index]; set => ((IList<WidgetAction>)_actions)[index] = value; }

        public Widget Parent { get; set; }
        public int Count => _actions.Count;
        public bool IsReadOnly => false;

        public ActionManager(Widget parent) =>
            Parent = parent;

        public void UpdateQuedActions() {
            for (var q = _qued_actions.Count - 1; q >= 0; q--) {
                var qued_action = _qued_actions[q];

                var any_duplicate = _actions.Any(qued_action.IsDuplicate);
                if (any_duplicate)
                    continue;

                qued_action.Parent = Parent;
                ((IList<WidgetAction>)_actions).Add(qued_action);
                _qued_actions.RemoveAt(q);
            }
        }

        public void Add<TAction>(TAction action, out TAction added_action)
        where TAction : WidgetAction {
            Add(action);
            added_action = action;
        }

        public void Add(WidgetAction action) {
            if (action.DuplicatePolicy == DuplicatePolicyType.parallel) {
                // do nothing?
            }
            else if (action.DuplicatePolicy == DuplicatePolicyType.wait) {
                if (SeesDuplicate(action)) {
                    _qued_actions.Add(action);
                    return;
                }
            }
            else if (action.DuplicatePolicy == DuplicatePolicyType.cancel) {
                if (SeesDuplicate(action))
                    return;
            }
            else if (action.DuplicatePolicy == DuplicatePolicyType.@override) {
                for (var i = _actions.Count - 1; i >= 0; i--) {
                    if (_actions[i].IsDuplicate(action)) {
                        action.OverrodeAction = _actions[i];
                        _actions.RemoveAt(i);
                    }
                }
            } else
                throw new Exception($"DuplicatePolicy {action.DuplicatePolicy} not supported.");

            _actions.Add(action);
            action.Parent = Parent;
        }

        public void Add(IEnumerable<WidgetAction> actions) {
            foreach (var action in actions)
                Add(action);
        }

        bool SeesDuplicate(WidgetAction action) =>
            _actions.Any(action.IsDuplicate);

        public void Insert(int index, WidgetAction item) =>
            throw new Exception($"Inserting {nameof(WidgetAction)}s is not supported.");
        // item.Parent = Parent;
        // ((IList<WidgetAction>)_actions).Insert(index, item);

        public void AddRange(IEnumerable<WidgetAction> actions) {
            foreach (var action in actions)
                Add(action);
        }

        public void Clear() =>
            throw new Exception($"Cannot clear an {nameof(ActionManager)}.");
        //foreach (WidgetAction action in _actions) action.DisconnectFromParent();
        //((IList<WidgetAction>)_actions).Clear();

        public bool Contains(WidgetAction item) => ((IList<WidgetAction>)_actions).Contains(item);
        public void CopyTo(WidgetAction[] array, int arrayIndex) => ((IList<WidgetAction>)_actions).CopyTo(array, arrayIndex);
        public IEnumerator<WidgetAction> GetEnumerator() => ((IList<WidgetAction>)_actions).GetEnumerator();
        public int IndexOf(WidgetAction item) => ((IList<WidgetAction>)_actions).IndexOf(item);
        public bool Remove(WidgetAction item) => _actions.Remove(item);

        public void RemoveAt(int index) =>
            throw new Exception($"Cannot remove an item from an {nameof(ActionManager)}.");
        //((IList<WidgetAction>)_actions)[index].DisconnectFromParent();
        //((IList<WidgetAction>)_actions).RemoveAt(index);
    }
}
