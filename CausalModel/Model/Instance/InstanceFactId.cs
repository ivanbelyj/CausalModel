using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Instance;

/// <summary>
/// The unique identifier of the fact in the resolved causal model.
/// If the fact is used in several model instances via blocks mechanism,
/// the id should reference to the actual instance from where we can get it
/// directly
/// </summary>
public class InstanceFactId : IEquatable<InstanceFactId>
{
    public string FactId { get; }

    /// <summary>
    /// Id of the model instance where the fact is located.
    /// </summary>
    public string ModelInstanceId { get; }
    public InstanceFactId(string factId, string causalInstanceId)
    {
        FactId = factId;
        ModelInstanceId = causalInstanceId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FactId, ModelInstanceId);
    }

    public override bool Equals(object? obj)
    {
        if (obj is InstanceFactId other)
        {
            return Equals(other);
        }
        else
            return false;
    }

    public bool Equals(InstanceFactId? other)
    {
        if (other == null) return false;
        return other.ModelInstanceId == ModelInstanceId
            && other.FactId == FactId;
    }

    public static bool operator ==(InstanceFactId? id1, InstanceFactId? id2)
    {
        return EqualityComparer<InstanceFactId>.Default.Equals(id1, id2);
    }

    public static bool operator !=(InstanceFactId? id1, InstanceFactId? id2)
    {
        return !(id1 == id2);
    }

    public override string ToString()
    {
        return $"InstanceId: {ModelInstanceId}, FactId: {FactId}";
    }

    public InstanceFactAddress ToAddress()
    {
        return new InstanceFactAddress(FactId, ModelInstanceId);
    }
}
