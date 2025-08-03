using System.Linq;
using Factory_Elements.Settings;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Furnace : BufferBlock
    {
        protected ElementSettings<int> Temperature;

        public override void Awake()
        {
            base.Awake();
            Temperature.Value = 1500;
            equalizationRate = 0.0f;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!base.AcceptsResource(sender, resource)) return false;
            if (resource is not Item) return false;
            ((Item)resource).Temperature = Temperature.Value;
            return base.TryInsertResource(sender, resource);
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
            ISetting[] settings = base.GetSettings();
            return settings.Append(Temperature).ToArray();
        }
    }
}