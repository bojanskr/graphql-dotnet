using GraphQL.Types;
using GraphQL.Utilities;

namespace GraphQL;

/// <summary>
/// Attribute for specifying additional information when matching a CLR type to a corresponding GraphType.
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class GraphQLMetadataAttribute : GraphQLAttribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public GraphQLMetadataAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified GraphType name.
    /// </summary>
    /// <param name="name"></param>
    public GraphQLMetadataAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// GraphType name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// GraphType description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Deprecation reason of the field or GraphType.
    /// </summary>
    public string? DeprecationReason { get; set; }

    /// <summary>
    /// Indicates if the marked method represents a field resolver or a subscription event stream resolver.
    /// </summary>
    public ResolverType ResolverType { get; set; }

    /// <summary>
    /// Indicates the CLR type that this graph type represents.
    /// </summary>
    public Type? IsTypeOf { get; set; }

    /// <inheritdoc/>
    public override void Modify(TypeConfig type)
    {
        type.Description = Description;
        type.DeprecationReason = DeprecationReason;

        if (IsTypeOf != null)
            type.IsTypeOfFunc = t => IsTypeOf.IsAssignableFrom(t.GetType());
    }

    /// <inheritdoc/>
    public override void Modify(FieldConfig field)
    {
        field.Description = Description;
        field.DeprecationReason = DeprecationReason;
    }

    /// <inheritdoc/>
    public override void Modify(EnumValueDefinition enumValueDefinition)
    {
        if (Name != null)
            enumValueDefinition.Name = Name;

        if (Description != null)
            enumValueDefinition.Description = Description == "" ? null : Description;

        if (DeprecationReason != null)
            enumValueDefinition.DeprecationReason = DeprecationReason == "" ? null : DeprecationReason;
    }

    /// <inheritdoc/>
    public override void Modify(IGraphType graphType)
    {
        if (Name != null)
            graphType.Name = Name;

        if (Description != null)
            graphType.Description = Description == "" ? null : Description;

        if (DeprecationReason != null)
            graphType.DeprecationReason = DeprecationReason == "" ? null : DeprecationReason;

        if (graphType is IObjectGraphType objectGraphType && IsTypeOf != null)
            objectGraphType.IsTypeOf = obj => IsTypeOf.IsAssignableFrom(obj.GetType());
    }

    /// <inheritdoc/>
    public override void Modify(FieldType fieldType, bool isInputType)
    {
        if (Name != null)
            fieldType.Name = Name;

        if (Description != null)
            fieldType.Description = Description == "" ? null : Description;

        if (DeprecationReason != null)
            fieldType.DeprecationReason = DeprecationReason == "" ? null : DeprecationReason;
    }
}

/// <summary>
/// Indicates if the specified method is a field resolver or event stream resolver.
/// </summary>
public enum ResolverType
{
    /// <summary>
    /// Indicates the specified method is a field resolver
    /// </summary>
    Resolver,
    /// <summary>
    /// Indicates the specified method is an source stream resolver
    /// </summary>
    StreamResolver
}
