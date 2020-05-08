using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes {
    [DataContract] public struct GenericDirections2D <T> {
        [DataMember] public T Up;
        [DataMember] public T Down;
        [DataMember] public T Left;
        [DataMember] public T Right;

        public GenericDirections2D(object[] instance_parameters) {
            Up = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Down = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Left = (T)Activator.CreateInstance(typeof(T), instance_parameters);
            Right = (T)Activator.CreateInstance(typeof(T), instance_parameters);
        }
        public GenericDirections2D(T up, T down, T left, T right)
        { Up = up; Down = down; Left = left; Right = right; }

        /// <summary> Returns a new list of <see cref="T"/> excluding all null values. </summary>
        public List<T> GetAllNotNull {
            get {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) {
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
    }
}
