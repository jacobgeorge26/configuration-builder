using System.Text.Json;
using Source.Helpers;
using Source.Models.Enums;
using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class Cheese : ISettings, IOverridable<Cheese>
{
    public string? Name { get; set; }
    
    public decimal? Price { get; set; }
    
    public Milk? Milk { get; set; }
    
    public List<string>? Flavours { get; set; }

    public Farm? Origin { get; set; }
    
    public Cheese OverrideFromJson(string? json)
    {
        if(string.IsNullOrWhiteSpace(json))
            return this;
        
        var newSettings = JsonSerializer.Deserialize<Cheese>(json, JsonHelpers.JsonSerializerOptions);
        return Override(newSettings);
    }

    public Cheese Override(Cheese? newSettings)
    {
        if(newSettings == null)
            return this;
        
        Name = newSettings.Name ?? Name;
        Price = newSettings.Price ?? Price;
        Milk = newSettings.Milk ?? Milk;
        Flavours = newSettings.Flavours ?? Flavours;
        
        Origin ??= new Farm();
        Origin.Override(newSettings.Origin);
        
        return this;
    }
}