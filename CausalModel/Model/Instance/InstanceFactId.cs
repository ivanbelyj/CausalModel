using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Instance;
public class InstanceFactId : IEquatable<InstanceFactId>
{
    public string FactId { get; }
    public string CausalInstanceId { get; }
    public InstanceFactId(string factId, string causalInstanceId)
    {
        FactId = factId;
        CausalInstanceId = causalInstanceId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FactId, CausalInstanceId);
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
        return other.CausalInstanceId == CausalInstanceId
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
        return $"InstanceId: {CausalInstanceId}, FactId: {FactId}";
    }
}
