using DownUnder.UI.Widgets.DataTypes;

namespace DownUnder.UI.Widgets {
    public class WidgetPostUpdateFlags
    {
        bool _send_to_back;
        bool _send_to_front;
        
        internal bool _updated { get; set; }
        public bool Delete { get; set; }
        public bool Cut { get; set; }
        public bool Copy { get; set; }
        public bool SendToBack 
        { 
            get => _send_to_back;
            set
            {
                _send_to_back = value;
                if (value) _send_to_front = false;
            } 
        }
        public bool SendToFront
        {
            get => _send_to_front;
            set
            {
                _send_to_front = value;
                if (value) _send_to_back = false;
            }
        }

        public void Reset() {
            _updated = false;
            Delete = false;
            Cut = false;
            Copy = false;
            _send_to_back = false;
            _send_to_front = false;
        }

        internal static void SetUpdatedFlagsToFalse(WidgetList widgets)
        {
            for (int i = 0; i < widgets.Count; i++) widgets[i]._post_update_flags._updated = false;
        }

        internal static bool UpdatedFlagsAreTrue(WidgetList widgets)
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                if (!widgets[i]._post_update_flags._updated) return false;
            }
            return true;
        }
    }
}
