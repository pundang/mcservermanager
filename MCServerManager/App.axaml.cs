using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MCServerManager.ViewModels;
using MCServerManager.Views;
using MCServerManager.Services;
using System;
using Serilog;
using System.IO;

namespace MCServerManager;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    readonly ServerProcessService ServerProcessService = new();
    readonly StorageManagerService StorageManagerService = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Server process graceful shutdown
            desktop.ShutdownRequested += async (_, e) =>
            {
                if (ServerProcessService.Info.Status is Models.ServerStatus.Running or Models.ServerStatus.Starting)
                {
                    e.Cancel = true; // pause shutdown
                    await ServerProcessService.StopAsync();
                    desktop.Shutdown(); // after stopping the process shut it down
                }
            };

            // Flush the logs
            desktop.Exit += (_, _) => Log.CloseAndFlush();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = mainViewModel };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // LOGGER
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(StorageManagerService.RootDirectory, $"{date}.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        services.AddLogging(builder => builder.AddSerilog(Log.Logger));

        // APP SERVICES
        services.AddSingleton<IServerProcessService>(ServerProcessService);
        services.AddSingleton<IStorageManagerService>(StorageManagerService);
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<IVersionManagerService, VersionManagerService>();
        services.AddSingleton<IServerSettingsService, ServerSettingsService>();
        services.AddTransient<MainViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<ConsoleViewModel>();
        services.AddTransient<ServerSettingsViewModel>();
        services.AddSingleton<VersionsViewModel>();
        services.AddSingleton<EulaViewModel>();
    }
}
