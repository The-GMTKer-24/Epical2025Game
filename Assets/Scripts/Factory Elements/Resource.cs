using Scriptable_Objects;

namespace Factory_Elements
{
    public class Resource
    {
        public readonly ResourceType ResourceType;

        public Resource(ResourceType resourceType)
        {
            this.ResourceType = resourceType;
        }
    }
}