// More or less sloppy/unnecessary than a tuple<ControllerType, int>
namespace DownUnder.Input
{
    public class Controller
    {
        public ControllerType type = ControllerType.null_;
        public int index = 0;

        public Controller()
        {
        }

        public Controller(ControllerType type_, int index_)
        {
            type = type_;
            index = index_;
        }

        public bool Equals(Controller controller)
        {
            return (type == controller.type) && (index == controller.index);
        }

        public object Clone()
        {
            Controller clone = new Controller();
            clone.type = type;
            clone.index = index;
            return clone;
        }
    }
}