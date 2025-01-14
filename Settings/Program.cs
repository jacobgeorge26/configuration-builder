using System.IO.Abstractions;
using SettingsBuilder.Services;
using SettingsBuilder.SettingsBuilder;

namespace SettingsBuilder;

sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.LoadSettings(new FileSystem());
        
        builder.Services.AddScoped<IConfiguration>(_ => builder.Configuration);

        builder.Services.AddHostedService<ResultsService>();

        using var host = builder.Build();

        host.Run();
    }
}