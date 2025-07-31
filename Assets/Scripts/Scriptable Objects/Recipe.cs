using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private Item[] inputs;
        [SerializeField] private Item[] outputs;
        [SerializeField] private float processingTime;
        
        public Item[] Inputs => inputs;

        public float ProcessingTime => processingTime;

        public Item[] Outputs => outputs;
    }
}