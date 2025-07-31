using Scriptable_Objects;

namespace Factory_Elements
{
    public class Fluid : Resource
    {
        public float amount;
        
        public Fluid(FluidType resourceType, float amount) : base(resourceType)
        {
            this.amount = amount;
        }
    }
}