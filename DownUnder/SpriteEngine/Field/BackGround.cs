using DownUnder.Utility;
using DownUnder.Utility.Images;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.SpriteEngine.Field
{
    public class BackGround
    {
        /// <summary>
        /// The spacing of this background. Calculated using the first BG tile of the array.
        /// </summary>
        int Spacing
        {
            get
            {
                if (tiles.Count == 0) return 0;
                if (tiles[0].Count == 0) return 0;
                return tiles[0][0].Spacing;
            }
        }

        readonly List<List<GroundTile>> tiles = new List<List<GroundTile>>();

        public float CurrentFrame { get; set; } = 0;
        public float AnimationSpeed { get; set; } = 0f;

        public BackGround(ImagePool image_pool, string default_tile, int width, int height)
        {
            tiles = SolidGroundTileArray(image_pool, default_tile, width, height);
        }

        static List<List<GroundTile>> SolidGroundTileArray(ImagePool image_pool, string default_tile, int width, int height)
        {
            List<List<GroundTile>> result = new List<List<GroundTile>>();
            for (int x = 0; x < width; x++)
            {
                result.Add(new List<GroundTile>());
                for (int y = 0; y < height; y++)
                {
                    result[x].Add(new GroundTile(image_pool, default_tile));
                }
            }

            return result;
        }

        public void Draw(SpriteBatch sprite_batch, Orientation orientation)
        {
            int spacing = Spacing;

            for (int x = 0; x < tiles.Count; x++)
            {
                for (int y = 0; y < tiles[0].Count; y++)
                {
                    foreach (Texture2D tile in tiles[x][y].GetTextures((int)CurrentFrame))
                    {
                        sprite_batch.Draw(tile, orientation.position + new Vector2(x * spacing, y * spacing), Color.White);
                    }
                }
            }
        }

        public void Update(float step)
        {
            CurrentFrame += AnimationSpeed * step;
        }
    }
}
