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
    
    private readonly CheeseSettings _expectedSettings;

    private readonly string _jsonFilePath;
    private const string ResourceName = "embedded-settings.json";

    public SettingsExtensionsTests()
    {
        _fileSystem = new MockFileSystem();
        _assemblyMock = new Mock<Assembly>();
        _environmentServiceMock = new Mock<IEnvironmentService>();
        
        var basePath = _fileSystem.Directory.CreateDirectory(Path.Combine("root")).FullName;
        
        _expectedSettings = new CheeseSettings
        {
            Cheese = new Cheese
            {
                Name = "test",
            }
        };
        
        // Json file
        var json = JsonSerializer.Serialize(_expectedSettings);
        _jsonFilePath = Path.Combine(basePath, "settings.json");
        _fileSystem.AddFile(_jsonFilePath, new MockFileData(json));
        
        // Embedded resource
        const string assemblyName = "Tests";
        _assemblyMock.Setup(x => x.GetName()).Returns(new AssemblyName(assemblyName));
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        _assemblyMock.Setup(x => x.GetManifestResourceStream($"{assemblyName}.{ResourceName}")).Returns(stream);
        
        // Environment variables
        var envVars = new Dictionary<string, string?>
        {
            {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Name)}", _expectedSettings.Cheese?.Name}
        };
        _environmentServiceMock.Setup(x => x.GetEnvironmentVariables()).Returns(envVars);
    }

    [Fact]
    public void ProcessJson_LoadsValues()
    {
        var result = new CheeseSettings().AddJsonFile(_jsonFilePath, _fileSystem);
        
        Assert.Equal(_expectedSettings.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void ProcessEmbeddedResource_LoadsValues()
    {
        var result = new CheeseSettings().AddEmbeddedResource(ResourceName, _assemblyMock.Object);
        
        Assert.Equal(_expectedSettings.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void ProcessEnvironmentVariables_LoadsValues()
    {
        var result = new CheeseSettings().AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(_expectedSettings.Cheese?.Name, result.Cheese?.Name);
    }
}