using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class ItemSource : Block
    {
        [SerializeField] private ResourceSet resourcePool;
        [SerializeField] private float timeBetweenGenerations;
        
        private readonly ElementSettings<Direction> outputDirectionSetting = new(Direction.South, "Output direction", "Which description to put items into");
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[]{outputDirectionSetting};
        }
    }
}
