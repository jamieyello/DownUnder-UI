using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    // https://stackoverflow.com/questions/12306/can-i-serialize-a-c-sharp-type-object
    // a version of System.Type that can be serialized
    [DataContract]
    public class SerializableType
    {
        public Type type;

        // when serializing, store as a string
        [DataMember]
        string TypeString
        {
            get
            {
                if (type == null)
                    return null;
                return type.FullName;
            }
            set
            {
                if (value == null)
                    type = null;
                else
                {
                    type = Type.GetType(value);
                }
            }
        }

        // constructors
        public SerializableType()
        {
            type = null;
        }
        public SerializableType(Type t)
        {
            type = t;
        }

        // allow SerializableType to implicitly be converted to and from System.Type
        static public implicit operator Type(SerializableType stype)
        {
            return stype.type;
        }
        static public implicit operator SerializableType(Type t)
        {
            return new SerializableType(t);
        }

        // overload the == and != operators
        public static bool operator ==(SerializableType a, SerializableType b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.type == b.type;
        }
        public static bool operator !=(SerializableType a, SerializableType b)
        {
            return !(a == b);
        }
        // we don't need to overload operators between SerializableType and System.Type because we already enabled them to implicitly convert

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        // overload the .Equals method
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to SerializableType return false.
            SerializableType p = obj as SerializableType;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (type == p.type);
        }
        public bool Equals(SerializableType p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (type == p.type);
        }
    }
}
