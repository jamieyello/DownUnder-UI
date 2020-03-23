using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class BehaviorList : IList<WidgetBehavior>
    {
        private readonly Widget _parent;

        public BehaviorList(Widget parent)
        {
            _parent = parent;
        }

        private List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();

        public WidgetBehavior this[int index] { get => ((IList<WidgetBehavior>)_behaviors)[index]; set => ((IList<WidgetBehavior>)_behaviors)[index] = value; }

        public int Count => ((IList<WidgetBehavior>)_behaviors).Count;

        public bool IsReadOnly => ((IList<WidgetBehavior>)_behaviors).IsReadOnly;

        public void Add(WidgetBehavior item)
        {
            ((IList<WidgetBehavior>)_behaviors).Add(item);
        }

        public void Clear()
        {
            ((IList<WidgetBehavior>)_behaviors).Clear();
        }

        public bool Contains(WidgetBehavior item)
        {
            return ((IList<WidgetBehavior>)_behaviors).Contains(item);
        }

        public void CopyTo(WidgetBehavior[] array, int arrayIndex)
        {
            ((IList<WidgetBehavior>)_behaviors).CopyTo(array, arrayIndex);
        }

        public IEnumerator<WidgetBehavior> GetEnumerator()
        {
            return ((IList<WidgetBehavior>)_behaviors).GetEnumerator();
        }

        public int IndexOf(WidgetBehavior item)
        {
            return ((IList<WidgetBehavior>)_behaviors).IndexOf(item);
        }

        public void Insert(int index, WidgetBehavior item)
        {
            ((IList<WidgetBehavior>)_behaviors).Insert(index, item);
        }

        public bool Remove(WidgetBehavior item)
        {
            return ((IList<WidgetBehavior>)_behaviors).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<WidgetBehavior>)_behaviors).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<WidgetBehavior>)_behaviors).GetEnumerator();
        }
    }
}
