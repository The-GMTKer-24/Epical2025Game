using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Machine", menuName = "Factory/Machine", order = 0)]
    public class Machine : FactoryElement
    {
        [SerializeField] private Recipe[] recipes;
        [SerializeField] private int capacity;

        public Recipe[] Recipes => recipes;

        public int Capacity => capacity;
    }
}