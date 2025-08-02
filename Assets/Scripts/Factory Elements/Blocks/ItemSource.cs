using System;
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

        private Queue<Resource> resources;

        private void Awake()
        {
            resources = new Queue<Resource>();
        }

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
            
            int cycles = 0;
            while (resources.Count > 0)
            {
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.TryInsertResource(this, resources.Peek()) && resources.Count > 0)
                    {
                        resources.Dequeue();
                    }
                }

                cycles++;
                if (cycles >= resources.Count * 2)
                {
                    break;
                }
            }
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

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            Debug.Log("Are you sure you should be able to deconstruct this");
            return new Dictionary<ResourceType, int>();
        }
    }
}