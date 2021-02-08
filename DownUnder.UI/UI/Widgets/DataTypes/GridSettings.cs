namespace DownUnder.UI.UI.Widgets.DataTypes
{
    public struct GridSettings
    {
        public enum AlignmentType
        {
            auto_align
        }

        public AlignmentType Alignment;

        public static GridSettings Default => new GridSettings() { Alignment = AlignmentType.auto_align };
    }
}
