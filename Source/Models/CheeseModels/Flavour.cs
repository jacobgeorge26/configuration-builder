using System.Text.Json;
using Source.Helpers;
using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class Flavour : ISettings, IOverridable<Flavour>
{
    public string? Description { get; set; }
    
    public Flavour OverrideFromJson(string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return this;
        
        var newSettings = JsonSerializer.Deserialize<Flavour>(json, JsonHelpers.JsonSerializerOptions);
        return Override(newSettings);
    }

    public Flavour Override(Flavour? newSettings)
    {
        if(newSettings == null)
            return this;
        
        Description = newSettings.Description ?? Description;
        return this;
    }
}