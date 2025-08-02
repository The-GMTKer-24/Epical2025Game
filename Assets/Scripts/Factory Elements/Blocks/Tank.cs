using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;

namespace Factory_Elements.Blocks
{
    public class Tank : BufferBlock
    {
        public const int STORAGE = 250;
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // If a tank empties out, it can be allowed to intake another liquid type.
            if (buffers.Values.ToArray().Length > 0 && buffers.Values.ToArray()[0].Quantity == 0)
            {
                buffers.Clear();
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
            if (buffers.Count == 0) return true;
            return base.AcceptsResource(sender, resource);
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (buffers.Count == 0)
            {
                List<Buffer> buffers = new List<Buffer>();
                buffers.Add(new Buffer(STORAGE, resource.ResourceType, true, true));
                setBuffers(buffers);
            }
            return base.TryInsertResource(sender, resource);
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { };
        }
    }
}