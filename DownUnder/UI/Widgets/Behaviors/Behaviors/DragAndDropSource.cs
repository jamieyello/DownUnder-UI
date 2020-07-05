using DownUnder.UI.Widgets.Interfaces;
using System;

namespace DownUnder.UI.Widgets.Behaviors {
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
            Parent.ParentWindow.DraggingObject = DragObject?.Clone();
            OnSetWindowClone?.Invoke(this, EventArgs.Empty);
        }
        private void DropObject(object sender, EventArgs args) => Parent.ParentWindow.HoveredWidgets.Primary?.HandleDrop(Parent.ParentWindow.DraggingObject);

        public event EventHandler OnSetWindowClone;

        public override object Clone() {
            DragAndDropSource c = new DragAndDropSource();
            c.DragObject = (ICloneable)DragObject.Clone();
            return c;
        }
    }
}
