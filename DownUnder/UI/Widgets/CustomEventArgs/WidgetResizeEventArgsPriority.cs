using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets
{
    public class ResizeEventArgsPriority : WidgetResizeEventArgs
    {
        public ResizeEventArgsPriority(RectangleF previous_area) : base(previous_area) { }

        public RectangleF? OverrideArea = null;
    }
}
