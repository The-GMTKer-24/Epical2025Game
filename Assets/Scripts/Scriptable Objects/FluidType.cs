using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Fluid", menuName = "Resources/Fluid")]
    public class FluidType : ResourceType
    {
        [SerializeField] private Color color;

        public Color Color => color;
    }
}