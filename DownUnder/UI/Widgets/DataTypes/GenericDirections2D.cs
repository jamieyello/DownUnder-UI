using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes {
    [DataContract] public class GenericDirections2D <T> {
        T up_backing;
        T down_backing;
        T left_backing;
        T right_backing;

        [DataMember] public T Up  { 
            get => up_backing; 
            set  {
                up_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            } 
        }
        [DataMember] public T Down {
            get => down_backing;
            set {
                down_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember] public T Left {
            get => left_backing;
            set {
                left_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }
        [DataMember] public T Right {
            get => right_backing;
            set {
                right_backing = value;
                OnValueAssign?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <param name="instance_parameters"> Parameters that should be used when creating all 4 directions. </param>
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

        public event EventHandler OnValueAssign;
    }
}
