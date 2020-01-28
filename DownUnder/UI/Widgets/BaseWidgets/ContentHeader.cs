using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    class ContentHeader : Widget
    {
        public ContentHeader(Widget parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            PaletteUsage = BaseColorScheme.PaletteCatagory.header;
        }

        public override List<Widget> Children => new List<Widget>();

        protected override object DerivedClone()
        {
            ContentHeader c = new ContentHeader();
            return c;
        }
    }
}
