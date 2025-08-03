using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Factory_Elements.Blocks
{
    public class WaterSource : Block
    {
        [SerializeField] private FluidType type;
        public void FixedUpdate()
        {
            foreach (IFactoryElement neighbor in neighbors)
            {
                neighbor.TryInsertResource(this, new Fluid(type));
            }
        }
        
        public override Direction? Rotation
        {
            get => null;
            set => throw new System.NotImplementedException();
        }

        public override bool Rotate(Direction direction)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsRotation => false;

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
            return new ISetting[] { };
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            return new Dictionary<ResourceType, int>();
        }
    }
}