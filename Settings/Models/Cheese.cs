namespace SettingsBuilder.Models;

public class Cheese : ISettings
{
    public string? Name { get; set; }
    
    public decimal? Price { get; set; }
    
    public Milk? Milk { get; set; }
    
    public List<string>? Flavours { get; set; }

    public Farm? Origin { get; set; }
}