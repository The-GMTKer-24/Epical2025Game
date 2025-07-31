using System;
using System.Collections.Generic;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Factory_Elements
{
    public class Factory : MonoBehaviour
    {
        public static Factory Instance { get; private set; }

        private Quadtree<IFactoryElement> factoryElements;
        [SerializeField] private RectInt bounds;
        [SerializeField] private int maxDepth;
        public void Awake()
        {
            Instance = this;
            factoryElements = new Quadtree<IFactoryElement>(2, maxDepth,bounds);
            
        }

        public bool CanPlace(FactoryElementType type, int2 location)
        {
            return !factoryElements.Overlaps(new RectInt(location.x, location.y, type.Size.x, type.Size.y));
        }

        public GameObject TryPlace(FactoryElementType type, int2 location, out bool placed)
        {
            
            if (!CanPlace(type, location))
            {
                placed = false;
                return null;
            }

            GameObject newFactoryElement = Instantiate(type.Prefab);
            IFactoryElement factoryElement = newFactoryElement.GetComponent<IFactoryElement>();
            factoryElements.Insert(factoryElement, new RectInt(location.x, location.y, factoryElement.FactoryElementType.Size.x, factoryElement.FactoryElementType.Size.y));
            List<IFactoryElement> nearby = factoryElements.ItemsInArea(new RectInt(location.x - 1, location.y - 1, factoryElement.FactoryElementType.Size.x +1 , factoryElement.FactoryElementType.Size.y + 1));
            foreach (IFactoryElement e in nearby)
            {
                if (FromFactoryElement(factoryElement).Overlaps(FromFactoryElement(e)))
                {
                    e.OnNeighborUpdate(factoryElement,true);
                    factoryElement.OnNeighborUpdate(e, true);
                }
            }
            placed = true;
            return newFactoryElement;
        }

        public Rect FromFactoryElement(IFactoryElement factoryElement)
        {
            return new Rect(factoryElement.Position.x, factoryElement.Position.x,
                factoryElement.FactoryElementType.Size.x,
                factoryElement.FactoryElementType.Size.y);
        }
    }
}