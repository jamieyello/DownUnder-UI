using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class MakeMousePointer : WidgetBehavior
    {
        public override string[] BehaviorIDs { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
        }

        protected override void Initialize()
        {
            
        }

        void Update(object sender, EventArgs args)
        {
            if (Parent.IsPrimaryHovered) Parent.ParentDWindow.UICursor = MouseCursor.Hand;
        }
    }
}
