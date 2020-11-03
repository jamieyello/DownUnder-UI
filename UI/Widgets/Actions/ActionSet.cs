using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Actions
{
    public class ActionSet : IList<WidgetAction>
    {
        readonly List<WidgetAction> actions;

        public ActionSet() 
        { 
            actions = new List<WidgetAction>();
        }
        public ActionSet(IEnumerable<WidgetAction> actions)
        {
            this.actions = new List<WidgetAction>(actions);
        }

        /// <summary> Returns true if every action in this set has been completed. </summary>
        public bool IsCompleted
        {
            get
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if (!actions[i].IsCompleted) return false;
                }
                return true;
            }
        }

        public WidgetAction this[int index] { get => ((IList<WidgetAction>)actions)[index]; set => ((IList<WidgetAction>)actions)[index] = value; }

        public int Count => ((ICollection<WidgetAction>)actions).Count;

        public bool IsReadOnly => ((ICollection<WidgetAction>)actions).IsReadOnly;

        public void Add(WidgetAction item)
        {
            ((ICollection<WidgetAction>)actions).Add(item);
        }

        public void Clear()
        {
            ((ICollection<WidgetAction>)actions).Clear();
        }

        public bool Contains(WidgetAction item)
        {
            return ((ICollection<WidgetAction>)actions).Contains(item);
        }

        public void CopyTo(WidgetAction[] array, int arrayIndex)
        {
            ((ICollection<WidgetAction>)actions).CopyTo(array, arrayIndex);
        }

        public IEnumerator<WidgetAction> GetEnumerator()
        {
            return ((IEnumerable<WidgetAction>)actions).GetEnumerator();
        }

        public int IndexOf(WidgetAction item)
        {
            return ((IList<WidgetAction>)actions).IndexOf(item);
        }

        public void Insert(int index, WidgetAction item)
        {
            ((IList<WidgetAction>)actions).Insert(index, item);
        }

        public bool Remove(WidgetAction item)
        {
            return ((ICollection<WidgetAction>)actions).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<WidgetAction>)actions).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)actions).GetEnumerator();
        }
    }
}
