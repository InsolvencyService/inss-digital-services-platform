// ReSharper disable UnusedMember.Global - Used by binder
namespace GovUk.Forms.Domain.Primitives;

public sealed record NodeId
{
    public NodeId()
    {
        Value = "";
    }

    public NodeId(string value)
    {
        Value = value;
    }
    
    public string Value { get; init; }
    
    public static implicit operator string(NodeId id) => id.Value;
    
    public static implicit operator NodeId(string value) => new(value);
    
    public override string ToString() => Value;
}