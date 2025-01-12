using System.IO.Abstractions;
using System.Text;

namespace Source.Extensions;

public static class ConfigurationBuilderExtensions
{
    
    public static IConfigurationBuilder ProcessJsonFile(this IConfigurationBuilder builder, string path, IFileSystem fileSystem)
    {
        var json = fileSystem.ReadFile(path);
        builder.AddJson(json);
        return builder;
    }
    
    public static IConfigurationBuilder AddJson(this IConfigurationBuilder builder, string? rawJson)
    {
        if(string.IsNullOrWhiteSpace(rawJson))
            return builder;
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawJson));
        builder.AddJsonStream(stream);
        
        return builder;
    }
}