namespace Source.Services;

public interface IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables();

    public Dictionary<string, string?> Filter(Dictionary<string, string?> allEnvVars, string[] filters);
    
    public T? Parse<T>(Dictionary<string, string?> allEnvVars, Type propertyTypeFilter, string? configSection);
}