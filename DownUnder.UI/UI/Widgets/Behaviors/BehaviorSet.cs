using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.UI.Widgets.Behaviors {
    public sealed class BehaviorSet : IList<WidgetBehavior> {
        readonly List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();

        public WidgetBehavior this[int index] { get => _behaviors[index]; set => _behaviors[index] = value; }

        public int Count => _behaviors.Count;
        public bool IsReadOnly { get; } = false;

        public void Add(WidgetBehavior item) => _behaviors.Add(item);
        public void Clear() => _behaviors.Clear();
        public bool Contains(WidgetBehavior item) => _behaviors.Contains(item);
        public void CopyTo(WidgetBehavior[] array, int arrayIndex) => _behaviors.CopyTo(array, arrayIndex);
        public int IndexOf(WidgetBehavior item) => _behaviors.IndexOf(item);
        public void Insert(int index, WidgetBehavior item) => _behaviors.Insert(index, item);
        public bool Remove(WidgetBehavior item) => _behaviors.Remove(item);
        public void RemoveAt(int index) => _behaviors.RemoveAt(index);
        public IEnumerator<WidgetBehavior> GetEnumerator() => _behaviors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _behaviors.GetEnumerator();
    }
}
