using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class DragAndDropClone : WidgetBehavior
    {
        public ICloneable DragObject;

        public DragAndDropClone()
        {

        }

        /// <summary>
        /// All Events should be added here.
        /// </summary>
        protected override void ConnectEvents()
        {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        /// <summary>
        /// All Events added in ConnectEvents should be removed here.
        /// </summary>
        internal override void DisconnectEvents()
        {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        private void SetDragObject(object sender, EventArgs args)
        {
            Parent.ParentWindow.DraggingObject = DragObject?.Clone();
        }

        private void DropObject(object sender, EventArgs args)
        {
            IAcceptsDrops victim = Parent.ParentWindow.HoveredWidgets.Primary;
            victim.HandleDrop(Parent.ParentWindow.DraggingObject);
        }
    }
}
