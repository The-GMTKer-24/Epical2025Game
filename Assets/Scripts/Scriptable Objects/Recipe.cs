using System;
using Scriptable_Objects;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Resources/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private ResourceQuantity[] inputs;
        [SerializeField] private ResourceQuantity[] outputs;
        [SerializeField] private float processingTime;
        [SerializeField] private float minimumTemperature = 0.0f;
            
        public ResourceQuantity[] Inputs => inputs;

        public float ProcessingTime => processingTime;

        public ResourceQuantity[] Outputs => outputs;
        
        public float MinimumTemperature => minimumTemperature;
    }
}

[Serializable]
public struct ResourceQuantity
{
    [SerializeField] private ResourceType type;
    [SerializeField] private int amount;

    public ResourceQuantity(ResourceType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public ResourceType Type => type;

    public int Amount => amount;
}