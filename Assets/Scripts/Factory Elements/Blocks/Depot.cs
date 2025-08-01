using Factory_Elements.Settings;

namespace Factory_Elements.Blocks
{
    public class Depot : BufferBlock
    {
        private const int STORAGE = 500;

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (!buffers.ContainsKey(resource.ResourceType)) return true;
            return buffers[resource.ResourceType].Capacity > buffers[resource.ResourceType].Quantity;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!buffers.ContainsKey(resource.ResourceType))
            {
                buffers.Add(resource.ResourceType, new Buffer(STORAGE, resource.ResourceType, true, true));
                buffers[resource.ResourceType].AddResource(resource);
                return true;
            }

            if (buffers[resource.ResourceType].Capacity > buffers[resource.ResourceType].Quantity)
            {
                buffers[resource.ResourceType].AddResource(resource);
                return true;
            }
            
            return false;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}