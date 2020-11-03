using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    class ToggleWindowFullscreen : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        /// <summary> Called when the parent widget is set. Use for initialization logic. </summary>
        protected override void Initialize()
        {
        }

        /// <summary> All Events should be added here. </summary>
        protected override void ConnectEvents()
        {
            Parent.OnClick += Toggle;
        }

        /// <summary> All Events added in ConnectEvents should be removed here. </summary>
        protected override void DisconnectEvents()
        {
            Parent.OnClick -= Toggle;
        }

        /// <summary> Return a copy of this behavior. This is used internally by the framework. </summary>
        public override object Clone()
        {
            ToggleWindowFullscreen c = new ToggleWindowFullscreen();
            return c;
        }

        void Toggle(object sender, EventArgs args)
        {
            Parent.ParentDWindow.GraphicsManager.HardwareModeSwitch = false;
            Parent.ParentDWindow.GraphicsManager.ToggleFullScreen();
        }
    }
}