using System.IO.Abstractions;
using Source.Extensions;
using Source.Models;

namespace Source.Helpers;

public static class SettingsHelpers
{
    public static void LoadSettings(this IHostApplicationBuilder builder, IFileSystem fileSystem)
    {
        builder.Configuration
            .ProcessJson(fileSystem.ReadFile("appsettings.json"));
        
        builder.BindSettings();
    }
    
    // There are two main methods available to set up the options values:
    // 1 - services.AddOptions<OptionsClass>().BindConfiguration("configSection")
    // 2 - services.Configure<OptionsClass>(configuration.GetSection("configSection))
    // Both produce the same result, but 1 allows for more customizations
    // https://stackoverflow.com/questions/55762813/what-is-the-difference-between-services-configure-and-services-addoptionst
    public static void BindSettings(this IHostApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.AddScoped<IConfiguration>(_ => applicationBuilder.Configuration);
        
        applicationBuilder.Services.AddOptions<Cheese>().BindConfiguration(nameof(Cheese));
    }
}