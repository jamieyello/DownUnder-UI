// More or less sloppy/unnecessary than a tuple<ControllerType, int>
namespace DownUnder.UI.Input {
    public sealed class Controller {
        public ControllerType Type { get; } = ControllerType.null_;
        public int Index { get; }

        public Controller() {
        }

        public Controller(Controller source) {
            Type = source.Type;
            Index = source.Index;
        }

        public Controller(
            ControllerType type,
            int index
        ) {
            Type = type;
            Index = index;
        }

        public bool Equals(Controller controller) =>
            Type == controller.Type && Index == controller.Index;

        public Controller Clone() =>
            new Controller(this);
    }
}