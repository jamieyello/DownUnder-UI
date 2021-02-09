namespace DownUnder.UI.UI.Widgets.DataTypes {
    public readonly struct GridSettings {
        public static readonly GridSettings Default = new GridSettings(AlignmentType.auto_align);

        public enum AlignmentType {
            auto_align
        }

        public AlignmentType Alignment { get; }

        public GridSettings(AlignmentType alignment) =>
            Alignment = alignment;
    }
}