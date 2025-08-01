using Scriptable_Objects;

namespace Factory_Elements
{
    public abstract class ResourceStack
    {
        public abstract ResourceType ResourceType { get; }

        public abstract int Quantity { get; }

        public abstract Resource QueryResource();

        public abstract Resource TakeResource();

        public abstract void AddResource(Resource resource);

        public static ResourceStack Create(ResourceType resourceType)
        {
            if (resourceType is ItemType itemType)
            {
                return new ItemStack(itemType);
            }
            else if (resourceType is FluidType fluidType)
            {
                return new FluidStack(fluidType);
            }
            throw new System.Exception("Invalid resource type");
        }
    }
}