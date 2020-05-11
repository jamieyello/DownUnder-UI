using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Widgets.Actions {
    public class ActionCollection : IList<WidgetAction> {
        private readonly Widget _parent;

        public ActionCollection(Widget parent) => _parent = parent;
        private List<WidgetAction> _actions = new List<WidgetAction>();
        public WidgetAction this[int index] { get => ((IList<WidgetAction>)_actions)[index]; set => ((IList<WidgetAction>)_actions)[index] = value; }
        public int Count => ((IList<WidgetAction>)_actions).Count;
        public bool IsReadOnly => ((IList<WidgetAction>)_actions).IsReadOnly;

        public void Add(WidgetAction item) {
            item.Parent = _parent;
            ((IList<WidgetAction>)_actions).Add(item);
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

        public void Insert(int index, WidgetAction item) {
            item.Parent = _parent;
            ((IList<WidgetAction>)_actions).Insert(index, item);
        }

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
