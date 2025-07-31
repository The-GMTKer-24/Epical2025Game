using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private ItemType[] inputs;
        [SerializeField] private ItemType[] outputs;
        [SerializeField] private float processingTime;
        
        public ItemType[] Inputs => inputs;

        public float ProcessingTime => processingTime;

        public ItemType[] Outputs => outputs;
    }
}