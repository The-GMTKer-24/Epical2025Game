using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements.Blocks
{
    public abstract class BufferBlock : Block
    {
        protected readonly Dictionary<ResourceType, int> bufferCapacity = new();
        protected readonly Dictionary<ResourceType, IResourceStack> buffers = new();
        protected readonly Dictionary<ResourceType, bool> canAcceptInput = new();
        protected readonly Dictionary<ResourceType, bool> canGiveOutput = new();

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            foreach (var buffer in buffers)
                if (resource.ResourceType == buffer.Key && buffer.Value.Quantity < bufferCapacity[buffer.Key] && canAcceptInput[buffer.Key])
                    return true;
            return false;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            foreach (var buffer in buffers)
                if (resource.ResourceType == buffer.Key && buffer.Value.Quantity < bufferCapacity[buffer.Key] && canAcceptInput[buffer.Key])
                {
                    buffer.Value.AddResource(resource);
                    return true;
                }

            return false;
        }
        
        // TODO Output to neighbors
    }
}