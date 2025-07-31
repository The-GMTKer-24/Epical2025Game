using Unity.Mathematics;

namespace Factory_Elements.Blocks
{
    public abstract class Block : IFactoryElement
    {
        private int2 position;
        public int2 Position => position;
        private int2 size;
        public int2 Size => size;
        
        public abstract void OnNeighborUpdate(IFactoryElement newNeighbor, bool added);
        public abstract bool AcceptsResource(IFactoryElement sender, Resource resource);
        public abstract bool TryInsertResource(IFactoryElement sender, Resource resource);
        public abstract ISetting[] GetSettings();
    }
}