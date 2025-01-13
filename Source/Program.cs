using System.IO.Abstractions;
using Source.Extensions;

namespace Source;

sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.LoadSettings(new FileSystem());
        
        builder.Services.AddScoped<IConfiguration>(_ => builder.Configuration);

        builder.Services.AddHostedService<Worker>();

        using var host = builder.Build();

        host.Run();
    }
}