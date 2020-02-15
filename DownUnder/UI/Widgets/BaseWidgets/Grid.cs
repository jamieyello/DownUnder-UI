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
        protected bool _disable_update_area = false;

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
            UpdateArea(false);
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
        
        public void SetCell(int x, int y, Widget widget, bool preserve_cell_area = false, bool update_parent = true)
        {
            widget.Parent = this;

            if (preserve_cell_area) widget.Area = _widgets[x][y].Area;
            _widgets[x][y] = widget;
            UpdateArea(update_parent);
        }

        /// <summary> Returns the WidgetArray used by this Grid. Does not include dividers. </summary>
        public WidgetArray ToWidgetArray()
        {
            return _widgets;
        }

        #endregion Public Methods

        #region Overrides

        private Point2 _area_position_backing = new Point2();

        /// <summary> Area of this widget. (Position relative to parent widget, if any) </summary>
        public override RectangleF Area
        {
            get => (Dimensions.X == 0 || Dimensions.Y == 0) ? new RectangleF() : _widgets.AreaCoverage.Value.WithPosition(_area_position_backing);
            set
            {
                //Console.WriteLine("setting area to " + value);
                bool previous_disable_update_area = _disable_update_area;
                _disable_update_area = true;

                float divider_height = 0f;
                foreach (var divider in dividers)
                {
                    divider_height += divider.Item1.Height;
                }

                _area_position_backing = value.Position;
                _widgets.Align(value.WithHeight(value.Height - divider_height).SizeOnly());
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