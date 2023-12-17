using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.ResolvingModelProvider;

/// <summary>
/// Provides resolved causal model instance data.
/// Resolved model includes all resolved blocks
/// (resolved blocks include their resolved blocks, etc.)
/// </summary>
public interface IResolvedModelProvider<TFactValue>
{
    InstanceFact<TFactValue> GetFact(InstanceFactId id);
    InstanceFact<TFactValue>? TryGetFact(InstanceFactId id);
    IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts(string modelInstanceId);
    IEnumerable<InstanceFact<TFactValue>> GetResolvedFacts();

    //IEnumerable<InstanceFact<TFactValue>> GetRootFacts();

    IEnumerable<InstanceFact<TFactValue>> GetExternalCauses(
        string modelInstanceId);

    //IEnumerable<InstanceFact<TFactValue>> TryGetInstanceBlocksConsequences(
    //    string modelInstanceId);
}
