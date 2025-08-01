using System;
using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements
{
    public class ItemStack : ResourceStack
    {
        private Queue<Item> items;
        private ItemType itemType;

        public override ResourceType ResourceType { get => itemType; }

        public override int Quantity => items.Count;

        public override Resource QueryResource()
        {
            return items.Peek();
        }

        public override Resource TakeResource()
        {
            return items.Dequeue();
        }

        public override void AddResource(Resource resource)
        {
            if (resource == null || resource.ResourceType != ResourceType || resource is not Item item)
                throw new ArgumentException("Resource type mismatch");
            items.Enqueue(item);
        }

        public ItemStack(ItemType type)
        {
            itemType = type;
            items = new Queue<Item>();
        }
    }
}