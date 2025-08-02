using System;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    /// <summary>
    ///     A factory element with input and output buffers. Handles I/O handling fully.
    /// </summary>
    public abstract class BufferBlock : Block
    {
        private readonly List<ResourceType> inputtableResourceTypes = new();
        private readonly List<ResourceType> outputtableResourceTypes = new();
        private readonly List<ResourceType> resourceTypes = new();
        protected Dictionary<ResourceType, Buffer> buffers = new();

        private int currentOutputNeighborIndex;
        [SerializeField]
        private List<int> resourceTypeIndexPerNeighbor = new();
        
        

        // this code is EVIL. I'm so sorry
        protected virtual void FixedUpdate()
        {
            // Try to output to neighbors if possible
            var previousOutputNeighborIndex = currentOutputNeighborIndex;
            currentOutputNeighborIndex++;
            if (currentOutputNeighborIndex >= neighbors.Count) currentOutputNeighborIndex = 0;
            while (currentOutputNeighborIndex != previousOutputNeighborIndex)
            {
                var neighbor = neighbors[currentOutputNeighborIndex];

                // Trying this neighbor, either a resource can be output or cycle to the next
                {
                    Debug.Log("Runnning: "+ currentOutputNeighborIndex + "Count is: "+ resourceTypeIndexPerNeighbor.Count);

                    resourceTypeIndexPerNeighbor[currentOutputNeighborIndex]++;
                    if (resourceTypeIndexPerNeighbor[currentOutputNeighborIndex] >= outputtableResourceTypes.Count)
                        resourceTypeIndexPerNeighbor[currentOutputNeighborIndex] = 0;
                    var resourceIndex = resourceTypeIndexPerNeighbor[currentOutputNeighborIndex];
                    resourceIndex++;
                    if (resourceIndex >= outputtableResourceTypes.Count) resourceIndex = 0;

                    while (resourceIndex != resourceTypeIndexPerNeighbor[currentOutputNeighborIndex])
                    {
                        var resourceType = outputtableResourceTypes[resourceIndex];
                        var buffer = buffers[resourceType];
                        if (buffer.Quantity != 0)
                            if (neighbor.AcceptsResource(this, buffer.QueryResource()))
                            {
                                neighbor.TryInsertResource(this, buffer.TakeResource());
                                return;
                            }

                        resourceIndex++;
                        if (resourceIndex >= outputtableResourceTypes.Count) resourceIndex = 0;
                    }
                }

                currentOutputNeighborIndex++;
                if (currentOutputNeighborIndex >= neighbors.Count) currentOutputNeighborIndex = 0;
            }
        }

        protected void setBuffers(IEnumerable<Buffer> buffers)
        {
            resourceTypes.Clear();
            inputtableResourceTypes.Clear();
            outputtableResourceTypes.Clear();
            this.buffers.Clear();
            foreach (var buffer in buffers)
            {
                this.buffers.Add(buffer.ResourceType, buffer);
                resourceTypes.Add(buffer.ResourceType);
                if (buffer.CanAcceptInput) inputtableResourceTypes.Add(buffer.ResourceType);

                if (buffer.CanGiveOutput) outputtableResourceTypes.Add(buffer.ResourceType);
            }
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (!buffers.TryGetValue(resource.ResourceType, out var buffer)) return false;
            return buffer.CanAcceptInput && buffer.Quantity < buffer.Capacity;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (AcceptsResource(sender, resource))
            {
                buffers[resource.ResourceType].AddResource(resource);
                Debug.Log("got item" + resource.ResourceType.name);
                return true;
            }

            return false;
        }

        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);
            // resourceTypeIndexPerNeighbor = new List<int>(neighbors.Count);
            resourceTypeIndexPerNeighbor = new List<int>();
            for (int i = 0; i < neighbors.Count; i++)
            {
                resourceTypeIndexPerNeighbor.Add(0);
            }
            
            currentOutputNeighborIndex = 0;
        }
    }

    public class Buffer
    {
        public readonly int Capacity;
        public bool CanAcceptInput;
        public bool CanGiveOutput;
        private ResourceStack Stack;

        public Buffer(int capacity, ResourceType resourceType, bool canAcceptInput, bool canGiveOutput)
        {
            Capacity = capacity;
            CanAcceptInput = canAcceptInput;
            CanGiveOutput = canGiveOutput;
            Stack = ResourceStack.Create(resourceType);
        }

        public ResourceType ResourceType => Stack.ResourceType;
        public int Quantity => Stack.Quantity;

        public Resource QueryResource()
        {
            return Stack.QueryResource();
        }

        public Resource TakeResource()
        {
            return Stack.TakeResource();
        }

        public void AddResource(Resource resource)
        {
            if (Quantity >= Capacity) throw new Exception("Buffer is full");

            if (ResourceType != resource.ResourceType) throw new Exception("Cannot add different resource type");

            Stack.AddResource(resource);
        }

        public void CreateResources(int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                var newResource = Resource.fromType(ResourceType);
                AddResource(newResource);
            }
        }

        public void ConsumeResources(int quantity)
        {
            if (quantity > Quantity) throw new Exception("Not enough resources");
            for (var i = 0; i < quantity; i++) TakeResource();
        }

        public void Empty()
        {
            Stack = ResourceStack.Create(ResourceType);
        }
    }
}