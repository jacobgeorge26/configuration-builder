using System.IO.Abstractions;
using Source.Helpers;

namespace Source;

sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.LoadSettings(new FileSystem());

        builder.Services.AddHostedService<Worker>();

        using var host = builder.Build();

        host.Run();
    }
}