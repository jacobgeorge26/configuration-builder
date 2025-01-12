using System.Reflection;
using System.Text;

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