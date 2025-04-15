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
        //Dynamic list for automatic updating trough MVVM
        public ObservableCollection<NetworkDevices> Devices { get; set; } = new();

        //Command to bind to a Button (Command={Binding PingAllCommand})
        public ICommand PingAllCommand { get; }

        public MainViewModel()
        {

            PingAllCommand = new Command(async () => await PingAllDevices());
        }

        public async Task<bool> Ping(string ip) //function to send the actual ping and wait for a reply
        {
            try
            {
                using Ping ping = new();
                var reply = await ping.SendPingAsync(ip, 1000); //Each second, send a ping
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task PingAllDevices()
        {
            foreach (var device in Devices)
            {
                device.IsOnline = await Ping(device.IPAddress); // Call the function and wait for the result
            }
        }
    }
}
