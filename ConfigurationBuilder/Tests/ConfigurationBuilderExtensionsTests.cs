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

    private readonly Settings _settings;

    public ConfigurationTests()
    {
        var fileSystem = new MockFileSystem();
        _assemblyMock = new Mock<Assembly>();
        _environmentServiceMock = new Mock<IEnvironmentService>();
        
        var basePath = fileSystem.Directory.CreateDirectory(Path.Combine("root")).FullName;
        
        _settings = new Settings
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
}