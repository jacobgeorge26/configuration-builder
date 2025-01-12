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

    private static MemoryStream CreateJsonStream(string json) => new(Encoding.UTF8.GetBytes(json));
}