using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using Source.Extensions;
using Source.Helpers;
using Source.Models;
using Source.Models.FileModels;
using Source.Services;
using Xunit;

namespace Tests;

public class ConfigurationTests
{
    private readonly IConfigurationBuilder _builder;

    private readonly Mock<Assembly> _assemblyMock;
    private readonly Mock<IEnvironmentService> _environmentServiceMock;

    private readonly CheeseSettings _settings;

    public ConfigurationTests()
    {
        var fileSystem = new MockFileSystem();
        _assemblyMock = new Mock<Assembly>();
        _environmentServiceMock = new Mock<IEnvironmentService>();
        
        var basePath = fileSystem.Directory.CreateDirectory(Path.Combine("root")).FullName;
        
        _settings = new CheeseSettings
        {
            Cheese = new Cheese
            {
                Name = "test",
            }
        };
        
        var jsonPath = Path.Combine(basePath, "settings.json");
        
        fileSystem.AddFile(jsonPath, new MockFileData(JsonSerializer.Serialize(_settings)));
        
        _builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
    }

    [Fact]
    public void ProcessJson_LoadsValues()
    {
        var json = JsonSerializer.Serialize(_settings, JsonHelpers.JsonSerializerOptions);
        _builder.ProcessJson(json);
        var configuration = _builder.Build();
        
        var result = new Cheese();
        configuration.GetSection(nameof(Cheese)).Bind(result);
        Assert.Equal(_settings.Cheese?.Name, result.Name);
    }
    
    [Fact]
    public void ProcessEmbeddedResource_LoadsValues()
    {
        const string fileName = "embedded-settings.json", assemblyName = "Tests";
        const string resourceName = $"{assemblyName}.{fileName}";

        var json = JsonSerializer.Serialize(_settings);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        _assemblyMock.Setup(x => x.GetName()).Returns(new AssemblyName(assemblyName));
        _assemblyMock.Setup(x => x.GetManifestResourceStream(resourceName)).Returns(stream);

         _builder.ProcessEmbeddedResource(fileName, _assemblyMock.Object);
         var configuration = _builder.Build();
            
        var result = new Cheese();
        configuration.GetSection(nameof(Cheese)).Bind(result);
        Assert.Equal(_settings.Cheese?.Name, result.Name);
    }
    
    [Fact]
    public void ProcessEnvironmentVariables_LoadsValues()
    {
        var envVars = new Dictionary<string, string?>
        {
            {$"{nameof(CheeseSettings.Cheese)}__{nameof(CheeseSettings.Cheese.Name)}", _settings.Cheese?.Name}
        };
        _environmentServiceMock.Setup(x => x.GetEnvironmentVariables()).Returns(envVars);
        
        _environmentServiceMock.Setup(x => x.Filter(It.IsAny<Dictionary<string, string?>>(), It.IsAny<string[]>()))
            .Returns((Dictionary<string, string?> vars, string[] filters) => new EnvironmentService().Filter(vars, filters));
        
        _environmentServiceMock.Setup(x => x.Parse<CheeseSettings>(It.IsAny<Dictionary<string, string?>>(), typeof(ISettings), It.IsAny<string?>()))
            .Returns((Dictionary<string, string?> vars, Type type, string filter) => new EnvironmentService().Parse<CheeseSettings>(vars, type, filter));
        
        _builder.ProcessEnvironmentVariables<CheeseSettings>(_environmentServiceMock.Object);
        var configuration = _builder.Build();
        
        var result = new Cheese();
        configuration.GetSection(nameof(Cheese)).Bind(result);
        Assert.Equal(_settings.Cheese?.Name, result.Name);
    }
}