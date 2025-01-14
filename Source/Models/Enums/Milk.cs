using System.Text.Json.Serialization;

namespace Source.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Milk
{
    Cow = 0,
    Goat = 1,
    Sheep = 2
}