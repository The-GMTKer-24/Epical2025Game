using Factory_Elements.Settings;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Depot : BufferBlock
    {
        [SerializeField] public float equalizationRate = 0.05f;
        public const int STORAGE = 500;

        public override Direction? Rotation => null;
        public override bool Rotate(Direction direction)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsRotation => false;

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (resource is Fluid) return false;
            if (!buffers.ContainsKey(resource.ResourceType)) return true;
            return buffers[resource.ResourceType].Capacity > buffers[resource.ResourceType].Quantity;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (resource is Item item)
            {
                if (!buffers.ContainsKey(item.ResourceType))
                {
                    buffers.Add(item.ResourceType, new Buffer(STORAGE, item.ResourceType, true, true));
                }

                if (buffers[item.ResourceType].Capacity > buffers[item.ResourceType].Quantity)
                {
                    buffers[item.ResourceType].AddResource(item);
                    item.EqualizationRate = equalizationRate;
                    return true;
                }

                return false;
            }

            return false;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}