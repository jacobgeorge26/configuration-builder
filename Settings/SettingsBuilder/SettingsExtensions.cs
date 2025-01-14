using System.IO.Abstractions;
using System.Text.Json;
using SettingsBuilder.Extensions;
using SettingsBuilder.Helpers;
using SettingsBuilder.Models;
using SettingsBuilder.Services.Interfaces;

namespace SettingsBuilder.SettingsBuilder;

public static class SettingsExtensions
{
    public static T Override<T>(this T settings, T? newSettings)
        where T : ISettings
    {
        foreach (var property in settings.GetType().GetProperties())
        {
            var value = property.GetValue(settings);
            var newValue = property.GetValue(newSettings);
            
            if (newValue is null)
                continue;

            if (typeof(ISettings).IsAssignableFrom(property.PropertyType))
            {
                var settingsValue = (value ?? Activator.CreateInstance(property.PropertyType)) as ISettings;
                var newSettingsValue = settingsValue?.Override(newValue as ISettings);
                property.SetValue(settings, newSettingsValue);
            }
            else
            {
                property.SetValue(settings, newValue);
            }
        }
        
        return settings;
    }
    
    public static T AddJsonFile<T>(this T settings, string? path, IFileSystem fileSystem)
        where T : IFileModel, ISettings
    {
        if(string.IsNullOrWhiteSpace(path))
            return settings;

        var json = fileSystem.ReadFile(path);

        return settings.AddJson(json);
    }

    public static T AddEmbeddedResource<T>(this T settings, string name, IAssemblyService assemblyService)
        where T : IFileModel, ISettings
    {
        var json = assemblyService.GetEmbeddedResource(name);
        
        return settings.AddJson(json);
    }
    
    public static T AddEnvironmentVariables<T>(this T settings, IEnvironmentService envService)
        where T : IFileModel, ISettings
    {
        var envVars = envService.GetEnvironmentVariables();

        var config = new ConfigurationBuilder().AddInMemoryCollection(envVars).Build();
        
        var newSettings = Activator.CreateInstance<T>();
        config.Bind(newSettings);
        
        settings.Override(newSettings);
        
        return settings;
    }
    
    private static T AddJson<T>(this T settings, string? json)
        where T : IFileModel, ISettings
    {
        if(string.IsNullOrWhiteSpace(json))
            return settings;

        var newSettings = JsonSerializer.Deserialize<T>(json, JsonHelpers.JsonSerializerOptions);

        settings.Override(newSettings);
        return settings;
    }
}