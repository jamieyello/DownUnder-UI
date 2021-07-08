using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI
{
    class GenericDiagonalDirections2D<T>
    {
        T top_left_backing;
        T top_right_backing;
        T bottom_left_backing;
        T bottom_right_backing;

        [DataMember]
        public T TopLeft
        {
            get => top_left_backing;
            set
            {
                top_left_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T TopRight
        {
            get => top_right_backing;
            set
            {
                top_right_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T BottomLeft
        {
            get => bottom_left_backing;
            set
            {
                bottom_left_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T BottomRight
        {
            get => bottom_right_backing;
            set
            {
                bottom_right_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }

        public GenericDiagonalDirections2D()
        {
            object[] instance_parameters = new object[0];
            TopLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            TopRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }

        public GenericDiagonalDirections2D(bool create_instances = true)
        {
            if (!create_instances) return;
            object[] instance_parameters = new object[0];
            TopLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            TopRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }

        public GenericDiagonalDirections2D(ValueType value)
        {
            TopLeft = (T)Convert.ChangeType(value, typeof(T));
            TopRight = (T)Convert.ChangeType(value, typeof(T));
            BottomLeft = (T)Convert.ChangeType(value, typeof(T));
            BottomRight = (T)Convert.ChangeType(value, typeof(T));
        }

        public GenericDiagonalDirections2D(ICloneable obj)
        {
            TopLeft = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            TopRight = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            BottomLeft = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            BottomRight = (T)Convert.ChangeType(obj.Clone(), typeof(T));
        }

        /// <param name="instance_parameters"> Parameters that should be used when creating all 4 directions. </param>
        public GenericDiagonalDirections2D(object[] instance_parameters)
        {
            TopLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            TopRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomLeft = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            BottomRight = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }
        public GenericDiagonalDirections2D(T top_left, T top_right, T bottom_left, T bottom_right)
        { TopLeft = top_left; TopRight = top_right; BottomLeft = bottom_left; BottomRight = bottom_right; }

        /// <summary> Returns a new list of <see cref="T"/> excluding all null values. </summary>
        public List<T> GetAllNotNull
        {
            get
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    List<T> result = new List<T>();
                    if (TopLeft != null) result.Add(TopLeft);
                    if (TopRight != null) result.Add(TopRight);
                    if (BottomLeft != null) result.Add(BottomLeft);
                    if (BottomRight != null) result.Add(BottomRight);
                    return result;
                }

                return new List<T>() { TopLeft, TopRight, BottomLeft, BottomRight };
            }
        }

        /// <summary> Returns true if any of the directional values are null. </summary>
        public bool HasNull => TopLeft == null || TopRight == null || BottomLeft == null || BottomRight == null;

        public event EventHandler OnValueAssign;

        public bool IsCloneable => typeof(T).IsValueType || typeof(ICloneable).IsAssignableFrom(typeof(T));

        public object Clone()
        {
            if (typeof(T).IsValueType)
            {
                GenericDiagonalDirections2D<T> result = new GenericDiagonalDirections2D<T>(false);
                result.TopLeft = TopLeft;
                result.TopRight = TopRight;
                result.BottomLeft = BottomLeft;
                result.BottomRight = BottomRight;
                return result;
            }
            else if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                GenericDiagonalDirections2D<T> result = new GenericDiagonalDirections2D<T>(false);
                if (TopLeft is ICloneable top_left) result.TopLeft = (T)top_left.Clone();
                if (TopRight is ICloneable top_right) result.TopRight = (T)top_right.Clone();
                if (BottomLeft is ICloneable bottom_left) result.BottomLeft = (T)bottom_left.Clone();
                if (BottomRight is ICloneable bottom_right) result.BottomRight = (T)bottom_right.Clone();
                return result;
            }

            throw new Exception($"Cannot clone {typeof(T)} because it is not a {nameof(ValueType)} and does not implement {nameof(ICloneable)}.");
        }
    }
}
