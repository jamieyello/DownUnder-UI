namespace DownUnder.UI.Widgets.DataTypes {
    public class WidgetUpdateFlags {
        public bool Delete { get; set; }
        public bool Cut { get; set; }
        public bool Copy { get; set; }

        public void Reset() {
            Delete = false;
            Cut = false;
            Copy = false;
        }
    }
}
