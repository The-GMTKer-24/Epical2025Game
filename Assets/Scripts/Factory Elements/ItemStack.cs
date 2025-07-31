using System;
using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements
{
    public class ItemStack : IResourceStack
    {
        private Queue<Item> items;

        public ResourceType ResourceType { get; }

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
            if (resource == null || resource.ResourceType != ResourceType || resource is not Item item)
                throw new ArgumentException("Resource type mismatch");
            items.Enqueue(item);
        }
    }
}