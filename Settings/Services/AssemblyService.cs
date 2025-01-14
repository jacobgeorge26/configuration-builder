using System.Reflection;
using Common.Services.Interfaces;

namespace Common.Services;

public class AssemblyService : IAssemblyService
{
    public string? GetEmbeddedResource(string name)
    {
        var entryAssembly = GetEntryAssembly();
        var entryResource = GetResourceFromAssembly(entryAssembly, name);
        
        if(entryResource is not null)
            return entryResource;
        
        var commonAssembly = GetCommonAssembly();
        var commonResource = GetResourceFromAssembly(commonAssembly, name);
        return commonResource;
    }
    
    public Assembly? GetEntryAssembly() => Assembly.GetEntryAssembly();
    
    public Assembly? GetCommonAssembly() => Assembly.GetExecutingAssembly();
    
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