using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Common.Models;
using Common.Services.Interfaces;
using Moq;
using SettingsBuilder.Models;

namespace SettingsBuilder.Tests.TestHelpers;

internal static class SettingsTestHelpers
{
    public static CheeseSettings CreateSettings(string? name = null, decimal? price = null, List<string>? flavours = null, Farm? origin = null) =>
    new()
    {
        Cheese = new Cheese
        {
            Name = name,
            Price = price,
            Flavours = flavours,
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
    
    public static (string resourceName, IAssemblyService) SetupEmbeddedResourceSettings(CheeseSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        return SetupEmbeddedResourceSettings(json);
    }

    public static (string resourceName, IAssemblyService) SetupEmbeddedResourceSettings(string json)
    {
        const string resourceName = "embedded-settings.json";

        var assemblyMock = new Mock<IAssemblyService>();
        assemblyMock.Setup(x => x.GetEmbeddedResource(resourceName)).Returns(json);
        
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