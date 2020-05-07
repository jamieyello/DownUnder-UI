using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.DataTypes {
    public class UpdateData {
        public UIInputState UIInputState { get; set; }
        public GameTime GameTime { get; set; }
        /// <summary> Slightly faster version of <see cref="GameTime.GetElapsedSeconds()"/> </summary>
        public float ElapsedSeconds { get; set; }
    }
}
