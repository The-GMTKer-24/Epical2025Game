using Scriptable_Objects;

namespace Factory_Elements
{
    public class Item : Resource
    {
        public float Temperature;

        public Item(ResourceType resourceType, float temperature) : base(resourceType)
        {
            this.Temperature = temperature;
        }
    }
}