using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Company.VSScripts
{
    static class Misc
    {
        public static string GetPathFileNameWithoutExtension(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string GetPathExtension(string path)
        {
            try
            {
                return Path.GetExtension(path);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string GetPathDirectoryName(string path)
        {
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string GetPathRoot(string path)
        {
            try
            {
                return Path.GetPathRoot(path);
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        //-///////////////////////////////////////////////////////////////////////
        //-///////////////////////////////////////////////////////////////////////

        public static readonly Encoding defaultXmlEncoding = new UTF8Encoding(false);//false=no BOM

        //-///////////////////////////////////////////////////////////////////////
        //-///////////////////////////////////////////////////////////////////////

        public static T LoadXml<T>(string fileName) where T : class
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(reader) as T;
            }
        }

        //-///////////////////////////////////////////////////////////////////////
        //-///////////////////////////////////////////////////////////////////////

        public static T LoadXmlOrCreateDefault<T>(string fileName) where T : class,new()
        {
            try
            {
                return LoadXml<T>(fileName);
            }
            catch (InvalidOperationException)
            {
            }
            catch (FileNotFoundException)
            {
            }

            return new T();
        }

        //-///////////////////////////////////////////////////////////////////////
        //-///////////////////////////////////////////////////////////////////////

        public static void SaveXml<T>(string fileName, T data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = defaultXmlEncoding;
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
            }
        }

        //-///////////////////////////////////////////////////////////////////////
        //-///////////////////////////////////////////////////////////////////////
    }
}
