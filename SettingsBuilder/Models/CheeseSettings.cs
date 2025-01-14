using Common.Models;

namespace SettingsBuilder.Models;

public class CheeseSettings : IFileModel, ISettings
{
    public Cheese? Cheese { get; set; }
}