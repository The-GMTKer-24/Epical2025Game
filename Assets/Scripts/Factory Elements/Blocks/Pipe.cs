using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Pipe : BufferBlock
    {
        // TODO: (not strictly necessary) replace with a generated singular pipe graph object, such that all pipes have equal pressure across a network
        private FluidType type = null;
        private Buffer buffer => buffers.Count == 0 ? null : buffers.GetEnumerator().Current.Value;
        public const int CAPACITY = 10;
        
        // Flushes all pipes in a pipe network
        // TODO: Integrate into UI
        private bool flushed = false;
        public void Flush()
        {
            if (flushed)
            {
                return;
            }
            this.flushed = true;
            buffer.Empty();
            foreach (IFactoryElement neighbor in neighbors)
            {
                if (neighbor is Pipe pipe)
                {
                    pipe.Flush();
                }
            }
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
                Buffer newBuffer = new Buffer(CAPACITY, resource.ResourceType, true, true);
                setBuffers(new [] {newBuffer});
            }
            else
            {
                buffer.AddResource(resource);
            }

            return true;
        }

        public void FixedUpdate()
        {
            base.FixedUpdate();

            if (buffer != null && buffer.Quantity == 0)
            {
                buffers.Remove(buffer.ResourceType);
            }
            
            int sumVolume = buffer.Quantity;
            List<Pipe> pipeList = new List<Pipe>();
            foreach (IFactoryElement neighbor in neighbors)
            {
                if (neighbor is Pipe pipe)
                {
                    sumVolume += pipe.buffer.Quantity;
                    pipeList.Add(pipe);
                }
            }
            int averageVolume = Mathf.RoundToInt((float)sumVolume / pipeList.Count);
            if (buffer.Quantity >= averageVolume)
            {
                foreach (Pipe pipe in pipeList)
                {
                    
                }
            }
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}