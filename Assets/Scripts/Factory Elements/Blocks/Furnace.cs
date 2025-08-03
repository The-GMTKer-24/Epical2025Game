using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UI.Inventory;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Furnace : Block
    {
        protected ElementSettings<float> Temperature;
        [SerializeField] protected int capacity;
        private LinkedList<Item> items; // A queue

        private int neighborIndex;

        public override void Awake()
        {
            base.Awake();
            Temperature = new ElementSettings<float>(1500f, "Temperature", "The temperature that items will be raised to.", SettingType.Float);
            items = new LinkedList<Item>();
        }
        
        public void FixedUpdate()
        {
            if (items.Count == 0) return;
            
            if (neighbors[neighborIndex].TryInsertResource(this, items.Last.Value))
            {
                items.RemoveLast();
                neighborIndex++;
                return;
            }
            int previousIndex = neighborIndex;
            neighborIndex++;
            if (neighborIndex >= neighbors.Count) neighborIndex = 0;
            while (neighborIndex != previousIndex)
            {
                if (neighbors[neighborIndex].TryInsertResource(this, items.Last.Value))
                {
                    items.RemoveLast();
                    neighborIndex = previousIndex + 1;
                    if (neighborIndex >= neighbors.Count) neighborIndex = 0;
                    return;
                }
                neighborIndex++;
                if (neighborIndex >= neighbors.Count) neighborIndex = 0;
            }
            neighborIndex = previousIndex + 1;
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (resource is Item) return items.Count < capacity;
            return false;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            Item item = resource as Item;
            item.Temperature = Temperature.Value;
            item.EqualizationRate = 0.0f;
            items.AddFirst(item);
            return true;
        }

        public override Direction? Rotation
        {
            get => null;
            set => throw new System.NotImplementedException();
        }

        public override bool Rotate(Direction direction)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsRotation => false;

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { Temperature };
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            Dictionary<ResourceType, int> heldResources = new();
            foreach (Item item in items)
            {
                heldResources.TryAdd(item.ResourceType, 0);
                heldResources[item.ResourceType]++;
            }
            return heldResources;
        }
    }
}