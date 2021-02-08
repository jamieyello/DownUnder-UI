namespace DownUnder.UI.UI.DataTypes {
    sealed class SetGetEvent<T> {
        public bool Completed { get; set; }
        public T Value { get; }

        public SetGetEvent() {
        }

        public SetGetEvent(T value) =>
            Value = value;
    }
}