using Microsoft.Xna.Framework;
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
        float _spacing_backing = 10f;
        float _last_spacing = 10f;

        public PixelGrid()
        {
            DefaultPixel = new GridPixel();
            DefaultPixel.ChangingColor.SetTargetValue(InitialColor);
        }
        
        public Color InitialColor { get; set; } = Color.Red;

        public GridPixel DefaultPixel { get; set; }

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
        
        public List<GridPixel> GetAllPixels()
        {
            List<GridPixel> result = new List<GridPixel>();

            foreach (List<GridPixel> line in _pixel_grid)
            {
                result.AddRange(line);
            }

            return result;
        }
        
        public void UpdateGridDimensions()
        {
            if (Spacing != _last_spacing) _pixel_grid.Clear();

            int required_x = (int)((Spacing + Size.X) / Spacing);
            int required_y = (int)((Spacing + Size.Y) / Spacing);

            // Case empty new grid
            if (required_x == 0 || required_y == 0)
            {
                _pixel_grid.Clear();
                return;
            }

            // Case empty old grid
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

            // At this point the new size is determined to be bigger or smaller than the existing grid (but not nonexistent)

            // Case bigger X
            if (required_x > _pixel_grid.Count)
            {
                int add_x = required_x - _pixel_grid.Count;
                int start_x = _pixel_grid.Count;
                for (int i = 0; i < add_x; i++)
                {
                    List<GridPixel> new_row = new List<GridPixel>();
                    for (int y = 0; y < _pixel_grid[0].Count; y++)
                    {
                        GridPixel new_pixel = (GridPixel)DefaultPixel.Clone();
                        new_pixel.Position = PositionOfPixel(start_x + i, y);
                        new_row.Add(new_pixel);
                    }
                    _pixel_grid.Add(new_row);
                }
            }

            // Case smaller X
            if (required_x < _pixel_grid.Count)
            {
                int remove_x = _pixel_grid.Count - required_x;
                for (int i = 0; i < remove_x; i++)
                {
                    _pixel_grid.RemoveAt(_pixel_grid.Count - 1);
                }
            }

            // Case bigger Y
            if (required_y > _pixel_grid[0].Count)
            {
                int add_y = required_y - _pixel_grid[0].Count;
                int start_y = _pixel_grid[0].Count;

                // Add to all columns
                for (int x = 0; x < _pixel_grid.Count; x++)
                {
                    for (int i = 0; i < add_y; i++)
                    {
                        GridPixel new_pixel = (GridPixel)DefaultPixel.Clone();
                        new_pixel.Position = PositionOfPixel(x, start_y + i);
                        _pixel_grid[x].Add(new_pixel);
                    }
                }
            }

            // Case smaller Y
            if (required_y < _pixel_grid[0].Count)
            {
                int remove_y = _pixel_grid[0].Count - required_y;

                for (int x = 0; x < _pixel_grid.Count; x++)
                {
                    for (int y = 0; y < remove_y; y++)
                    {
                        _pixel_grid[x].RemoveAt(_pixel_grid[x].Count - 1);
                    }
                }
            }
        }

        private Point2 PositionOfPixel(int x, int y)
        {
            return new Point2
                (
                Spacing + Spacing * x,
                Spacing + Spacing * y
                );
        }
    }
}
