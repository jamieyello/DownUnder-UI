using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes
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
