namespace DownUnder.UI.UI.Widgets {
    sealed class WidgetHoverTimer {
        const float _MAX_WAIT_TIME = 10_000f;
        readonly float _wait_time;
        float _current_time;
        bool _is_silenced;

        public bool IsTriggered => !_is_silenced && _current_time > _wait_time;

        public WidgetHoverTimer(float wait_time) =>
            _wait_time = wait_time;

        public void Update(float step) {
            _current_time += step;
            if (_current_time > _MAX_WAIT_TIME)
                _current_time = _MAX_WAIT_TIME; // prevent shenanigans
        }

        public void Silence() =>
            _is_silenced = true;

        public void SoftReset() =>
            _current_time = 0f;

        public void HardReset() {
            _is_silenced = false;
            _current_time = 0f;
        }
    }
}
