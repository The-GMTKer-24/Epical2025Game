using Scriptable_Objects;

namespace Factory_Elements
{
    public class Fluid : Resource
    {
        protected Fluid(FluidType resourceType, float amount) : base(resourceType)
        {
        }
    }
}