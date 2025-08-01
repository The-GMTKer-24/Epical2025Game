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

        public GameObject TryPlace(FactoryElementType type, int2 location, out bool placed)
        {
            if (!CanPlace(type, location))
            {
                placed = false;
                return null;
            }

            if (type.Prefab == null)
                throw new Exception(
                    $"Tried to create a factory element {type.name} that has no associated unity object. ");
            var newFactoryElement = Instantiate(type.Prefab);
            var factoryElement = newFactoryElement.GetComponent<IFactoryElement>();
            factoryElements.Insert(factoryElement,
                new IntRect(location.x, location.y, factoryElement.FactoryElementType.Size.x,
                    factoryElement.FactoryElementType.Size.y));
            IntRect suroundingsToCHeck = new IntRect(location.x - 1, location.y - 1,
                factoryElement.FactoryElementType.Size.x + 2, factoryElement.FactoryElementType.Size.y + 2);
            
            var nearby = factoryElements.ItemsInArea(new IntRect(location.x - 1, location.y - 1,
                factoryElement.FactoryElementType.Size.x + 2, factoryElement.FactoryElementType.Size.y + 2));

            foreach (var e in nearby)
                if (FromFactoryElement(factoryElement).Overlaps(FromFactoryElement(e)) && e != factoryElement)
                {
                    e.OnNeighborUpdate(factoryElement, true);
                    factoryElement.OnNeighborUpdate(e, true);
                }

            placed = true;
            return newFactoryElement;
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