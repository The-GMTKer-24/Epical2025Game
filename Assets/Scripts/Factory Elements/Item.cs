using Scriptable_Objects;

namespace Factory_Elements
{
    public class Item : Resource
    {
        public float Temperature;

        public Item(ItemType resourceType, float temperature) : base(resourceType)
        {
            Temperature = temperature;
        }
    }
}