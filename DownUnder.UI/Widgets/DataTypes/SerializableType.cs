using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes {
    // https://stackoverflow.com/questions/12306/can-i-serialize-a-c-sharp-type-object
    // a version of System.Type that can be serialized
    [DataContract]
    public sealed class SerializableType {
        public Type type;

        // when serializing, store as a string
        [DataMember]
        string TypeString {
            get => type?.FullName;
            set => type = value == null ? null : Type.GetType(value);
        }

        // constructors
        public SerializableType() {
        }

        public SerializableType(Type t) {
            type = t;
        }

        // allow SerializableType to implicitly be converted to and from System.Type
        public static implicit operator Type(SerializableType stype) => stype.type;
        public static implicit operator SerializableType(Type t) => new SerializableType(t);

        // overload the == and != operators
        public static bool operator ==(SerializableType a, SerializableType b) {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (a == null || b == null)
                return false;

            // Return true if the fields match:
            return a.type == b.type;
        }

        public static bool operator !=(SerializableType a, SerializableType b) => !(a == b);

        // we don't need to overload operators between SerializableType and System.Type because we already enabled them to implicitly convert

        // TODO: can be problems if the hashcode can vary
        // TODO: changing the hash will change the bucket that the object is supposed to be in
        // TODO: so a dictionary will have problems and possibly lose the object
        public override int GetHashCode() =>
            type.GetHashCode();

        public override bool Equals(object obj) =>
            obj != null && obj is SerializableType st && type == st.type;

        public bool Equals(SerializableType st) =>
            st != null && type == st.type;
    }
}
