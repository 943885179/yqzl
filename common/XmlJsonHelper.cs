using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace common
{
   public class XmlJsonHelper
    {
        /// <summary>
        /// json转xml
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public static XmlDocument JsonToXml(string jsonText) {
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonText, "root");
            return doc;
        }

        /// <summary>
        /// xml转json
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static string XmlToJson(string xmlFileName) {

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFileName);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }
    }
}
