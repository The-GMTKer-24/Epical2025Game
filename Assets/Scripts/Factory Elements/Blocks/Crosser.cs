using System;
using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Factory_Elements.Blocks
{
    public class Crosser : OneBlock
    {
        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (sender is Crosser) return false; // You cannot chain!!!
            Direction fromDirection = neighboralDirections[sender];
            Direction toDirection = OppositeDirection(fromDirection);
            return directionalNeighbors[toDirection].AcceptsResource(this, resource);
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            Direction fromDirection = neighboralDirections[sender];
            Direction toDirection = OppositeDirection(fromDirection);
            Assert.IsTrue(directionalNeighbors[toDirection].TryInsertResource(this, resource));
            return true;
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            return new Dictionary<ResourceType, int>();
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

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}