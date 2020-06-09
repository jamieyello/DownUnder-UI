﻿using System;
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
        public WidgetBehavior this[int index] { get => ((IList<WidgetBehavior>)_behaviors)[index]; set => ((IList<WidgetBehavior>)_behaviors)[index] = value; }
        public T GetFirst<T>() {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == typeof(T)) return (T)Convert.ChangeType(behavior, typeof(T));
            }

            return default;
        }
        public int Count => ((IList<WidgetBehavior>)_behaviors).Count;
        public bool IsReadOnly => ((IList<WidgetBehavior>)_behaviors).IsReadOnly;

        public void Add(WidgetBehavior item) {
            item.Parent = _parent;
            ((IList<WidgetBehavior>)_behaviors).Add(item);
        }

        public void AddRange(IEnumerable<WidgetBehavior> behaviors) {
            foreach (WidgetBehavior behavior in behaviors) Add(behavior);
        }

        public void AddRange(BehaviorCollection items) => AddRange(items.ToList());
        
        public void Clear() {
            foreach (WidgetBehavior behavior in _behaviors) behavior.Disconnect();
            ((IList<WidgetBehavior>)_behaviors).Clear();
        }

        public bool Contains(WidgetBehavior item) => ((IList<WidgetBehavior>)_behaviors).Contains(item);
        public void CopyTo(WidgetBehavior[] array, int arrayIndex) => ((IList<WidgetBehavior>)_behaviors).CopyTo(array, arrayIndex);
        public IEnumerator<WidgetBehavior> GetEnumerator() => ((IList<WidgetBehavior>)_behaviors).GetEnumerator();
        public int IndexOf(WidgetBehavior item) => ((IList<WidgetBehavior>)_behaviors).IndexOf(item);
        
        public void Insert(int index, WidgetBehavior item) {
            item.Parent = _parent;
            ((IList<WidgetBehavior>)_behaviors).Insert(index, item);
        }

        public bool Remove(WidgetBehavior item) {
            if (((IList<WidgetBehavior>)_behaviors).Remove(item)) {
                item.Disconnect();
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
