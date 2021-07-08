using System;

namespace DownUnder.UI.Widgets.Behaviors.Functional {
    public sealed class DragAndDropSource : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public ICloneable DragObject;

        public event EventHandler OnSetWindowClone;

        protected override void Initialize() {
        }

        protected override void ConnectEvents() {
            Parent.OnDrag += SetDragObject;
            Parent.OnDrop += DropObject;
        }

        protected override void DisconnectEvents() {
            Parent.OnDrag -= SetDragObject;
            Parent.OnDrop -= DropObject;
        }

        void SetDragObject(object sender, EventArgs args) {
            Parent.ParentDWindow.DraggingObject = DragObject?.Clone();
            OnSetWindowClone?.Invoke(this, EventArgs.Empty);
        }

        void DropObject(object sender, EventArgs args) =>
            Parent.ParentDWindow.HoveredWidgets.Primary?.HandleDrop(Parent.ParentDWindow.DraggingObject);

        public override object Clone() =>
            new DragAndDropSource {
                DragObject = (ICloneable)DragObject.Clone()
            };
    }
}
