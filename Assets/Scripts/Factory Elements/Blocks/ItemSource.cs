using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Factory_Elements.Blocks
{
    public class ItemSource : Block
    {
        [SerializeField] private ResourceSet resourcePool;
        [SerializeField] private int ticksBetweenGenerations;
        [SerializeField] private int capacity;
        private int lastGenerationAt;
        private Queue<IFactoryElement> nextUpNeighbor;

        private Queue<Resource> resources = new();

        // Update is called once per frame
        private void FixedUpdate()
        {
            lastGenerationAt--;
            if (lastGenerationAt <= 0 && resources.Count < capacity)
            {
                lastGenerationAt = ticksBetweenGenerations;
                var toCreate = resourcePool.Resources[Random.Range(0, resourcePool.Resources.Length)];
                switch (toCreate)
                {
                    case ItemType item:
                        resources.Enqueue(new Item(item, Factory.Instance.roomTemperature)); // Is celcius :D
                        break;
                    case FluidType fluid:
                        resources.Enqueue(new Fluid(fluid));
                        break;
                }
            }

            nextUpNeighbor = new Queue<IFactoryElement>();
            foreach (var neighbor in neighbors) nextUpNeighbor.Enqueue(neighbor);

            var failedToExport = new Queue<Resource>();
            var nextNeighborBatch = new Queue<IFactoryElement>();
            while (resources.Count > 0)
            {
                foreach (var neighbor in
                         nextUpNeighbor
                             .ToArray()) // There is a case where an item that will miss it's only output option but that's only for one tick and hopefully will not come up often
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
            return new ISetting[] { };
        }
    }
}