using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent()]
[AutoGenerateComponentState(true)]
[Access(typeof(OccluderSystem), Other = AccessPermissions.ReadExecute)]
public sealed partial class OccluderComponent : Component, IComponentTreeEntry<OccluderComponent>, ISerializationHooks
{
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Local-space convex polygon vertices.
    /// </summary>
    [DataField("polygon", customTypeSerializer: typeof(PhysicsHullSerializer)), AutoNetworkedField]
    private Vector2[] _polygon =
    [
        new(-0.5f, 0.5f),
        new(0.5f, 0.5f),
        new(0.5f, -0.5f),
        new(-0.5f, -0.5f),
    ];

    /// <summary>
    /// Pre-RT288 AABB form still present on some saved maps. Converted to polygon.
    /// </summary>
    [DataField("boundingBox")]
    private Box2? _legacyBoundingBox;

    public ReadOnlySpan<Vector2> Polygon => _polygon;

    internal Vector2[] PolygonArray
    {
        get => _polygon;
        set => _polygon = value;
    }

    [ViewVariables]
    public Box2 LocalBounds { get; internal set; } = Box2.Empty;

    public EntityUid? TreeUid { get; set; }
    public DynamicTree<ComponentTreeEntry<OccluderComponent>>? Tree { get; set; }

    public bool AddToTree => Enabled;
    public bool TreeUpdateQueued { get; set; } = false;

    [ViewVariables]
    public byte OccludingEdges;

    [ViewVariables]
    public (EntityUid TreeUid, Box2 Bounds)? LastTreeBounds;

    void ISerializationHooks.AfterDeserialization()
    {
        if (_legacyBoundingBox is not { } box)
            return;

        var left = MathF.Min(box.Left, box.Right);
        var right = MathF.Max(box.Left, box.Right);
        var bottom = MathF.Min(box.Bottom, box.Top);
        var top = MathF.Max(box.Bottom, box.Top);
        _polygon =
        [
            new(left, top),
            new(right, top),
            new(right, bottom),
            new(left, bottom),
        ];
        _legacyBoundingBox = null;
    }
}
