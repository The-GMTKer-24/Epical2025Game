using System;
using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Pipe : BufferBlock
    {
        [SerializeField] public int capacity = 10;
        [SerializeField] Sprite[] pipeSprites; // bitwise, 1 represents a connection and digits from North clockwise
        // 0000 (0): No connections
        // 1000 (8): North connection
        // 0100 (4): East connection

        // TODO: (not strictly necessary) replace with a generated singular pipe graph object, such that all pipes have equal pressure across a network
        private readonly FluidType type = null;

        // Flushes all pipes in a pipe network
        // TODO: Integrate into UI
        private bool flushed;
        private Buffer buffer => buffers.Count == 0 ? null : buffers.GetEnumerator().Current.Value;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (buffer != null && buffer.Quantity == 0) buffers.Remove(buffer.ResourceType);

            foreach (var neighbor in neighbors)
                if (neighbor is not Pipe)
                    while (neighbor.TryInsertResource(this, buffer.QueryResource()))
                        buffer.TakeResource();

            var sumVolume = buffer.Quantity;
            var pipeList = new List<Pipe>();
            foreach (var neighbor in neighbors)
                if (neighbor is Pipe pipe)
                {
                    sumVolume += pipe.buffer.Quantity;
                    pipeList.Add(pipe);
                }

            var averageVolume = Mathf.RoundToInt((float)sumVolume / pipeList.Count);
            if (buffer.Quantity >= averageVolume)
                foreach (var pipe in pipeList)
                    while (pipe.buffer.Quantity < averageVolume)
                        if (pipe.TryInsertResource(this, buffer.QueryResource()))
                            buffer.TakeResource();
        }

        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);
            
            Dictionary<Direction, int2> relatives = new();
            relatives.Add(Direction.North, new int2(0, 1));
            relatives.Add(Direction.East, new int2(1, 0));
            relatives.Add(Direction.South, new int2(0, -1));
            relatives.Add(Direction.West, new int2(-1, 0));

            int spriteID = 0;
            
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int2 checkPosition = position + relatives[direction];
                IFactoryElement neighbor = Factory.Instance.FromLocation(checkPosition);
                if (neighbor is not null)
                {
                    switch (direction)
                    {
                        case Direction.North: spriteID += 8; break;
                        case Direction.East: spriteID += 4; break;
                        case Direction.South: spriteID += 2; break;
                        case Direction.West: spriteID += 1; break;
                    }
                }
            }
            
            Sprite sprite = pipeSprites[spriteID];
            this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        public void Flush()
        {
            if (flushed) return;
            flushed = true;
            buffer.Empty();
            foreach (var neighbor in neighbors)
                if (neighbor is Pipe pipe)
                    pipe.Flush();
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
            if (resource is not Fluid) return false;
            if (type is null) return true;
            if (resource.ResourceType != type) return false;
            if (buffer.Quantity >= buffer.Capacity) return false;
            if (sender is Pipe pipe) return pipe.buffer.Quantity > buffer.Quantity;
            return true;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            if (buffer is null)
            {
                var newBuffer = new Buffer(capacity, resource.ResourceType, true, true);
                setBuffers(new[] { newBuffer });
            }
            else
            {
                buffer.AddResource(resource);
            }

            return true;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}