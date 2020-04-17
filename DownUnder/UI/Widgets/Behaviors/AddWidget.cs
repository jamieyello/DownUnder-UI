using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class AddWidget : WidgetBehavior
    {
        public ICloneable DragObject;

        /// <summary>
        /// All Events should be added here.
        /// </summary>
        protected override void ConnectToParent()
        {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        /// <summary>
        /// All Events added in ConnectEvents should be removed here.
        /// </summary>
        internal override void DisconnectFromParent()
        {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        private void SetDragObject(object sender, EventArgs args) => Parent.ParentWindow.DraggingObject = DragObject?.Clone();
        private void DropObject(object sender, EventArgs args) => ((IAcceptsDrops)Parent.ParentWindow.HoveredWidgets.Primary)?.HandleDrop(Parent.ParentWindow.DraggingObject);

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
