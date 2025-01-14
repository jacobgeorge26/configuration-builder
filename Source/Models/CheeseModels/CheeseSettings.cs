using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class CheeseSettings : IFileModel, ISettings
{
    public Cheese? Cheese { get; set; }
}