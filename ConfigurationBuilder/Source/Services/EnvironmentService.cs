namespace Source.Services;

public class EnvironmentService : IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables() => Environment.GetEnvironmentVariables() as Dictionary<string, string?> ?? new();
}