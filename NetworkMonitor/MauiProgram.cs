using Microsoft.Extensions.Logging;
using NetworkMonitor.Functions;


namespace NetworkMonitor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		ScanNetwork scan = new();
		scan.GetDevices();
        


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();

	}
}
