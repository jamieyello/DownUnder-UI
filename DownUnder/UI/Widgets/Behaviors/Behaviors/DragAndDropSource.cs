using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors {
    public class DragAndDropSource : WidgetBehavior {
        public ICloneable DragObject;

        protected override void ConnectToParent() {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        protected override void DisconnectFromParent() {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        private void SetDragObject(object sender, EventArgs args) {
            Parent.ParentWindow.DraggingObject = DragObject?.Clone();
            OnSetWindowClone?.Invoke(this, EventArgs.Empty);
        }
        private void DropObject(object sender, EventArgs args) => ((IAcceptsDrops)Parent.ParentWindow.HoveredWidgets.Primary)?.HandleDrop(Parent.ParentWindow.DraggingObject);

        public event EventHandler OnSetWindowClone;

        public override object Clone() {
            DragAndDropSource c = new DragAndDropSource();
            c.DragObject = (ICloneable)DragObject.Clone();
            return c;
        }
    }
}
