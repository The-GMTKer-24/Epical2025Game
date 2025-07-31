using System;
using Scriptable_Objects;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Resources/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private ItemQuantity[] inputs;
        [SerializeField] private ItemQuantity[] outputs;
        [SerializeField] private float processingTime;

        public ItemQuantity[] Inputs => inputs;

        public float ProcessingTime => processingTime;

        public ItemQuantity[] Outputs => outputs;
    }
}

[Serializable]
public struct ItemQuantity
{
    [SerializeField] private ItemType type;
    [SerializeField] private int amount;

    public ItemType Type => type;

    public int Amount => amount;
}