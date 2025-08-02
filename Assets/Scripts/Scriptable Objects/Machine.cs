using System.Collections.Generic;
using Factory_Elements;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Machine", menuName = "Factory/Machine", order = 0)]
    public class Machine : FactoryElementType
    {
        [SerializeField] private List<ResourceQuantity> itemCost;
        [SerializeField] private Recipe[] recipes;
        [SerializeField] private int capacity;

        public Recipe[] Recipes => recipes;

        public int Capacity => capacity;
        
        public List<ResourceQuantity> ItemCost => itemCost;
    }
}