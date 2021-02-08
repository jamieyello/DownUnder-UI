using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DownUnder.UI.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.UI.Widgets.Behaviors.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using static DownUnder.UI.UI.Widgets.DataTypes.GeneralVisualSettings;

namespace DownUnder.UI.UI.Widgets.Behaviors {
    public sealed class GroupBehaviorCollection : IList<GroupBehaviorPolicy> {
        readonly List<GroupBehaviorPolicy> _policies = new List<GroupBehaviorPolicy>();

        public GroupBehaviorCollection() {
        }

        public GroupBehaviorCollection(
            IEnumerable<GroupBehaviorPolicy> policies
        ) {
            foreach (var policy in policies)
                _policies.Add((GroupBehaviorPolicy)policy.Clone());
        }

        // TODO: these instantiate on every read
        public static GroupBehaviorCollection BasicDesktopFunctions =>
            new GroupBehaviorCollection {
                new GroupBehaviorPolicy { Behavior = new ApplyInputScrolling() },
                new GroupBehaviorPolicy { Behavior = new SpawnRightClickDropDown() }
            };

        public static GroupBehaviorCollection BasicVisuals =>
            new GroupBehaviorCollection {
                new GroupBehaviorPolicy { Behavior = new DrawBackground() },
                new GroupBehaviorPolicy { Behavior = new DrawOutline() }
            };

        static GroupBehaviorCollection PlasmaOverrides { get {
            var glow = MouseGlow.SubtleGray;
            glow.ActivationPolicy = MouseGlow.MouseGlowActivationPolicy.hovered_over;

            var blue = ShadingBehavior.SubtleBlue;
            blue.BorderVisibility = 0.3f;
            blue.GradientVisibility = new Point2(0.2f, 0.2f);

            var result = new GroupBehaviorCollection {
                new GroupBehaviorPolicy { Behavior = new MouseGlow(), NecessaryVisualRole = VisualRoleType.button },
                new GroupBehaviorPolicy { Behavior = new ShadingBehavior { ShadeColor = Color.White, BorderWidth = 10, BorderVisibility = 0.05f, GradientVisibility = new Point2(0.05f, 0.03f) }, NecessaryVisualRole = VisualRoleType.text_edit_widget },
                new GroupBehaviorPolicy { Behavior = new BlurBackground(), NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy { Behavior = glow, NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy { Behavior = blue, NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy { Behavior = new Neurons(), NecessaryVisualRole = VisualRoleType.flashy_background },
            };

            return result;
        } }

        public static GroupBehaviorCollection PlasmaDesktop =>
            BasicDesktopFunctions.WithOverrides(BasicVisuals.WithOverrides(PlasmaOverrides));

        public GroupBehaviorCollection WithOverrides(
            IEnumerable<GroupBehaviorPolicy> policies
        ) {
            var result = new GroupBehaviorCollection(this);
            foreach (var new_policy in policies) {
                while (result.HasConflictWith(new_policy) != -1)
                    result._policies.RemoveAt(HasConflictWith(new_policy));
                result._policies.Add(new_policy);
            }
            return result;
        }

        public int HasConflictWith(GroupBehaviorPolicy policy) {
            for (var i = 0; i < _policies.Count; i++)
                if (_policies[i].ConflictsWith(policy))
                    return i;

            return -1;
        }

        public List<Type> GetBehaviorTypes() =>
            _policies
            .Select(t => t.Behavior.GetType())
            .ToList();

        #region IList

        public GroupBehaviorPolicy this[int index] { get => _policies[index]; set => _policies[index] = value; }
        public int Count => _policies.Count;
        public bool IsReadOnly { get; } = false;

        public void Add(GroupBehaviorPolicy item) => _policies.Add(item);
        public void Clear() => _policies.Clear();
        public bool Contains(GroupBehaviorPolicy item) => _policies.Contains(item);
        public void CopyTo(GroupBehaviorPolicy[] array, int arrayIndex) => _policies.CopyTo(array, arrayIndex);
        public IEnumerator<GroupBehaviorPolicy> GetEnumerator() => _policies.GetEnumerator();
        public int IndexOf(GroupBehaviorPolicy item) => _policies.IndexOf(item);
        public void Insert(int index, GroupBehaviorPolicy item) => _policies.Insert(index, item);
        public bool Remove(GroupBehaviorPolicy item) => _policies.Remove(item);
        public void RemoveAt(int index) => _policies.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_policies).GetEnumerator();

        #endregion
    }
}
