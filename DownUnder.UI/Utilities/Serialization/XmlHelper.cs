using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DownUnder.Content.Utilities.Serialization
{
    public static class XmlHelper
    {
        /// <summary> Serializes an object to an XML file. </summary>
        public static void ToXmlFile(Object obj, string filePath, IEnumerable<Type> known_types = null)
        {
            DataContractSerializerSettings s = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            s.KnownTypes = known_types;
            var xs = new DataContractSerializer(obj.GetType(), s);
            using (FileStream writer = new FileStream(filePath, FileMode.Create))
            {
                xs.WriteObject(writer, obj);
            }
        }

        /// <summary> Deserializes an object from an XML file. </summary>
        public static T FromXmlFile<T>(string fileName, IEnumerable<Type> known_types = null)
        {
            DataContractSerializerSettings s = new DataContractSerializerSettings() { PreserveObjectReferences = true };
            s.KnownTypes = known_types;
            s.SerializeReadOnlyTypes = true; //?
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(T), s);

                    T result = (T)ser.ReadObject(reader, true);
                    return result;
                }
            }
        }
    }
}