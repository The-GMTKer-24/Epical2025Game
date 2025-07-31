using UnityEngine;

namespace Scriptable_Objects
{
    public class ResourceType : ScriptableObject
    {
        [SerializeField] private Sprite icon;

        public Sprite Icon => icon;
    }
}