using System;
using System.Collections.Generic;
using System.Drawing;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UI.Inventory;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    /// <summary>
    ///     A factory element with input and output buffers. Handles I/O handling fully.
    /// </summary>
    public abstract class BufferBlock : Block
    {
        [SerializeField] public float equalizationRate = 0.025f;

        [SerializeField] protected ElementSettings<OutputPipeSetting> configuration;

        protected readonly List<ResourceType> inputtableResourceTypes = new();
        protected readonly List<ResourceType> outputtableResourceTypes = new();
        protected readonly List<ResourceType> resourceTypes = new();
        protected Dictionary<ResourceType, Buffer> buffers = new();
        protected Dictionary<IFactoryElement, List<OutputLocation>> outputs = new();

        private int currentOutputNeighborIndex;
        private List<int> resourceTypeIndexPerNeighbor = new();

        public Dictionary<ResourceType, Buffer> Buffers => buffers;

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
                    // Debug.Log("Runnning: "+ currentOutputNeighborIndex + "Count is: "+ resourceTypeIndexPerNeighbor.Count);

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
                        bool canAccept = resourceType is not FluidType;
                        foreach (OutputLocation location in outputs[neighbor])
                        {
                            if (configuration.Value.PipeSettingsFromLocation[location] == resourceType) canAccept = true;
                        }
                        if (buffer.Quantity != 0 && canAccept)
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
            
            Dictionary<OutputLocation, FluidType> pipeSettings = new();
            List<FluidType> fluidTypes = new();
            FluidType defaultType = null;
            
            foreach (var buffer in buffers)
            {
                this.buffers.Add(buffer.ResourceType, buffer);
                resourceTypes.Add(buffer.ResourceType);
                if (buffer.CanAcceptInput) inputtableResourceTypes.Add(buffer.ResourceType);
                if (buffer.CanGiveOutput)
                {
                    outputtableResourceTypes.Add(buffer.ResourceType);
                    if (buffer.ResourceType is FluidType fluidType && !fluidTypes.Contains(fluidType))
                    {
                        fluidTypes.Add(fluidType);
                    }
                }
            }

            if (fluidTypes.Count == 0)
            {
                configuration = new ElementSettings<OutputPipeSetting>(null, "Pipe Settings", "Sets which fluid types output from which sides", SettingType.PipeSettings);
            }
            else
            {
                fluidTypes.Insert(0, null);

                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    int dimension = factoryElementType.Size.x;
                    if (direction == Direction.East || direction == Direction.West) dimension = factoryElementType.Size.y;
                    for (int i = 0; i < dimension; i++)
                    {
                        pipeSettings.Add(new OutputLocation(direction, i), defaultType);
                    }
                }

                configuration = new ElementSettings<OutputPipeSetting>(new OutputPipeSetting(pipeSettings, fluidTypes),
                    "Pipe Settings", "Sets which fluid types output from which sides", SettingType.PipeSettings );
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
                if (resource is Item item)
                {
                    item.EqualizationRate = equalizationRate;
                }

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

            if (configuration?.Value?.PipeSettingsFromLocation == null)
            {
                return;
            }

            if (added)
            {
                outputs.Add(newNeighbor, new List<OutputLocation>());
                for (int x = newNeighbor.Position.x;
                     x < newNeighbor.Position.x + newNeighbor.FactoryElementType.Size.x;
                     x++)
                {
                    for (int y = newNeighbor.Position.y;
                         y < newNeighbor.Position.y + newNeighbor.FactoryElementType.Size.y;
                         y++)
                    {
                        foreach (OutputLocation location in configuration.Value.PipeSettingsFromLocation.Keys)
                        {
                            if (location.GetLocation(this).Equals(new int2(x, y)))
                            {
                                if (added)
                                {
                                    outputs[newNeighbor].Add(location);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                outputs.Remove(newNeighbor);
            }
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            Dictionary<ResourceType, int> heldResources = new();
            foreach (Buffer buffer in buffers.Values)
            {
                heldResources.Add(buffer.ResourceType, buffer.Quantity);
            }

            return heldResources;
        }
        
        public override ISetting[] GetSettings()
        {
            return new ISetting[] { configuration };
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

    public class OutputPipeSetting
    {
        [SerializeField] public Dictionary<OutputLocation, FluidType> PipeSettingsFromLocation;
        public List<FluidType> AllowedFluidTypes;

        public OutputPipeSetting(Dictionary<OutputLocation, FluidType> fluidTypes, List<FluidType> allowedFluidTypes)
        {
            PipeSettingsFromLocation = fluidTypes;
            AllowedFluidTypes = allowedFluidTypes;
        }
    }

    public class OutputLocation : IComparable
    {
        public readonly Direction Direction;
        public readonly int Index; // increasing x and y

        public OutputLocation(Direction direction, int index)
        {
            Direction = direction;
            Index = index;
        }

        public int2 GetLocation(IFactoryElement building)
        {
            switch (Direction)
            {
                case Direction.North:
                    return building.Position + new int2(Index, building.FactoryElementType.Size.y);
                case Direction.East:
                    return building.Position + new int2(building.FactoryElementType.Size.x, Index);
                case Direction.South:
                    return building.Position + new int2(-1, Index);
                case Direction.West:
                    return building.Position + new int2(Index, -1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is OutputLocation other1 && other1.Direction == this.Direction) return Index.CompareTo(other1.Index);
            if (obj is OutputLocation other) return Direction.CompareTo(other.Direction);
            return 0;
        }
    }
}