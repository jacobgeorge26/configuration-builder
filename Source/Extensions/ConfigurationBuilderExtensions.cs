using System.Reflection;
using System.Text;
using System.Text.Json;
using Source.Helpers;
using Source.Models;
using Source.Models.FileModels;
using Source.Services;

namespace Source.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder ProcessJson(this IConfigurationBuilder builder, string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return builder;
        
        builder.AddJsonStream(CreateJsonStream(json));
        
        return builder;
    }

    public static IConfigurationBuilder ProcessEmbeddedResource(this IConfigurationBuilder builder, string name, Assembly? assembly)
    {
        var json = GetJsonFromEmbeddedResource(name, assembly);
        
        if(string.IsNullOrWhiteSpace(json))
            return builder;
        
        builder.AddJsonStream(CreateJsonStream(json));
        
        if(string.IsNullOrWhiteSpace(json))
            return builder;
        
        return builder;
    }
    
    public static IConfigurationBuilder ProcessEnvironmentVariables<T>(this IConfigurationBuilder builder, IEnvironmentService envService)
        where T : IFileModel
    {
        var envVars = envService.GetEnvironmentVariables();
        
        foreach (var entry in envService.Filter(envVars, []))
        {
            envVars.Add(entry.Key, entry.Value);
        }
    
        var settings = envService.Parse<T>(envVars, typeof(ISettings), null);
        
        var json = JsonSerializer.Serialize(settings, JsonHelpers.JsonSerializerOptions);
        
        builder.AddJsonStream(CreateJsonStream(json));
        
        return builder;
    }

    private static MemoryStream CreateJsonStream(string json) => new(Encoding.UTF8.GetBytes(json));
    
    private static string? GetJsonFromEmbeddedResource(string fileName, Assembly? assembly)
    {
        if(assembly is null)
            return null;
        
        var fullResourceName = $"{assembly.GetName().Name}.{fileName}";
    
        var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream is null)
            return null;
    
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}