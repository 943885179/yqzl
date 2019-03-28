using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl
{
    public class YQUntil
    {

        public static int HEAD_LEN_NEW = 222;//报文头长度
        public static int HEAD_LEN_OLD = 6;
        public static String CHARSET = "GBK";//请求编码方式
        private static String fmtTime = "yyyyMMddHHmmss";
        // private static  int TIME_OUT = 120000; //超时时间，单位为毫秒，默认2分钟
        public static int PROTOCAL_TCP = 0;
        public static int PROTOCAL_HTTP = 1;
        public static int PROTOCAL_HTTPS = 2;
        /// <summary>
        /// 组装报文
        /// </summary>
        /// <param name="yqdm">银企代码20位</param>
        /// <param name="bsnCode">交易代码</param>
        /// <param name="xmlBody">xml主体报文</param>
        /// <returns></returns>
        public static String asemblyPackets(string yqdm, String bsnCode, String xmlBody)
        {
            DateTime now = DateTime.Now;//请求时间
            StringBuilder buf = new StringBuilder();
            buf.Append("A00101");
            //编码
            var encodinglength = "0";
            String encoding = "01";
            if (CHARSET.Equals("GBK"))
            {
              //  System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                encoding = "01";
                encodinglength = Encoding.GetEncoding("GBK").GetBytes(xmlBody).Length.ToString();
            }
            else if (CHARSET.Equals("utf-8") || CHARSET.Equals("utf8"))
            {
                encoding = "02";
                encodinglength = Encoding.GetEncoding("utf-8").GetBytes(xmlBody).Length.ToString();
            }
            buf.Append(encoding);//编码

            buf.Append("01");//通讯协议为TCP/IP 01 01:tcpip 缺省02：http 03：webservice现在只支持：TCPIP接入
            //银企代码20位
            yqdm = yqdm.PadLeft(20, '0');
            buf.Append(yqdm);//银企代码
                             //报文体数据的字节长度

            buf.Append(encodinglength.PadLeft(10, '0'));

            buf.Append(bsnCode.PadRight(6, ' '));//交易码-左对齐
            buf.Append("00000");//操作员代码-用户可自定义,5位
            buf.Append("01");//服务类型 01请求
            buf.Append(now.ToString(fmtTime)); //请求日期时间

            String requestLogNo = "YQMFPA" + now.ToString(fmtTime); //唯一流水号设置
            buf.Append(requestLogNo);//请求方系统流水号

            buf.Append("000000"); //返回码,请求时必须填写000000 非“000000”代表交易受理异常或失败
            string fhms = "";
            buf.Append(fhms.PadLeft(100, ' '));

            buf.Append(0); //后续包标志0-结束包，1-还有后续包 目前仅支持0
            buf.Append("000");//请求次数目前仅支持000
            buf.Append("0");//签名标识 0不签0-不签名1 - 签名目前仅支持填0
            buf.Append("1");//签名数据包格式N	0-裸签1 - PKCS7目前仅支持送1
            string qmsf = "";
            buf.Append(qmsf.PadLeft(12, ' ')); //签名算法
            int qmsflength = 0;
            buf.Append(qmsflength.ToString().PadLeft(10, '0')); //签名数据长度签名报文数据长度, 目前仅支填写0
            buf.Append(0);//附件数目0-没有,默认为0；有的话填具体个数，最多9个
            buf.Append(xmlBody);//报文体
            return buf.ToString();
        }
    }

}
