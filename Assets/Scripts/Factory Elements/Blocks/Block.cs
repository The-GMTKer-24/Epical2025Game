using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public abstract class Block : IFactoryElement
    {
        protected int2 position;
        protected FactoryElementType factoryElementType;
        public int2 Position => position;
        
        public FactoryElementType FactoryElementType { get => factoryElementType; }

        public abstract void OnNeighborUpdate(IFactoryElement newNeighbor, bool added);
        public abstract bool AcceptsResource(IFactoryElement sender, Resource resource);
        public abstract bool TryInsertResource(IFactoryElement sender, Resource resource);
        public abstract ISetting[] GetSettings();
    }
}