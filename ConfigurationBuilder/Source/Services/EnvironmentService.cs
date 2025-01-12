using System.Text.Json;
using Source.Helpers;

namespace Source.Services;

public class EnvironmentService : IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables() => Environment.GetEnvironmentVariables() as Dictionary<string, string?> ?? new();
    
    public Dictionary<string, string?> Filter(Dictionary<string, string?> allEnvVars, string[] filters)
    {
        var filteredEnvVars = new Dictionary<string, string?>();
        foreach (var (key, value) in allEnvVars)
        {
            foreach (var filter in filters)
            {
                if (!key.StartsWith(filter, StringComparison.OrdinalIgnoreCase)) continue;
                
                filteredEnvVars.Add(key.Replace(filter, string.Empty).Trim('_').Trim(':'), value);
            }
        }
    
        return filteredEnvVars;
    }
    
    public T? Parse<T>(Dictionary<string, string?> allEnvVars, Type propertyTypeFilter, string? configSection = null) => (T?)Parse(allEnvVars, typeof(T), propertyTypeFilter, configSection);
    
    private object? Parse(Dictionary<string, string?> allEnvVars, Type targetType, Type propertyTypeFilter, string? configSection = null)
    {
        var filteredEnvVars = configSection is null ? allEnvVars : Filter(allEnvVars, [configSection]);
        
        var json = JsonSerializer.Serialize(filteredEnvVars, JsonHelpers.JsonSerializerOptions);
        
        var settingsObject = JsonSerializer.Deserialize(json, targetType, JsonHelpers.JsonSerializerOptions);
        settingsObject ??= Activator.CreateInstance(targetType);
        
        if(settingsObject is null)
            return null;
        
        foreach(var property in settingsObject.GetType().GetProperties())
        {
            if(!propertyTypeFilter.IsAssignableFrom(property.PropertyType)) continue;
            
            var nestedSettings = Parse(filteredEnvVars, property.PropertyType, propertyTypeFilter, property.Name);
            if(nestedSettings is null)
                continue;
            
            property.SetValue(settingsObject, nestedSettings);
        }
        
        return settingsObject;
    }
}