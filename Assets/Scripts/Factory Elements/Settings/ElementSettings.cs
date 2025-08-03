using System;
using UI.Inventory;

namespace Factory_Elements.Settings
{
    [Serializable]
    public class ElementSettings<T> : ISetting
    {
        public ElementSettings(T value, string name, string description, SettingType settingType)
        {
            Value = value;
            Name = name;
            Description = description;
            SettingType = settingType;
        }

        public T Value { get; set; }

        public string Name { get; }

        public string Description { get; }

        public object ValueUntyped
        {
            get => Value!;
            set
            {
                Value = (T)Convert.ChangeType(value, typeof(T));
                SettingUpdated?.Invoke();
            }
        }

        public Type Type => typeof(T);
        public SettingType SettingType { get; }

        public event Action SettingUpdated;
    }
}