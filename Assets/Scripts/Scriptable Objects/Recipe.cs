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

        public ResourceQuantity[] Inputs => inputs;

        public float ProcessingTime => processingTime;

        public ResourceQuantity[] Outputs => outputs;
    }
}

[Serializable]
public struct ResourceQuantity
{
    [SerializeField] private ResourceType type;
    [SerializeField] private int amount;

    public ResourceType Type => type;

    public int Amount => amount;
}