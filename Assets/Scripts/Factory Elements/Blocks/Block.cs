using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public abstract class Block : MonoBehaviour, IFactoryElement
    {
        [SerializeField] protected FactoryElementType factoryElementType;

        protected List<IFactoryElement> neighbors = new();
        protected int2 position;

        public int2 Position
        {
            get => position;
            set => position = value;
        }

        public abstract Direction? Rotation { get; }
        public abstract bool Rotate(Direction direction);
        public abstract bool SupportsRotation { get; }

        public FactoryElementType FactoryElementType => factoryElementType;

        public virtual void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            if (added)
                neighbors.Add(newNeighbor);
            else
                neighbors.Remove(newNeighbor);
        }

        public abstract bool AcceptsResource(IFactoryElement sender, Resource resource);
        public abstract bool TryInsertResource(IFactoryElement sender, Resource resource);
        public abstract ISetting[] GetSettings();
        public abstract Dictionary<ResourceType, int> GetHeldResources();
    }
}