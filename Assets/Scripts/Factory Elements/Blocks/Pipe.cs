using Factory_Elements.Settings;
using Scriptable_Objects;

namespace Factory_Elements.Blocks
{
    public class Pipe : BufferBlock
    {
        // TODO: (not strictly necessary) replace with a generated singular pipe graph object, such that all pipes have equal pressure across a network
        // TODO: If keeping current system, add a pipe network flush option in UI
        private FluidType type = null;
        private Buffer buffer => buffers.Count == 0 ? null : buffers.GetEnumerator().Current.Value;
        public const int CAPACITY = 10;

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
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}