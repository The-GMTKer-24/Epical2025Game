using System;
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
            var newFactoryElement = Instantiate(type.Prefab, transform);
            newFactoryElement.name =$"{type.name}@({location.x}, {location.y})";
            var factoryElement = newFactoryElement.GetComponent<IFactoryElement>();
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

            var nearby = factoryElements.ItemsInArea(new IntRect(location.x - 1, location.y - 1,
                factoryElement.FactoryElementType.Size.x + 2, factoryElement.FactoryElementType.Size.y + 2));

            foreach (var e in nearby)
                if (e != factoryElement)
                {
                    e.OnNeighborUpdate(factoryElement, true);
                    factoryElement.OnNeighborUpdate(e, true);
                }

            placed = true;
            return newFactoryElement;
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
            var bounds = new IntRect(location.x, location.y, 1, 1);
            var nearbyElements = factoryElements.ItemsInArea(bounds);
            if (nearbyElements.Count == 0) return null;

            if (nearbyElements.Count == 1) return nearbyElements[0];

            throw new Exception("Factory elements cannot overlap!");
        }
    }
}