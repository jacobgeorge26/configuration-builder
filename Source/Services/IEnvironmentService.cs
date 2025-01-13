namespace Source.Services;

public interface IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables();

}