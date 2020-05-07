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
    /// <summary> A grid of <see cref="Widget"/>s. Cells are empty <see cref="Layout"/>s by default. </summary>
    public class Grid : Widget {
        /// <summary> A jagged array of all the contained <see cref="Widget"/>s. (Widgets[x][y]) </summary>
        private readonly WidgetArray _widgets = new WidgetArray();
        /// <summary> A list of dividers in a tuple with their y index. </summary>
        private readonly List<Tuple<Widget, int>> dividers = new List<Tuple<Widget, int>>();
        /// <summary> When set to false the <see cref="_area_cache"/> will update next time <see cref="Area"/> is read. </summary>
        private bool _area_cache_updated = false;
        /// <summary> Using a cache for the <see cref="Area"/> of this <see cref="Widget"/> greatly improves performance. </summary>
        private RectangleF _area_cache;
        /// <summary> When set to true <see cref="_area_cache"/> will be used. Only disabled for ruling out bugs. </summary>
        private const bool _USE_AREA_CACHE = true;
        private Point2 _area_position_backing = new Point2();

        /// <summary> The number of <see cref="Widget"/>s tall and wide this <see cref="Grid"/> consists of. </summary>
        public Point Dimensions => _widgets.Dimensions;

        public Grid(IParent parent = null) : base(parent) => SetDefaults(false);
        public Grid(IParent parent, int x_length, int y_length, Widget filler = null, bool debug = false) : base(parent) {
            if (filler == null) filler = DefaultCell();
            filler.Parent = this;
            _widgets = new WidgetArray(x_length, y_length, filler);
            _widgets.SetParent(this);
            SetDefaults(debug);
        }
        private void SetDefaults(bool debug) => _widgets.Align(new RectangleF(0, 0, 100, 100));
        
        /// <summary> Add a divider to the given row. </summary>
        public void InsertDivider(Widget divider, int y) {
            divider.Parent = this;
            divider.Width = Width;
            dividers.Add(new Tuple<Widget, int>(divider, y));

            SignalChildAreaChanged();
        }

        /// <summary> Find and remove the given divider. </summary>
        public bool RemoveDivider(Widget divider) {
            for (int i = 0; i < dividers.Count; i++) {
                if (dividers[i].Item1 == divider) {
                    dividers.RemoveAt(i);
                    SignalChildAreaChanged();
                    return true;
                }
            }

            return false;
        }

        // unfinished
        private void InsertRow(List<Widget> widgets, int y) {
            if (widgets.Count != Dimensions.X) throw new Exception("Given list of widgets' length doesn't match the X dimension of this grid.");
            SignalChildAreaChanged();
        }

        // unfinished
        private void InsertColumn(List<Widget> widgets, int x) {
            if (widgets.Count != Dimensions.Y) throw new Exception("Given list of widgets' length doesn't match the Y dimension of this grid.");
            SignalChildAreaChanged();
        }

        /// <summary> Returns a list of dividers that exist on the given row. </summary>
        public List<Widget> DividersAt(int y) {
            List<Widget> result = new List<Widget>();

            foreach (var divider in dividers) {
                if (divider.Item2 == y) result.Add(divider.Item1);
            }

            return result;
        }

        /// <summary> The default cell is a <see cref="Layout"/>. </summary>
        protected Layout DefaultCell() {
            // Create cell
            Layout default_widget = new Layout(this);
            default_widget.OutlineSides = Directions2D.DR;
            default_widget.FitToContentArea = true;
            return default_widget;
        }

        private void Align(RectangleF value) {
            float divider_height = 0f;
            foreach (var divider in dividers) divider_height += divider.Item1.Height;

            _area_position_backing = value.Position;
            _widgets.Align(value.WithHeight(value.Height - divider_height).SizeOnly());
            RectangleF? new_area = _widgets.AreaCoverage;
            if (new_area == null) return;
            _area_cache = new_area.Value.WithPosition(_area_position_backing);
            _area_cache_updated = true;

            foreach (var divider in dividers) {
                divider.Item1.Area = new RectangleF(
                    new_area.Value.X,
                    _widgets.InsertSpaceY(divider.Item2, divider.Item1.Height),
                    new_area.Value.Width,
                    divider.Item1.Height
                    );
            }
        }

        public Widget GetCell(int x, int y) => _widgets[x][y];
        public Point IndexOf(Widget widget) => _widgets.IndexOf(widget);
        
        public void SetCell(int x, int y, Widget widget, bool preserve_cell_area = true, bool update_parent = true) {
            widget.Parent = this;

            if (preserve_cell_area) widget.Area = _widgets[x][y].Area;
            _widgets[x][y] = widget;
        }

        protected override object DerivedClone() 
        {
            Grid c = new Grid(null, Dimensions.X, Dimensions.Y);
            Console.WriteLine("WARNING: Grid clone incomplete.");
            return c;
        }

        protected override void HandleChildDelete(Widget widget) => throw new NotImplementedException();
        /// <summary> Area of this <see cref="Widget"/>. (Position relative to <see cref="Parent"/>, if any) </summary>
        public override RectangleF Area {
            get {
                if (Dimensions.X == 0 || Dimensions.Y == 0) return new RectangleF();
                if (_area_cache_updated && _USE_AREA_CACHE) return _area_cache;
                _area_cache_updated = true;
                _area_cache = _widgets.AreaCoverage.Value.WithPosition(_area_position_backing);
                return _area_cache;
            }
            set => Align(value);
        }
        internal override void SignalChildAreaChanged() {
            _area_cache_updated = false;
            base.SignalChildAreaChanged();
        }
        /// <summary> All <see cref="Widget"/>s this <see cref="Widget"/> owns. </summary>
        public override WidgetList Children {
            get {
                WidgetList children = new WidgetList();
                children.AddRange(_widgets.ToWidgetList());
                foreach(var divider in dividers) children.Add(divider.Item1);
                return children;
            }
        }
    }
}