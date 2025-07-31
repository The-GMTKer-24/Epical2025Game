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
            return factoryElements.Overlaps(new Rect(location.x, location.y, type.Size.x, type.Size.y));
        }

        public bool TryPlace(FactoryElementType type, int2 location)
        {
            
            throw new NotImplementedException();
        }
        
        
    }
}