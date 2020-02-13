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
                bool previous_disable_update_area = _disable_update_area;
                _disable_update_area = true;

                float divider_height = 0f;
                foreach (var divider in dividers)
                {
                    divider_height += divider.Item1.Height;
                }

                _widgets.Align(value.WithHeight(value.Height - divider_height));
                RectangleF? new_area = _widgets.AreaCoverage;
                if (new_area == null) return;

                foreach (var divider in dividers)
                {
                    divider.Item1.Area = new RectangleF(
                        new_area.Value.X,
                        _widgets.InsertSpaceY(divider.Item2, divider.Item1.Height),
                        new_area.Value.Width,
                        divider.Item1.Height
                        );
                }

                _disable_update_area = previous_disable_update_area;
                base.UpdateArea(true);
            }
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