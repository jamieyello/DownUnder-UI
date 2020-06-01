using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DownUnder.Content.Utilities.Serialization
{
    public static class XmlHelper
    {
        /// <summary> Serializes an object to an XML file. </summary>
        public static void ToXmlFile(Object obj, string filePath)
        {
            var xs = new DataContractSerializer(obj.GetType());
            using FileStream writer = new FileStream(filePath, FileMode.Create);
            xs.WriteObject(writer, obj);
        }

        /// <summary> Deserializes an object from an XML file. </summary>
        public static T FromXmlFile<T>(string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            using XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(T));

            T result = (T)ser.ReadObject(reader, true);

            return result;
        }
    }
}