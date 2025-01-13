using System.IO.Abstractions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Source.Helpers;
using Source.Models.CheeseModels;
using Source.Services;

namespace Source.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void LoadSettings(this IHostApplicationBuilder builder, IFileSystem fileSystem)
    {
        var settings = new CheeseSettings()
            .AddJsonFile("Inputs/settings.json", fileSystem)
            .AddEmbeddedResource("Inputs.embedded-settings.json", Assembly.GetEntryAssembly())
            .AddEnvironmentVariables(new EnvironmentService());

        builder.BindCheeseSettings(settings);
    }
    
    // There are two main methods available to set up the options values:
    // 1 - services.AddOptions<OptionsClass>().BindConfiguration("configSection")
    // 2 - services.Configure<OptionsClass>(configuration.GetSection("configSection))
    // Both produce the same result, but 1 allows for more customizations
    // https://stackoverflow.com/questions/55762813/what-is-the-difference-between-services-configure-and-services-addoptionst
    public static void BindCheeseSettings(this IHostApplicationBuilder applicationBuilder, CheeseSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, JsonHelpers.JsonSerializerOptions);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        applicationBuilder.Configuration.AddJsonStream(stream);
        
        applicationBuilder.Services.AddOptions<Cheese>().BindConfiguration("Cheese");
    }
}