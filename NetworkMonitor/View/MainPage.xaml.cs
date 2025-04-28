using NetworkMonitor.Functions;
using System.Security.Cryptography.X509Certificates;

namespace NetworkMonitor;

public partial class MainPage : ContentPage
{
    ScanNetwork scanNetwork = new();

    public MainPage()
    {
        InitializeComponent();
    }

}

