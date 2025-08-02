using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements
{
    public class Item : Resource
    {
        private float previousTemperature; // Celsius
        private float lastTemperatureSetTimestamp; // Seconds
        private float currentEqualizationRate; // % / second, must be between 0 and 1. Lower values maintain temperature longer.

        public float Temperature
        {
            get
            {
                float elapsedTime = Time.time - lastTemperatureSetTimestamp;
                float previousDeviation = previousTemperature - Factory.Instance.roomTemperature;
                float newDeviation = previousDeviation * Mathf.Pow(1.0f - currentEqualizationRate, elapsedTime);
                return newDeviation + Factory.Instance.roomTemperature;
            }
        }

        public float EqualizationRate
        {
            get => currentEqualizationRate;
            set
            {
                currentEqualizationRate = value;
                previousTemperature = Temperature;
                lastTemperatureSetTimestamp = Time.time;
            }
        }

        public Item(ItemType resourceType, float temperature) : base(resourceType)
        {
            previousTemperature = temperature;
            lastTemperatureSetTimestamp = Time.time;
        }
    }
}