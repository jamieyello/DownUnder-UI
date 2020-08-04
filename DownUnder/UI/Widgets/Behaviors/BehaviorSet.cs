using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class BehaviorSet : IList<WidgetBehavior>
    {
        private readonly List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();

        public WidgetBehavior this[int index] { get => _behaviors[index]; set => _behaviors[index] = value; }

        public int Count => _behaviors.Count;

        public bool IsReadOnly => ((ICollection<WidgetBehavior>)_behaviors).IsReadOnly;

        public void Add(WidgetBehavior item)
        {
            _behaviors.Add(item);
        }

        public void Clear()
        {
            _behaviors.Clear();
        }

        public bool Contains(WidgetBehavior item)
        {
            return _behaviors.Contains(item);
        }

        public void CopyTo(WidgetBehavior[] array, int arrayIndex)
        {
            _behaviors.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WidgetBehavior> GetEnumerator()
        {
            return _behaviors.GetEnumerator();
        }

        public int IndexOf(WidgetBehavior item)
        {
            return _behaviors.IndexOf(item);
        }

        public void Insert(int index, WidgetBehavior item)
        {
            _behaviors.Insert(index, item);
        }

        public bool Remove(WidgetBehavior item)
        {
            return _behaviors.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _behaviors.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _behaviors.GetEnumerator();
        }
    }
}
