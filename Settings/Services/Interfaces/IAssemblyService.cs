using System.Reflection;

namespace Common.Services.Interfaces;

public interface IAssemblyService
{
    public string? GetEmbeddedResource(string name);

    public Assembly? GetEntryAssembly();

    public Assembly? GetCommonAssembly();
}