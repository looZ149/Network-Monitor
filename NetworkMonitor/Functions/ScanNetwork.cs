﻿using NetworkMonitor.Models;
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
        public int count = 0;

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

        //Quantum code error
        //Run code with a breakpoint -> code works - Sometimes throws an error tho?
        //Run code without a breakpoint -> code doesnt work??
        //Thanks copilot

        public async Task<ObservableCollection<NetworkDevices>> ScanLocalNetwork(string baseip)
        {
            
            var devicesOnline = new ObservableCollection<NetworkDevices>();
            var tasks = new List<Task>(); // Create a list of Tasks to ping multiple IP's at once
                                          //Ping is not "thread safe" which leads to unexpected behavior. Means it can not be safely used by multiple threads
                                          //Every task need to use their own Ping function to make it work properly
                                          //Since its a share Instance of the ping, its causing race conditions because every task executes it on their own
            // ^ doesnt work
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
                            
                            //Lock the list so we ensure only one Task adds his result to it.
                            lock (devicesOnline)
                            {
                                devicesOnline.Add(new NetworkDevices { IPAddress = ip, IsOnline = true, DeviceID = count });
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

        //Function gets called on Launch - Gets all active devices and populates the list with ips
        public void GetIP()
        {
            localip = LocalIP();
            baseip = string.Join('.', localip.Split('.').Take(3));
        }

    }
}
