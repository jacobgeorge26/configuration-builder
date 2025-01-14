using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Moq;
using Source.Models.CheeseModels;
using Source.Services;

namespace Tests.TestHelpers;

internal class SettingsTestHelpers
{
    public static CheeseSettings CreateSettings(string name, decimal? price = null, List<string>? tests = null, Farm? origin = null) =>
    new()
    {
        Cheese = new Cheese
        {
            Name = name,
            Price = price,
            Flavours = tests,
            Origin = origin
        }
    };

    public static (string path, MockFileSystem fileSystem) SetupJsonSettings(CheeseSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        return SetupJsonSettings(json);
    }

    public static (string path, MockFileSystem fileSystem) SetupJsonSettings(string json)
    {
        var path = Path.Combine("folder", "settings.json");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(path, new MockFileData(json));
        return (path, fileSystem);
    }
    
    public static (string resourceName, Assembly) SetupEmbeddedResourceSettings(CheeseSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        return SetupEmbeddedResourceSettings(json);
    }

    public static (string resourceName, Assembly) SetupEmbeddedResourceSettings(string json)
    {
        const string assemblyName = "Flavours", resourceName = "embedded-settings.json";

        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetName()).Returns(new AssemblyName(assemblyName));
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        assemblyMock.Setup(x => x.GetManifestResourceStream($"{assemblyName}.{resourceName}")).Returns(stream);

        return (resourceName, assemblyMock.Object);
    }
    
    public static IEnvironmentService SetupEnvVarSettings(CheeseSettings settings)
    {
        var envVars = new Dictionary<string, string?>
        {
            {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Name)}", settings.Cheese?.Name},
            {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Price)}", settings.Cheese?.Price?.ToString()},
            {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Origin)}:{nameof(CheeseSettings.Cheese.Origin.Location)}", settings.Cheese?.Origin?.Location}
        };
        
        foreach (var (value, index) in settings.Cheese?.Flavours?.Select((value, i) => ( value, i )) ?? [])
        {
            envVars.Add($"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Flavours)}:{index}", value);
        }

        return SetupEnvVarSettings(envVars);
    }

    public static IEnvironmentService SetupEnvVarSettings(Dictionary<string, string?> envVars)
    {
        var environmentServiceMock = new Mock<IEnvironmentService>();
        environmentServiceMock.Setup(x => x.GetEnvironmentVariables()).Returns(envVars);
        return environmentServiceMock.Object;
    }
}