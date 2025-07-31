using Scriptable_Objects;

namespace Factory_Elements
{
    public interface IResourceStack
    {
        public ResourceType ResourceType { get; }

        public int Quantity { get; }

        public Resource QueryResource();

        public Resource TakeResource();

        public void AddResource(Resource resource);
    }
}