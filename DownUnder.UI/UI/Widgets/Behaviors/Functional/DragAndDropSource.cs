using System;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional {
    public class DragAndDropSource : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public ICloneable DragObject;

        protected override void Initialize()
        {

        }

        protected override void ConnectEvents() {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        protected override void DisconnectEvents() {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        private void SetDragObject(object sender, EventArgs args) {
            Parent.ParentDWindow.DraggingObject = DragObject?.Clone();
            OnSetWindowClone?.Invoke(this, EventArgs.Empty);
        }
        private void DropObject(object sender, EventArgs args) => Parent.ParentDWindow.HoveredWidgets.Primary?.HandleDrop(Parent.ParentDWindow.DraggingObject);

        public event EventHandler OnSetWindowClone;

        public override object Clone() {
            DragAndDropSource c = new DragAndDropSource();
            c.DragObject = (ICloneable)DragObject.Clone();
            return c;
        }
    }
}
