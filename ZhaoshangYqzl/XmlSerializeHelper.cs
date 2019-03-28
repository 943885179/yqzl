using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZhaoshangYqzl
{/// <summary>
 /// XML序列化公共处理类
 /// </summary>
    public static class XmlSerializeHelper
    {
        /// <summary>
        /// 将实体对象转换成XML
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj">实体对象</param>
        public static string XmlSerialize<T>(T obj)
        {
            try
            {
                //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("gb2312"));
                // XmlTextWriter textWriter = new XmlTextWriter(ms, Encoding.GetEncoding("UTF-8"));
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                string xmlMessage = Encoding.GetEncoding("gb2312").GetString(ms.GetBuffer());
                ms.Close();
                sw.Close();
                return xmlMessage;

            }
            catch (Exception ex)
            {
                throw new Exception("将实体对象转换成XML异常", ex);
            }
        }

        /// <summary>
        /// 将XML转换成实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="strXML">XML</param>
        public static T DESerializer<T>(string strXML) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(strXML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成实体对象异常", ex);
            }
        }
    }
}
