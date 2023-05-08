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
        // Get appsettings.json file name
        var entryAssembly = Assembly.GetEntryAssembly();
        var mySettingsAttribute = entryAssembly?.GetCustomAttribute<AppSettingsFileAttribute>();
        Console.WriteLine($"AppSettingsFile Name: " + mySettingsAttribute?.Name);

        // Load appsettings.json file as part of configuration
        var executingAssembly = Assembly.GetExecutingAssembly();
        using (var jsonStream = executingAssembly.GetManifestResourceStream($"ConsoleApp.{mySettingsAttribute?.Name}"))
        {
            if (jsonStream == null)
            {
                throw new Exception($"Could not find appsettings.json file: {mySettingsAttribute?.Name}");
            }

            var config = new ConfigurationBuilder()
                .AddJsonStream(jsonStream)
                .Build();
            
            var mySettings = config.Get<MySettings>();
            Console.WriteLine($"AppEnv: " + mySettings?.AppEnv);
        }

        // check if debug or release
        var isDebug = executingAssembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
        Console.WriteLine("Is Debug: " + isDebug);



        var serviceCollection = new ServiceCollection();
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(serviceCollection);
        var container = containerBuilder.Build();
        var serviceProvider = new AutofacServiceProvider(container);
    }
}

public class MySettings
{
    public required string AppEnv { get; init; }

    public required string MyCustomSetting { get; init; }

    public required string MyOtherSetting { get; init; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class AppSettingsFileAttribute : Attribute
{
    public AppSettingsFileAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}