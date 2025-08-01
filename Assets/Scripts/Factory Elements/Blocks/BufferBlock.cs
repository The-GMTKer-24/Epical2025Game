using System;
using System.Collections.Generic;
using Scriptable_Objects;

namespace Factory_Elements.Blocks
{
    /// <summary>
    /// A factory element with input and output buffers. Handles I/O handling fully.
    /// </summary>
    public abstract class BufferBlock : Block
    {
        private List<ResourceType> resourceTypes = new List<ResourceType>();
        private List<ResourceType> inputtableResourceTypes = new List<ResourceType>();
        private List<ResourceType> outputtableResourceTypes = new List<ResourceType>();
        protected Dictionary<ResourceType, Buffer> buffers = new();

        protected void setBuffers(IEnumerable<Buffer> buffers)
        {
            foreach (Buffer buffer in buffers)
            {
                this.buffers.Add(buffer.ResourceType, buffer);
                resourceTypes.Add(buffer.ResourceType);
                if (buffer.CanAcceptInput)
                {
                    inputtableResourceTypes.Add(buffer.ResourceType);
                }

                if (buffer.CanGiveOutput)
                {
                    outputtableResourceTypes.Add(buffer.ResourceType);
                }
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
                return true;
            }
            return false;
        }

        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);
            resourceTypeIndexPerNeighbor = new List<int>(neighbors.Count);
            currentOutputNeighborIndex = 0;
        }

        private int currentOutputNeighborIndex = 0;
        private List<int> resourceTypeIndexPerNeighbor = new();

        // this code is EVIL. I'm so sorry
        protected virtual void FixedUpdate()
        {
            // Try to output to neighbors if possible
            int previousOutputNeighborIndex = currentOutputNeighborIndex;
            currentOutputNeighborIndex++;
            if (currentOutputNeighborIndex >= neighbors.Count)
            {
                currentOutputNeighborIndex = 0;
            }
            while (currentOutputNeighborIndex != previousOutputNeighborIndex)
            {
                IFactoryElement neighbor = neighbors[currentOutputNeighborIndex];
                
                // Trying this neighbor, either a resource can be output or cycle to the next
                {
                    resourceTypeIndexPerNeighbor[currentOutputNeighborIndex]++;
                    if (resourceTypeIndexPerNeighbor[currentOutputNeighborIndex] >= outputtableResourceTypes.Count)
                    {
                        resourceTypeIndexPerNeighbor[currentOutputNeighborIndex] = 0;
                    }
                    int resourceIndex = resourceTypeIndexPerNeighbor[currentOutputNeighborIndex];
                    resourceIndex++;
                    if (resourceIndex >= outputtableResourceTypes.Count)
                    {
                        resourceIndex = 0;
                    }

                    while (resourceIndex != resourceTypeIndexPerNeighbor[currentOutputNeighborIndex])
                    {
                        ResourceType resourceType = outputtableResourceTypes[resourceIndex];
                        Buffer buffer = buffers[resourceType];
                        if (buffer.Quantity != 0)
                        {
                            if (neighbor.AcceptsResource(this, buffer.QueryResource()))
                            {
                                neighbor.TryInsertResource(this, buffer.TakeResource());
                                return;
                            }
                        }
                        resourceIndex++;
                        if (resourceIndex >= outputtableResourceTypes.Count)
                        {
                            resourceIndex = 0;
                        }
                    }
                }

                currentOutputNeighborIndex++;
                if (currentOutputNeighborIndex >= neighbors.Count)
                {
                    currentOutputNeighborIndex = 0;
                }
            }
        }
    }

    public class Buffer
    {
        public readonly int Capacity;
        private ResourceStack Stack;
        public bool CanAcceptInput;
        public bool CanGiveOutput;

        public Buffer(int capacity, ResourceType resourceType, bool canAcceptInput, bool canGiveOutput)
        {
            Capacity = capacity;
            CanAcceptInput = canAcceptInput;
            CanGiveOutput = canGiveOutput;
            Stack = ResourceStack.Create(ResourceType);
        }
        
        public ResourceType ResourceType {get => Stack.ResourceType;}
        public int Quantity {get => Stack.Quantity;}
        public Resource QueryResource() { return Stack.QueryResource(); }
        public Resource TakeResource() { return Stack.TakeResource(); }

        public void AddResource(Resource resource) 
        {
            if (Quantity >= Capacity) 
            {
                throw new Exception("Buffer is full");
            }

            if (ResourceType != resource.ResourceType)
            {
                throw new Exception("Cannot add different resource type");
            }
            
            Stack.AddResource(resource);
        }

        public void CreateResources(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Resource newResource = Resource.fromType(ResourceType);
                AddResource(newResource);
            }
        }

        public void ConsumeResources(int quantity)
        {
            if (quantity > Quantity)
            {
                throw new Exception("Not enough resources");
            }
            for (int i = 0; i < quantity; i++)
            {
                TakeResource();
            }
        }

        public void Empty()
        {
            Stack = ResourceStack.Create(ResourceType);
        }
    }

}