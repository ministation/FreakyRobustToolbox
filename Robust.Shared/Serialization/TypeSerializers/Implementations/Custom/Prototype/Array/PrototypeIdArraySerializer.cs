using System;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

// TODO: make AbstractProtoId and change every prototype to use it...
public class AbstractPrototypeIdArraySerializer<TPrototype> : ITypeValidator<string[], SequenceDataNode>,
    ITypeValidator<string[], ValueDataNode> where TPrototype : class, IPrototype, IInheritingPrototype
{
    public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedSequenceNode(node.Select(x =>
            x is ValueDataNode valueDataNode
                ? ProtoIdSerializer<TPrototype>.Validate(dependencies, valueDataNode)
                : new ErrorNode(x, $"Cannot cast node {x} to ValueDataNode.")).ToList());
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
        => ProtoIdSerializer<TPrototype>.Validate(dependencies, node);
}

/// <summary>
/// Compatibility alias for content that still references the pre-288 name.
/// </summary>
[Obsolete("Use AbstractPrototypeIdArraySerializer<TPrototype>")]
public sealed class PrototypeIdArraySerializer<TPrototype> : AbstractPrototypeIdArraySerializer<TPrototype>
    where TPrototype : class, IPrototype, IInheritingPrototype
{
}
