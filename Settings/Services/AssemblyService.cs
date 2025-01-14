using System.Reflection;
using SettingsBuilder.Services.Interfaces;

namespace SettingsBuilder.Services;

public class AssemblyService : IAssemblyService
{
    public string? GetEmbeddedResource(string name)
    {
        var entryAssembly = GetAssembly();
        return GetResourceFromAssembly(entryAssembly, name);
    }
    
    public Assembly? GetAssembly() => Assembly.GetEntryAssembly();
    
    private static string? GetResourceFromAssembly(Assembly? assembly, string name)
    {
        if(assembly is null)
            return null;
        
        var allResources = assembly.GetManifestResourceNames();

        var matchingResources = allResources.Where(x => x.EndsWith(name)).ToList();
        
        if(matchingResources.Count == 0)
            return null;
        
        var resource = matchingResources.First();
            
        var stream = assembly.GetManifestResourceStream(resource);
        if (stream is null)
            return null;
        
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}