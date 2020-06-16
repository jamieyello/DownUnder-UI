using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class BehaviorCollection : IList<WidgetBehavior>
    {
        private readonly Widget _parent;
        public BehaviorCollection(Widget parent) => _parent = parent;
        private readonly List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();

        public WidgetBehavior this[int index] { get => _behaviors[index]; set => ((IList<WidgetBehavior>)_behaviors)[index] = value; }
        public bool HasBehaviorOfType(Type type) {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == type) return true;
            }

            return false;
        }
        public T GetFirst<T>() {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == typeof(T)) return (T)Convert.ChangeType(behavior, typeof(T));
            }

            return default;
        }
        public int Count => ((IList<WidgetBehavior>)_behaviors).Count;
        public bool IsReadOnly => ((IList<WidgetBehavior>)_behaviors).IsReadOnly;

        public void Add(WidgetBehavior behavior) {
            behavior.Parent = _parent;
            ((IList<WidgetBehavior>)_behaviors).Add(behavior);
        }        
        
        public void Add<T>(T behavior, out T added_behavior) {
            if (!(behavior is WidgetBehavior behavior_)) throw new Exception($"Given item is not a {nameof(WidgetBehavior)}.");
            behavior_.Parent = _parent;
            ((IList<WidgetBehavior>)_behaviors).Add(behavior_);
            added_behavior = behavior;
        }

        public void AddRange(IEnumerable<WidgetBehavior> behaviors) {
            foreach (WidgetBehavior behavior in behaviors) Add(behavior);
        }

        public void AddRange(BehaviorCollection behaviors) => AddRange(behaviors.ToList());
        
        public void Clear() {
            foreach (WidgetBehavior behavior in _behaviors) behavior.Disconnect();
            ((IList<WidgetBehavior>)_behaviors).Clear();
        }

        public bool Contains(WidgetBehavior behavior) => ((IList<WidgetBehavior>)_behaviors).Contains(behavior);
        public void CopyTo(WidgetBehavior[] array, int arrayIndex) => ((IList<WidgetBehavior>)_behaviors).CopyTo(array, arrayIndex);
        public IEnumerator<WidgetBehavior> GetEnumerator() => ((IList<WidgetBehavior>)_behaviors).GetEnumerator();
        public int IndexOf(WidgetBehavior behavior) => ((IList<WidgetBehavior>)_behaviors).IndexOf(behavior);
        
        public void Insert(int index, WidgetBehavior behavior) {
            behavior.Parent = _parent;
            ((IList<WidgetBehavior>)_behaviors).Insert(index, behavior);
        }

        public bool Remove(WidgetBehavior behavior) {
            if (((IList<WidgetBehavior>)_behaviors).Remove(behavior)) {
                behavior.Disconnect();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index) {
            ((IList<WidgetBehavior>)_behaviors)[index].Disconnect();
            ((IList<WidgetBehavior>)_behaviors).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IList<WidgetBehavior>)_behaviors).GetEnumerator();
    }
}
