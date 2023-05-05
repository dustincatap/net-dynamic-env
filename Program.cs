using System.Diagnostics;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp;

public static class Program
{
    static void Main(string[] args)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        using var jsonStream = executingAssembly
#if PROD
            .GetManifestResourceStream("ConsoleApp.appsettings.prod.json");
#else
            .GetManifestResourceStream("ConsoleApp.appsettings.dev.json");
#endif
        var config = new ConfigurationBuilder()
            .AddJsonStream(jsonStream)
            .Build();

        // get `APP_SETTINGS_FILE` constant value
        var appSettingsFile = Environment.GetEnvironmentVariable("APP_SETTINGS_FILE");
        Console.WriteLine($"appSettingsFile: {appSettingsFile}");


        // check if debug or release
        var isDebug = executingAssembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
        Console.WriteLine("Is Debug: " + isDebug);
        
        // get `MySettingsAttribute` attribute value
        var entryAssembly = Assembly.GetEntryAssembly();
        var mySettingsAttribute = entryAssembly?.GetCustomAttribute<MySettingsAttribute>();
        Console.WriteLine(mySettingsAttribute?.AppEnv);


        var mySettings = config.Get<MySettings>();
        Console.WriteLine(mySettings.AppEnv);

        // var serviceCollection = new ServiceCollection();
        // var containerBuilder = new ContainerBuilder();
        // containerBuilder.Populate(serviceCollection);
        // var container = containerBuilder.Build();
        // var serviceProvider = new AutofacServiceProvider(container);
    }
}

public class MySettings
{
    public required string AppEnv { get; init; }

    public required string MyCustomSetting { get; init; }

    public required string MyOtherSetting { get; init; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class MySettingsAttribute : Attribute
{
    public MySettingsAttribute(string appEnv, string myCustomSetting, string myOtherSetting)
    {
        AppEnv = appEnv;
        MyCustomSetting = myCustomSetting;
        MyOtherSetting = myOtherSetting;
    }

    public string AppEnv { get; }

    public string MyCustomSetting { get; }

    public string MyOtherSetting { get; }
}