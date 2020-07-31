using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static DownUnder.UI.Widgets.Actions.WidgetAction;

namespace DownUnder.UI.Widgets.Actions {
    [DataContract] public class ActionCollection 
    {
        public Widget Parent { get; set; }
        [DataMember] private readonly List<WidgetAction> _actions = new List<WidgetAction>();

        public ActionCollection(Widget parent) => Parent = parent;
        private readonly List<WidgetAction> _qued_actions = new List<WidgetAction>();
        public WidgetAction this[int index] { get => ((IList<WidgetAction>)_actions)[index]; set => ((IList<WidgetAction>)_actions)[index] = value; }
        public int Count => ((IList<WidgetAction>)_actions).Count;
        public bool IsReadOnly => ((IList<WidgetAction>)_actions).IsReadOnly;

        public void UpdateQuedActions() {
            for (int q = _qued_actions.Count - 1; q >= 0; q--) {
                bool add = true;
                for (int a = 0; a < _actions.Count; a++) {
                    if (_qued_actions[q].IsDuplicate(_actions[a])) {
                        add = false;
                        break;
                    }
                }
                if (add) {
                    _qued_actions[q].Parent = Parent;
                    ((IList<WidgetAction>)_actions).Add(_qued_actions[q]);
                    _qued_actions.RemoveAt(q);
                }
            }
        }

        public void Add<T>(T action, out T added_action)
        {
            if (!(action is WidgetAction action_)) throw new Exception($"Given item is not a { nameof(WidgetAction) }.");
            Add(action_);
            added_action = action;
        }

        public void Add(WidgetAction action) {
            if (action.DuplicatePolicy == DuplicatePolicyType.parallel) { }
            else if (action.DuplicatePolicy == DuplicatePolicyType.wait) {
                if (SeesDuplicate(action)) {
                    _qued_actions.Add(action);
                    return;
                }
            }
            else if (action.DuplicatePolicy == DuplicatePolicyType.cancel) {
                if (SeesDuplicate(action)) return;
            }
            else if (action.DuplicatePolicy == DuplicatePolicyType.@override) {
                for (int i = _actions.Count - 1; i >= 0; i--) {
                    if (_actions[i].IsDuplicate(action))
                    {
                        action.OverrodeAction = _actions[i];
                        _actions.RemoveAt(i);
                    }
                }
            } else throw new System.Exception($"DuplicatePolicy {action.DuplicatePolicy} not supported.");

            _actions.Add(action);
            action.Parent = Parent;
            
            return;
        }

        private bool SeesDuplicate(WidgetAction action)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                if (action.IsDuplicate(_actions[i])) return true;
            }

            return false;
        }

        public void Insert(int index, WidgetAction item)
        {
            throw new System.Exception($"Inserting {nameof(WidgetAction)}s is not supported.");
            item.Parent = Parent;
            ((IList<WidgetAction>)_actions).Insert(index, item);
        }

        public void AddRange(IEnumerable<WidgetAction> actions) {
            foreach (WidgetAction action in actions) Add(action);
        }

        public void Clear() {
            throw new Exception($"Cannot clear an {nameof(ActionCollection)}.");
            //foreach (WidgetAction action in _actions) action.DisconnectFromParent();
            //((IList<WidgetAction>)_actions).Clear();
        }

        public bool Contains(WidgetAction item) => ((IList<WidgetAction>)_actions).Contains(item);
        public void CopyTo(WidgetAction[] array, int arrayIndex) => ((IList<WidgetAction>)_actions).CopyTo(array, arrayIndex);
        public IEnumerator<WidgetAction> GetEnumerator() => ((IList<WidgetAction>)_actions).GetEnumerator();
        public int IndexOf(WidgetAction item) => ((IList<WidgetAction>)_actions).IndexOf(item);

        public bool Remove(WidgetAction item) {
            return _actions.Remove(item);
        }

        public void RemoveAt(int index) {
            throw new Exception($"Cannot remove an item from an {nameof(ActionCollection)}.");
            //((IList<WidgetAction>)_actions)[index].DisconnectFromParent();
            //((IList<WidgetAction>)_actions).RemoveAt(index);
        }
    }
}
