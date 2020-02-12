using DownUnder.UI.Widgets.DataTypes;
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
        private WidgetArray _widgets = new WidgetArray();

        /// <summary> A list of dividers in a tuple with their y index. </summary>
        private List<Tuple<Widget, int>> dividers = new List<Tuple<Widget, int>>();

        /// <summary> This is broken for a possibly obvious reason. It might not matter. </summary>
        private const int _RESIZING_ACCURACY = 1;

        /// <summary> When set to true this widget will ignore children's invoking of UpdateArea(). </summary>
        private bool _disable_update_area = false;

        #endregion

        #region Public Properties

        /// <summary> The number of widgets tall and wide this grid consists of. </summary>
        public Point Dimensions => _widgets.Dimensions;

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
            _disable_update_area = true;
            SetDefaults();

            if (filler == null)
            {
                filler = DefaultCell();
            }

            filler.Parent = this;

            _widgets = new WidgetArray(x_length, y_length, filler);
            _disable_update_area = false;
            UpdateArea(true);
        }

        private void SetDefaults()
        {
            Size = new Point2(100, 100);
            DrawBackground = false;
        }

        #endregion

        #region Private Methods
        
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

        //this is currently not used
        private void SpaceAllCells()
        {
            if (_widgets.Count == 0 || _widgets[0].Count == 0) return;

            Point2 position = new Point2();

            for (int x = 0; x < _widgets.Count; x++)
            {
                position.Y = 0;
                for (int y = 0; y < _widgets[0].Count; y++)
                {
                    foreach (var divider in dividers)
                    {
                        if (y == divider.Item2) position.Y += divider.Item1.Height;
                    }

                    _widgets[x][y].Position = position;
                    position.Y += _widgets[x][y].Height;
                }
                position.X += _widgets[x][0].Width;
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
                        _widgets[0][divider.Item2 - 1].Position.WithOffset(
                        new Point2(0, _widgets[0][divider.Item2 - 1].Height + same_row_divider_offset));
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
            return _widgets[x][y];
        }

        public Point IndexOf(Widget widget)
        {
            return _widgets.IndexOf(widget);
        }
        
        public void SetCell(int x, int y, Widget widget, bool update_parent = false)
        {
            widget.Parent = this;

            widget.Area = _widgets[x][y].Area;
            _widgets[x][y] = widget;
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

            if (!(_widgets[x][y] is Layout))
            {
                throw new Exception($"Cannot add widget to cell[{x}][{y}], because cell[{x}][{y}] is not a Layout. widgets[{x}][{y}].GetType() = {_widgets[x][y].GetType()}");
            } 
            ((Layout)_widgets[x][y]).AddWidget(widget);
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
                bool previous_area_update = _disable_update_area;
                _disable_update_area = true;
                RectangleF original_area = Area;
                base.Area = value;
                RectangleF new_area = area_backing;
                
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
                
                _widgets.ExpandAll(expansion_size);

                foreach (var divider in dividers)
                {
                    divider.Item1.Height *= expansion_size.Y;
                }

                _disable_update_area = previous_area_update;
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
                if (_widgets[i][row].IsFixedHeight) return _widgets[i][row].Height;
            }

            return -1f;
        }

        float FixedWidthOfColumn(int column)
        {
            for (int i = 0; i < Dimensions.Y; i++)
            {
                if (_widgets[column][i].IsFixedWidth) return _widgets[column][i].Width;
            }

            return -1f;
        }

        private RectangleF GetContentArea()
        {
            if (Dimensions.X == 0 || Dimensions.Y == 0)
            {
                return new RectangleF();
            }

            return ((RectangleF)_widgets.AreaCoverage).WithPosition(base.Area.Position);
        }

        protected override void UpdateArea(bool update_parent)
        {
            if (_disable_update_area) return;
            _widgets.Align();
            base.UpdateArea(update_parent);
        }

        protected override object DerivedClone(Widget parent = null)
        {
            throw new NotImplementedException();
        }

        /// <summary> All widgets this widget owns. </summary>
        public override WidgetList Children
        {
            get
            {
                WidgetList children = new WidgetList();
                children.ToList().AddRange(_widgets.ToWidgetList());

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