using System.Diagnostics.CodeAnalysis;
using Source.Extensions;
using Source.Models.CheeseModels;
using Tests.TestHelpers;
using Xunit;

namespace Tests.Extensions;

public class SettingsExtensionsTests
{
    [Fact]
    public void AddJsonFile_LoadsValues()
    {
        var expected = SettingsTestHelpers.CreateSettings("Cheddar");
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(expected);
        
        var result = new CheeseSettings().AddJsonFile(path, fileSystem);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEmbeddedResource_LoadsValues()
    {
        var expected = SettingsTestHelpers.CreateSettings("Gouda");
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(expected);
        
        var result = new CheeseSettings().AddEmbeddedResource(name, assembly);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Fact]
    public void AddEnvironmentVariables_LoadsValues()
    {
        var expected = SettingsTestHelpers.CreateSettings("Brie");
        var envService = SettingsTestHelpers.SetupEnvVarSettings(expected);
        
        var result = new CheeseSettings().AddEnvironmentVariables(envService);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddJsonFile_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(jsonSettings);
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(resourceSettings);
        var envService = SettingsTestHelpers.SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEmbeddedResource(name, assembly)
            .AddEnvironmentVariables(envService)
            .AddJsonFile(path, fileSystem);
        
        Assert.Equal(jsonSettings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(jsonSettings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(jsonSettings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(jsonSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddEmbeddedResource_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(jsonSettings);
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(resourceSettings);
        var envService = SettingsTestHelpers.SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEnvironmentVariables(envService)
            .AddJsonFile(path, fileSystem)
            .AddEmbeddedResource(name, assembly);
        
        Assert.Equal(resourceSettings.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(resourceSettings.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(resourceSettings.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(resourceSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }

    [Theory, MemberData(nameof(OverrideData))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public void AddEnvironmentVariables_OverridesCorrectly(CheeseSettings jsonSettings, CheeseSettings resourceSettings, CheeseSettings envVarSettings)
    {
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(jsonSettings);
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(resourceSettings);
        var envService = SettingsTestHelpers.SetupEnvVarSettings(envVarSettings);
        
        var result = new CheeseSettings()
            .AddEmbeddedResource(name, assembly)
            .AddJsonFile(path, fileSystem)
            .AddEnvironmentVariables(envService);
        
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
        var expected = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        var envService = SettingsTestHelpers.SetupEnvVarSettings(expected);
        
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(new CheeseSettings());
        
        var result = new CheeseSettings()
            .AddEnvironmentVariables(envService)
            .AddJsonFile(path, fileSystem);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEmbeddedResource_NullValues_DoNotOverride()
    {
        var expected = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(expected);
        
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(new CheeseSettings());
        
        var result = new CheeseSettings()
            .AddJsonFile(path, fileSystem)
            .AddEmbeddedResource(name, assembly);
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Fact]
    public void AddEnvironmentVariables_NullValues_DoNotOverride()
    {
        var expected = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(expected);
        
        var envService = SettingsTestHelpers.SetupEnvVarSettings(new CheeseSettings());

        var result = new CheeseSettings()
            .AddEmbeddedResource(name, assembly)
            .AddEnvironmentVariables(envService);
        
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
            SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"}), 
            SettingsTestHelpers.CreateSettings("Gouda", (decimal?)5.23, ["resource"], new Farm{ Location = "Netherlands"}), 
            SettingsTestHelpers.CreateSettings("Brie", (decimal?)3.48, ["envVar"], new Farm{ Location = "France"})
        ],
        // Number
        [
            SettingsTestHelpers.CreateSettings("Cheddar", price: (decimal?)0.99), 
            SettingsTestHelpers.CreateSettings("Gouda", price: -1), 
            SettingsTestHelpers.CreateSettings("Brie", price: 0)
        ],
        // Enumerable
        [
            SettingsTestHelpers.CreateSettings("Cheddar", tests: ["json"]), 
            SettingsTestHelpers.CreateSettings("Gouda", tests: ["resource, resource2"]), 
            SettingsTestHelpers.CreateSettings("Brie", tests: [])
        ],
        // Object
        [
            SettingsTestHelpers.CreateSettings("Cheddar", origin: new Farm{ Location = "UK"}), 
            SettingsTestHelpers.CreateSettings("Gouda", origin: new Farm{ Location = ""}), 
            SettingsTestHelpers.CreateSettings("Brie", origin: new Farm{ Location = "  "})
        ]
    ];
    

}