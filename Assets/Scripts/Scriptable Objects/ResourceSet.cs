using System;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Resource Set", menuName = "Sets/Resource Set", order = 0)]
    public class ResourceSet : ScriptableObject
    {
        [SerializeField] private ResourceType[] resourceTypes = Array.Empty<ResourceType>();

        public ResourceType[] Resources => resourceTypes;
    }
}