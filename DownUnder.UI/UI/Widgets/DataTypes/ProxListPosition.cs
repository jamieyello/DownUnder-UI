using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    public sealed class ProxListPosition {
        readonly ProxList _parent;
        internal Point box { get; private set; }

        internal ProxListPosition(ProxList parent, Point2 position) {
            _parent = parent;
            _position_backing = position;
            box = parent.AddToArray(this, position);
        }

        Point2 _position_backing;
        public Point2 Position {
            get => _position_backing;
            set {
                _position_backing = value;
                box = _parent.UpdateBox(this, value);
            }
        }

        public float X {
            get => _position_backing.X;
            set => Position = new Point2(value, _position_backing.Y);
        }

        public float Y {
            get => _position_backing.Y;
            set => Position = new Point2(_position_backing.Y, value);
        }
    }
}