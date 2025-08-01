using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Pipe : BufferBlock
    {
        [SerializeField] public int capacity = 10;

        // TODO: (not strictly necessary) replace with a generated singular pipe graph object, such that all pipes have equal pressure across a network
        private readonly FluidType type = null;

        // Flushes all pipes in a pipe network
        // TODO: Integrate into UI
        private bool flushed;
        private Buffer buffer => buffers.Count == 0 ? null : buffers.GetEnumerator().Current.Value;

        public void FixedUpdate()
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

        public void Flush()
        {
            if (flushed) return;
            flushed = true;
            buffer.Empty();
            foreach (var neighbor in neighbors)
                if (neighbor is Pipe pipe)
                    pipe.Flush();
        }

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