namespace SettingsBuilder.Services.Interfaces;

public interface IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables();
}