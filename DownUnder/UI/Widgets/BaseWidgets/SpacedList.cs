using System;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    public class SpacedList : Layout
    {
        #region Private Fields

        private bool _disable_update_area = false;

        #endregion

        public float ListSpacing { get; set; } = 30f;

        public SpacedList(Widget parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            OnListChange += SignalChildAreaChanged;
            EmbedChildren = false;
        }

        public override RectangleF Area
        {
            get => base.Area;
            set
            {
                base.Area = value;
                _disable_update_area = true;
                _widgets.AlignHorizontalWrap(area_backing.Width, false, ListSpacing);
                _disable_update_area = false;
            }
        }

        private void SignalChildAreaChanged(object sender, EventArgs args)
        {
            SignalChildAreaChanged();
        }

        internal override void SignalChildAreaChanged()
        {
            if (_disable_update_area) return;
            _widgets.AlignHorizontalWrap(area_backing.Width, debug_output, ListSpacing);
            base.SignalChildAreaChanged();
        }
    }
}
