using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace DownUnder.Utilities.DrawingEffects
{
    /// <summary> A resizable grid used for various special effects. </summary>
    public class PixelGrid
    {
        List<List<GridPixel>> _pixel_grid = new List<List<GridPixel>>();
        Point2 _size_backing = new Point2();
        float _spacing_backing = 8f;

        public GridPixel DefaultPixel { get; set; } = new GridPixel();

        public float Spacing
        {
            get => _spacing_backing;
            set
            {
                if (value < 0f) throw new Exception("PixelGrid spacing cannot be negative.");
                _spacing_backing = value;
                UpdateGridDimensions();
            }
        }
        
        public Point2 Size
        {
            get => _size_backing;
            set
            {
                if (value.IsNegative()) throw new Exception("PixelGrid size cannot be negative.");
                _size_backing = value;
                UpdateGridDimensions();
            }
        }

        public void Update(float step)
        {
            foreach (List<GridPixel> line in _pixel_grid)
            {
                foreach (GridPixel pixel in line)
                {
                    pixel.Update(step);
                }
            }
        }

        public void Draw(SpriteBatch sprite_batch, Point2 offset)
        {
            foreach (List<GridPixel> line in _pixel_grid)
            {
                foreach (GridPixel pixel in line)
                {
                    pixel.Draw(sprite_batch, offset);
                }
            }
        }

        private void UpdateGridDimensions()
        {
            int required_x = (int)((Spacing + Size.X) / Spacing);
            int required_y = (int)((Spacing + Size.Y) / Spacing);

            // Case empty new grid
            if (required_x == 0 || required_y == 0)
            {
                _pixel_grid.Clear();
                return;
            }

            // Case empty old grid (done)
            if (_pixel_grid.Count == 0 || _pixel_grid[0].Count == 0)
            {
                _pixel_grid.Clear();
                Point2 pos = new Point2();
                for (int x = 0; x < required_x; x++)
                {
                    pos.X += Spacing;
                    pos.Y = 0f;
                    _pixel_grid.Add(new List<GridPixel>());
                    for (int y = 0; y < required_y; y++)
                    {
                        pos.Y += Spacing;
                        GridPixel pixel = (GridPixel)DefaultPixel.Clone();
                        pixel.Position = pos;
                        _pixel_grid[x].Add(pixel);
                    }
                }
                return;
            }

            // Case same grid
            if (required_x == _pixel_grid.Count && required_y == _pixel_grid[0].Count)
            {
                return;
            }
        }
    }
}
