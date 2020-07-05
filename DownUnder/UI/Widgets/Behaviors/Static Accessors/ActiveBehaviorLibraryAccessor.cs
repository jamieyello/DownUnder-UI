using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.Behaviors {
    public class ActiveBehaviorLibraryAccessor {
        public static DragAndDropSource DragAndDropSource => new DragAndDropSource();
        public static GridFormat GridFormat(int x, int y, Widget filler = null) => new GridFormat(x, y, filler);
        public static GridFormat GridFormat(Point dimensions, Widget filler = null) => new GridFormat(dimensions, filler);
        //public static AddPropertyEditChildren AddPropertyEditChildren() => new AddPropertyEditChildren();
    }
}
