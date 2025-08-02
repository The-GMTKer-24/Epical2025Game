using System;
using System.Collections.Generic;
using Factory_Elements;
using Factory_Elements.Blocks;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

public class Decoration : Block
{
    public override Direction? Rotation
    {
        get => null;
        set => throw new NotImplementedException();
    }

    public override bool Rotate(Direction direction)
    {
        throw new NotImplementedException();
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
        return Array.Empty<ISetting>();
    }

    public override Dictionary<ResourceType, int> GetHeldResources()
    {
        return new Dictionary<ResourceType, int>();
    }
}
