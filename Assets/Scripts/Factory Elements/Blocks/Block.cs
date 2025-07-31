using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public abstract class Block : MonoBehaviour, IFactoryElement
    {
        protected int2 position;
        [SerializeField]
        protected FactoryElementType factoryElementType;
        public int2 Position
        {
            get => position;
            set => position = value;
        }
        protected List<IFactoryElement> neighbors = new List<IFactoryElement>();

        public FactoryElementType FactoryElementType { get => factoryElementType; }

        public void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            if (added)
            {
                neighbors.Add(newNeighbor);
            }
            else
            {
                neighbors.Remove(newNeighbor);
            }
        }
        
        public abstract bool AcceptsResource(IFactoryElement sender, Resource resource);
        public abstract bool TryInsertResource(IFactoryElement sender, Resource resource);
        public abstract ISetting[] GetSettings();
    }
}