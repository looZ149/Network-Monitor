using System.Net;
using System.Net.Sockets;

namespace NetworkMonitor.Functions
{
    class ScanNetwork
    {
        public static string LocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName()); // Get IPHostEntry -> Contains HostName, Addresslist

            foreach(var ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork) // Look trough every IP and check if its AddressFamily is a IPv4 (InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Couldnt find local ip"); // In case we cant find the local ip, throw an error
        }


    }
}
