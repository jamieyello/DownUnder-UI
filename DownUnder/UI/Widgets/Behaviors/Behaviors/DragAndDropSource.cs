using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors {
    public class DragAndDropSource : WidgetBehavior {
        public ICloneable DragObject;

        protected override void ConnectToParent() {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        internal override void DisconnectFromParent() {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        private void SetDragObject(object sender, EventArgs args) => Parent.ParentWindow.DraggingObject = DragObject?.Clone();
        private void DropObject(object sender, EventArgs args) => ((IAcceptsDrops)Parent.ParentWindow.HoveredWidgets.Primary)?.HandleDrop(Parent.ParentWindow.DraggingObject);

        public override object Clone() {
            throw new NotImplementedException();
        }
    }
}
