using System.Text.Json.Serialization;

namespace Common.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Milk
{
    Cow = 0,
    Goat = 1,
    Sheep = 2
}