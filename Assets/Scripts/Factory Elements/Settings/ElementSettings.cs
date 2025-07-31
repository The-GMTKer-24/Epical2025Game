using System;

public class ElementSettings<T> : ISetting
{
    public ElementSettings(T value, string name, string description)
    {
        Value = value;
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }
    public object ValueUntyped
    {
        get => Value!;
        set => Value = (T)Convert.ChangeType(value, typeof(T));
    }

    public Type Type => typeof(T);

    public T Value { get; set; }
}