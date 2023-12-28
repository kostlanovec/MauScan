using Camera.MAUI;
using MauScan.Data;
using Microsoft.Extensions.Logging;

namespace MauScan
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "qrdatabase.db");

            // Register MainPage and Database services
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<Database>(s, dbPath));
            builder.Services.AddSingleton(typeof(IFilePicker), FilePicker.Default);
            // Additional configuration or services registration can go here

            return builder.Build();
        }
    }
}