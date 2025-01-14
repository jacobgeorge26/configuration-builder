using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Moq;
using SettingsBuilder.Models;
using SettingsBuilder.Services.Interfaces;
using SettingsBuilder.SettingsBuilder;
using Xunit;

namespace SettingsBuilder.Tests.SettingsBuilder;

public class SettingsExtensionsTests
{
    private readonly CheeseSettings _settings;
    private string _jsonPath = null!;
    private MockFileSystem _fileSystem = null!;
    private string _resourceName = null!;
    private Mock<IAssemblyService> _assemblyServiceMock = null!;
    private Mock<IEnvironmentService> _environmentServiceMock = null!;
    
    // ReSharper disable once ConvertConstructorToMemberInitializers
    public SettingsExtensionsTests()
    {
        _settings = new CheeseSettings
        {
            Cheese = new Cheese
            {
                Name = "Cheddar",
                Price = (decimal)0.99,
                Flavours = ["item"],
                Origin = new Origin { Location = "UK" }
            }
        };
    }
    
    private void SetupAddJsonFile(CheeseSettings settings) => SetupAddJsonFile(JsonSerializer.Serialize(settings));

    private void SetupAddJsonFile(string json)
    {
        _jsonPath = Path.Combine("folder", "settings.json");
        _fileSystem = new MockFileSystem();
        _fileSystem.AddFile(_jsonPath, new MockFileData(json));
    }
    
    private void SetupAddEmbeddedResource(CheeseSettings settings) => SetupAddEmbeddedResource(JsonSerializer.Serialize(settings));

    private void SetupAddEmbeddedResource(string json)
    {
        _resourceName = "embedded-settings.json";

        _assemblyServiceMock = new Mock<IAssemblyService>();
        _assemblyServiceMock.Setup(x => x.GetEmbeddedResource(_resourceName)).Returns(json);
    }
    
    private void SetupAddEnvironmentVariables(CheeseSettings settings)
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

