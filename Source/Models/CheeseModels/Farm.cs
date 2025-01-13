using System.Text.Json;
using Source.Helpers;
using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class Farm : ISettings, IOverridable<Farm>
{
    public string? Name { get; set; }

    public string? Location { get; set; }

    public Farm OverrideFromJson(string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return this;
        
        var newSettings = JsonSerializer.Deserialize<Farm>(json, JsonHelpers.JsonSerializerOptions);
        return Override(newSettings);
    }

    public Farm Override(Farm? newSettings)
    {
        if(newSettings == null)
            return this;
        
        Name = newSettings.Name ?? Name;
        Location = newSettings.Location ?? Location;
        return this;
    }
}