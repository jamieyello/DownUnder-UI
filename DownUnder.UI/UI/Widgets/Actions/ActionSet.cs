using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.UI.Widgets.Actions {
    public sealed class ActionSet : IList<WidgetAction> {
        readonly List<WidgetAction> _actions;

        public WidgetAction this[int index] { get => _actions[index]; set => _actions[index] = value; }

        public int Count => ((ICollection<WidgetAction>)_actions).Count;
        public bool IsReadOnly => ((ICollection<WidgetAction>)_actions).IsReadOnly;

        /// <summary> Returns true if every action in this set has been completed. </summary>
        public bool IsCompleted => _actions.All(t => t.IsCompleted);

        public ActionSet() =>
            _actions = new List<WidgetAction>();

        public ActionSet(IEnumerable<WidgetAction> actions) =>
            _actions = new List<WidgetAction>(actions);

        public void Add(WidgetAction item) => _actions.Add(item);
        public void Clear() => _actions.Clear();
        public bool Contains(WidgetAction item) => _actions.Contains(item);
        public void CopyTo(WidgetAction[] array, int arrayIndex) => _actions.CopyTo(array, arrayIndex);

        public IEnumerator<WidgetAction> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _actions.GetEnumerator();

        public int IndexOf(WidgetAction item) => _actions.IndexOf(item);
        public void Insert(int index, WidgetAction item) => _actions.Insert(index, item);
        public bool Remove(WidgetAction item) => _actions.Remove(item);
        public void RemoveAt(int index) => _actions.RemoveAt(index);
    }
}
