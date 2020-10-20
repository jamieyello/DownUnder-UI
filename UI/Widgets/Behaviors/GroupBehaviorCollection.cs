using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets
{
    public class GroupBehaviorCollection : IList<GroupBehaviorPolicy>
    {
        private List<GroupBehaviorPolicy> policies = new List<GroupBehaviorPolicy>();

        public static GroupBehaviorCollection StandardPC =>
            new GroupBehaviorCollection()
            {
                //new GroupBehaviorPolicy() { Behavior = new ScrollBar() },
                new GroupBehaviorPolicy() { Behavior = new DrawBackground() },
                new GroupBehaviorPolicy() { Behavior = new DrawOutline() },
            };

        #region IList

        public GroupBehaviorPolicy this[int index] { get => ((IList<GroupBehaviorPolicy>)policies)[index]; set => ((IList<GroupBehaviorPolicy>)policies)[index] = value; }

        public int Count => ((ICollection<GroupBehaviorPolicy>)policies).Count;

        public bool IsReadOnly => ((ICollection<GroupBehaviorPolicy>)policies).IsReadOnly;

        public void Add(GroupBehaviorPolicy item)
        {
            ((ICollection<GroupBehaviorPolicy>)policies).Add(item);
        }

        public void Clear()
        {
            ((ICollection<GroupBehaviorPolicy>)policies).Clear();
        }

        public bool Contains(GroupBehaviorPolicy item)
        {
            return ((ICollection<GroupBehaviorPolicy>)policies).Contains(item);
        }

        public void CopyTo(GroupBehaviorPolicy[] array, int arrayIndex)
        {
            ((ICollection<GroupBehaviorPolicy>)policies).CopyTo(array, arrayIndex);
        }

        public IEnumerator<GroupBehaviorPolicy> GetEnumerator()
        {
            return ((IEnumerable<GroupBehaviorPolicy>)policies).GetEnumerator();
        }

        public int IndexOf(GroupBehaviorPolicy item)
        {
            return ((IList<GroupBehaviorPolicy>)policies).IndexOf(item);
        }

        public void Insert(int index, GroupBehaviorPolicy item)
        {
            ((IList<GroupBehaviorPolicy>)policies).Insert(index, item);
        }

        public bool Remove(GroupBehaviorPolicy item)
        {
            return ((ICollection<GroupBehaviorPolicy>)policies).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<GroupBehaviorPolicy>)policies).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)policies).GetEnumerator();
        }

        #endregion
    }
}
