using NetworkMonitor.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;



namespace NetworkMonitor.Functions
{
    class ScanNetwork
    {
        public string? localip;
        public string? baseip;

        //dont need as async task cuz we will run this function on every start of the App
        //should be rather fast + we execute it on the start so.. should be fine?
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
            throw new Exception("Couldnt find local ip"); // In case we cant find the local ip, throw exception with a msg, still continue
        }

        public ObservableCollection<NetworkDevices> ScanLocalNetwork(string baseip)
        {
            ObservableCollection<NetworkDevices> devicesOnline = new();
            var tasks = new List<Task>(); // Create a list of Tasks to ping multiple IP's at once
            using Ping ping = new Ping();

            for (int i = 0; i < 255; i++)
            {
                string ip = $"{baseip}.{i}";
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {

                        var reply = await ping.SendPingAsync(ip, 100);
                        if (reply.Status == IPStatus.Success)
                        {
                            //Lock the list so we ensure only one Task adds his result to it.
                            lock (devicesOnline)
                            {
                                devicesOnline.Add(new NetworkDevices { IPAddress = ip, IsOnline = true });
                            }

                        }
                    }
                    catch
                    {
                        //can pretty much just let any error pass ?
                    }
                }));
            }
            return devicesOnline;
        }

        //Function gets called on Launch - Gets all active devices and populates the list with ips
        public void GetDevices()
        {
            localip = LocalIP();
            baseip = string.Join('.', localip.Split('.').Take(3));
        }

    }
}
