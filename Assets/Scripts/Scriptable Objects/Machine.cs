using System.Collections.Generic;
using Factory_Elements;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Machine", menuName = "Factory/Machine", order = 0)]
    public class Machine : FactoryElementType
    {
        [SerializeField] private Recipe[] recipes;

        public Recipe[] Recipes => recipes;
    }
}