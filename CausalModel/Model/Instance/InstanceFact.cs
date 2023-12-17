using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

/// <summary>
/// Represents a fact from the causal model instance
/// </summary>
public class InstanceFact<TFactValue>
{
    public Fact<TFactValue> Fact { get; }
    public InstanceFactId InstanceFactId { get; }

    public InstanceFact(Fact<TFactValue> fact, string modelInstanceId)
    {
        Fact = fact;
        InstanceFactId = new InstanceFactId(fact.Id, modelInstanceId);
    }

    public override string ToString()
    {
        return Fact.ToString() + " " + InstanceFactId.ToString();
    }
}
