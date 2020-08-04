using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UI.Widgets
{
    // Things that were worth making but not worth putting in Widget
    public static class WidgetExtensions {
        /// <summary> Get this <see cref="Widget"/>'s <see cref="Widget.Children"/> ordered by depth. </summary>
        public static WidgetList GetChildrenByDepth(this Widget widget) {
            WidgetList result = new WidgetList();
            List<Tuple<Widget, int>> ordered_list = widget.GetAllChildrenWithDepth().OrderBy(t => t.Item2).ToList();
            for (int i = 0; i < ordered_list.Count; i++) result.Add(ordered_list[i].Item1);
            return result;
        }

        private static List<Tuple<Widget, int>> GetAllChildrenWithDepth(this Widget w_this, int parent_depth = 0) {
            List<Tuple<Widget, int>> results = new List<Tuple<Widget, int>> { new Tuple<Widget, int>(w_this, parent_depth++) };
            foreach (Widget child in w_this.Children) results.AddRange(GetAllChildrenWithDepth(child, parent_depth));
            return results;
        }

        public static Widget GetParentUp(this Widget widget, int parent_up) {
            Widget result = widget;
            for (int i = 0; i < parent_up; i++)
            {
                if (result.ParentWidget == null) throw new Exception($"Attempted to get the #{parent_up} parent of the given {nameof(Widget)}. Only {i} parents exist.");
                result = result.ParentWidget;
            }
            return result;
        }
    }
}
