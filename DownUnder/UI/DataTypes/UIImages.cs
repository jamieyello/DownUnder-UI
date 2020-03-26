using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace DownUnder.UI.DataTypes
{
    /// <summary> Used by a DWindow to hold textures used by contained widgets. Loads from disk on creation. </summary>
    public class UIImages
    {
        /// <summary> Icon for adding a folder. </summary>
        public Texture2D AddFolder { get; }

        /// <summary> Icon for browsing a directory. </summary>
        public Texture2D BrowseFolder { get; }

        /// <summary> Icon for a folder. </summary>
        public Texture2D Folder { get; }

        /// <summary> Iconed to represent an open folder. </summary>
        public Texture2D OpenedFolder { get; }
        
        public UIImages(GraphicsDevice graphics)
        {
            FileStream af_fs = new FileStream("UI/Images/add folder.png", FileMode.Open);
            AddFolder = Texture2D.FromStream(graphics, af_fs);
            af_fs.Dispose();

            FileStream bf_fs = new FileStream("UI/Images/browse folder.png", FileMode.Open);
            AddFolder = Texture2D.FromStream(graphics, bf_fs);
            bf_fs.Dispose();

            FileStream f_fs = new FileStream("UI/Images/folder.png", FileMode.Open);
            AddFolder = Texture2D.FromStream(graphics, f_fs);
            f_fs.Dispose();

            FileStream of_fs = new FileStream("UI/Images/opened folder.png", FileMode.Open);
            AddFolder = Texture2D.FromStream(graphics, of_fs);
            of_fs.Dispose();
        }

        public void Dispose()
        {
            AddFolder?.Dispose();
            BrowseFolder?.Dispose();
            Folder?.Dispose();
            OpenedFolder?.Dispose();
        }

        ~UIImages()
        {
            Dispose();
        }
    }
}
