using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements.Blocks
{
    public abstract class BufferBlock : Block
    {
        private readonly Dictionary<ResourceType, int> bufferCapacity = new Dictionary<ResourceType, int>();
        private readonly Dictionary<ResourceType, IResourceStack> buffers = new Dictionary<ResourceType, IResourceStack>();

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            foreach (KeyValuePair<ResourceType, IResourceStack> buffer in buffers)
            {
                if (resource.ResourceType == buffer.Key && buffer.Value.Quantity < bufferCapacity[buffer.Key])
                {
                    return true;
                }
            }
            return false;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            foreach (KeyValuePair<ResourceType, IResourceStack> buffer in buffers)
            {
                if (resource.ResourceType == buffer.Key && buffer.Value.Quantity < bufferCapacity[buffer.Key])
                {
                    buffer.Value.AddResource(resource);
                    return true;
                }
            }
            return false;
        }
    }
}