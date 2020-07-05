using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class CommonBehaviorCollection : IList<WidgetBehavior>, INeedsWidgetParent
    {
        public Widget Parent { get; set; }
        public EventList<string> DisallowedIDs { get; set; } = new EventList<string>();
        
        List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();

        public WidgetBehavior this[int index] { get => ((IList<WidgetBehavior>)_behaviors)[index]; set => ((IList<WidgetBehavior>)_behaviors)[index] = value; }

        public int Count => ((ICollection<WidgetBehavior>)_behaviors).Count;

        public bool IsReadOnly => ((ICollection<WidgetBehavior>)_behaviors).IsReadOnly;

        public CommonBehaviorCollection() { }
        public CommonBehaviorCollection(Widget parent)
        {
            Parent = parent;
        }

        public void Add(WidgetBehavior item)
        {
            ((ICollection<WidgetBehavior>)_behaviors).Add(item);
        }

        public void Clear()
        {
            ((ICollection<WidgetBehavior>)_behaviors).Clear();
        }

        public bool Contains(WidgetBehavior item)
        {
            return ((ICollection<WidgetBehavior>)_behaviors).Contains(item);
        }

        public void CopyTo(WidgetBehavior[] array, int arrayIndex)
        {
            ((ICollection<WidgetBehavior>)_behaviors).CopyTo(array, arrayIndex);
        }

        public IEnumerator<WidgetBehavior> GetEnumerator()
        {
            return ((IEnumerable<WidgetBehavior>)_behaviors).GetEnumerator();
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
            return ((ICollection<WidgetBehavior>)_behaviors).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<WidgetBehavior>)_behaviors).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_behaviors).GetEnumerator();
        }
    }
}
