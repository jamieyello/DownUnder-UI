using System;
using System.Collections.Generic;
using System.Linq;
using DownUnder.UI.UI.Widgets;
using DownUnder.UI.UI.Widgets.DataTypes;

namespace DownUnder.UI.Utilities.Extensions {
    // Things that were worth making but not worth putting in Widget
    public static class WidgetExtensions {
        /// <summary> Get this <see cref="Widget"/>'s <see cref="Widget.Children"/> ordered by depth. </summary>
        public static WidgetList GetChildrenByDepth(
            this Widget widget
        ) {
            var result = new WidgetList();
            var ordered_list = widget.GetAllChildrenWithDepth().OrderBy(t => t.Item2).ToList();
            for (var i = 0; i < ordered_list.Count; i++)
                result.Add(ordered_list[i].Item1);
            return result;
        }

        static List<Tuple<Widget, int>> GetAllChildrenWithDepth(
            this Widget w_this,
            int parent_depth = 0
        ) {
            var results = new List<Tuple<Widget, int>> {
                new Tuple<Widget, int>(w_this, parent_depth++)
            };

            foreach (var child in w_this.Children)
                results.AddRange(GetAllChildrenWithDepth(child, parent_depth));

            return results;
        }

        public static Widget GetParentUp(
            this Widget widget,
            int parent_up
        ) {
            var result = widget;
            for (var i = 0; i < parent_up; i++)
                result = result.ParentWidget
                    ?? throw new Exception($"Attempted to get the #{parent_up} parent of the given {nameof(Widget)}. Only {i} parents exist.");
            return result;
        }
    }
}
