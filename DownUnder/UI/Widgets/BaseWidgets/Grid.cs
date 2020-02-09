using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// Add IsFixed size to Area.Set
// throw error if multiple widgets are added that have conficting fixed sizes

namespace DownUnder.UI.Widgets.BaseWidgets
{
    /// <summary> A grid of widgets. Cells are empty Layouts by default. </summary>
    public class Grid : Widget
    {
        #region Fields

        /// <summary> A jagged array of all the contained widgets. (Widgets[x][y]) </summary>
        private List<List<Widget>> widgets = new List<List<Widget>>();

        /// <summary> A list of dividers in a tuple with their y index. </summary>
        private List<Tuple<Widget, int>> dividers = new List<Tuple<Widget, int>>();

        /// <summary> This is broken for a possibly obvious reason. It might not matter. </summary>
        private const int _RESIZING_ACCURACY = 1;

        #endregion Fields

        #region Public Properties

        /// <summary> The number of widgets tall and wide this grid consists of. </summary>
        public Point Dimensions
        {
            get
            {
                if (widgets.Count == 0)
                {
                    return new Point();
                }

                return new Point(widgets.Count, widgets[0].Count);
            }
        }

        #endregion Public Properties

        #region Constructors

        public Grid(IWidgetParent parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        public Grid(IWidgetParent parent, int x_length, int y_length, Widget filler = null)
            : base(parent)
        {
            SetDefaults();

            if (filler == null)
            {
                filler = DefaultCell();
            }

            filler.Parent = this;

            CreateWidgetGrid(x_length, y_length, filler);
        }

        private void SetDefaults()
        {
            Size = new Point2(100, 100);
            DrawBackground = false;
        }

        #endregion Constructors

        #region Private Methods

        private void ExpandAllWidgets(Point2 modifier)
        {
            RectangleF new_area;
            for (int x = 0; x < widgets.Count; x++)
            {
                for (int y = 0; y < widgets[0].Count; y++)
                {
                    new_area = widgets[x][y].Area;
                    new_area.Width *= modifier.X;
                    new_area.Height *= modifier.Y;
                    widgets[x][y].Area = new_area;
                }
            }

            foreach (var divider in dividers)
            {
                divider.Item1.Height *= modifier.Y;
            }
        }

        protected void CreateWidgetGrid(int x_length, int y_length, Widget filler)
        {
            for (int x = 0; x < x_length; x++)
            {
                widgets.Add(new List<Widget>());
                for (int y = 0; y < y_length; y++)
                {
                    object clone = (Widget)filler.Clone(this);
                    widgets[x].Add((Widget)clone);
                }
            }
            UpdateArea(true);
        }

        /// <summary> Add a divider to the given row. </summary>
        public void InsertDivider(Widget divider, int y)
        {
            divider.Parent = this;
            divider.Width = Width;
            dividers.Add(new Tuple<Widget, int>(divider, y));

            UpdateArea(true);
        }

        /// <summary> Find and remove the given divider. </summary>
        public bool RemoveDivider(Widget divider)
        {
            for (int i = 0; i < dividers.Count; i++)
            {
                if (dividers[i].Item1 == divider)
                {
                    dividers.RemoveAt(i);
                    UpdateArea(true);
                    return true;
                }
            }

            return false;
        }

        // unfinished
        private void InsertRow(List<Widget> widgets, int y)
        {
            if (widgets.Count != Dimensions.X)
            {
                throw new Exception("Given list of widgets' length doesn't match the X dimension of this grid.");
            }

            UpdateArea(true);
        }

        // unfinished
        private void InsertColumn(List<Widget> widgets, int x)
        {
            if (widgets.Count != Dimensions.Y)
            {
                throw new Exception("Given list of widgets' length doesn't match the Y dimension of this grid.");
            }

            UpdateArea(true);
        }

        private void AlignWidgets()
        {
            if (Dimensions.X == 0 || Dimensions.Y == 0) return;
            AutoSizeAllWidgets();
            SpaceAllCells();
        }

        /// <summary> This will find the longest/tallest widget in each row/collumn and make every other element match. </summary>
        private void AutoSizeAllWidgets()
        {
            for (int x = 0; x < widgets.Count; x++)
            {
                AutoSizeCollumn(x);
            }

            for (int y = 0; y < widgets[0].Count; y++)
            {
                AutoSizeRow(y);
            }
        }

        private void AutoSizeCollumn(int collumn)
        {
            float x_width = 0;
            for (int y = 0; y < widgets[0].Count; y++)
            {
                x_width = MathHelper.Max(x_width, widgets[collumn][y].Width);
            }

            for (int y = 0; y < widgets[0].Count; y++)
            {
                if (widgets[collumn][y].IsFixedWidth)
                {
                    x_width = widgets[collumn][y].Width;
                    break;
                }
            }

            for (int y = 0; y < widgets[0].Count; y++)
            {
                widgets[collumn][y].Width = x_width;
            }
        }

        private void AutoSizeRow(int row)
        {
            float max_y = 0;
            for (int x = 0; x < widgets.Count; x++)
            {
                max_y = MathHelper.Max(max_y, widgets[x][row].Height);
            }

            for (int x = 0; x < widgets.Count; x++)
            {
                if (widgets[x][row].IsFixedHeight)
                {
                    max_y = widgets[x][row].Height;
                    break;
                }
            }

            for (int x = 0; x < widgets.Count; x++)
            {
                widgets[x][row].Height = max_y;
            }
        }
        
        private void SpaceAllCells()
        {
            if (widgets.Count == 0 || widgets[0].Count == 0) return;

            Point2 position = new Point2();

            for (int x = 0; x < widgets.Count; x++)
            {
                position.Y = 0;
                for (int y = 0; y < widgets[0].Count; y++)
                {
                    foreach (var divider in dividers)
                    {
                        if (y == divider.Item2) position.Y += divider.Item1.Height;
                    }

                    widgets[x][y].Position = position;
                    position.Y += widgets[x][y].Height;
                }
                position.X += widgets[x][0].Width;
            }

            foreach (var divider in dividers)
            {
                // get offset of divider existing in the same row as other dividers
                float same_row_divider_offset = 0f;
                foreach (Widget divider_at in DividersAt(divider.Item2))
                {
                    if (divider_at == divider.Item1) break;
                    else same_row_divider_offset += divider_at.Height;
                }
                
                // if the divider's index is 0 set its position to 0,0
                if (divider.Item2 == 0)
                {
                    divider.Item1.Position = new Point2(0, same_row_divider_offset);
                }
                // set the height to under the previous row
                else
                {
                    divider.Item1.Position =
                        widgets[0][divider.Item2 - 1].Position.AddPoint2(
                        new Point2(0, widgets[0][divider.Item2 - 1].Height + same_row_divider_offset));
                }
            }
        }

        /// <summary> Returns a list of dividers that exist on the given row. </summary>
        public List<Widget> DividersAt(int y)
        {
            List<Widget> result = new List<Widget>();

            foreach (var divider in dividers)
            {
                if (divider.Item2 == y) result.Add(divider.Item1);
            }

            return result;
        }

        /// <summary> The default cell is a layout </summary>
        protected Layout DefaultCell()
        {
            // Create cell
            Layout default_widget = new Layout(this);
            default_widget.SnappingPolicy = DiagonalDirections2D.TopRight_BottomLeft_TopLeft_BottomRight;
            default_widget.OutlineSides = Directions2D.DownRight;
            default_widget.FitToContentArea = true;
            return default_widget;
        }

        #endregion Private Methods

        #region Public Methods

        public Widget GetCell(int x, int y)
        {
            return widgets[x][y];
        }

        public Point IndexOf(Widget widget)
        {
            if (widgets.Count == 0) return new Point(-1, -1);

            for (int x = 0; x < widgets.Count; x++)
            {
                for (int y = 0; y < widgets[0].Count; y++)
                {
                    if (widgets[x][y] == widget) return new Point(x, y);
                }
            }

            return new Point(-1, -1);
        }
        
        public void SetCell(int x, int y, Widget widget, bool update_parent = false)
        {
            widget.Parent = this;

            widget.Area = widgets[x][y].Area;
            widgets[x][y] = widget;
            UpdateArea(update_parent);
        }

        public void AddToCell(int x, int y, Widget widget, bool update_parent = false)
        {
            if (widget.IsOwned)
            {
                if (widget.Parent != this)
                {
                    throw new Exception($"(Name = {Name}, widget.Name {widget.Name}) Cannot use widget owned by something else.");
                }
            }

            if (!(widgets[x][y] is Layout))
            {
                throw new Exception($"Cannot add widget to cell[{x}][{y}], because cell[{x}][{y}] is not a Layout. widgets[{x}][{y}].GetType() = {widgets[x][y].GetType()}");
            } 
            ((Layout)widgets[x][y]).AddWidget(widget);
            UpdateArea(update_parent);
        }

        #endregion Public Methods

        #region Overrides

        /// <summary> Area of this widget. (Position relative to parent widget, if any) </summary>
        public override RectangleF Area
        {
            get
            {
                return GetContentArea();
            }
            set
            {
                RectangleF original_area = Area;
                base.Area = value;
                RectangleF new_area = area_backing;

                if (debug_output)
                {
                    Console.WriteLine($"Setting area {original_area} to {new_area}");
                    Console.WriteLine($"Current fixed area {FixedContentSizeTotal()}");
                }
                
                // Update dividers width
                foreach (var divider in dividers)
                {
                    divider.Item1.Width = new_area.Width;
                }

                Point2 fixed_size = FixedContentSizeTotal();
                Point2 expansion_size = new Point2(
                    (new_area.Width - fixed_size.X) / (original_area.Width - fixed_size.X),
                    (new_area.Height - fixed_size.Y) / (original_area.Height - fixed_size.Y))
                    .Max(new Point2(0.001f, 0.001f));

                ExpandAllWidgets(expansion_size);

                UpdateArea(false);
            }
        }

        /// <summary> The total height/width of contained widgets that won't resize. </summary>
        private Point2 FixedContentSizeTotal()
        {
            Point2 fixed_size = new Point2();

            for (int i = 0; i < Dimensions.X; i++)
            {
                float fixed_width = FixedWidthOfColumn(i);
                if (fixed_width != -1f)
                {
                    fixed_size.X += fixed_width;
                }
            }

            for (int i = 0; i < Dimensions.Y; i++)
            {
                float fixed_height = FixedHeightOfRow(i);
                if (fixed_height != -1f)
                {
                    fixed_size.Y += fixed_height;
                }
            }

            foreach (var divider in dividers)
            {
                if (divider.Item1.IsFixedHeight) fixed_size.Y += divider.Item1.Height;
            }

            return fixed_size;
        }

        float FixedHeightOfRow(int row)
        {
            for (int i = 0; i < Dimensions.X; i++)
            {
                if (widgets[i][row].IsFixedHeight) return widgets[i][row].Height;
            }

            return -1f;
        }

        float FixedWidthOfColumn(int column)
        {
            for (int i = 0; i < Dimensions.Y; i++)
            {
                if (widgets[column][i].IsFixedWidth) return widgets[column][i].Width;
            }

            return -1f;
        }

        private RectangleF GetContentArea()
        {
            if (Dimensions.X == 0 || Dimensions.Y == 0)
            {
                return new RectangleF();
            }

            Point2 size = new Point();

            for (int x = 0; x < widgets.Count; x++)
            {
                size.X += widgets[x][0].Width;
            }

            if (widgets.Count != 0)
            {
                for (int y = 0; y < widgets[0].Count; y++)
                {
                    size.Y += widgets[0][y].Height;
                }
            }

            foreach (var divider in dividers)
            {
                size.Y += divider.Item1.Height;
            }

            return new RectangleF(base.Area.Position, size);
        }

        protected override void UpdateArea(bool update_parent)
        {
            AlignWidgets();
            base.UpdateArea(update_parent);
        }

        protected override object DerivedClone(Widget parent = null)
        {
            throw new NotImplementedException();
        }

        /// <summary> All widgets this widget owns. </summary>
        public override List<Widget> Children
        {
            get
            {
                List<Widget> children = new List<Widget>();
                Point2 dimensions = Dimensions;
                for (int x = 0; x < dimensions.X; x++)
                {
                    for (int y = 0; y < dimensions.Y; y++)
                    {
                        children.Add(widgets[x][y]);
                    }
                }

                foreach(var divider in dividers)
                {
                    children.Add(divider.Item1);
                }

                return children;
            }
        }

        #endregion Overrides
    }
}