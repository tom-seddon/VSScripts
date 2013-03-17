using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Company.VSScripts
{
    class Xml
    {
        public static readonly Encoding defaultXmlEncoding = new UTF8Encoding(false);//false=no BOM

        private static XmlWriterSettings _defaultXmlWriterSettings = null;

        public static XmlWriterSettings defaultXmlWriterSettings
        {
            get
            {
                if (_defaultXmlWriterSettings == null)
                {
                    _defaultXmlWriterSettings = new XmlWriterSettings();

                    _defaultXmlWriterSettings.Encoding = defaultXmlEncoding;
                    _defaultXmlWriterSettings.Indent = true;
                }

                return _defaultXmlWriterSettings;
            }
        }

        public static void SaveXml<T>(string fileName, T obj) where T:class
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(fileName, defaultXmlWriterSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj);
                }
            }
            catch (System.Exception ex)
            {
            	// ...???
                Console.Write(ex.ToString());
            }
        }

        public static T LoadXml<T>(string fileName) where T:class
        {
            T result;

            try
            {
                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    result = (T)serializer.Deserialize(reader);
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
                return null;
            }

            return result;
        }
    }
}
