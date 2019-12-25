using DownUnder.Utility.Images;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DownUnder.SpriteEngine.Field
{
    public enum CollisionType
    {
        None,
        Solid
    }

    public class GroundTile : ICloneable
    {
        protected List<ImageSetIndex> tiles = new List<ImageSetIndex>();

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public ImagePool ImagePool { get; set; }

        private Vector2 spacing_backing = new Vector2();
        public Vector2 Size
        {
            get
            {
                if (spacing_backing == Vector2.Zero) return spacing_backing;
                if (ImagePool == null) return Vector2.Zero;
                if (tiles.Count == 0) return Vector2.Zero;

                spacing_backing = ImagePool.GetDimensions(tiles[0], 0);
                return spacing_backing;
            }
            set => spacing_backing = value;
        }
        public int Spacing
        {
            get
            {
                return (int)Size.X;
            }
        }

        public CollisionType CollisionType { get; set; } = CollisionType.None;

        public GroundTile() {}
        public GroundTile(ImagePool image_pool, string tile_name)
        {
            ImagePool = image_pool;
            AddImage(tile_name);
        }
        public GroundTile(ImagePool image_pool, string[] tile_names)
        {
            ImagePool = image_pool;
            foreach (string tile_name in tile_names)
            {
                AddImage(tile_name);
            }
        }

        public bool AddImage(string tile_name)
        {
            ImageSetIndex image_set_index = ImagePool.CheckAndLoadSet(tile_name);

            if (image_set_index.index != -1)
            {
                tiles.Add(image_set_index);
                return true;
            }

            return false;
        }

        public List<Texture2D> GetTextures(int frame)
        {
            List<Texture2D> textures = new List<Texture2D>();
            foreach (ImageSetIndex tile in tiles)
            {
                textures.Add(ImagePool.GetTexture(tile, frame % ImagePool.GetSetLength(tile)));
            }

            return textures;
        }

        public object Clone()
        {
            GroundTile clone = new GroundTile();
            clone.ImagePool = ImagePool;
            
            for (int i = 0; i < tiles.Count; i++)
            {
                clone.tiles.Add((ImageSetIndex)tiles[i].Clone());
            }

            clone.spacing_backing = spacing_backing;
            clone.X = X;
            clone.Y = Y;
            clone.CollisionType = CollisionType;

            return clone;
        }
    }
}
