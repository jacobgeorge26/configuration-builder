using System.Reflection;

namespace SettingsBuilder.Services.Interfaces;

public interface IAssemblyService
{
    public string? GetEmbeddedResource(string name);

    public Assembly? GetEntryAssembly();

    public Assembly? GetCommonAssembly();
}