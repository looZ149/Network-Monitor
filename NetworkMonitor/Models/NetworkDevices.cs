using System.ComponentModel;

//Model for MVVM

namespace NetworkMonitor.Models
{
    public class NetworkDevices : INotifyPropertyChanged
    {

        public int DeviceID { get; set; }
        public string? IPAddress { get; set; }
        public string? macAddress { get; set; }
        

        bool _isOnline;

        public bool IsOnline //Need a public var for Databinding
        {
            get => _isOnline;
            set
            {
                if (_isOnline == value)
                    return;
                _isOnline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        string? _manuFactor;

        public string ManuFactor
        {
            get => _manuFactor;
            set
            {
                if (_manuFactor == value) 
                    return;
                _manuFactor = value;
                OnPropertyChanged(nameof(ManuFactor));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
