using System;

public interface ISetting
{
    string Name { get; }
    string Description { get; }
    object ValueUntyped { get; set; }
    Type Type { get; }
}
