using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DownUnder.UI.Utilities.Serialization
{
    public static class XmlHelper {
        static DataContractSerializerSettings Settings(IEnumerable<Type> known_types) =>
            new DataContractSerializerSettings() {
            PreserveObjectReferences = true,
            SerializeReadOnlyTypes = true,
            KnownTypes = known_types };

        /// <summary> Serializes an object to an XML file. </summary>
        public static void ToXmlFile(Object obj, string filePath, IEnumerable<Type> known_types = null) {
            var xs = new DataContractSerializer(obj.GetType(), Settings(known_types));
            using FileStream writer = new FileStream(filePath, FileMode.Create);

            xs.WriteObject(writer, obj);
        }

        /// <summary> Deserializes an object from an XML file. </summary>
        public static T FromXmlFile<T>(string fileName, IEnumerable<Type> known_types = null) {
            var ser = new DataContractSerializer(typeof(T), Settings(known_types));
            using var fs = new FileStream(fileName, FileMode.Open);
            using var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            
            return (T)ser.ReadObject(reader, true);
        }

        /// <summary> Serializes an object to an XML string. </summary>
        public static string ToXml(object obj, IEnumerable<Type> known_types = null) {
            var xs = new DataContractSerializer(obj.GetType(), Settings(known_types));

            using var memStm = new MemoryStream();
            xs.WriteObject(memStm, obj);

            memStm.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(memStm);

            return streamReader.ReadToEnd();
        }

        /// <summary> Deserializes an object from an XML string. </summary>
        public static T FromXml<T>(string xml, IEnumerable<Type> known_types = null) {
            var ser = new DataContractSerializer(typeof(T), Settings(known_types));
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(xml));
            using var reader = XmlDictionaryReader.CreateTextReader(ms, new XmlDictionaryReaderQuotas());

            return (T)ser.ReadObject(reader, true);
        }
    }
}