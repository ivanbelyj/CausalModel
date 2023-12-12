using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;

/// <summary>
/// Provides resolved causal model instance data
/// </summary>
public interface IResolvedModelProvider<TFactValue> : IInstanceFactProvider<TFactValue>
{
    //string RootModelInstanceId { get; }
    IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
        InstanceFactId abstractFactId);
    IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(InstanceFactId id);
    IEnumerable<InstanceFact<TFactValue>> GetRootCauses();
}
