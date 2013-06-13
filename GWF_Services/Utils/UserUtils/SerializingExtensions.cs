using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization;

namespace GWF_WebServices.Utils.UserUtils
{
    public static class SerializingExtensions
    {
        public static string XMLSerialize(this object obj)
        {
            string result = "";
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// XML数据反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象</typeparam>
        /// <param name="xmlstr"><returns></returns>
        public static object XMLDeserialize(this string xmlstr)
        {
            byte[] newBuffer = System.Text.Encoding.UTF8.GetBytes(xmlstr);

            if (newBuffer.Length == 0)
            {
                return default(object);
            }
            using (MemoryStream ms = new MemoryStream(newBuffer))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(object));
                return serializer.ReadObject(ms);
            }
        }

    }
}