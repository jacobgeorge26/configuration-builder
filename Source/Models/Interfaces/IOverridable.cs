namespace Source.Models.Interfaces;

public interface IOverridable<T>
{
    public T OverrideFromJson(string? json);
    
    public T Override(T? newSettings);
}