using System;
using Scriptable_Objects;

namespace Factory_Elements
{
    public class FluidStack : ResourceStack
    {
        private readonly FluidType fluidType;
        private int quantity;

        public FluidStack(FluidType type)
        {
            fluidType = type;
            quantity = 0;
        }

        public override ResourceType ResourceType => fluidType;

        public override int Quantity => quantity;

        public override Resource QueryResource()
        {
            if (quantity == 0) return null;
            return new Fluid(fluidType);
        }

        public override Resource TakeResource()
        {
            quantity--;
            if (quantity < 0) throw new Exception("Cannot take a fluid that doesn't exist");
            return new Fluid(fluidType);
        }

        public override void AddResource(Resource resource)
        {
            if (resource is Fluid && resource.ResourceType == fluidType)
                quantity++;
            else
                throw new Exception("Cannot add an item or a different fluid to a fluid stack");
        }
    }
}