using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements
{
    public class ItemStack : IResourceStack
    {
        private readonly ResourceType internalResourceType;
        private Queue<Item> items;
        
        public ResourceType ResourceType => internalResourceType;
        public int Quantity => items.Count;

        public Resource QueryResource()
        {
            return items.Peek();
        }

        public Resource TakeResource()
        {
            return items.Dequeue();
        }

        public void AddResource(Resource resource)
        {
            if (resource == null || resource.ResourceType != internalResourceType || resource is not Item)
            {
                throw new System.ArgumentException("Resource type mismatch");
            }
            items.Enqueue((Item) resource);
        }
    }
}