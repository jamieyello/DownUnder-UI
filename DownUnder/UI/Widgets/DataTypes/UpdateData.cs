using DownUnder.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets {
    [DataContract] public class UpdateData {
        public UIInputState UIInputState { get; set; }
        public GameTime GameTime { get; set; }
        /// <summary> Slightly faster version of <see cref="GameTime.GetElapsedSeconds()"/> </summary>
        public float ElapsedSeconds { get; set; }
    }
}
