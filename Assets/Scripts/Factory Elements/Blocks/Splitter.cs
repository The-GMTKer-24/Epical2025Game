using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Factory_Elements.Blocks
{
    public class Splitter : Block
    {
        [SerializeField] public float equalizationRate = 0.10f;
        
        protected Dictionary<Direction, ElementSettings<DirectionConfig>> configuration;
        protected Dictionary<Direction, Item> heldItems;
        protected Dictionary<Direction, IFactoryElement> directionalNeighbors;
        protected Dictionary<IFactoryElement, Direction> neighboralDirections; // lol
        
        private List<Direction> inputDirections = new List<Direction>();
        private List<Direction> outputDirections = new List<Direction>();

        public void Awake()
        {
            configuration = new Dictionary<Direction, ElementSettings<DirectionConfig>>();
            heldItems = new Dictionary<Direction, Item>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                configuration.Add(direction, new ElementSettings<DirectionConfig>(new DirectionConfig(direction), direction.ToString() + " face settings", "The direction of flow and priority of the " + direction.ToString() + " face."));
                heldItems.Add(direction, null);
                configuration[direction].SettingUpdated += onSettingUpdate;
            }
            directionalNeighbors = new Dictionary<Direction, IFactoryElement>();
            neighboralDirections = new Dictionary<IFactoryElement, Direction>();
        }

        public override Direction? Rotation => null;
        public override bool Rotate(Direction direction)
        {
            throw new NotImplementedException();
        }

        public override bool SupportsRotation => false;

        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);
            
            Dictionary<Direction, int2> relatives = new();
            relatives.Add(Direction.North, new int2(0, 1));
            relatives.Add(Direction.East, new int2(1, 0));
            relatives.Add(Direction.South, new int2(0, -1));
            relatives.Add(Direction.West, new int2(-1, 0));
            
            directionalNeighbors.Clear();
            neighboralDirections.Clear();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int2 checkPosition = position + relatives[direction];
                IFactoryElement neighbor = Factory.Instance.FromLocation(checkPosition);
                directionalNeighbors.Add(direction, neighbor);
                neighboralDirections.Add(neighbor, direction);
            }
        }

        private void onSettingUpdate()
        {
            Debug.Log("Splitter settings updated");
            // Determine input faces from output faces
            inputDirections = new List<Direction>();
            outputDirections = new List<Direction>();
            foreach (ElementSettings<DirectionConfig> config in configuration.Values)
            {
                if (config.Value.Input)
                {
                    inputDirections.Add(config.Value.Direction);
                }
                else
                {
                    outputDirections.Add(config.Value.Direction);
                }
            }
        }

        // 50/50 shot this works tbh
        void FixedUpdate()
        {
            // Find the highest priority input that has a queued item
            int highestPriorityInput = Int32.MinValue;
            Direction? chosenInput = null;
            foreach (Direction direction in inputDirections)
            {
                if (heldItems[direction] == null)
                {
                    continue;
                }
                int priority = configuration[direction].Value.Priority;
                if (priority > highestPriorityInput)
                {
                    highestPriorityInput = priority;
                    chosenInput = direction;
                }
            }

            // Execute only if there is an available item
            if (chosenInput is Direction inputDirection)
            {
                Item chosenItem = heldItems[inputDirection];
                // Run through potential outputs from highest to lowest priority
                List<Direction> priorities =
                    outputDirections.OrderByDescending(direction => configuration[direction].Value.Priority).ToList();
                foreach (Direction direction in priorities)
                {
                    if (heldItems[direction] != null)
                    {
                        if (directionalNeighbors[direction] != null &&
                            directionalNeighbors[direction].TryInsertResource(this, chosenItem))
                        {
                            heldItems[inputDirection] = null;
                            break;
                        }
                    }
                }
            }
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (resource is Fluid) return false;
            Direction direction = neighboralDirections[sender];
            return heldItems[direction] == null;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            Direction direction = neighboralDirections[sender];
            if (resource is Item item)
            {
                item.EqualizationRate = equalizationRate;
                heldItems[direction] = item;
                return true;
            }

            throw new Exception("what");
        }

        public override ISetting[] GetSettings()
        {
            return configuration.Values.ToArray<ISetting>();
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            Dictionary<ResourceType, int> heldResources = new();
            foreach (Item heldItem in heldItems.Values)
            {
                if (heldItem == null) continue;
                heldResources.TryAdd(heldItem.ResourceType, 0);
                heldResources[heldItem.ResourceType]++;
            }
            return heldResources;
        }
    }

    public class DirectionConfig
    {
        public Direction Direction;
        private bool input;

        public bool Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
                if (Input)
                {
                    SortType = null;
                    // TODO: In UI, ensure that sorted resource type is not an available setting for input faces
                }
            }
        }

        public bool Output { get { return !Input; } set { Input = !value; } }
        public int Priority;
        public ResourceType SortType;

        public DirectionConfig(Direction direction)
        {
            this.Direction = direction;
            input = false;
            Priority = 0;
            SortType = null;
        }
    }
}