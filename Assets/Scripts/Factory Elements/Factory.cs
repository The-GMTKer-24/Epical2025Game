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
        public void Awake()
        {
            Instance = this;
            if (Instance != null) throw new Exception("Multiple instances exist, replacing old instance");
        }

        public bool CanPlace(FactoryElementType type, int2 location)
        {
            return !factoryElements.Overlaps(new Rect(location.x, location.y, type.Size.x, type.Size.y));
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
            factoryElements.Insert(factoryElement, new Rect(location.x, location.y, factoryElement.FactoryElementType.Size.x, factoryElement.FactoryElementType.Size.y));
            List<IFactoryElement> nearby = factoryElements.ItemsInArea(new Rect(location.x - 1, location.y - 1, factoryElement.FactoryElementType.Size.x +1 , factoryElement.FactoryElementType.Size.y + 1));
            foreach (IFactoryElement e in nearby)
            {
                new Rect(location.x, location.y, factoryElement.FactoryElementType.Size.x,
                    factoryElement.FactoryElementType.Size.y);
            }
            placed = true;
            return newFactoryElement;
        }
        
        
    }
}