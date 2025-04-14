using NetworkMonitor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkMonitor.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<NetworkDevices> Devices { get; set; } = new();

        public ICommand PingAllCommand { get; }

        public MainViewModel()
        {

        }
    }
}
