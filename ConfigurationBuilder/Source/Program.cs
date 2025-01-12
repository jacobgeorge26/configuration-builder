using Source.Helpers;
using Source.Services;

namespace Source;

sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.LoadSettings();

        builder.Services.AddHostedService<Worker>();

        using var host = builder.Build();

        host.Run();
    }
}