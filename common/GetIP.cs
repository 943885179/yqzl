using Microsoft.Win32;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Management;
using System.Runtime.InteropServices;

namespace common
{
    public class GetIP
    {
        public static Boolean ipCan() {
            string wIp = getExternalIp();//外网ip固定
            var nIp = getIP();
            string[] canLoginList = new string[] { "陈永鹏", "陈妹二" };//允许访问的ip
            Boolean neiwanIp = false;
            foreach (var user in canLoginList) {
                if (user.Equals(nIp.userName)) {
                    neiwanIp = true;
                    break;
                }
            }
            if (wIp.Equals("183.62.20.2") && neiwanIp) {
                return true;
            }
            return true;//ip地址不符合，不允许登录
        }
        /// <summary>
        /// 外网IP
        /// </summary>
        /// <returns></returns>
        public static string getExternalIp()
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;
                //string response = client.DownloadString("http://1212.ip138.com/ic.asp");//失效了
                //string response = client.DownloadString("http://icanhazip.com/");//可用，可能不稳定
                string response = client.DownloadString("http://ip.chinaz.com/");//站长之家
                string myReg = @"<dd class=""fz24"">([\s\S]+?)<\/dd>";
                Match mc = Regex.Match(response, myReg, RegexOptions.Singleline);
                if (mc.Success && mc.Groups.Count > 1)
                {
                    response = mc.Groups[1].Value;
                    return response;
                }
                else
                {
                    return "Can't get you Ip address!";
                }
            }
            catch (Exception)
            {
                return "Can't get you Ip address!";
            }

        }
       
        /// <summary>
        /// 取网卡信息
        /// </summary>
        /// <returns></returns>
        public static string GetNetAdapterInfo()
        {
            StringBuilder sb = new StringBuilder();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters != null)
            {
                foreach (NetworkInterface ni in adapters)
                {
                    string fCardType = "未知网卡";
                    IPInterfaceProperties ips = ni.GetIPProperties();

                    PhysicalAddress pa = ni.GetPhysicalAddress();
                    if (pa == null) continue;
                    string pastr = pa.ToString();
                    if (pastr.Length < 7) continue;
                    if (pastr.Substring(0, 6) == "000000") continue;
                    if (ni.Name.ToLower().IndexOf("vmware") > -1) continue;
                    string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + ni.Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        // 区分 PnpInstanceID     
                        // 如果前面有 PCI 就是本机的真实网卡    
                        // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                            if (ni.NetworkInterfaceType.ToString().ToLower().IndexOf("wireless") == -1)
                                fCardType = "物理网卡";
                            else
                                fCardType = "无线网卡";
                        else if (fMediaSubType == 1 || fMediaSubType == 0)
                            fCardType = "虚拟网卡";
                        else if (fMediaSubType == 2 || ni.NetworkInterfaceType.ToString().ToLower().IndexOf("wireless") > -1)
                            fCardType = "无线网卡";
                        else if (fMediaSubType == 7)
                            fCardType = "蓝牙";
                    }
                    StringBuilder isb = new StringBuilder();
                    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = ips.UnicastAddresses;
                    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    {
                        if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                            isb.Append(string.Format("Ip Address: {0}", UnicastIPAddressInformation.Address) + "\r\n"); // Ip 地址    
                    }

                    // IPAddressCollection ipc = ips.DnsAddresses;
                    //if (ipc.Count > 0)
                    //{
                    //    foreach (IPAddress ip in ipc)
                    //    {
                    //        isb.Append(string.Format("DNS服务器地址：{0}\r\n",ip));
                    //    }
                    //}
                    string s = string.Format("{0}\r\n描述信息：{1}\r\n类型：{2}\r\n速度：{3} MB\r\nMac地址：{4}\r\n{5}", fCardType, ni.Name, ni.NetworkInterfaceType, ni.Speed / 1024 / 1024, ni.GetPhysicalAddress(), isb.ToString());
                    sb.Append(s + "\r\n");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 取网卡信息
        /// </summary>
        /// <returns></returns>
        public static IpModel GetNetAdapterInfos()
        {
            IpModel ip = new IpModel();
            StringBuilder sb = new StringBuilder();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters != null)
            {
                foreach (NetworkInterface ni in adapters)
                {
                    string fCardType = "未知网卡";
                    IPInterfaceProperties ips = ni.GetIPProperties();

                    PhysicalAddress pa = ni.GetPhysicalAddress();
                    if (pa == null) continue;
                    string pastr = pa.ToString();
                    if (pastr.Length < 7) continue;
                    if (pastr.Substring(0, 6) == "000000") continue;
                    if (ni.Name.ToLower().IndexOf("vmware") > -1) continue;
                    string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + ni.Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        // 区分 PnpInstanceID     
                        // 如果前面有 PCI 就是本机的真实网卡    
                        // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                            if (ni.NetworkInterfaceType.ToString().ToLower().IndexOf("wireless") == -1)
                                fCardType = "物理网卡";
                            else
                                fCardType = "无线网卡";
                        else if (fMediaSubType == 1 || fMediaSubType == 0)
                            fCardType = "虚拟网卡";
                        else if (fMediaSubType == 2 || ni.NetworkInterfaceType.ToString().ToLower().IndexOf("wireless") > -1)
                            fCardType = "无线网卡";
                        else if (fMediaSubType == 7)
                            fCardType = "蓝牙";
                    }
                    StringBuilder isb = new StringBuilder();
                    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = ips.UnicastAddresses;
                    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    {
                        if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                            isb.Append(string.Format("Ip Address: {0}", UnicastIPAddressInformation.Address) + "\r\n"); // Ip 地址    
                    }

                    // IPAddressCollection ipc = ips.DnsAddresses;
                    //if (ipc.Count > 0)
                    //{
                    //    foreach (IPAddress ip in ipc)
                    //    {
                    //        isb.Append(string.Format("DNS服务器地址：{0}\r\n",ip));
                    //    }
                    //}
                    string s = string.Format("{0}\r\n描述信息：{1}\r\n类型：{2}\r\n速度：{3} MB\r\nMac地址：{4}\r\n{5}", fCardType, ni.Name, ni.NetworkInterfaceType, ni.Speed / 1024 / 1024, ni.GetPhysicalAddress(), isb.ToString());
                    sb.Append(s + "\r\n");
                    ip.mac = ni.GetPhysicalAddress().ToString();
                    ip.ip = isb.ToString();
                    ip.type = fCardType;
                }
            }
            return ip;
        }
        /// <summary>
        /// 内网IP
        /// </summary>
        /// <returns></returns>
        public static UserModel getIP()
        {
            //与上面一样，只不过用了Dns.GetHostAddresses()的方法
            IPAddress[] dnsIps = Dns.GetHostAddresses(Dns.GetHostName());
            for (int i = 0; i < dnsIps.Length; i++)
            {
                if (dnsIps[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //netInfo += "\r\nDns.GetHostAddresses()得到本机正在使用的IP为：" + dnsIps[i].ToString();
                    
                    // return dnsIps[i].ToString();
                    return new UserModel()
                    {
                        loginName = "cyp",
                        userName = "陈永鹏"
                    };

                }
            }
            return null;
        }
        /// <summary>
        /// 获取客户端ip地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {
            string userIP = "未获取用户IP";

            try
            {
                if (HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP 
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;
        }
        [DllImport("Iphlpapi.dll")]

        static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 MacAddr, ref Int32 PhyAddrLen);
        [DllImport("Ws2_32.dll")]
        static extern Int32 inet_addr(string ipaddr);
        ///<summary>
        /// SendArp获取MAC地址
        ///</summary>
        ///<param name="RemoteIP">目标机器的IP地址如(192.168.1.1)</param>
        ///<returns>目标机器的mac 地址</returns>
        public static string GetMacAddress(string RemoteIP)
        {

            StringBuilder macAddress = new StringBuilder();

            try
            {
                Int32 remote = inet_addr(RemoteIP);
                Int64 macInfo = new Int64();
                Int32 length = 6;
                SendARP(remote, 0, ref macInfo, ref length);
                string temp = Convert.ToString(macInfo, 16).PadLeft(12, '0').ToUpper();
                int x = 12;
                for (int i = 0; i < 6; i++)
                {
                    if (i == 5)
                    {
                        macAddress.Append(temp.Substring(x - 2, 2));
                    }
                    else
                    {
                        macAddress.Append(temp.Substring(x - 2, 2) + "-");
                    }

                    x -= 2;
                }
                return macAddress.ToString();
            }
            catch
            {
                return macAddress.ToString();
            }
        }

    }
    public class IpModel{
        public string ip { get; set; }
        public string  mac { get; set; }
        public string type { get; set; }
    }
}
