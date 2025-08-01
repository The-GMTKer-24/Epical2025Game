using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Factory_Elements.Blocks
{
    public class ItemSource : Block
    {
        [SerializeField] private ResourceSet resourcePool;
        [SerializeField] private int ticksBetweenGenerations;
        [SerializeField] private int capacity;
        
        private Queue<Resource> resources = new();
        private Queue<IFactoryElement> nextUpNeighbor;
        private int lastGenerationAt;
        
        // Update is called once per frame
        void FixedUpdate()
        {
            lastGenerationAt--;
            if (lastGenerationAt <= 0 && resources.Count < capacity)
            {
                lastGenerationAt = ticksBetweenGenerations;
                ResourceType toCreate = resourcePool.Resources[Random.Range(0, resourcePool.Resources.Length)];
                switch (toCreate)
                {
                    case ItemType item:
                        resources.Enqueue(new Item(item, Factory.ROOM_TEMPERATURE)); // Is celcius :D
                        break;
                    case FluidType fluid:
                        resources.Enqueue(new Fluid(fluid));
                        break;
                }
            }

            nextUpNeighbor = new Queue<IFactoryElement>();
            foreach (var neighbor in neighbors)
            {
                nextUpNeighbor.Enqueue(neighbor);
            }

            Queue<Resource> failedToExport = new Queue<Resource>();
            Queue<IFactoryElement> nextNeighborBatch = new Queue<IFactoryElement>();
            while (resources.Count > 0)
            {
                foreach (IFactoryElement neighbor in nextUpNeighbor.ToArray()) // There is a case where an item that will miss it's only output option but that's only for one tick and hopefully will not come up often
                {
                    if (neighbor.TryInsertResource(this, resources.Peek()))
                    {
                        resources.Dequeue();
                        break;
                    }
                    nextNeighborBatch.Enqueue(neighbor);
                }

                nextUpNeighbor = nextNeighborBatch;
                failedToExport.Enqueue(resources.Dequeue());
            }
            resources = failedToExport;
        }
        

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[]{};
        }
    }
}
