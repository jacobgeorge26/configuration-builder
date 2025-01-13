using System.Collections;

namespace Source.Services;

public class EnvironmentService : IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables()
    {
        var vars = Environment.GetEnvironmentVariables();
        var dict = new Dictionary<string, string?>();
        foreach (DictionaryEntry entry in vars)
        {
            var key = entry.Key.ToString();
            if(key is null)
                continue;
            
            key = key.Replace("__", ":");
            
            dict.Add(key, entry.Value?.ToString());
        }
        
        return dict;
    }
}