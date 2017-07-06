using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;

namespace WindowsFormsApplication1
{
    class HostName
    {
        static HostName instance;
        string hostName = Dns.GetHostName();

        public IPAddress GetLocalIP(string hostName)
        {
            hostName = Dns.GetHostName();
            return Dns.GetHostByName(hostName).AddressList[1];
        }
        public IPAddress GetLocalIP()
        {
            hostName = Dns.GetHostName();
            return Dns.GetHostByName(hostName).AddressList[1];
        }
        public static HostName GetInstanceHostName()
        {
            if (instance == null)
            {
                return new HostName();
            }
            return instance;
        }
    }
}
