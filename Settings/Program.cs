using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SettingsBuilder.ConfigurationBuilder;
using SettingsBuilder.Helpers;
using SettingsBuilder.Models;
using SettingsBuilder.Services;
using SettingsBuilder.SettingsBuilder;

namespace SettingsBuilder;

sealed class Program
{
    private const string JsonFile = "settings.json";
    private const string EmbeddedFile = "embedded-settings.json";
    
    public static void Main(string[] args)
    {
        Console.WriteLine("---------------");
        Console.WriteLine("Results");
        Console.WriteLine("---------------");

        SettingsBuilder(args);
        ConfigurationBuilder(args);
    }

    private static void SettingsBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        var settings = new CheeseSettings()
            .AddJsonFile(JsonFile, new FileSystem())
            .AddEmbeddedResource(EmbeddedFile, new AssemblyService())
            .AddEnvironmentVariables(new EnvironmentService());
        
        var json = JsonSerializer.Serialize(settings, JsonHelpers.JsonSerializerOptions);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        builder.Configuration.AddJsonStream(stream);
        
        LoadServices(builder);
        using var host = builder.Build();
        
        var result = host.Services.GetService<IOptions<Cheese>>();
        ShowResults("Settings Builder", result?.Value);
    }
    
    private static void ConfigurationBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Configuration
            .AddJsonFileOverride(JsonFile, new FileSystem())
            .AddEmbeddedResource(EmbeddedFile, new AssemblyService())
            .AddEnvironmentVariablesOverride(new EnvironmentService());
        
        LoadServices(builder);
        using var host = builder.Build();
        
        var result = host.Services.GetService<IOptions<Cheese>>();
        ShowResults("Configuration Builder", result?.Value);
    }
    
    private static void LoadServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IConfiguration>(_ => builder.Configuration);
        // There are two main methods available to set up the options values:
        // 1 - services.AddOptions<OptionsClass>().BindConfiguration("configSection")
        // 2 - services.Configure<OptionsClass>(configuration.GetSection("configSection))
        // Both produce the same result, but 1 allows for more customizations
        // https://stackoverflow.com/questions/55762813/what-is-the-difference-between-services-configure-and-services-addoptionst
        builder.Services.AddOptions<Cheese>().BindConfiguration("Cheese");
    }

    private static void ShowResults(string jobName, object? result)
    {
        var json = JsonSerializer.Serialize(result, JsonHelpers.JsonSerializerOptions);
        Console.WriteLine(jobName);
        Console.WriteLine("---------------");
        Console.WriteLine(json);
        Console.WriteLine("---------------");
    }
}