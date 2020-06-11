using DownUnder.Utility;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors.BehaviorObjects
{
    [DataContract] public class BorderSettings : ICloneable
    {
        [DataMember] public ChangingValue<float> SpaceOccupy = new ChangingValue<float>();

        public object Clone()
        {
            BorderSettings c = new BorderSettings();
            c.SpaceOccupy = (ChangingValue<float>)SpaceOccupy.Clone();
            return c;
        }

        internal void Update(float step)
        {
            SpaceOccupy.Update(step);
        }
    }
}
