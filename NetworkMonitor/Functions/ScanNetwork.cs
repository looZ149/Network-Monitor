using NetworkMonitor.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Threading.Tasks;



namespace NetworkMonitor.Functions
{
    class ScanNetwork
    {
        public string? localip;
        public string? baseip;
        public int count = 1;


        //Function gets called on Launch - Gets all active devices and populates the list with ips
        public void GetIP()
        {
            localip = LocalIP();
            baseip = string.Join('.', localip.Split('.').Take(3));
        }

        //dont need as async task cuz we will run this function on every start of the App
        //should be rather fast + we execute it on the start so.. should be fine?
        public static string LocalIP() 
        {
            var host = Dns.GetHostEntry(Dns.GetHostName()); // Get IPHostEntry -> Contains HostName, Addresslist


            //If we have a VM installed, we got atleast 1 other Ethernet adapter, so we get a wrong result here
            //1st entry that matches the IF is NOT our routers ip
            //Solutions: Skip 1st entry, return 2nd?
            //           Hardcode 192.168.178 as baseip? Probably worst solution tbh
            //What if the 1st entry being wrong is just in MY case??
            //Solutions: We could add all IPv4's into a list, and iterate trough all "baseips"
            //In my case: 510 pings
            //Startup would take twice as long as before..
            //Any other idea to identify the "real" baseip?
            //Nop
            //Fixed in the ScanLocalNetwork function. Tbf, to lazy to figure out another solution rn. Just hardcoded it.

            foreach(var ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork) // Look trough every IP and check if its AddressFamily is a IPv4 (InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Couldnt find local ip"); // In case we cant find the local ip, throw exception with a msg, still continue
        }

        public string CutStringToCompany(string body)
        {
            //Maybe "a bit" inconvenient but hey... it works
            //Can shorten it (probably) but 2lazy rn
            string[] split = body.Split("name");
            string[] split2 = split[1].Split("address");
            string[] split3 = split2[0].Split('"');

            return split3[2];
        }


        public async Task<string> MacLookUp(string macAddress)
        {
            var client = new HttpClient();
            var query = macAddress.Replace(":", "%3a"); //Should do the job to Format the Address to be ready to query it in the URl
            string result;
            var request = new HttpRequestMessage
            {

                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://mac-address-lookup1.p.rapidapi.com/static_rapid/mac_lookup/?query={query}"),
                Headers =
                {
                    { "x-rapidapi-key", "d9c2915098mshd5a371c2b843acbp12eb71jsn59379032f983" },
                    { "x-rapidapi-host", "mac-address-lookup1.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                result = CutStringToCompany(body);
            }
            
            return result;
        }

        public string GetMacAddress(string ipAddress) //Cant get own MAC??
        {
            string MacAddress = string.Empty;
            Process process = new Process();
            process.StartInfo.FileName = "arp";
            process.StartInfo.Arguments = "-a " + ipAddress;
            process.StartInfo.UseShellExecute = false; // Dont need the shell  
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true; // Dont need a window, we dump the result into a var  

            process.Start();
            var Output = process.StandardOutput.ReadToEnd();
            string[] outputStrings = Output.Split('-');
            if (outputStrings.Length >= 6)
            {
                MacAddress = outputStrings[3][^2..] // ^2 - Get 2 last characters - ^ start from end
                            + "-" + outputStrings[4] + "-" + outputStrings[5] + "-" + outputStrings[6] + "-" + outputStrings[7]
                            + "-" + outputStrings[8][..2]; // get 2 first chars

            }
            else
            {
                return "empty";
            }

            return MacAddress;
        }

        //Quantum code error
        //Run code with a breakpoint -> code works - Sometimes throws an error tho?
        //Run code without a breakpoint -> code doesnt work??
        //Thanks copilot

        public async Task<ObservableCollection<NetworkDevices>> ScanLocalNetwork(string baseip)
        {
            count = 0; //Reset the count to 0 on each execution. Otherwise Rescanning the Network displays wrong IDs in the UI

            var devicesOnline = new ObservableCollection<NetworkDevices>();
            var tasks = new List<Task>(); // Create a list of Tasks to ping multiple IP's at once
                                          //Ping is not "thread safe" which leads to unexpected behavior. Means it can not be safely used by multiple threads
                                          //Every task need to use their own Ping function to make it work properly
                                          //Since its a share Instance of the ping, its causing race conditions because every task executes it on their own


            if (baseip != "192.168.178")
                baseip = "192.168.178";


            for (int i = 0; i < 255; i++)
            {

                string ip = $"{baseip}.{i}";
                tasks.Add(Task.Run(async () => //Run multiple tasks at the same time(async)
                {
                    try
                    {
                        using Ping ping = new Ping();
                        
                        var reply = await ping.SendPingAsync(ip, 100); //If we do ping.Send it takes ages, therefor we use SendPingAsync
                        if (reply.Status == IPStatus.Success)
                        {
                            var MacAddress = GetMacAddress(ip);
                            //var Manufacturer = await MacLookUp(MacAddress); // Querys to fast, need to give it a 1sec delay 
                            //Lock the list so we ensure only one Task adds his result to it.
                            lock (devicesOnline)
                            {   
                                //Adds the devices before the Manufacturer got found
                                //Need to wait for the function to be finished first
                                devicesOnline.Add(new NetworkDevices { IPAddress = ip, IsOnline = true, 
                                                      DeviceID = count, macAddress = MacAddress });
                            }
                            count++;
                        }
                    }
                    catch
                    {
                        //can pretty much just let any error pass ?
                    }
                }));
            }
            await Task.WhenAll(tasks);
            return devicesOnline;
        }
    }
}
