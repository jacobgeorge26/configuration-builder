using System.IO.Abstractions;
using System.Text;
using SettingsBuilder.Extensions;
using SettingsBuilder.Services.Interfaces;

namespace SettingsBuilder.ConfigurationBuilder;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddJsonFileOverride(this IConfigurationBuilder builder, string path, IFileSystem fileSystem) 
        => builder.AddJson(fileSystem.ReadFile(path));

    public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, string name, IAssemblyService assemblyService) 
        => builder.AddJson(assemblyService.GetEmbeddedResource(name));

    public static IConfigurationBuilder AddEnvironmentVariablesOverride(this IConfigurationBuilder builder, IEnvironmentService envService)
    {
        var envVars = envService.GetEnvironmentVariables();
        return builder.AddInMemoryCollection(envVars);
    }
    
    private static IConfigurationBuilder AddJson(this IConfigurationBuilder builder, string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return builder;
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return builder.AddJsonStream(stream);
    }
}