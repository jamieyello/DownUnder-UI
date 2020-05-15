using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static DownUnder.UI.Widgets.Actions.WidgetAction;

namespace DownUnder.UI.Widgets.Actions {
    public class ActionCollection : IList<WidgetAction> {
        private readonly Widget _parent;

        public ActionCollection(Widget parent) => _parent = parent;
        private List<WidgetAction> _actions = new List<WidgetAction>();
        private List<WidgetAction> _qued_actions = new List<WidgetAction>();
        public WidgetAction this[int index] { get => ((IList<WidgetAction>)_actions)[index]; set => ((IList<WidgetAction>)_actions)[index] = value; }
        public int Count => ((IList<WidgetAction>)_actions).Count;
        public bool IsReadOnly => ((IList<WidgetAction>)_actions).IsReadOnly;

        public void UpdateQuedActions() {
            for (int q = _qued_actions.Count - 1; q >= 0; q--) {
                bool add = true;
                for (int a = 0; a < _actions.Count; a++) {
                    if (_qued_actions[q].Matches(_actions[a])) {
                        add = false;
                        break;
                    }
                }
                if (add) {
                    _qued_actions[q].Parent = _parent;
                    ((IList<WidgetAction>)_actions).Add(_qued_actions[q]);
                    _qued_actions.RemoveAt(q);
                }
            }
        }

        public void Add(WidgetAction action) {
            if (action.Policy == DuplicatePolicy.parallel) {
                action.Parent = _parent;
                ((IList<WidgetAction>)_actions).Add(action);
                return;
            }
            if (action.Policy == DuplicatePolicy.wait) {
                foreach (WidgetAction _action in _actions) {
                    if (_action.Matches(action)) _qued_actions.Add(action);
                    return;
                }
                action.Parent = _parent;
                ((IList<WidgetAction>)_actions).Add(action);
                return;
            }
            if (action.Policy == DuplicatePolicy.cancel) {
                foreach (WidgetAction _action in _actions) {
                    if (_action.Matches(action)) return;
                }
                action.Parent = _parent;
                ((IList<WidgetAction>)_actions).Add(action);
                return;
            }
            if (action.Policy == DuplicatePolicy.override_) {
                for (int i = _actions.Count - 1; i >= 0; i--) {
                    if (_actions[i].Matches(action))_actions.RemoveAt(i);
                }
                action.Parent = _parent;
                ((IList<WidgetAction>)_actions).Add(action);
                return;
            }
            throw new System.Exception($"DuplicatePolicy {action.Policy} not supported.");
        }

        public void Insert(int index, WidgetAction item)
        {
            throw new System.Exception($"Inserting {nameof(WidgetAction)}s is not supported.");
            item.Parent = _parent;
            ((IList<WidgetAction>)_actions).Insert(index, item);
        }

        public void AddRange(List<WidgetAction> items) {
            for (int i = 0; i < items.Count; i++) Add(items[i]);
        }

        public void AddRange(ActionCollection items) => AddRange(items.ToList());

        public void Clear() {
            foreach (WidgetAction action in _actions) action.DisconnectFromParent();
            ((IList<WidgetAction>)_actions).Clear();
        }

        public bool Contains(WidgetAction item) => ((IList<WidgetAction>)_actions).Contains(item);
        public void CopyTo(WidgetAction[] array, int arrayIndex) => ((IList<WidgetAction>)_actions).CopyTo(array, arrayIndex);
        public IEnumerator<WidgetAction> GetEnumerator() => ((IList<WidgetAction>)_actions).GetEnumerator();
        public int IndexOf(WidgetAction item) => ((IList<WidgetAction>)_actions).IndexOf(item);

        public bool Remove(WidgetAction item) {
            if (((IList<WidgetAction>)_actions).Remove(item)) {
                item.DisconnectFromParent();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index) {
            ((IList<WidgetAction>)_actions)[index].DisconnectFromParent();
            ((IList<WidgetAction>)_actions).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IList<WidgetAction>)_actions).GetEnumerator();
    }
}
