using NetworkMonitor.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace NetworkMonitor.Functions
{
    class ScanNetwork
    {
        public string localip;
        public string baseip;
        // List of NetworkDevices so we can safe the devices directly while scanning
        public List<NetworkDevices> Devices { get; set; } = new(); 



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
        

        //Get the first 3 oktets so we can iterate trough the last oktet and check for devices
        public void GetBaseIP()
        {
            localip = LocalIP();
            baseip = string.Join('.', localip.Split('.').Take(3));

        }

        //Useless if we populate the List directly if we have a successful ping?
        //public async Task GetDevices()
        //{
        //    Devices.Clear();
        //    var foundDevices = await ScanLocalNetwork(baseip);
        //    foreach(var device in foundDevices)
        //    {
        //        Devices.Add(new NetworkDevices { IPAddress = foundDevices.})
        //    }
        //}

        public void ScanLocalNetwork(string baseip)
        {
            string IP;
            var onlineHosts = new List<NetworkDevices>();
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
                            lock (onlineHosts)
                            {
                                onlineHosts.Add(new NetworkDevices { IPAddress = ip});
                            }

                        }
                    }
                    catch 
                    {
                       //can pretty much just let any error pass ?
                    }
                }));
            }
            
        }


       
    }
}
