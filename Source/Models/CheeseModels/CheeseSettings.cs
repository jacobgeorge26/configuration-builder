using System.Text.Json;
using Source.Helpers;
using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class CheeseSettings : IFileModel, ISettings, IOverridable<CheeseSettings>
{
    public Cheese? Cheese { get; set; }
    
    public CheeseSettings OverrideFromJson(string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return this;
        
        var newSettings = JsonSerializer.Deserialize<CheeseSettings>(json, JsonHelpers.JsonSerializerOptions);
        return Override(newSettings);
    }

    public CheeseSettings Override(CheeseSettings? newSettings)
    {
        if(newSettings == null)
            return this;
        
        Cheese ??= new Cheese();
        Cheese.Override(newSettings.Cheese);
        
        return this;
    }
}