        SetupAddEnvironmentVariables(envVars);
    }

    private void SetupAddEnvironmentVariables(Dictionary<string, string?> envVars)
    {
        _environmentServiceMock = new Mock<IEnvironmentService>();
        _environmentServiceMock.Setup(x => x.GetEnvironmentVariables()).Returns(envVars);
    }
    
    [Fact]
    public void AddJsonFile_LoadsValues()
    {
        SetupAddJsonFile(_settings);
        
        var result = new CheeseSettings().AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory]
    [InlineData("Gouda")]
    [InlineData("")]
    public void AddJsonFile_StringProperty_Overrides(string name)
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Name = name }
        };
        SetupAddJsonFile(expected);
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddJsonFile_NumberProperty_Overrides()
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Price = (decimal)1.99 }
        };
        SetupAddJsonFile(expected);
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddJsonFile_EnumerableProperty_Overrides(int itemCount)
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Flavours = []}};
        for (var i = 0; i < itemCount; i++)
        {
            expected.Cheese.Flavours.Add($"item{i}");
        }
        SetupAddJsonFile(_settings);
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
    }
    
    [Fact]
    public void AddJsonFile_NestedClassProperty_Overrides()
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Origin = new Origin{Location = "France"}}};
        SetupAddJsonFile(expected);
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddJsonFile_NullValueOverride_DoesNotOverride()
    {
        SetupAddJsonFile(new CheeseSettings());
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
        
    [Theory]
    [InlineData("Cow")]
    [InlineData("0")]
    public void AddJsonFile_SupportsEnums(string rawValue)
    {
        Assert.True(Enum.TryParse(rawValue, out Milk enumValue));
    
        var settings = new CheeseSettings
        {
            Cheese = new Cheese { Milk = enumValue }
        };
        var json = JsonSerializer.Serialize(settings).Replace(enumValue.ToString(), rawValue);

        SetupAddJsonFile(json);
        
        var result = _settings.AddJsonFile(_jsonPath, _fileSystem);
    
        Assert.Equal(enumValue, result.Cheese?.Milk);
    }
    
    [Fact]
    public void AddEmbeddedResource_LoadsValues()
    {
        SetupAddEmbeddedResource(_settings);
        
        var result = new CheeseSettings().AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory]
    [InlineData("Gouda")]
    [InlineData("")]
    public void AddEmbeddedResource_StringProperty_Overrides(string name)
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Name = name }
        };
        SetupAddEmbeddedResource(expected);
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEmbeddedResource_NumberProperty_Overrides()
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Price = (decimal)1.99 }
        };
        SetupAddEmbeddedResource(expected);
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddEmbeddedResource_EnumerableProperty_Overrides(int itemCount)
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Flavours = []}};
        for (var i = 0; i < itemCount; i++)
        {
            expected.Cheese.Flavours.Add($"item{i}");
        }
        SetupAddEmbeddedResource(_settings);
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
    }
    
    [Fact]
    public void AddEmbeddedResource_NestedClassProperty_Overrides()
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Origin = new Origin{Location = "France"}}};
        SetupAddEmbeddedResource(expected);
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEmbeddedResource_NullValueOverride_DoesNotOverride()
    {
        SetupAddEmbeddedResource(new CheeseSettings());
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
        
    [Theory]
    [InlineData("Cow")]
    [InlineData("0")]
    public void AddEmbeddedResource_SupportsEnums(string rawValue)
    {
        Assert.True(Enum.TryParse(rawValue, out Milk enumValue));
    
        var settings = new CheeseSettings
        {
            Cheese = new Cheese { Milk = enumValue }
        };
        var json = JsonSerializer.Serialize(settings).Replace(enumValue.ToString(), rawValue);

        SetupAddEmbeddedResource(json);
        
        var result = _settings.AddEmbeddedResource(_resourceName, _assemblyServiceMock.Object);
    
        Assert.Equal(enumValue, result.Cheese?.Milk);
    }
    
        [Fact]
    public void AddEnvironmentVariables_LoadsValues()
    {
        SetupAddEnvironmentVariables(_settings);
        
        var result = new CheeseSettings().AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory]
    [InlineData("Gouda")]
    [InlineData("")]
    public void AddEnvironmentVariables_StringProperty_Overrides(string name)
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Name = name }
        };
        SetupAddEnvironmentVariables(expected);
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEnvironmentVariables_NumberProperty_Overrides()
    {
        var expected = new CheeseSettings
        {
            Cheese = new Cheese { Price = (decimal)1.99 }
        };
        SetupAddEnvironmentVariables(expected);
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddEnvironmentVariables_EnumerableProperty_Overrides(int itemCount)
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Flavours = []}};
        for (var i = 0; i < itemCount; i++)
        {
            expected.Cheese.Flavours.Add($"item{i}");
        }
        SetupAddEnvironmentVariables(_settings);
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
    }
    
    [Fact]
    public void AddEnvironmentVariables_NestedClassProperty_Overrides()
    {
        var expected = new CheeseSettings{Cheese = new Cheese{Origin = new Origin{Location = "France"}}};
        SetupAddEnvironmentVariables(expected);
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEnvironmentVariables_NullValueOverride_DoesNotOverride()
    {
        SetupAddEnvironmentVariables(new CheeseSettings());
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
        
        Assert.Equal(_settings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(_settings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(_settings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(_settings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
        
    [Theory]
    [InlineData("Cow")]
    [InlineData("0")]
    public void AddEnvironmentVariables_SupportsEnums(string rawValue)
    {
        Assert.True(Enum.TryParse(rawValue, out Milk enumValue));
    
        var envVars = new Dictionary<string, string?>
        {
            {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Milk)}", rawValue}
        };
        SetupAddEnvironmentVariables(envVars);
        
        var result = _settings.AddEnvironmentVariables(_environmentServiceMock.Object);
    
        Assert.Equal(enumValue, result.Cheese?.Milk);
    }
}