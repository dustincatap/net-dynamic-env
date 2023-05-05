using Microsoft.Extensions.Configuration;

namespace ConsoleApp;

public class Startup
{
    IConfigurationRoot Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

        Configuration = builder.Build();
    }
}