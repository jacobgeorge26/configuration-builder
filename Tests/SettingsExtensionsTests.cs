using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Moq;
using Source.Extensions;
using Source.Models.CheeseModels;
using Source.Services;
using Xunit;

namespace Tests;

public class SettingsExtensionsTests
{
    private readonly MockFileSystem _fileSystem;
    private readonly Mock<Assembly> _assemblyMock;
    private readonly Mock<IEnvironmentService> _environmentServiceMock;

    private static readonly string JsonFilePath = Path.Combine("folder", "settings.json");

    private const string ResourceName = "embedded-settings.json";
    
    // ReSharper disable once ConvertConstructorToMemberInitializers
    public SettingsExtensionsTests()
    {
        _fileSystem = new MockFileSystem();
        _assemblyMock = new Mock<Assembly>();
        _environmentServiceMock = new Mock<IEnvironmentService>();
    }

    [Fact]
    public void AddJsonFile_LoadsValues()
    {
        var expected = CreateSettings("Cheddar");
        SetupJsonSettings(expected);
        
        var result = new CheeseSettings().AddJsonFile(JsonFilePath, _fileSystem);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEmbeddedResource_LoadsValues()
    {
        var expected = CreateSettings("Gouda");
        SetupEmbeddedResourceSettings(expected);
        
        var result = new CheeseSettings().AddEmbeddedResource(ResourceName, _assemblyMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEnvironmentVariables_LoadsValues()
    {
        var expected = CreateSettings("Brie");
        SetupEnvVarSettings(expected);
        
        var result = new CheeseSettings().AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddJsonFile_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        SetupJsonSettings(jsonSettings);
        SetupEmbeddedResourceSettings(resourceSettings);
        SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEmbeddedResource(ResourceName, _assemblyMock.Object)
            .AddEnvironmentVariables(_environmentServiceMock.Object)
            .AddJsonFile(JsonFilePath, _fileSystem);
        
        Assert.Equal(jsonSettings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(jsonSettings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(jsonSettings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(jsonSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddEmbeddedResource_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        SetupJsonSettings(jsonSettings);
        SetupEmbeddedResourceSettings(resourceSettings);
        SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEnvironmentVariables(_environmentServiceMock.Object)
            .AddJsonFile(JsonFilePath, _fileSystem)
            .AddEmbeddedResource(ResourceName, _assemblyMock.Object);
        
        Assert.Equal(resourceSettings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(resourceSettings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(resourceSettings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(resourceSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddEnvironmentVariables_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        SetupJsonSettings(jsonSettings);
        SetupEmbeddedResourceSettings(resourceSettings);
        SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEmbeddedResource(ResourceName, _assemblyMock.Object)
            .AddJsonFile(JsonFilePath, _fileSystem)
            .AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(envVarSettings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(envVarSettings.Cheese?.Price, result.Cheese?.Price);
        // Setting empty list is not supported via env var
        if(envVarSettings.Cheese?.Flavours?.Count > 0)
            Assert.Equal(envVarSettings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(envVarSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Fact]
    public void AddJsonFile_NullValues_DoNotOverride()
    {
        var expected = CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        SetupEnvVarSettings(expected);
        
        SetupJsonSettings(new CheeseSettings());
        
        var result = new CheeseSettings()
            .AddEnvironmentVariables(_environmentServiceMock.Object)
            .AddJsonFile(JsonFilePath, _fileSystem);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEmbeddedResource_NullValues_DoNotOverride()
    {
        var expected = CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        SetupJsonSettings(expected);
        
        SetupEmbeddedResourceSettings(new CheeseSettings());
        
        var result = new CheeseSettings()
            .AddJsonFile(JsonFilePath, _fileSystem)
            .AddEmbeddedResource(ResourceName, _assemblyMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEnvironmentVariables_NullValues_DoNotOverride()
    {
        var expected = CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        SetupEmbeddedResourceSettings(expected);
        
        SetupEnvVarSettings(new CheeseSettings());

        var result = new CheeseSettings()
            .AddEmbeddedResource(ResourceName, _assemblyMock.Object)
            .AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    // Test data for overriding values - empty or populated values only
    // No null values as this is not expected to override
    public static IEnumerable<object[]> OverrideData =>
    [
        // New values
        [
            CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"}), 
            CreateSettings("Gouda", (decimal?)5.23, ["resource"], new Farm{ Location = "Netherlands"}), 
            CreateSettings("Brie", (decimal?)3.48, ["envVar"], new Farm{ Location = "France"})
        ],
        // Number
        [
            CreateSettings("Cheddar", price: (decimal?)0.99), 
            CreateSettings("Gouda", price: -1), 
            CreateSettings("Brie", price: 0)
        ],
        // Enumerable
        [
            CreateSettings("Cheddar", tests: ["json"]), 
            CreateSettings("Gouda", tests: ["resource, resource2"]), 
            CreateSettings("Brie", tests: [])
        ],
        // Object
        [
            CreateSettings("Cheddar", origin: new Farm{ Location = "UK"}), 
            CreateSettings("Gouda", origin: new Farm{ Location = ""}), 
            CreateSettings("Brie", origin: new Farm{ Location = "  "})
        ],
    ];
    
    private static CheeseSettings CreateSettings(string name, decimal? price = null, List<string>? tests = null, Farm? origin = null) =>
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

    private void SetupJsonSettings(CheeseSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        _fileSystem.AddFile(JsonFilePath, new MockFileData(json));
    }
    
    private void SetupEmbeddedResourceSettings(CheeseSettings settings)
    {
        const string assemblyName = "Flavours";
        _assemblyMock.Setup(x => x.GetName()).Returns(new AssemblyName(assemblyName));
        
        var json = JsonSerializer.Serialize(settings);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        _assemblyMock.Setup(x => x.GetManifestResourceStream($"{assemblyName}.{ResourceName}")).Returns(stream);
    }
    
    private void SetupEnvVarSettings(CheeseSettings settings)
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
        
        _environmentServiceMock.Setup(x => x.GetEnvironmentVariables()).Returns(envVars);
    }
    
}