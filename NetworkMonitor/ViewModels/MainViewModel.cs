using NetworkMonitor.Models;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using NetworkMonitor.Functions;

//ViewModel for MVVM -> Connection between View and Model
//Contains the Logic and manages the data and gets bound to the view

namespace NetworkMonitor.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<NetworkDevices> Devices { get; set; } = new();
        ScanNetwork scanNetwork = new();

        public ICommand PingAllCommand { get; }
        

        public MainViewModel()
        {
            scanNetwork.GetIP();
            Devices = Task.Run(() => scanNetwork.ScanLocalNetwork(scanNetwork.baseip)).Result; //Need to Task.Run it and retrieve the result like this,
                                                                                               //So we get it as actual ObservableCollection back
                                                                                               //else we get an converting error
            PingAllCommand = new Command(async () => await PingAllDevices());
        }



        public void AddDevice() 
        { 
            //Create a new window??
            //Put a Label under it which takes the Input and adds it to Devices?
        }

        public async Task PingDeviceID(int deviceID)
        {
            //Creat a new window??
            //Put a label under it which takes the Input and pings the device?
        }

        public async Task PingAllDevices()
        {
            foreach (var device in Devices)
            {
                using Ping ping = new Ping();
                var reply = await ping.SendPingAsync(device.IPAddress, 100);
                if(reply.Status != IPStatus.Success)
                {
                    device.IsOnline = false;
                }
            }
        }
    }
}
