using System;

namespace Factory_Elements.Settings
{
    [Serializable]
    public class ElementSettings<T> : ISetting
    {
        public ElementSettings(T value, string name, string description)
        {
            Value = value;
            Name = name;
            Description = description;
        }

        public T Value { get; set; }

        public string Name { get; }

        public string Description { get; }

        public object ValueUntyped
        {
            get => Value!;
            set
            {
                SettingUpdated?.Invoke();
                Value = (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public Type Type => typeof(T);

        public event Action SettingUpdated;
    }
}