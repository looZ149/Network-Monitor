using NetworkMonitor.Models;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using NetworkMonitor.Functions;
using System.ComponentModel;

//ViewModel for MVVM -> Connection between View and Model
//Contains the Logic and manages the data and gets bound to the view

namespace NetworkMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NetworkDevices> Devices { get; set; } = new();
        ScanNetwork scanNetwork = new();

        public ICommand PingAllCommand { get; }
        public ICommand AddDeviceCommand { get; }
        public ICommand PingDeviceIDCommand { get; }
        public ICommand RescanCommand { get; }

        
        string? deviceIP;
        string? deviceID;
        public string? DeviceIP
        {
            get => deviceIP;
            set
            {
                deviceIP = value;
                OnPropertyChanged(nameof(DeviceIP));
            }
        }
        public string? DeviceID
        {
            get => deviceID;
            set
            {
                deviceID = value;
                OnPropertyChanged(nameof(DeviceID));
            }
        }
        

        public MainViewModel()
        {
            scanNetwork.GetIP();
            Devices = Task.Run(() => scanNetwork.ScanLocalNetwork(scanNetwork.baseip)).Result; //Need to Task.Run it and retrieve the result like this,
                                                                                               //So we get it as actual ObservableCollection back
                                                                                               //else we get an converting error
            PingAllCommand = new Command(async () => await PingAllDevices());
            AddDeviceCommand = new Command(() => AddDevice());
            PingDeviceIDCommand = new Command(() => PingDeviceID());
            RescanCommand = new Command(async () => await RescanNetwork());
        }

        public async Task PingAllDevices()
        {
            foreach (var device in Devices)
            {
                using Ping ping = new Ping();
                var reply = await ping.SendPingAsync(device.IPAddress, 100);
                if (reply.Status != IPStatus.Success)
                {
                    device.IsOnline = false;
                }
            }
        }

        public void AddDevice() 
        {
            if (string.IsNullOrWhiteSpace(deviceIP))
                return;

            string? ip = DeviceIP;
            Devices.Add(new NetworkDevices { IPAddress = ip, DeviceID = scanNetwork.count, IsOnline = false });

        }

        public async Task PingDeviceID()
        {
            if (string.IsNullOrWhiteSpace(deviceID))
                return;
            int id = Convert.ToInt32(DeviceID);
            string ip = Devices[id].IPAddress;
            using Ping ping = new Ping();
            var reply = await ping.SendPingAsync(ip, 100);
            if(reply.Status != IPStatus.Success)
            {
                Devices[id].IsOnline = false;
                return;
            }
        }

        public async Task RescanNetwork()
        {
            Task.Run(async () => await scanNetwork.ScanLocalNetwork(scanNetwork.baseip));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    // Change the access modifier of the `DeviceIP` field in the `MainPage` class to `public`
    // to resolve the CS0122 error.


}
