using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder {
    [DataContract] public class GenericDirections2D <T> : ICloneable, IEnumerable<T>
    {
        T up_backing;
        T down_backing;
        T left_backing;
        T right_backing;

        public T GetDirection(Direction2D direction) => this[direction];
        public void SetDirection(Direction2D direction, T value) => this[direction] = value;
        public T this[Direction2D direction]
        {
            get => direction == Direction2D.up ? up_backing :
                    direction == Direction2D.down ? down_backing :
                    direction == Direction2D.left ? left_backing :
                    right_backing;
            set
            {
                if (direction == Direction2D.up) Up = value;
                if (direction == Direction2D.down) Down = value;
                if (direction == Direction2D.left) Left = value;
                if (direction == Direction2D.right) Right = value;
            }
        }

        public List<T> GetDirections(Directions2D directions) => this[directions];
        public List<T> this[Directions2D directions]
        {
            get
            {
                List<T> result = new List<T>();
                if (directions.Up) result.Add(Up);
                if (directions.Down) result.Add(Down);
                if (directions.Left) result.Add(Left);
                if (directions.Right) result.Add(Right);
                return result;
            }
        }

        [DataMember]
        public T Up
        {
            get => up_backing;
            set
            {
                up_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T Down
        {
            get => down_backing;
            set
            {
                down_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T Left
        {
            get => left_backing;
            set
            {
                left_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember]
        public T Right
        {
            get => right_backing;
            set
            {
                right_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }

        public GenericDirections2D()
        {
            object[] instance_parameters = new object[0];
            Up = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Down = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Left = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Right = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }

        public GenericDirections2D(bool create_instances = true)
        {
            if (!create_instances) return;
            object[] instance_parameters = new object[0];
            Up = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Down = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Left = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Right = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }

        public GenericDirections2D(ValueType value)
        {
            Up = (T)Convert.ChangeType(value, typeof(T));
            Down = (T)Convert.ChangeType(value, typeof(T));
            Left = (T)Convert.ChangeType(value, typeof(T));
            Right = (T)Convert.ChangeType(value, typeof(T));
        }

        public GenericDirections2D(ICloneable obj)
        {
            Up = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            Down = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            Left = (T)Convert.ChangeType(obj.Clone(), typeof(T));
            Right = (T)Convert.ChangeType(obj.Clone(), typeof(T));
        }

        /// <param name="instance_parameters"> Parameters that should be used when creating all 4 directions. </param>
        public GenericDirections2D(object[] instance_parameters)
        {
            Up = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Down = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Left = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Right = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }
        public GenericDirections2D(T up, T down, T left, T right)
        { Up = up; Down = down; Left = left; Right = right; }

        /// <summary> Returns a new list of <see cref="T"/> excluding all null values. </summary>
        public List<T> GetAllNotNull
        {
            get
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    List<T> result = new List<T>();
                    if (Up != null) result.Add(Up);
                    if (Down != null) result.Add(Down);
                    if (Left != null) result.Add(Left);
                    if (Right != null) result.Add(Right);
                    return result;
                }

                return new List<T>() { Up, Down, Left, Right };
            }
        }

        /// <summary> Returns true if any of the directional values are null. </summary>
        public bool HasNull => Up == null || Down == null || Left == null || Right == null;

        public event EventHandler OnValueAssign;

        public bool IsCloneable => typeof(T).IsValueType || typeof(ICloneable).IsAssignableFrom(typeof(T));

        public object Clone()
        {
            if (typeof(T).IsValueType)
            {
                GenericDirections2D<T> result = new GenericDirections2D<T>(false);
                result.Up = Up;
                result.Down = Down;
                result.Left = Left;
                result.Right = Right;
                return result;
            }
            else if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                GenericDirections2D<T> result = new GenericDirections2D<T>(false);
                if (Up is ICloneable up) result.Up = (T)up.Clone();
                if (Down is ICloneable down) result.Down = (T)down.Clone();
                if (Left is ICloneable left) result.Left = (T)left.Clone();
                if (Right is ICloneable right) result.Right = (T)right.Clone();
                return result;
            }

            throw new Exception($"Cannot clone {typeof(T)} because it is not a {nameof(ValueType)} and does not implement {nameof(ICloneable)}.");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            yield return up_backing;
            yield return down_backing;
            yield return left_backing;
            yield return right_backing;
        }
    }
}
