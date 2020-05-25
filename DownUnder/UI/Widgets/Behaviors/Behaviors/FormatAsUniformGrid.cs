using DownUnder.UI.DataTypes;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors
{
    public class FormatAsUniformGrid : WidgetBehavior
    {
        private int width;
        private int height;

        public FormatAsUniformGrid(int width, int height, Widget filler = null)
        {
            this.width = width;
            this.height = height;
        }

        protected override void ConnectToParent()
        {
            throw new NotImplementedException();
        }

        internal override void DisconnectFromParent()
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
