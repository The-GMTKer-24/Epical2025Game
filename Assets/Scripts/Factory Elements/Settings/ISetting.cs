using System;
using UI.Inventory;

namespace Factory_Elements.Settings
{
    public interface ISetting
    {
        string Name { get; }
        string Description { get; }
        object ValueUntyped { get; set; }
        Type Type { get; }
        SettingType SettingType { get; }
    }
}