using System.IO;
using System.Windows;
using FilesCombiner.Configuration;
using FilesCombiner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FilesCombiner
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;
        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string logDirectory = Path.Combine(userProfilePath, "Downloads/FilesCombiner_logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFileName = $"FilesCombiner_log_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
            string fullLogFilePath = Path.Combine(logDirectory, logFileName);

            return Host.CreateDefaultBuilder()
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .MinimumLevel.Warning()
                        .MinimumLevel.Override("FilesCombiner", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.File(
                            formatter: new CustomLogFormatter(),
                            path: fullLogFilePath,
                            rollingInterval: RollingInterval.Infinite
                        );
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<FileCombinerService>();
                    services.AddScoped<MainWindow>();
                });
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            using (_host)
            {
                await _host.StopAsync();
                Log.CloseAndFlush();
            }
        }
    }

}
