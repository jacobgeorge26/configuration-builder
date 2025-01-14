namespace SettingsBuilder.Models;

public class Cheese : IOverridable
{
    public string? Name { get; set; }
    
    public decimal? Price { get; set; }
    
    public Milk? Milk { get; set; }
    
    public Origin? Origin { get; set; }
    
    public List<string>? Flavours { get; set; }
    
    public List<Note>? Notes { get; set; }
}