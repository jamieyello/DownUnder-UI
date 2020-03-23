using DownUnder.UI.Widgets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A <see cref="WidgetBehavior"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. </summary>
    public abstract class WidgetBehavior : IWidgetChild
    {
        private Widget _parent;

        public WidgetBehavior() { }
        public WidgetBehavior(Widget parent)
        {
            ((IWidgetChild)this).Parent = parent;
        }

        bool IWidgetChild.IsInitialized => ((IWidgetChild)this).Parent != null;

        Widget IWidgetChild.Parent
        {
            get => _parent;
            set
            {
                if (((IWidgetChild)this).IsInitialized)
                {
                    if (_parent == value) return;
                    throw new Exception("WidgetBehaviors cannot be reused.");
                }
                _parent = value;
                AddEvents(value);
            }
        }

        protected abstract void AddEvents(Widget parent);
    }
}
