namespace Common.Services.Interfaces;

public interface IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables();
}