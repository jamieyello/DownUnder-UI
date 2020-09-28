using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNameSpace
{
    class TemplateBehavior : WidgetBehavior
    {
        /// <summary>
        /// Allows the framework identify the purpose of your behavior.
        /// </summary>
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        /// <summary>
        /// Initialize your behavior's graphics here using <see cref="WidgetBehavior.Parent"/>. Called before ConnectEvents.
        /// </summary>
        protected override void Initialize()
        {
            
        }

        /// <summary>
        /// Connect all methods to the parent Widget here.
        /// </summary>
        protected override void ConnectEvents()
        {
            
        }

        /// <summary>
        /// Disconnect all methods from the parent Widget here.
        /// </summary>
        protected override void DisconnectEvents()
        {
            
        }

        /// <summary>
        /// Return the initial state of this WidgetBehavior to be used by another Widget.
        /// </summary>
        public override object Clone()
        {
            TemplateBehavior c = new TemplateBehavior();
            return c;
        }
    }
}
