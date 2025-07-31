using UnityEngine;

namespace Scriptable_Objects
{
    public class ResourceType : ScriptableObject
    {
        [SerializeField] private string resourceName;
        
        public string ResourceName => resourceName;
    }
}