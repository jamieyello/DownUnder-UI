using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.UI.UI.DataTypes {
    /// <summary> Used by a DWindow to hold textures used by contained widgets. Loads from disk on creation. </summary>
    public sealed class UIImages : IDisposable {
        /// <summary> Icon for adding a folder. </summary>
        public Texture2D AddFolder { get; }

        /// <summary> Icon for browsing a directory. </summary>
        public Texture2D BrowseFolder { get; }

        /// <summary> Icon for a folder. </summary>
        public Texture2D Folder { get; }

        /// <summary> Iconed to represent an open folder. </summary>
        public Texture2D OpenedFolder { get; }

        public UIImages(GraphicsDevice graphics) {
            using (var af_fs = new FileStream("UI/Images/add folder.png", FileMode.Open))
                AddFolder = Texture2D.FromStream(graphics, af_fs);

            using (var bf_fs = new FileStream("UI/Images/browse folder.png", FileMode.Open))
                BrowseFolder = Texture2D.FromStream(graphics, bf_fs);

            using (var f_fs = new FileStream("UI/Images/folder.png", FileMode.Open))
                Folder = Texture2D.FromStream(graphics, f_fs);

            using (var of_fs = new FileStream("UI/Images/opened folder.png", FileMode.Open))
                OpenedFolder = Texture2D.FromStream(graphics, of_fs);
        }

        ~UIImages() =>
            Dispose();

        public void Dispose() {
            AddFolder?.Dispose();
            BrowseFolder?.Dispose();
            Folder?.Dispose();
            OpenedFolder?.Dispose();
        }
    }
}