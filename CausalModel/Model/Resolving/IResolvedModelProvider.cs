using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving;

/// <summary>
/// Provides resolved causal model data.
/// Resolved model includes all resolved blocks
/// (resolved blocks include their resolved blocks, etc.)
/// </summary>
public interface IResolvedModelProvider<TFactValue>
{
    InstanceFact<TFactValue> GetFact(InstanceFactAddress address);
    InstanceFact<TFactValue>? TryGetFact(InstanceFactAddress address);
    IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts(string modelInstanceId);
    IEnumerable<InstanceFact<TFactValue>> GetResolvedFacts();
    //IEnumerable<InstanceFact<TFactValue>>? TryGetExternalCauses(
    //    string modelInstanceId);
    IModelProvider<TFactValue> GetModelProvider(string modeInstanceId);
}
