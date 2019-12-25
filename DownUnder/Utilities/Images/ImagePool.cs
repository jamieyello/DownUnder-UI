using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// This will be an object designed to cut down on RAM usage by allowing
// multiple objects to use the same textures.

// Images sets need to be (in the content manager) in a specified folder. They need to be named
// by number.

// Use object with image_pool.CheckAndLoadSet(String set_name);
// where image_set_name would equal the asset path to the images folder.

// Then use image_pool.GetImage() to get the Texture2D.

// Inefficient Example:
//      image_pool.CheckAndLoadSet("Images/Stick Figure/Run Loop/");
//      ...
//      loaded_image_sets[image_pool.IndexOf("Images/Stick Figure/Run Loop/")][frame]

// Using the image pool efficiently means remembering the index of the image set
// you want to use. Passing a string to GetImage(string) instead of using the overloaded
// GetImage(int) means you're asking it to look up the index number every time you use it.

// Efficient Example:
//      image_pool.CheckAndLoadSet("Images/Stick Figure/Run Loop/");
//      int running_animation_index = image_pool.IndexOf("Images/Stick Figure/Run Loop/");
//      ...
//      image_pool.GetImage(running_animation_index, number);

// Note: CheckAndLoadSet will return the index of the loaded set as well.

namespace DownUnder.Utility.Images
{
    /// <summary>
    /// An object designed to cut down on RAM usage by allowing multiple objects to use the same textures.
    /// </summary>
    public class ImagePool
    {
        private ContentManager Content;
        private List<ImageSet> image_sets = new List<ImageSet>();

        public ImagePool(ContentManager content_)
        {
            Content = content_;
        }

        public List<ImageSetIndex> CheckAndLoadSets(List<String> image_sets)
        {
            List<ImageSetIndex> indexes = new List<ImageSetIndex>();
            for (int i = 0; i < image_sets.Count; i++)
            {
                indexes.Add(CheckAndLoadSet(image_sets[i]));
            }
            return indexes;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="set_name">Example: "Images/Stick Figure/Run Loop"</param>
        /// <returns></returns>
        public ImageSetIndex CheckAndLoadSet(String set_name)
        {
            int set_index = IndexOf(set_name);
            if (set_index > -1) { return new ImageSetIndex(set_index, set_name); } // Return if set is already loaded.

            return LoadSet(set_name);
        }

        private ImageSetIndex LoadSet(String set_name)
        {
            // Index of the image_sets list currently being filled
            int set_index = -1;
            ImageSet new_set = new ImageSet() { name = set_name };
            bool load_collision = false;
            Image image = new Image();

            for (int i = 0; true; i++)
            {
                if (!ResourceExists(set_name, i))
                {
                    Debug.WriteLine("ImagePool.LoadSet: Failed to load " + set_name);
                    return new ImageSetIndex(set_index, set_name);
                } // Check if file (the .xnb file) exists in the content folder.

                if (!ResourceExists(set_name + "\\Collision", i)) { load_collision = true; } else { load_collision = false; }

                if (i == 0) // Add the new ImageSet (only done once)
                {
                    set_index = image_sets.Count();
                    image_sets.Add(new_set);
                }

                Debug.WriteLine("ImagePool.LoadSet: Loading image - " + i.ToString());

                // Load images in the most efficient way.
                int image_index = image_sets[set_index].images.Count;
                image = new Image();
                image_sets[set_index].images.Add(image);
                image_sets[set_index].images[image_index].texture = Content.Load<Texture2D>(Directory.GetCurrentDirectory() + "\\Content\\" + set_name + "\\" + i.ToString());
                if (load_collision) image_sets[set_index].images[image_index].SetCollision(Content.Load<Texture2D>(Directory.GetCurrentDirectory() + "\\Content\\" + set_name + "\\" + i));
            }
        }

        private bool ResourceExists(String set_name)
        {
            set_name = Directory.GetCurrentDirectory() + "\\Content\\" + set_name;
            set_name = set_name.Replace('/', '\\');
            Debug.WriteLine(set_name);
            if (!Directory.Exists(set_name)) { return false; }
            return ResourceExists(set_name, 0);
        }

        private bool ResourceExists(String set_name, int index)
        {
            set_name = Directory.GetCurrentDirectory() + "\\Content\\" + set_name + "\\" + index + ".xnb";
            set_name = set_name.Replace('/', '\\');
            Debug.WriteLine(set_name);
            return File.Exists(set_name);
        }

        /// <summary>
        /// Removes all textures and resets the object.
        /// </summary>
        public void Clear()
        {
            image_sets.Clear();
        }

        public void Remove(String set_name)
        {
            Remove(IndexOf(set_name));
        }

        public void Remove(int set_index)
        {
            image_sets.RemoveAt(set_index);
        }

        /// <summary>
        /// Searches the image pool for the given name and returns the index.
        /// </summary>
        /// <param name="image_set"></param>
        /// <returns></returns>
        public int IndexOf(String set)
        {
            for (int i = 0; i < image_sets.Count(); i++)
            {
                if (set == image_sets[i].name) return i;
            }
            return -1;
        }

        public void Draw(ref SpriteBatch sprite_batch, ImageSetIndex set, int image_index, Orientation orientation)
        {
            sprite_batch.Draw(
                image_sets[set.index].images[image_index].texture,
                orientation.position,
                null,
                Color.White,
                0f,
                new Vector2(),
                orientation.scale.X,
                SpriteEffects.None,
                0f);
        }

        public int GetSetLength(ImageSetIndex set)
        {
            return image_sets[set.index].images.Count;
        }

        public Texture2D GetTexture(ImageSetIndex set, int image_index)
        {
            var result = image_sets[set.index].images[image_index].texture;
            return result;
        }

        public Vector2 GetDimensions(ImageSetIndex set, int image_index)
        {
            return new Vector2(
                image_sets[set.index].images[image_index].texture.Width,
                image_sets[set.index].images[image_index].texture.Width);
        }

        public int GetWidth(ImageSetIndex set, int image_index)
        {
            return image_sets[set.index].images[image_index].texture.Width;
        }

        public int GetHeight(ImageSetIndex set, int image_index)
        {
            return image_sets[set.index].images[image_index].texture.Height;
        }

        public bool GetCollision(ImageSetIndex set, int image_index, Vector2 point)
        {
            return image_sets[set.index].images[image_index].GetCollision(point);
        }
    }
}