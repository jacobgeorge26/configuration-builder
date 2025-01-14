using System.Text.Json;
using Common.Models;
using SettingsBuilder.Extensions;
using SettingsBuilder.Models;
using SettingsBuilder.Tests.TestHelpers;
using Xunit;

namespace SettingsBuilder.Tests.Extensions;

public class SettingsExtensionsTests
{
    [Theory, CombinatorialData]
    public void SettingsExtensions_LoadsValues(SettingsSource source)
    {
        var expected = SettingsTestHelpers.CreateSettings("Cheddar");
        var result = GetResult(source, expected);
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
    }

    [Theory, CombinatorialData]
    public void SettingsExtensions_StringProperty_Overrides(SettingsSource baseSource, SettingsSource overrideSource, [CombinatorialValues("Gouda", "")] string name)
    {
        var baseSettings = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["item"], new Farm { Location = "UK" });
        var expected = SettingsTestHelpers.CreateSettings(name: name);

        var result = GetResult(overrideSource,  expected, GetResult(baseSource, baseSettings));
        
        Assert.Equal(expected.Cheese?.Name ?? baseSettings.Cheese?.Name, result.Cheese?.Name);
    }
    
    [Theory, CombinatorialData]
    public void SettingsExtensions_NumberProperty_Overrides(SettingsSource baseSource, SettingsSource overrideSource, [CombinatorialValues(3.49, 0)] decimal price)
    {
        var baseSettings = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["item"], new Farm { Location = "UK" });
        var expected = SettingsTestHelpers.CreateSettings(price: price);

        var result = GetResult(overrideSource,  expected, GetResult(baseSource, baseSettings));
        
        Assert.Equal(expected.Cheese?.Price ?? baseSettings.Cheese?.Price, result.Cheese?.Price);
    }
    
    [Theory, CombinatorialData]
    public void SettingsExtensions_EnumerableProperty_Overrides(SettingsSource baseSource, SettingsSource overrideSource, [CombinatorialValues(0, 1, 2)] int itemCount)
    {
        var baseSettings = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["item"], new Farm { Location = "UK" });
        var items = new List<string>();
        for (var i = 0; i < itemCount; i++)
        {
            items.Add($"item{i}");
        }

        // Setting empty list is not supported via env var
        if (overrideSource == SettingsSource.EnvironmentVariable && itemCount == 0)
            return;
        
        var expected = SettingsTestHelpers.CreateSettings(flavours: items);
    
        var result = GetResult(overrideSource,  expected, GetResult(baseSource, baseSettings));
        
        Assert.Equal(expected.Cheese?.Flavours ?? baseSettings.Cheese?.Flavours, result.Cheese?.Flavours);
    }
    
    [Theory, CombinatorialData]
    public void SettingsExtensions_ClassProperty_Overrides(SettingsSource baseSource, SettingsSource overrideSource, [CombinatorialValues("France", "")] string location)
    {
        var baseSettings = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["item"], new Farm { Location = "UK" });
        var expected = SettingsTestHelpers.CreateSettings(origin: new Farm { Location = location });

        var result = GetResult(overrideSource,  expected, GetResult(baseSource, baseSettings));
        
        Assert.Equal(expected.Cheese?.Origin?.Location ?? baseSettings.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
    
    [Theory, CombinatorialData]
    public void SettingsExtensions_NullValueOverride_DoesNotOverride(SettingsSource baseSource, SettingsSource overrideSource)
    {
        var expected = SettingsTestHelpers.CreateSettings("Cheddar", (decimal?)0.99, ["json"], new Farm{ Location = "UK"});
        
        var result = GetResult(overrideSource,  new CheeseSettings(), GetResult(baseSource, expected));
        
        Assert.Equal(expected.Cheese?.Name, result.Cheese?.Name);
        Assert.Equal(expected.Cheese?.Price, result.Cheese?.Price);
        Assert.Equal(expected.Cheese?.Flavours, result.Cheese?.Flavours);
        Assert.Equal(expected.Cheese?.Origin?.Location, result.Cheese?.Origin?.Location);
    }
        
    [Theory, CombinatorialData]
    public void SettingsExtensions_SupportsEnums(SettingsSource source, [CombinatorialValues("Cow", "0")] string value)
    {
        Assert.True(Enum.TryParse(value, out Milk expected));
    
        var settings = new CheeseSettings
        {
            Cheese = new Cheese { Milk = expected }
        };
        
        var json = JsonSerializer.Serialize(settings)
            .Replace(expected.ToString(), value);
        
        Func<CheeseSettings> func = source switch
        {
            SettingsSource.JsonFile => () =>
            {
                var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(json);
                return  new CheeseSettings().AddJsonFile(path, fileSystem);
            },
            SettingsSource.EmbeddedResource => () =>
            {
                var (name, assembly) = SettingsTestHelpers.SetupEmbeddedResourceSettings(json);
                return new CheeseSettings().AddEmbeddedResource(name, assembly);   
            },
            SettingsSource.EnvironmentVariable => () =>
            {
                var envService = SettingsTestHelpers.SetupEnvVarSettings(new Dictionary<string, string?>
                    {
                        {$"{nameof(CheeseSettings.Cheese)}:{nameof(CheeseSettings.Cheese.Milk)}", value}
                    });
        
                return new CheeseSettings().AddEnvironmentVariables(envService);
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    
        Assert.Equal(expected, func.Invoke().Cheese?.Milk);
    }
    
    private static CheeseSettings GetResult(SettingsSource source, CheeseSettings expected, CheeseSettings? baseSettings = null)
    {
        baseSettings ??= new CheeseSettings();
        Func<CheeseSettings> func = source switch
        {
            SettingsSource.JsonFile => () =>
            {
                var (path, fileSystem) = SettingsTestHelpers.SetupJsonSettings(expected);
                return baseSettings.AddJsonFile(path, fileSystem);
            },
            SettingsSource.EmbeddedResource => () =>
            {
                var (name, assemblyService) = SettingsTestHelpers.SetupEmbeddedResourceSettings(expected);
                return baseSettings.AddEmbeddedResource(name, assemblyService);
            },
            SettingsSource.EnvironmentVariable => () =>
            {
                var envService = SettingsTestHelpers.SetupEnvVarSettings(expected);
                return baseSettings.AddEnvironmentVariables(envService);
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
        return func.Invoke();
    }
}