using System;
using System.Collections.Generic;
using Game_Info;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Factory_Elements
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] public float roomTemperature = 20.0f; // Degrees Celsius

        [SerializeField] private RectInt bounds;
        [SerializeField] private int maxDepth;
        [SerializeField] private int maxItemsPerNode;
        [SerializeField] private float sellRefundRate;

        private Quadtree<IFactoryElement> factoryElements;

        public static Factory Instance { get; private set; }
        public RectInt Bounds => bounds;

        public void Awake()
        {
            Instance = this;
            factoryElements =
                new Quadtree<IFactoryElement>(new IntRect(bounds.x, bounds.y, bounds.width, bounds.height),
                    maxItemsPerNode, maxDepth);
        }

        public bool CanPlace(FactoryElementType type, int2 location)
        {
            return !factoryElements.Overlaps(new IntRect(location.x, location.y, type.Size.x, type.Size.y));
        }

        public GameObject TryPlace(FactoryElementType type, int2 location, Direction rotation, out bool placed)
        {
            if (!CanPlace(type, location))
            {
                placed = false;
                return null;
            }


            if (type.Prefab == null)
                throw new Exception(
                    $"Tried to create a factory element {type.name} that has no associated unity object. ");
            Tuple<List<ResourceQuantity>, int> cost = BuildingManager.EvaluateCost(type.Cost);
            if (cost.Item2 <= GameInfo.Instance.Money)
            {
                GameInfo.Instance.SpendMoney(cost.Item2);
                foreach (ResourceQuantity quantity in cost.Item1)
                {
                    for (int i = 0; i < quantity.Amount; i++)
                    {
                        Player.Player.Instance.RemoveItem(quantity.Type);
                    }
                }
            }
            else
            {
                placed = false;
                return null;
            }

            GameObject newFactoryElement = Instantiate(type.Prefab, transform);
            newFactoryElement.name =$"{type.name}@({location.x}, {location.y})";
            IFactoryElement factoryElement = newFactoryElement.GetComponent<IFactoryElement>();
            if (!factoryElement.SupportsRotation)
            {
                rotation = Direction.North;
            }
            factoryElements.Insert(factoryElement,
                calculateRotatedRectangle(location, factoryElement.FactoryElementType.Size.x, factoryElement.FactoryElementType.Size.y, rotation));
            factoryElement.Position = location;
            if (factoryElement.SupportsRotation)
            {
                factoryElement.Rotation = rotation;
            }

            IntRect up = new IntRect(location.x, location.y + factoryElement.FactoryElementType.Size.y,
                factoryElement.FactoryElementType.Size.x, 1);
            IntRect down = new IntRect(location.x, location.y - 1,
                factoryElement.FactoryElementType.Size.x, 1);
            IntRect left = new IntRect(location.x - 1, location.y, 1, factoryElement.FactoryElementType.Size.y);
            IntRect right = new IntRect(location.x + factoryElement.FactoryElementType.Size.x, location.y, 1, factoryElement.FactoryElementType.Size.y);
            List<IFactoryElement> nearby = factoryElements.ItemsInArea(up);
            nearby.AddRange(factoryElements.ItemsInArea(down));
            nearby.AddRange(factoryElements.ItemsInArea(left));
            nearby.AddRange(factoryElements.ItemsInArea(right));

            foreach (IFactoryElement e in nearby)
                if (e != factoryElement)
                {
                    e.OnNeighborUpdate(factoryElement, true);
                    factoryElement.OnNeighborUpdate(e, true);
                }

            placed = true;
            return newFactoryElement;
        }

        public IFactoryElement TryRemove(int2 location, out bool removed)
        {
            IFactoryElement toRemove = FromLocation(location);
            if (toRemove != null)
            {
                if (!toRemove.FactoryElementType.IsPermanent)
                {
                    factoryElements.Remove(toRemove);
                    foreach (KeyValuePair<ResourceType, int> heldResource in toRemove.GetHeldResources())
                    {
                        for (int i = 0; i < heldResource.Value; i++)
                        {
                            Player.Player.Instance.AddResource(Resource.fromType(heldResource.Key));
                        }
                    }
                    GameInfo.Instance.GainMoney((int)(BuildingManager.EvaluateCost(toRemove.FactoryElementType.Cost).Item2 * sellRefundRate));
                    FactoryElementType factoryElement = toRemove.FactoryElementType;
                    List<IFactoryElement> nearby = factoryElements.ItemsInArea(new IntRect(location.x - 1, location.y - 1,
                        factoryElement.Size.x + 2, factoryElement.Size.y + 2));

                    foreach (IFactoryElement e in nearby)
                        e.OnNeighborUpdate(toRemove, false);
                    removed = true;
                    return toRemove;
                }
                else
                {
                    removed = false;
                }
            }
            else
            {
                removed = false;
            }
            return null;
        }

        private IntRect calculateRotatedRectangle(int2 location, int width, int height, Direction rotation)
        {
            // // These functions will work by assuming the rectangle is at 0,0. It will perform the basic rotation and then offset with the real location
            // if (rotation == Direction.North)
            // {
                // This is the simplest case
                return new IntRect(location.x, location.y, width, height);
            // } 
            // if (rotation == Direction.South)
            // {
            //     // Our rectangle will just be offset by the height of it
            //     return new IntRect(location.x - width, location.y - height, width, height);
            // }
            // if (rotation == Direction.East)
            // {
            //     // This will have the same x as the normal case, but an offset y. The width and height will also be flipped
            //     return new IntRect(location.x, location.y - width, height, width);
            // }
            // // West case. Like the east case but the x is offset by the height
            // return new IntRect(location.x-height, location.y, height, width);
        }

        public IntRect FromFactoryElement(IFactoryElement factoryElement)
        {
            return new IntRect(factoryElement.Position.x, factoryElement.Position.x,
                factoryElement.FactoryElementType.Size.x,
                factoryElement.FactoryElementType.Size.y);
        }

        public IFactoryElement FromLocation(int2 location)
        {
            IntRect bounds = new IntRect(location.x, location.y, 1, 1);
            List<IFactoryElement> nearbyElements = factoryElements.ItemsInArea(bounds);
            if (nearbyElements.Count == 0) return null;

            if (nearbyElements.Count == 1) return nearbyElements[0];

            throw new Exception("Factory elements cannot overlap!");
        }
    }
}